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
using System.Xml;
using Org.BouncyCastle.Crypto;
using System.ComponentModel;

namespace Locknote.Helpers.Objects
{
    public class Page : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_id;
        private string m_path; //directory the page lives in
        private string m_title;
        private string m_content;

        public Page(string path, string id)
        {
            m_title = "";
            m_content = "";
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

        public void Save(AsymmetricCipherKeyPair keypair)
        {
            //create the metadata
            StringWriter wtr = new StringWriter();
            XmlWriter xmlWtr = XmlWriter.Create(wtr);
            xmlWtr.WriteStartDocument();
            xmlWtr.WriteStartElement("page");
            xmlWtr.WriteElementString("title", m_title);
            xmlWtr.WriteElementString("content", m_content);
            xmlWtr.WriteEndElement();
            xmlWtr.WriteEndDocument();
            xmlWtr.Close();

            //encrypt the metadata
            byte[] rawMetadata = Encoding.UTF8.GetBytes(wtr.GetStringBuilder().ToString());
            byte[] encMetadata = Crypto.AsymmetricEncrypt(rawMetadata, ref keypair);
            Eraser.SecureErase(rawMetadata);

            //write the encrypted metadata to a file
            File.WriteAllBytes(PageFile, encMetadata);
        }

        public bool Load(AsymmetricCipherKeyPair keypair)
        {
            //read the encrypted metadata from file
            byte[] encMetadata = File.ReadAllBytes(PageFile);

            //decrypt the metadata
            byte[] decMetadata = Crypto.AsymmetricDecrypt(encMetadata, ref keypair);
            if (decMetadata == null)
                return false;

            //start parsing the metadata
            string metadata = Encoding.UTF8.GetString(decMetadata);
            XmlReader rdr = XmlReader.Create(new StringReader(metadata));

            //check for page root tag
            if (!rdr.ReadToFollowing("page"))
                return false;

            //get the page title
            if (!rdr.ReadToFollowing("title"))
                return false;
            rdr.Read();
            this.Title = rdr.Value;

            //get the page content
            if (!rdr.ReadToFollowing("content"))
                return false;
            rdr.Read();
            this.Content = rdr.Value;

            //close the reader
            rdr.Close();
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

        public string PageFile
        {
            get
            {
                return Path.Combine(m_path, m_id + ".xml");
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

        public string Content
        {
            get
            {
                return m_content;
            }
            set
            {
                m_content = value;
            }
        }
    }
}
