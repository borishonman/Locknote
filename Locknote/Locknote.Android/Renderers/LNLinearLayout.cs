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

using Android.App;
using Android.Widget;
using Android.Util;
using Android.Graphics;

namespace Locknote.Droid.Renderers
{
    class LNLinearLayout : LinearLayout
    {
        public LNLinearLayout(Android.Content.Context context) : base(context)
        {
        }


        public LNLinearLayout(Android.Content.Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public LNLinearLayout(Android.Content.Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public LNLinearLayout(Android.Content.Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {

        }

        public Action<bool, int> onKeyboardShown;

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int height = MeasureSpec.GetSize(heightMeasureSpec);
            Activity activity = this.Context as Activity;
            Rect rect = new Rect();
            activity.Window.DecorView.GetWindowVisibleDisplayFrame(rect);
            int VisibleHeight = rect.Height();
            Point size = new Point();
            activity.WindowManager.DefaultDisplay.GetSize(size);
            int screenHeight = size.Y;
            int diff = screenHeight - VisibleHeight;
            if (onKeyboardShown != null)
            {
                // assume all soft keyboards are at least 128 pixels high
                // screenHeight - height means that when user long click the editor past and copy menu will be shown, it is the height of menu
                onKeyboardShown.Invoke((diff > 128) && VisibleHeight != 0, VisibleHeight - (screenHeight - height));
            }
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }
    }
}