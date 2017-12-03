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

using Locknote.Custom_Views;
using Locknote.Helpers.Objects;

using Locknote.ViewModels;

namespace Locknote.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PagesPage : ContentPage
	{
        private Section m_sec;
		public PagesPage (Section sec)
		{
			InitializeComponent ();

            mnu_new_page.Clicked += Mnu_new_page_Clicked;

            this.ListView = listview;
            m_sec = sec;

            listview.ItemTapped += new EventHandler((o, e) =>
            {
                Locknote.Helpers.Objects.Page p = (Locknote.Helpers.Objects.Page)o;
                PageEditor pe = new PageEditor(p);
                pe.Title = p.Title;
                ((NavigationPage)((HomeMDP)Application.Current.MainPage).Detail).PushAsync(pe);
            });

            listview.ItemLongTapped += new LNListView.ItemLongTappedHandler((o, e) =>
            {
                Locknote.Helpers.Objects.Page pg = (Locknote.Helpers.Objects.Page)o;
                EditNotebookPrompt p = new EditNotebookPrompt() { Title = pg.Title, PromptTitle = "Edit Page", Placeholder = "A Page", IsNavPage = true };
                p.OnPromptSaved += new Prompt.PromptClosedEventListener(() =>
                {
                    pg.Title = p.Title;
                    LocknoteMgr.GetInstance().SaveNotebooks(true);
                    listview.ItemsSource = m_sec.Pages;
                });
                p.DeleteClicked += new EventHandler((o2, e2) =>
                {
                    Prompt p2 = new Prompt() { PromptTitle = "Are you sure?", PositiveButtonText = "Yes", NegativeButtonText = "No", IsNavPage = true };
                    p2.OnPromptSaved += new Prompt.PromptClosedEventListener(() =>
                    {
                        sec.DeletePage(pg);
                        p.Dismiss();
                    });
                    p2.Show(((HomeMDP)Application.Current.MainPage).Detail);
                });
                p.Show(((HomeMDP)Application.Current.MainPage).Detail);
            });
        }

        private void Mnu_new_page_Clicked(object sender, EventArgs e)
        {
            if (sender == mnu_new_page)
            {
                TextEntryPrompt p = new TextEntryPrompt() { IsNavPage = true, PositiveButtonText = "Create", NegativeButtonText = "Cancel", PromptTitle = "New Page", Hint = "A Page" };
                p.OnPromptSaved += new Prompt.PromptClosedEventListener(() =>
                {
                    m_sec.NewPage(p.Text);
                    LocknoteMgr.GetInstance().SaveNotebooks(true);
                });
                p.Show(((HomeMDP)Application.Current.MainPage).Detail);
            }
        }

        public LNListView ListView { get; set; }
	}
}