using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Locknote.Helpers.Objects;

namespace Locknote.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PagePage : ContentPage
	{
        Section m_sec;

		public PagePage (Section sec)
		{
			InitializeComponent ();

            m_sec = sec;
		}
	}
}