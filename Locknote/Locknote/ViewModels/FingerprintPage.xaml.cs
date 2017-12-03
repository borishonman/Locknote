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

using Locknote.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Locknote.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FingerprintPage : ContentPage
	{
		public FingerprintPage (EventHandler evt, IFingerprint fp, string pass)
		{
			InitializeComponent ();

            if (pass != "")
            {
                fp.GetFingerprint(new EventHandler((o, e) =>
                {
                    byte[] data = (byte[])o;
                    evt(data, new EventArgs());
                }), true, Encoding.UTF8.GetBytes(pass));
            }
            else
            {
                fp.GetFingerprint(new EventHandler((o, e) =>
                {
                    byte[] data = (byte[])o;
                    evt(data, new EventArgs());
                }), false, ConfigFactory.GetInstance().EncryptedPassword);
            }

            
            

            btn_skip.Clicked += new EventHandler((o, e) =>
            {
                fp.CancelFingerprint();
                evt(null, new EventArgs());
            });
        }
	}
}