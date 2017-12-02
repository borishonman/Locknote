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

using Android.App;

using Locknote.Helpers;

using Locknote.Droid;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(Storage))]
namespace Locknote.Droid
{
    class Storage : IStorage
    {
        public string GetDirectory()
        {
            string path = Path.Combine(Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "Android/data"), Application.Context.PackageName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}