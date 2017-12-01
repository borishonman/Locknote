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
using Android.Widget;

using Locknote.Custom_Views;
using Locknote.Droid.Renderers;
using Xamarin.Forms.Platform.Android;

using TEditor;
using TEditor.Abstractions;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.ExportRenderer(typeof(LNWebView), typeof(LNWebViewRenderer))]
namespace TEditor
{
    class LNWebViewRenderer : ViewRenderer
    {
        private LNLinearLayout m_lay;
        private LinearLayout m_toolbarParent;
        private LinearLayout m_toolbar;
        private TEditorWebView m_wv;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            ((LNWebView)Element).GetHTML = GetHTML;
            ((LNWebView)Element).SetHTML = SetHTML;

            if (Control == null)
            {
                //the master layout
                m_lay = new LNLinearLayout(Context) { Orientation = Orientation.Vertical, WeightSum = 1 };

                //the toolbar
                m_toolbarParent = new LinearLayout(Context);
                HorizontalScrollView sv = new HorizontalScrollView(Context);
                m_toolbar = new LinearLayout(Context);
                BuildToolbar();
                sv.AddView(m_toolbar);
                m_toolbarParent.AddView(sv);

                //the webview
                m_wv = new TEditorWebView(Context);
                m_wv.SetHTML(((LNWebView)Element).HTML);


                //add the webview and toolbar to the master layout
                m_lay.AddView(m_toolbarParent, new LNLinearLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent));
                m_lay.AddView(m_wv, new LNLinearLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent));

                //set the keyboard handler and the native control
                m_lay.onKeyboardShown += HandleSoftKeyboardShwon;
                SetNativeControl(m_lay);
            }
        }

        public string SetHTML(string html)
        {
            m_wv.SetHTML(html);
            return html;
        }

        public string GetHTML(EventHandler evt)
        {
            Task<string> tsk = m_wv.GetHTML();
            tsk.ContinueWith(t2 =>
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { evt(tsk.Result, null); });
            });
            return "";
        }

        public void BuildToolbar()
        {
            ToolbarBuilder builder = TEditorImplementation.ToolbarBuilder;
            if (builder == null)
                builder = new ToolbarBuilder().AddAll();

            foreach (var item in builder)
            {
                ImageButton imagebutton = new ImageButton(Context);
                imagebutton.Click += (sender, e) =>
                {
                    item.ClickFunc.Invoke(m_wv.RichTextEditor);
                };
                string imagename = item.ImagePath.Split('.')[0];
                int resourceId = (int)typeof(Resource.Drawable).GetField(imagename).GetValue(null);
                imagebutton.SetImageResource(resourceId);
                m_toolbar.AddView(imagebutton);
            }
        }

        public void HandleSoftKeyboardShwon(bool shown, int newHeight)
        {
            if (shown)
            {
                m_toolbar.Visibility = Android.Views.ViewStates.Visible;
                int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                //_toolbarLayout.Measure(widthSpec, heightSpec);
                //int toolbarHeight = _toolbarLayout.MeasuredHeight == 0 ? (int)(ToolbarFixHeight * Resources.DisplayMetrics.Density) : _toolbarLayout.MeasuredHeight;
                //int topToolbarHeight = _topToolBar.MeasuredHeight == 0 ? (int)(ToolbarFixHeight * Resources.DisplayMetrics.Density) : _topToolBar.MeasuredHeight;
                //int editorHeight = newHeight - toolbarHeight - topToolbarHeight;
                //_editorWebView.LayoutParameters.Height = editorHeight;
                //_editorWebView.LayoutParameters.Width = LinearLayout.LayoutParams.MatchParent;
                m_wv.RequestLayout();
            }
            else
            {
                if (newHeight != 0)
                {
                    //_toolbarLayout.Visibility = Android.Views.ViewStates.Invisible;
                    m_wv.LayoutParameters = new LinearLayout.LayoutParams(-1, -1);
                }
            }
        }
    }
}