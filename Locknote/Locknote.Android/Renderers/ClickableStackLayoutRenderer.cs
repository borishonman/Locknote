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

using Android.Content;
using Android.Views;

using Locknote.Custom_Views;
using Locknote.Droid.Renderers;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(ClickableStackLayout), typeof(ClickableStackLayoutRenderer))]
namespace Locknote.Droid.Renderers
{
    class ClickableStackLayoutRenderer : ViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                Android.Widget.LinearLayout v = new Android.Widget.LinearLayout(Context);
                v.Clickable = true;
                SetNativeControl(v);
            }

            Control.Click += new EventHandler((o, ee) =>
            {
                if (((ClickableStackLayout)Element).Enabled)
                    ((ClickableStackLayout)Element).OnClicked();
            });

            Control.Touch += new EventHandler<TouchEventArgs>((o, ee) =>
            {
                if (!((ClickableStackLayout)Element).Enabled)
                {
                    ee.Handled = false;
                    return;
                }

                if (ee.Event.Action == MotionEventActions.Down)
                {
                    int[] attrs = { Resource.Attribute.colorAccent };
                    Control.SetBackgroundColor(Context.Theme.ObtainStyledAttributes(Resource.Style.MainTheme, attrs).GetColor(0, Android.Graphics.Color.Black));
                }
                else if (ee.Event.Action == MotionEventActions.Up)
                {
                    Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
                }

                ee.Handled = false;
            });
        }
    }
}