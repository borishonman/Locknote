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

namespace Locknote.Custom_Views
{
    class LNWebView : StackLayout
    {
        public Func<EventHandler, string> GetHTML;
        public Func<string, string> SetHTML;

        private string m_html;

        public LNWebView()
        {
            GetHTML = null;
            SetHTML = null;
            m_html = "";
        }

        public void Get_HTML(EventHandler evt)
        {
            if (GetHTML != null)
                GetHTML((o, e) =>
                {
                    m_html = (string)o;
                    evt(o, e);
                });
        }

        public void Set_HTML(string html)
        {
            m_html = html;
            if (SetHTML != null)
                SetHTML(html);
        }

        public string HTML
        {
            get
            {
                return m_html;
            }
        }
    }
}
