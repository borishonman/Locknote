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

namespace Locknote.ViewModels
{
    class EditNotebookPrompt : Prompt
    {
        public event EventHandler DeleteClicked;

        private Entry m_txtTitle;
        private Button m_btnDel;

        public EditNotebookPrompt()
        {
            m_txtTitle = new Entry() { Placeholder = "Notebook Name" };
            m_btnDel = new Button() { BackgroundColor = Color.Red, Text="Delete" };
            m_btnDel.Clicked += new EventHandler((o, e) =>
            {
                if (DeleteClicked != null)
                    DeleteClicked(o, e);
            });

            AddView(m_txtTitle);
            AddView(m_btnDel);

            this.PromptTitle = "Edit Notebook";
            this.PositiveButtonText = "Save";
            this.NegativeButtonText = "Cancel";
        }

        public new string Title
        {
            get
            {
                return m_txtTitle.Text;
            }
            set
            {
                m_txtTitle.Text = value;
            }
        }

        public string Placeholder
        {
            get
            {
                return m_txtTitle.Placeholder;
            }
            set
            {
                m_txtTitle.Placeholder = value;
            }
        }
    }
}
