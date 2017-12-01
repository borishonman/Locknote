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
	public partial class SectionsPage : ContentPage
	{
        private Notebook m_nb;

		public SectionsPage (Notebook nb)
		{
			InitializeComponent ();

            m_nb = nb;

            mnu_new_section.Clicked += Mnu_new_section_Clicked;

            listview.ItemTapped += new EventHandler((o, e) =>
            {
                Section sec = (Section)o;
                PagesPage pp = new PagesPage(sec);
                pp.Title = sec.Title + " | Pages";
                pp.ListView.ItemsSource = sec.Pages;
                ((NavigationPage)((HomeMDP)Application.Current.MainPage).Detail).PushAsync(pp);
            });

            listview.ItemLongTapped += new LNListView.ItemLongTappedHandler((o, e) =>
            {
                Section sec = (Section)o;
                EditNotebookPrompt p = new EditNotebookPrompt() { Title = sec.Title, PromptTitle = "Edit Section", Placeholder="A Section", IsNavPage = true };
                p.OnPromptSaved += new Prompt.PromptClosedEventListener(() =>
                {
                    sec.Title = p.Title;
                    LocknoteMgr.GetInstance().SaveNotebooks(true);
                    listview.ItemsSource = m_nb.Sections;
                });
                p.DeleteClicked += new EventHandler((o2, e2) =>
                {
                    Prompt p2 = new Prompt() { PromptTitle = "Are you sure?", PositiveButtonText = "Yes", NegativeButtonText = "No", IsNavPage = true };
                    p2.OnPromptSaved += new Prompt.PromptClosedEventListener(() =>
                    {
                        m_nb.DeleteSection(sec);
                        p.Dismiss();
                    });
                    p2.Show(((HomeMDP)Application.Current.MainPage).Detail);
                });
                p.Show(((HomeMDP)Application.Current.MainPage).Detail);
            });
		}

        private void Mnu_new_section_Clicked(object sender, EventArgs e)
        {
            if (sender == mnu_new_section)
            {
                TextEntryPrompt p = new TextEntryPrompt() { IsNavPage = true, PositiveButtonText = "Create", NegativeButtonText = "Cancel", PromptTitle = "New Section", Hint = "A Section" };
                p.OnPromptSaved += new Prompt.PromptClosedEventListener(() =>
                {
                    m_nb.NewSection(p.Text);
                    LocknoteMgr.GetInstance().SaveNotebooks(true);
                });
                p.Show(((HomeMDP)Application.Current.MainPage).Detail);
            }
        }

        public LNListView ListView
        {
            get
            {
                return listview;
            }
        }
    }
}