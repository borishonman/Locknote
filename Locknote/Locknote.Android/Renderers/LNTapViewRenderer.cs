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

using Android.Views;

using Locknote.Custom_Views;
using Locknote.Droid.Renderers;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(LNTapView), typeof(LNTapViewRenderer))]
namespace Locknote.Droid.Renderers
{
    class LNTapViewRenderer : ViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                Android.Views.View v = new Android.Views.View(Context);
                SetNativeControl(v);
            }

            Control.Touch += new EventHandler<TouchEventArgs>((o, ee) =>
            {
                if (ee.Event.Action == MotionEventActions.Up)
                {
                    ((LNTapView)Element).OnTapped(ee.Event.GetX(), ee.Event.GetY());
                }
            });
        }
    }
}