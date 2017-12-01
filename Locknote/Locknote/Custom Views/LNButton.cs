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
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using Locknote.Helpers;

namespace Locknote.Custom_Views
{
    class LNFrameRenderer : Frame
    {

    }
    class LNButton : ContentView
    {
        public event EventHandler Clicked;

        private Label m_label;
        private Xamarin.Forms.Color m_bgColor;
        private bool m_enabled;

        public LNButton()
        {
            m_label = new Label() { VerticalOptions=Xamarin.Forms.LayoutOptions.FillAndExpand, HorizontalOptions=Xamarin.Forms.LayoutOptions.FillAndExpand,HorizontalTextAlignment=Xamarin.Forms.TextAlignment.Center,TextColor=Xamarin.Forms.Color.White };
            this.Content = m_label;

            this.Padding = new Xamarin.Forms.Thickness(20 * 2, 10);
            
            m_bgColor = ((Xamarin.Forms.Color)Xamarin.Forms.Application.Current.Resources["Primary"]);
            BackgroundColorHighlight = ((Xamarin.Forms.Color)Xamarin.Forms.Application.Current.Resources["Highlight1"]);
            //BackgroundColorHighlight = m_bgColor.WithLuminosity(0.4);
            this.BackgroundColor = m_bgColor;

            m_enabled = true;
        }

        public void OnClicked()
        {
            if (Clicked != null)
                Clicked(this, new EventArgs());
        }

        public Color BackgroundColorHighlight { get; set; }

        public bool Enabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                m_enabled = value;
                this.BackgroundColor = value ? m_bgColor : m_bgColor.MultiplyAlpha(0.5);
            }
        }

        public bool FontBold
        {
            get
            {
                return m_label.FontAttributes == FontAttributes.Bold;
            }
            set
            {
                m_label.FontAttributes = value ? FontAttributes.Bold : FontAttributes.None;
            }
        }
        public string Text
        {
            get
            {
                return m_label.Text;
            }
            set
            {
                m_label.Text = value;
            }
        }
        public double FontSize
        {
            get
            {
                return m_label.FontSize;
            }
            set
            {
                m_label.FontSize = value;
            }
        }
    }
}
