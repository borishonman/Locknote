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
    public partial class HomeMDPMaster : ContentPage
    {
        public ListView ListView;
        public HomeMDPMaster()
        {
            InitializeComponent();

            ListView = lst_notebooks;

            object version = "";
            App.Current.Resources.TryGetValue("version", out version);
            lbl_version.Text = version.ToString();

            mst_lock_unlock.Clicked += new EventHandler((o, e) =>
            {
                LocknoteMgr.GetInstance().SecureErase();
                ((App)Application.Current).ResumeApp();
            });
            mst_settings.Clicked += new EventHandler((o, e) =>
            {
                ((NavigationPage)((HomeMDP)Application.Current.MainPage).Detail).PushAsync(new SettingsPage());
                ((HomeMDP)Application.Current.MainPage).IsPresented = false;
            });
            mst_new_notebook.Clicked += new EventHandler((o, e) =>
            {
                TextEntryPrompt p = new TextEntryPrompt() { IsNavPage = true, PositiveButtonText = "Create", NegativeButtonText = "Cancel", PromptTitle = "New Notebook", Hint = "A Notebook" };
                p.OnPromptSaved += new Prompt.PromptClosedEventListener(() =>
                {
                    LocknoteMgr.GetInstance().NoteManager.NewNotebook(p.Text);
                    LocknoteMgr.GetInstance().SaveNotebooks(true);
                    ((HomeMDP)Application.Current.MainPage).IsPresented = true;
                });
                p.Show(((HomeMDP)Application.Current.MainPage).Detail);
                ((HomeMDP)Application.Current.MainPage).IsPresented = false;
            });

            lst_notebooks.ItemTapped += new EventHandler((o, e) =>
            {
                Notebook nb = (Notebook)o;
                SectionsPage sp = new SectionsPage(nb);
                sp.Title = nb.Title + " | Sections";
                sp.ListView.ItemsSource = nb.Sections;
                ((HomeMDP)Application.Current.MainPage).Detail = new NavigationPage(sp);
                ((HomeMDP)Application.Current.MainPage).IsPresented = false;
            });

            lst_notebooks.ItemLongTapped += new LNListView.ItemLongTappedHandler((o, e) =>
            {
                Notebook nb = (Notebook)o;
                EditNotebookPrompt p = new EditNotebookPrompt() { Title = nb.Title, IsNavPage = true };
                Xamarin.Forms.Page pg = ((NavigationPage)((HomeMDP)Application.Current.MainPage).Detail).CurrentPage;
                p.OnPromptSaved += new Prompt.PromptClosedEventListener(() =>
                {
                    nb.Title = p.Title;
                    if (pg.GetType() == typeof(SectionsPage))
                        pg.Title = p.Title + " | Sections";
                    LocknoteMgr.GetInstance().SaveNotebooks(true);
                    lst_notebooks.ItemsSource = LocknoteMgr.GetInstance().NoteManager.Notebooks;

                });
                p.DeleteClicked += new EventHandler((o2, e2) =>
                {
                    Prompt p2 = new Prompt() { PromptTitle = "Are you sure?", PositiveButtonText = "Yes", NegativeButtonText = "No", IsNavPage = true };
                    p2.OnPromptSaved += new Prompt.PromptClosedEventListener(() =>
                    {
                        LocknoteMgr.GetInstance().NoteManager.DeleteNotebook(nb);
                        p.Dismiss();
                    });
                    p2.Show(((HomeMDP)Application.Current.MainPage).Detail);
                });
                p.Show(((HomeMDP)Application.Current.MainPage).Detail);
                ((HomeMDP)Application.Current.MainPage).IsPresented = false;
            });
        }
    }
}