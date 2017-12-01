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
using Xamarin.Forms.Platform.Android;
using Locknote.Droid.Renderers;

[assembly: Xamarin.Forms.ExportRenderer(typeof(LNListViewItem), typeof(LNListViewItemRenderer))]
namespace Locknote.Droid.Renderers
{
    class LNListViewItemRenderer : ViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                Android.Views.View v = new Android.Views.View(Context);
                v.Click += new EventHandler((o, ee) =>
                {
                    ((LNListViewItem)Element).OnTap();
                });
                v.LongClick += new EventHandler<LongClickEventArgs>((o, ee) =>
                {
                    v.SetBackgroundColor(Android.Graphics.Color.White);
                    ((LNListViewItem)Element).OnLongTap();
                });
                v.Touch += new EventHandler<TouchEventArgs>((o, ee) =>
                {
                    bool contains = (ee.Event.GetX() < v.Width) && (ee.Event.GetY() < v.Height);

                    if (ee.Event.Action == MotionEventActions.Down)
                        v.SetBackgroundColor(((LNListViewItem)Element).TouchDownBackground.ToAndroid());
                    else if (ee.Event.Action == MotionEventActions.Up || ee.Event.Action == MotionEventActions.Cancel)
                        v.SetBackgroundColor(Android.Graphics.Color.White);

                    ee.Handled = false;
                });
                SetNativeControl(v);
            }
        }
    }
}