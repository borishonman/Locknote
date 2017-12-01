/*
    This file is part of Locknote.
    Locknote is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    Locknote is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
    You should have received a copy of the GNU General Public License
    along with Locknote.  If not, see <http://www.gnu.org/licenses/>.
 */

using Org.BouncyCastle.Crypto;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using Xamarin.Forms;

namespace Locknote.Helpers.Objects
{
    public class Section : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_id;
        private string m_path; //the directory that this section lives in
        private string m_title;
        private ObservableCollection<Page> m_pages; //Page ID -> Page
        public Section(string path, string id)
        {
            m_pages = new ObservableCollection<Page>();
            m_title = null;
            m_path = path;
            m_id = id;
        }

        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Page NewPage(string title)
        {
            //initial page ID is the hashed string of the current date and time
            string pageID = Crypto.HashStringStr(DateTime.Now.ToFileTimeUtc().ToString());
            Page newPage = new Page(this.SectionDirectory, pageID);
            newPage.Title = title;
            m_pages.Add(newPage);
            return newPage;
        }
        public void DeletePage(Page pg)
        {
            m_pages.Remove(pg);
        }

        public void Save(AsymmetricCipherKeyPair keypair)
        {
            //create the section's directory if it doesn't exist
            Directory.CreateDirectory(SectionDirectory);

            //create the metadata
            StringWriter wtr = new StringWriter();
            XmlWriter xmlWtr = XmlWriter.Create(wtr);
            xmlWtr.WriteStartDocument();
            xmlWtr.WriteStartElement("section");
            xmlWtr.WriteElementString("title", m_title);
            foreach (Page p in m_pages)
            {
                xmlWtr.WriteElementString("page", p.ID);
                //also save the page
                p.Save(keypair);
            }
            xmlWtr.WriteEndElement();
            xmlWtr.WriteEndDocument();
            xmlWtr.Close();

            //encrypt the metadata
            byte[] rawMetadata = Encoding.UTF8.GetBytes(wtr.GetStringBuilder().ToString());
            byte[] encMetadata = Crypto.AsymmetricEncrypt(rawMetadata, ref keypair);
            Eraser.SecureErase(rawMetadata);

            //write the encrypted metadata to a file
            string absPathMetadata = Path.Combine(SectionDirectory, (string)Application.Current.Resources["MetadataFile"]);
            File.WriteAllBytes(absPathMetadata, encMetadata);
        }

        public bool Load(AsymmetricCipherKeyPair keypair)
        {
            //read the encrypted metadata
            string absPathMetadata = Path.Combine(SectionDirectory, (string)Application.Current.Resources["MetadataFile"]);
            byte[] encMetadata = File.ReadAllBytes(absPathMetadata);

            //decrypt the metadata
            byte[] decMetadata = Crypto.AsymmetricDecrypt(encMetadata, ref keypair);
            if (decMetadata == null)
                return false;

            //start parsing the metadata
            string metadata = Encoding.UTF8.GetString(decMetadata);
            XmlReader rdr = XmlReader.Create(new StringReader(metadata));

            //check for root tag
            if (!rdr.ReadToFollowing("section"))
                return false;

            //get the section title
            if (!rdr.ReadToFollowing("title"))
                return false;
            rdr.Read();
            this.Title = rdr.Value;

            //get the pages
            while (rdr.ReadToFollowing("page"))
            {
                rdr.Read();
                Page newPage = new Page(this.SectionDirectory, rdr.Value);
                if (!newPage.Load(keypair))
                    return false;
                m_pages.Add(newPage);
            }

            Eraser.SecureErase(decMetadata);
            Eraser.SecureErase(metadata);

            return true;
        }

        public string ID
        {
            get
            {
                return m_id;
            }
        }

        private string SectionDirectory
        {
            get
            {
                return Path.Combine(m_path, m_id);
            }
        }

        public string Title
        {
            get
            {
                return m_title;
            }
            set
            {
                m_title = value;
                NotifyPropertyChanged("Title");
            }
        }

        public ObservableCollection<Page> Pages
        {
            get
            {
                return m_pages;
            }
        }
    }
}
