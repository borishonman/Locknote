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
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Locknote.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TutorialPage : TabbedPage
    {
        public event EventHandler Complete;

        public TutorialPage ()
        {
            InitializeComponent();

            Complete = null;

            btn_next.Clicked += Btn_next_Clicked;
            btn_next2.Clicked += Btn_next_Clicked;
            btn_next3.Clicked += Btn_next_Clicked;
            btn_next4.Clicked += Btn_next_Clicked;
        }

        private void Btn_next_Clicked(object sender, EventArgs e)
        {
            //get the currently selected item
            Xamarin.Forms.Page curSel = this.CurrentPage;
            int curIndex = 0;
            foreach (Page p in this.Children)
            {
                if (p == curSel)
                    break;
                curIndex++;
            }
            //set the selected item to the next item in the list
            if (curIndex < this.Children.Cast<ContentPage>().ToList().Count-1)
            {
                this.CurrentPage = this.Children.Cast<ContentPage>().ToList()[curIndex + 1];
            }
            else
            {
                if (Complete != null)
                    Complete(this, new EventArgs());
            }
        }
    }
}