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

namespace Locknote.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PageEditor : ContentPage
	{
        private Locknote.Helpers.Objects.Page m_pg;

        public PageEditor (Locknote.Helpers.Objects.Page pg)
		{
			InitializeComponent ();

            m_pg = pg;

            mnu_save.Clicked += Mnu_save_Clicked;
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            webview.Set_HTML(m_pg.Content);
        }

        private void Mnu_save_Clicked(object sender, EventArgs e)
        {
            webview.Get_HTML(new EventHandler((o, e2) =>
            {
                string html = (string)o;
                m_pg.Content = html;
                LocknoteMgr.GetInstance().SavePage(m_pg, false);
            }));
        }
    }
}