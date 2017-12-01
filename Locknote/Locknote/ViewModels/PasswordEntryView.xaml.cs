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

namespace Locknote.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PasswordEntryView : ContentView
	{
        public event EventHandler OnSave;

		public PasswordEntryView ()
		{
			InitializeComponent ();

            txt_pass.TextChanged += Txt_pass_TextChanged;
            txt_pass_confirm.TextChanged += Txt_pass_confirm_TextChanged;
            btn_save_password.Clicked += new EventHandler((o, e) =>
            {
                if (OnSave != null)
                    OnSave(this, new EventArgs());
            });
		}

        public string Text
        {
            get
            {
                return txt_pass.Text;
            }
        }

        private void CalculateSecurityFactor()
        {
            string pass = txt_pass.Text;
            bool lower = false, upper = false, num = false, special = false;
            int fac = 0;

            foreach (char c in pass)
            {
                if (Char.IsLetter(c) && Char.IsLower(c) && !lower)
                {
                    lower = true;
                    fac++;
                }
                else if (Char.IsLetter(c) && Char.IsUpper(c) && !upper)
                {
                    upper = true;
                    fac++;
                }
                else if (Char.IsNumber(c) && !num)
                {
                    num = true;
                    fac++;
                }
                else if (Char.IsSymbol(c) && !special)
                {
                    special = true;
                    fac++;
                }
            }

            lbl_sec_length.TextColor = (pass.Length >= 8) ? Color.Green : Color.Red;
            lbl_sec_lower.TextColor = lower ? Color.Green : Color.Red;
            lbl_sec_upper.TextColor = upper ? Color.Green : Color.Red;
            lbl_sec_num.TextColor = num ? Color.Green : Color.Red;
            lbl_sec_special.TextColor = special ? Color.Green : Color.Red;

            if (pass.Length < 8)
                fac -= 2;
            if (fac < 0)
                fac = 0;

            switch (fac)
            {
                case 0:
                case 1:
                    lbl_pass_sec.Text = "Insecure";
                    lbl_pass_sec.TextColor = Color.Red;
                    break;
                case 2:
                    lbl_pass_sec.Text = "Not really secure";
                    lbl_pass_sec.TextColor = Color.Orange;
                    break;
                case 3:
                    lbl_pass_sec.Text = "Moderately Secure";
                    lbl_pass_sec.TextColor = Color.YellowGreen;
                    break;
                case 4:
                    lbl_pass_sec.Text = "Secure";
                    lbl_pass_sec.TextColor = Color.Green;
                    break;
            }
        }

        private void CheckMatch()
        {
            lbl_pass_match.Text = txt_pass.Text.Equals(txt_pass_confirm.Text) ? " " : "Passwords do not match";
            btn_save_password.Enabled = txt_pass.Text.Equals(txt_pass_confirm.Text);
        }

        private void Txt_pass_confirm_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckMatch();
        }

        private void Txt_pass_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckMatch();
            CalculateSecurityFactor();
        }
    }
}