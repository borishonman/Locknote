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

[assembly: Xamarin.Forms.ExportRenderer(typeof(LNButton), typeof(LNButtonRenderer))]
namespace Locknote.Droid.Renderers
{
    class LNButtonRenderer : ViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                Android.Views.View v = new Android.Views.View(Context);
                v.Clickable = true;
                SetNativeControl(v);
            }

            Control.Click += new EventHandler((o, ee) =>
            {
                if (((LNButton)Element).Enabled)
                    ((LNButton)Element).OnClicked();
            });

            Control.Touch += new EventHandler<TouchEventArgs>((o, ee) =>
            {
                if (!((LNButton)Element).Enabled)
                {
                    ee.Handled = false;
                    return;
                }

                if (ee.Event.Action == MotionEventActions.Down)
                {
                    Control.SetBackgroundColor(((LNButton)Element).BackgroundColorHighlight.ToAndroid());
                }
                else if (ee.Event.Action == MotionEventActions.Up)
                {
                    Control.SetBackgroundColor(Element.BackgroundColor.ToAndroid());
                }

                ee.Handled = false;
            });
        }
    }
}