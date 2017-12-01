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

using Locknote.Custom_Views;

namespace Locknote.ViewModels
{
    class Prompt : ContentPage
    {
        public delegate void PromptClosedEventListener();
        public event PromptClosedEventListener OnPromptSaved;
        public event PromptClosedEventListener OnPromptDismissed;

        private StackLayout m_layout;
        private Label m_title;
        private Grid m_btnsLayout;
        protected LNButton m_btnSave;
        protected LNButton m_btnCancel;

        private Xamarin.Forms.Page m_orig;

        private bool m_restore;

        public Prompt()
        {
            this.OnPromptSaved = null;
            this.OnPromptDismissed = null;

            m_layout = new StackLayout() { VerticalOptions = LayoutOptions.Center, Padding = 20 };
            m_title = new Label() { HorizontalTextAlignment = TextAlignment.Center, FontSize = 14 };

            m_btnSave = new LNButton();
            m_btnCancel = new LNButton();

            m_btnsLayout = new Grid()
            {
                ColumnDefinitions = new ColumnDefinitionCollection() {
                    new ColumnDefinition() { Width=GridLength.Auto },
                    new ColumnDefinition() { Width=GridLength.Star },
                    new ColumnDefinition() { Width=GridLength.Auto },
                }
            };

            m_btnsLayout.Children.Add(m_btnSave, 2, 0);
            m_btnsLayout.Children.Add(m_btnCancel, 0, 0);

            m_layout.Children.Add(m_title);
            m_layout.Children.Add(m_btnsLayout);

            PositiveButtonText = "Save";
            NegativeButtonText = "Cancel";

            m_restore = true;

            this.Content = m_layout;
        }

        public void AddView(View v)
        {
            m_layout.Children.Insert(m_layout.Children.Count - 1, v);
        }

        public bool IsNavPage { get; set; }

        public string PromptTitle
        {
            get
            {
                return m_title.Text;
            }
            set
            {
                m_title.Text = value;
                this.Title = value;
            }
        }

        public string PositiveButtonText
        {
            get
            {
                return m_btnSave.Text;
            }
            set
            {
                m_btnSave.Text = value;
            }
        }
        public string NegativeButtonText
        {
            get
            {
                return m_btnCancel.Text;
            }
            set
            {
                m_btnCancel.Text = value;
            }
        }

        public bool PositiveButtonVisible
        {
            get
            {
                return m_btnSave.IsVisible;
            }
            set
            {
                m_btnSave.IsVisible = value;
            }
        }
        public bool NegativeButtonVisible
        {
            get
            {
                return m_btnCancel.IsVisible;
            }
            set
            {
                m_btnCancel.IsVisible = value;
            }
        }

        public bool RestorePage
        {
            get
            {
                return m_restore;
            }
            set
            {
                m_restore = value;
            }
        }

        public void Show(Xamarin.Forms.Page page)
        {
            m_orig = page;

            m_btnSave.Clicked += M_btn_Clicked;
            m_btnCancel.Clicked += M_btn_Clicked;

            if (IsNavPage)
                ((NavigationPage)m_orig).PushAsync(this);
            else
                Application.Current.MainPage = this;
        }

        protected void M_btn_Clicked(object sender, EventArgs e)
        {
            Dismiss();
            if (sender == m_btnSave && this.OnPromptSaved != null)
                this.OnPromptSaved();
            else if (sender == m_btnCancel && this.OnPromptDismissed != null)
                this.OnPromptDismissed();
        }

        public void Show()
        {
            Show(Application.Current.MainPage);
        }

        public void Show(NavigationPage page)
        {
            Show(page);
        }

        public void Dismiss()
        {
            if (m_orig != null && m_restore)
            {
                if (IsNavPage)
                    ((NavigationPage)m_orig).PopAsync();
                else
                    Application.Current.MainPage = m_orig;
            }
        }

        protected void PromptSaved()
        {
            if (OnPromptSaved != null)
                OnPromptSaved();
        }
    }
}