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
using Xamarin.Forms.Xaml;

using Locknote.Helpers;
using Locknote.ViewModels;

namespace Locknote.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
        private IConfig m_config;
		public SettingsPage ()
		{
			InitializeComponent ();

            m_config = ConfigFactory.GetInstance();

            //initialize to the current settings
            chk_lock_on_suspend.IsToggled = m_config.LockOnSuspend;
            chk_save_on_suspend.IsToggled = m_config.SaveOnSuspend;
            chk_fingerprint.IsToggled = m_config.UseFingerprint;

            //we need to handle the fingerprint enable separately
            chk_fingerprint.Toggled += new EventHandler<ToggledEventArgs>((o, e) =>
            {
                if (chk_fingerprint.IsToggled)
                {
                    IFingerprint fp = FingerprintFactory.GetInstance();
                    fp.InitReader();
                    if (fp.IsReady())
                    {
                        PasswordPrompt pmt = new PasswordPrompt() { IsNavPage = true, PromptTitle="Verify your Password", PositiveButtonText="Verify", RestorePage=true };
                        pmt.OnPromptSaved += new Prompt.PromptClosedEventListener(() =>
                        {
                            if (pmt.Password == null || pmt.Password.Length == 0)
                                return;

                            //attempt to decrypt the private key, just as a verification method
                            if (!LocknoteMgr.GetInstance().DecryptPrivateKey(pmt.Password))
                            { //decryption failed, incorrect password was entered
                                NotificationFactory.ShortAlert("Password is incorrect");
                                pmt.Show(((NavigationPage)((HomeMDP)Application.Current.MainPage).Detail));
                                return;
                            }

                            //we verified the password is correct, now we can prompt the user to scan a fingerprint
                            Page back = Application.Current.MainPage;
                            Application.Current.MainPage = new FingerprintPage(new EventHandler((oo, ee) =>
                            {
                                byte[] data = (byte[])oo; //page returns the encrypted password
                                if (data != null)
                                { //only if was not skipped
                                  //encrypt the password and save it
                                    ConfigFactory.GetInstance().EncryptedPassword = data;
                                    ConfigFactory.GetInstance().UseFingerprint = true;
                                    NotificationFactory.ShortAlert("Fingerprint unlock enabled");
                                }
                                else
                                {
                                    ConfigFactory.GetInstance().EncryptedPassword = new byte[] { 0 };
                                    ConfigFactory.GetInstance().UseFingerprint = false;
                                    chk_fingerprint.IsToggled = false;
                                }
                                Application.Current.MainPage = back;
                            }), fp, pmt.Password);
                        });
                        pmt.OnPromptDismissed += new Prompt.PromptClosedEventListener(() =>
                        {
                            chk_fingerprint.IsToggled = false;
                        });
                        pmt.Show(((NavigationPage)((HomeMDP)Application.Current.MainPage).Detail));
                    }
                }
                else
                {
                    ConfigFactory.GetInstance().EncryptedPassword = new byte[] { 0 };
                    ConfigFactory.GetInstance().UseFingerprint = false;
                }
            });

            //set the button handlers
            btn_save.Clicked += new EventHandler((o, e) =>
            {
                m_config.LockOnSuspend = chk_lock_on_suspend.IsToggled;
                m_config.SaveOnSuspend = chk_save_on_suspend.IsToggled;

                NotificationFactory.ShortAlert("Settings Saved!");
            });
            btn_change_password.Clicked += new EventHandler((o, e) =>
            {
                PasswordEntryView pep = new PasswordEntryView();
                pep.OnSave += new EventHandler((oo, ee) =>
                {
                    LocknoteMgr.GetInstance().ReencryptPrivateKey(pep.Text);
                    ((NavigationPage)((HomeMDP)Application.Current.MainPage).Detail).PopAsync();
                    NotificationFactory.ShortAlert("Password changed");
                });
                Xamarin.Forms.ContentPage pg = new ContentPage();
                pg.Content = pep;
                ((NavigationPage)((HomeMDP)Application.Current.MainPage).Detail).PushAsync(pg);
            });
		}
	}
}