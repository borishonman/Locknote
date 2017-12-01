﻿/*
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
    class TappedEventArgs : EventArgs
    {
        public TappedEventArgs(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        public float X { get; set; }
        public float Y { get; set; }
    }
    class LNTapView : ContentView
    {
        public delegate void TappedEventHandler(object sender, TappedEventArgs e);
        public event TappedEventHandler Tapped;

        public void OnTapped(float x, float y)
        {
            if (Tapped != null)
                Tapped(this, new TappedEventArgs(x, y));
        }
    }
}
