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

using Android.Content;

using Locknote.Helpers;
using Locknote.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(ConfigAndroid))]
namespace Locknote.Droid
{
    class ConfigAndroid : IConfig
    {
        private ISharedPreferences m_prefs;
        private ISharedPreferencesEditor m_prefsEditor;

        public ConfigAndroid()
        {
            Context ctx = Android.App.Application.Context;
            m_prefs = ctx.GetSharedPreferences(ctx.Resources.GetString(Resource.String.app_config_file), FileCreationMode.Private);
            m_prefsEditor = m_prefs.Edit();
        }

        public bool IsSetUp
        {
            get
            {
                return m_prefs.GetBoolean("issetup", false);
            }
            set
            {
                m_prefsEditor.PutBoolean("issetup", value);
                m_prefsEditor.Commit();
            }
        }

        public bool LockOnSuspend
        {
            get
            {
                return m_prefs.GetBoolean("lockonsuspend", false);
            }
            set
            {
                m_prefsEditor.PutBoolean("lockonsuspend", value);
                m_prefsEditor.Commit();
            }
        }
    }
}