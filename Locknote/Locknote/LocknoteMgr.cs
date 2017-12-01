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

using Locknote.Helpers;
using Locknote.Views;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;

namespace Locknote
{
    class LocknoteMgr
    {
        private byte[] m_encPrivKey;
        private byte[] m_decPrivKey;
        private byte[] m_pubKey;
        private NoteManager m_mgr;
        private static LocknoteMgr m_theMgr = null;

        private LocknoteMgr()
        {
            m_mgr = new NoteManager();
        }

        public static LocknoteMgr GetInstance()
        {
            if (m_theMgr == null)
                m_theMgr = new LocknoteMgr();
            return m_theMgr;
        }

        public bool LoadKeys()
        {
            KeyManager.LoadKeys(out m_encPrivKey, out m_pubKey);
            return (m_encPrivKey != null) && (m_pubKey != null);
        }

        public void ReencryptPrivateKey(string pass)
        {
            //encrypt the private key
            m_encPrivKey = Crypto.EncryptKey(m_decPrivKey, pass);

            //save the keys to file
            KeyManager.SaveKeys(m_encPrivKey, m_pubKey);
        }

        public bool DecryptPrivateKey(string pass)
        {
            m_decPrivKey = Crypto.DecryptKey(m_encPrivKey, pass);
            if (m_decPrivKey == null)
                return false;

            //erase the encrypted data
            Eraser.SecureErase(m_encPrivKey);

            return true;
        }

        public void Start()
        {
            m_mgr.OnNotebooksLoaded += new EventHandler((o, e) =>
            {
                if ((bool)o == false)
                    return;
                //create the master detail page
                HomeMDP mdp = new HomeMDP();
                ((HomeMDPMaster)mdp.Master).ListView.ItemsSource = m_mgr.Notebooks;
                //display page
                mdp.NavigationPage.PushAsync(new HomePage());
                App.Current.MainPage = mdp;
            });
            AsymmetricCipherKeyPair kp = new AsymmetricCipherKeyPair(PublicKeyFactory.CreateKey(m_pubKey), PrivateKeyFactory.CreateKey(m_decPrivKey));
            m_mgr.LoadNotebooks(kp);
        }

        public void SaveNotebooks(bool background)
        {
            AsymmetricCipherKeyPair kp = new AsymmetricCipherKeyPair(PublicKeyFactory.CreateKey(m_pubKey), PrivateKeyFactory.CreateKey(m_decPrivKey));
            m_mgr.SaveNotebooks(kp, background);
        }
        public void SavePage(Locknote.Helpers.Objects.Page page, bool background)
        {
            AsymmetricCipherKeyPair kp = new AsymmetricCipherKeyPair(PublicKeyFactory.CreateKey(m_pubKey), PrivateKeyFactory.CreateKey(m_decPrivKey));
            m_mgr.SavePage(page, kp, background);
        }

        public void SecureErase()
        {
            Eraser.SecureErase(m_encPrivKey);
            Eraser.SecureErase(m_pubKey);
            Eraser.SecureErase(m_decPrivKey);
            Eraser.SecureErase(m_mgr);
            m_encPrivKey = null;
            m_pubKey = null;
            m_decPrivKey = null;
            m_mgr.ClearHandlers();
        }

        public NoteManager NoteManager
        {
            get
            {
                return m_mgr;
            }
        }

        public bool Loaded
        {
            get
            {
                return (m_encPrivKey != null) && (m_pubKey != null);
            }
        }
    }
}
