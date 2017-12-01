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

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Xamarin.Forms;
using System.Xml;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Locknote.Helpers.Objects
{
    public class Notebook : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_id;
        private string m_path; //directory the notebook lives in
        private string m_title; //title of the notebook
        private ObservableCollection<Section> m_sections;

        public Notebook(string path, string id)
        {
            m_sections = new ObservableCollection<Section>();
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

        public Section NewSection(string title)
        {
            //initial section ID is the hashed string of the current date and time
            string secID = Crypto.HashStringStr(DateTime.Now.ToFileTimeUtc().ToString());
            Section newSec = new Section(this.NotebookDirectory, secID);
            newSec.Title = title;
            m_sections.Add(newSec);
            return newSec;
        }

        public void DeleteSection(Section sec)
        {
            m_sections.Remove(sec);
        }

        public void Save(AsymmetricCipherKeyPair keypair)
        {
            //create the notebook's directory if it doesn't exist
            Directory.CreateDirectory(NotebookDirectory);

            //create the metadata
            StringWriter wtr = new StringWriter();
            XmlWriter xmlWtr = XmlWriter.Create(wtr);
            xmlWtr.WriteStartDocument();
            xmlWtr.WriteStartElement("notebook");
            xmlWtr.WriteElementString("title", m_title);
            foreach (Section sec in m_sections)
            {
                xmlWtr.WriteElementString("section", sec.ID);
                //also save the section
                sec.Save(keypair);
            }
            xmlWtr.WriteEndElement();
            xmlWtr.WriteEndDocument();
            xmlWtr.Close();

            //encrypt the metadata
            byte[] rawMetadata = Encoding.UTF8.GetBytes(wtr.GetStringBuilder().ToString());
            byte[] encMetadata = Crypto.AsymmetricEncrypt(rawMetadata, ref keypair);
            Eraser.SecureErase(rawMetadata);

            //write the encrypted metadata to a file
            string absPathMetadata = Path.Combine(NotebookDirectory, (string)Application.Current.Resources["MetadataFile"]);
            File.WriteAllBytes(absPathMetadata, encMetadata);
        }

        public bool Load(AsymmetricCipherKeyPair keypair)
        {
            //load the encrypted metadata
            string absPathMetadata = Path.Combine(NotebookDirectory, (string)Application.Current.Resources["MetadataFile"]);
            byte[] encMetadata = File.ReadAllBytes(absPathMetadata);

            //decrypt the metadata
            byte[] decMetadata = Crypto.AsymmetricDecrypt(encMetadata, ref keypair);
            if (decMetadata == null)
                return false;

            //get the metadata
            string metadata = Encoding.UTF8.GetString(decMetadata);

            //start parsing the metadata
            XmlReader rdr = XmlReader.Create(new StringReader(metadata));

            //check for the root tag
            if (!rdr.ReadToFollowing("notebook"))
                return false;

            //get the notebook title
            if (!rdr.ReadToFollowing("title"))
                return false;
            rdr.Read();
            this.Title = rdr.Value;

            //get the sections
            while (rdr.ReadToFollowing("section"))
            {
                rdr.Read();
                Section newSection = new Section(NotebookDirectory, rdr.Value);
                if (!newSection.Load(keypair))
                    return false;
                m_sections.Add(newSection);
            }
            Eraser.SecureErase(metadata);
            Eraser.SecureErase(decMetadata);

            return true;
        }

        public string NotebookDirectory
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

        public ObservableCollection<Section> Sections
        {
            get
            {
                return m_sections;
            }
        }
    }
}
