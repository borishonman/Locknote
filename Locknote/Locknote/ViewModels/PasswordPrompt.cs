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
    class PasswordPrompt : Prompt
    {
        private Entry m_pass;
        public PasswordPrompt()
        {
            this.RestorePage = false;
            this.PromptTitle = "Enter password to unlock private key";
            m_pass = new Entry() { IsPassword = true,Placeholder="Private Key Password" };
            m_pass.Completed += new EventHandler((o, e) =>
            {
                PromptSaved();

            });
            AddView(m_pass);
        }

        public string Password
        {
            get
            {
                return m_pass.Text;
            }
            set
            {
                m_pass.Text = value;
            }
        }
    }
}
