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

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Org.BouncyCastle.Security;

namespace Locknote.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TapRandomizer : ContentView
	{
        private const int NUM_TAPS = 20;

        public event EventHandler OnRandomized;

        private List<byte> m_rndSeed;
        private int m_taps;

        public TapRandomizer ()
		{
			InitializeComponent ();

            m_rndSeed = new List<byte>();
            m_taps = 0;

            this.lbl_tap_counter.Text = "0 / " + NUM_TAPS.ToString();

            view_tap_view.Tapped += View_tap_view_Tapped;
		}

        private void View_tap_view_Tapped(object sender, Custom_Views.TappedEventArgs e)
        {
            SecureRandom rnd = new SecureRandom();
            rnd.SetSeed(BitConverter.GetBytes(e.X + e.Y));
            byte[] bytes = new byte[BitConverter.GetBytes(e.X + e.Y).Length];
            rnd.NextBytes(bytes);
            m_taps++;
            lbl_tap_counter.Text = m_taps.ToString() + " / " + NUM_TAPS.ToString();
            foreach (byte b in bytes)
            {
                m_rndSeed.Add(b);
            }

            if (m_taps == NUM_TAPS)
            {
                if (OnRandomized != null)
                    OnRandomized(this, new EventArgs());
            }
        }

        public byte[] Seed
        {
            get
            {
                return m_rndSeed.ToArray();
            }
        }
    }
}