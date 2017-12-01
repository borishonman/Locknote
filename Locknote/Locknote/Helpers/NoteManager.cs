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
using System.IO;
using Xamarin.Forms;

using Locknote.Helpers.Objects;
using Org.BouncyCastle.Crypto;
using System.Threading;
using System.Collections.ObjectModel;

namespace Locknote.Helpers
{
    class NoteManager
    {
        public event EventHandler OnNotebooksLoaded;

        //private Dictionary<string, Notebook> m_notebooks;
        private ObservableCollection<Notebook> m_notebooks;

        public NoteManager()
        {
            //m_notebooks = new Dictionary<string, Notebook>();
            m_notebooks = new ObservableCollection<Notebook>();
        }

        public void ClearHandlers()
        {
            OnNotebooksLoaded = null;
        }

        public Notebook NewNotebook(string title)
        {
            //initial notebook ID is the hashed string of the current date and time
            string nbID = Crypto.HashStringStr(DateTime.Now.ToFileTimeUtc().ToString());
            Notebook newNb = new Notebook(GetNotebookDir(), nbID);
            newNb.Title = title;
            m_notebooks.Add(newNb);
            return newNb;
        }

        public void DeleteNotebook(Notebook nb)
        {
            //delete it from the file system
            Directory.Delete(nb.NotebookDirectory, true);
            //remove it within the app
            m_notebooks.Remove(nb);
        }

        public void SaveNotebooks(AsymmetricCipherKeyPair keypair, bool background)
        {
            Xamarin.Forms.Page oldPage = App.Current.MainPage;

            if (!background)
            {
                //create the activity indicator layout
                ContentPage actPage = new ContentPage();
                StackLayout actLayout = new StackLayout() { VerticalOptions = LayoutOptions.CenterAndExpand };
                ActivityIndicator actInd = new ActivityIndicator(); ;
                actLayout.Children.Add(actInd);
                actLayout.Children.Add(new Label() { Text = "Saving Notebooks", TextColor = Color.DarkGray, HorizontalTextAlignment = TextAlignment.Center });
                actPage.Content = actLayout;

                //show wait animation
                actInd.IsRunning = true;
                App.Current.MainPage = actPage;
            }

            Thread t = new Thread(new ParameterizedThreadStart((o) =>
            {
                foreach (Notebook nb in m_notebooks)
                {
                    nb.Save(keypair);
                }
                if (!background)
                    Device.BeginInvokeOnMainThread(() => {App.Current.MainPage = oldPage;});
            }));
            t.Start();
        }

        public void SavePage(Locknote.Helpers.Objects.Page page, AsymmetricCipherKeyPair keypair, bool background)
        {
            Xamarin.Forms.Page oldPage = App.Current.MainPage;

            if (!background)
            {
                //create the activity indicator layout
                ContentPage actPage = new ContentPage();
                StackLayout actLayout = new StackLayout() { VerticalOptions = LayoutOptions.CenterAndExpand };
                ActivityIndicator actInd = new ActivityIndicator(); ;
                actLayout.Children.Add(actInd);
                actLayout.Children.Add(new Label() { Text = "Saving Page", TextColor = Color.DarkGray, HorizontalTextAlignment = TextAlignment.Center });
                actPage.Content = actLayout;

                //show wait animation
                actInd.IsRunning = true;
                App.Current.MainPage = actPage;
            }

            Thread t = new Thread(new ParameterizedThreadStart((o) =>
            {
                page.Save(keypair);
                if (!background)
                    Device.BeginInvokeOnMainThread(() => { App.Current.MainPage = oldPage; });
            }));
            t.Start();
        }

        public static string GetNotebookDir()
        {
            string absAppStorage = StorageFactory.GetInstance().GetDirectory();
            string absPathNotebooks = Path.Combine(absAppStorage, (string)Application.Current.Resources["Folder_Notebooks"]);
            return absPathNotebooks;
        }

        public void LoadNotebooks(AsymmetricCipherKeyPair keypair)
        {
            //create the activity indicator layout
            ContentPage actPage = new ContentPage();
            StackLayout actLayout = new StackLayout() { VerticalOptions = LayoutOptions.CenterAndExpand };
            ActivityIndicator actInd = new ActivityIndicator(); ;
            actLayout.Children.Add(actInd);
            actLayout.Children.Add(new Label() { Text = "Decrypting Notebooks", TextColor = Color.DarkGray, HorizontalTextAlignment = TextAlignment.Center });
            actPage.Content = actLayout;

            //show wait animation
            actInd.IsRunning = true;
            Xamarin.Forms.Page oldPage = App.Current.MainPage;
            App.Current.MainPage = actPage;

            Thread t = new Thread(new ParameterizedThreadStart((o) =>
            {
                if (!Directory.Exists(GetNotebookDir()))
                {
                    //Device.BeginInvokeOnMainThread(() => { App.Current.MainPage = oldPage; });
                    Device.BeginInvokeOnMainThread(() => { App.Current.MainPage = oldPage; OnNotebooksLoaded(true, new EventArgs()); }); //there were no notebooks to load, so just return
                    return; 
                }

                //enumerate through all the "notebooks" (i.e. subdirectories of the root directory)
                foreach (string notebookFldr in Directory.EnumerateDirectories(GetNotebookDir()))
                {
                    string nbId = Path.GetFileName(notebookFldr);
                    Notebook newNotebook = new Notebook(GetNotebookDir(), nbId);
                    if (!newNotebook.Load(keypair))
                    {
                        Device.BeginInvokeOnMainThread(() => { App.Current.MainPage = oldPage; });
                        Device.BeginInvokeOnMainThread(() => { OnNotebooksLoaded(false, new EventArgs()); });
                        return;
                    }
                    m_notebooks.Add(newNotebook);
                }

                Device.BeginInvokeOnMainThread(() => { App.Current.MainPage = oldPage; });
                Device.BeginInvokeOnMainThread(() => { OnNotebooksLoaded(true, new EventArgs()); });
            }));
            t.Start();
        }

        public ObservableCollection<Notebook> Notebooks
        {
            get
            {
                return m_notebooks;
            }
        }
    }
}
