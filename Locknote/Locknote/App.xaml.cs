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

using Xamarin.Forms;
using Locknote.Views;
using Locknote.Helpers;
using Locknote.ViewModels;
using System.IO;
using System.Text;

namespace Locknote
{
	public partial class App : Application
	{
        private IConfig m_config;
        private LocknoteMgr m_ln;

		public App ()
		{
			InitializeComponent();

            m_config = ConfigFactory.GetInstance();

            m_ln = LocknoteMgr.GetInstance();

            //attempt to load the keys
            bool keys = m_ln.LoadKeys();

            if (!m_config.IsSetUp)
            { //not set up
                //check to see if the keys are there, if so, ask if the user wants to restore the keys
                if (keys)
                {
                    Prompt p = new Prompt() { PromptTitle = "Found existing keys, would you like to import them?", PositiveButtonText = "Yes", NegativeButtonText = "No" };
                    p.OnPromptSaved += new Prompt.PromptClosedEventListener(() =>
                    { //Yes - mark as set up
                        m_config.IsSetUp = true;
                        OnStart();
                    });
                    p.OnPromptDismissed += new Prompt.PromptClosedEventListener(() =>
                    { //No - start the tutorial
                        m_ln.SecureErase();
                        StartTutorial();
                    });
                    p.Show();
                }
                else
                { //no keys were found, just start the tutorial
                    StartTutorial();
                }
            }
		}

		protected override void OnStart ()
		{
            //do nothing here if the app isn't set up yet
            if (!m_config.IsSetUp)
                return;

			//check to make sure both keys were loaded
            if (!m_ln.Loaded)
            {
                string t = "Could not load either your private key or public key\n";
                t += "\nMake sure the files private.key.pem and public.key.pem exist in the directory ";
                t += StorageFactory.GetInstance().GetDirectory();
                Prompt pt = new Prompt() { PromptTitle = t, PositiveButtonVisible = true, PositiveButtonText="Continue Anyway", NegativeButtonVisible = false };
                pt.OnPromptSaved += new Prompt.PromptClosedEventListener(() =>
                {
                    m_config.IsSetUp = false;
                    m_ln.SecureErase();
                    StartTutorial();
                });
                pt.Show();
                return;
            }

            //start the app resume code
            ResumeApp();
		}

		protected override void OnSleep ()
		{
            //is user currently editing a page?
            if (MainPage.GetType() == typeof(HomeMDP) && ((NavigationPage)((HomeMDP)MainPage).Detail).CurrentPage.GetType() == typeof(PageEditor))
            {
                //get the pageeditor
                PageEditor pe = (PageEditor)((NavigationPage)((HomeMDP)MainPage).Detail).CurrentPage;
                //do a temp save so non-saved content is restored on resume
                pe.TempSave();
                //check settings, save page if set
                if (m_config.SaveOnSuspend)
                {
                    pe.SavePage(this, new EventArgs());
                    NotificationFactory.ShortAlert("Page Saved!");
                }
            }
            //check settings, lock app if set
            if (m_config.LockOnSuspend)
                LocknoteMgr.GetInstance().SecureErase();
        }

		protected override void OnResume ()
		{
            //if editing a page, do a temp save so non-saved content is restored on resume
            if (MainPage.GetType() == typeof(HomeMDP) && ((NavigationPage)((HomeMDP)MainPage).Detail).CurrentPage.GetType() == typeof(PageEditor))
                ((PageEditor)((NavigationPage)((HomeMDP)MainPage).Detail).CurrentPage).TempLoad();

            //if set to lock on suspend, need to follow normal resume process
            if (m_config.LockOnSuspend)
                ResumeApp();
		}

        private void StartTutorial()
        {
            TutorialPage tutPage = new TutorialPage();
            tutPage.Complete += new EventHandler((o, e) =>
            {
                FirstTimeSetup fts = new FirstTimeSetup();
                fts.OnSetupComplete += new EventHandler((o2, e2) =>
                {
                    //setup is complete, save that in the app config
                    m_config.IsSetUp = true;
                    //attempt to load the keys and run the normal app start code
                    m_ln.LoadKeys();
                    this.OnStart();
                });
                this.MainPage = new NavigationPage(fts) { Title = "Locknote First Time Setup" };
            });

            //check to see if notebooks exist already
            if (Directory.Exists(NoteManager.GetNotebookDir()))
            {
                Prompt p = new Prompt() { PromptTitle = "Found notebooks. Running first time set up will delete these notebooks. Would you like to continue anyway?", NegativeButtonText = "No", PositiveButtonText = "Yes" };
                p.OnPromptSaved += new Prompt.PromptClosedEventListener(() =>
                {
                    MainPage = tutPage;
                });
                p.Show();
            }
            else
            {
                MainPage = tutPage;
            }
        }

        /*
         * Called on app resume/start after the private key is decrypted
         */
        private void StartAppFinal()
        {
            m_ln.Start();
        }

        /*
         * Decrypt the private key and start the app
         */
        public void ResumeApp()
        {
            //attempt to load the keys if not already loaded
            if (!m_ln.Loaded)
                m_ln.LoadKeys();

            //prompt the user to enter a password to decrypt the private key
            PasswordPrompt p = new PasswordPrompt() { NegativeButtonVisible = false, PositiveButtonText = "Decrypt" };
            p.OnPromptSaved += new Prompt.PromptClosedEventListener(() =>
            {
                if (p.Password == null || p.Password.Length == 0)
                    return;

                //attempt to decrypt the public key
                if (!m_ln.DecryptPrivateKey(p.Password))
                { //decryption failed, incorrect password was entered
                    NotificationFactory.ShortAlert("Password is incorrect");
                    p.Show();
                    return;
                }

                //erase the entered password
                Eraser.SecureErase(p.Password);
                p.Password = "";

                //private key decrypted, start the app normally
                StartAppFinal();
            });
            //check if fingerprint auth is enabled
            if (m_config.UseFingerprint && m_config.EncryptedPassword.Length > 1)
            {
                //prompt for fingerprint
                IFingerprint fp = FingerprintFactory.GetInstance();
                fp.InitReader();
                if (fp.IsReady())
                {
                    Application.Current.MainPage = new FingerprintPage(new EventHandler((oo, ee) =>
                    {
                        byte[] data = (byte[])oo; //page returns the decrypted password

                        //go to password entry if skipped
                        if (data == null)
                        {
                            p.Show();
                            return;
                        }

                        //decrypt the password
                        string pass = Encoding.UTF8.GetString(data);

                        //attempt to decrypt the private key
                        if (!m_ln.DecryptPrivateKey(pass))
                        { //decryption failed
                            return;
                        }

                        //erase the decrypted password
                        Eraser.SecureErase(pass);
                        p.Password = "";

                        //private key decrypted, start the app normally
                        StartAppFinal();
                    }), fp, "");
                }
            }
            else
            {
                p.Show();
            }
        }
    }
}
