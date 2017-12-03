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