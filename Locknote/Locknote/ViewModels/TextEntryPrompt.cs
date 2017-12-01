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

namespace Locknote.ViewModels
{
    class TextEntryPrompt : Prompt
    {
        private Entry m_field;

        public TextEntryPrompt()
        {
            m_field = new Entry();
            m_field.Completed += new EventHandler((o, e) =>
            {
                M_btn_Clicked(m_btnSave, e);
            });
            this.AddView(m_field);
        }

        public Keyboard Keyboard
        {
            get
            {
                return m_field.Keyboard;
            }
            set
            {
                m_field.Keyboard = value;
            }
        }
        public string Text
        {
            get
            {
                return m_field.Text;
            }
            set
            {
                m_field.Text = value;
            }
        }
        public string Hint
        {
            get
            {
                return m_field.Placeholder;
            }
            set
            {
                m_field.Placeholder = value;
            }
        }
    }
}