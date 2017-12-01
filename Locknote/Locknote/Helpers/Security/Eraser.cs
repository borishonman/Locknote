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

using Locknote.Helpers.Objects;
using System.Runtime.InteropServices;

namespace Locknote.Helpers
{
    class Eraser
    {
        public static void SecureErase(string obj)
        {
            if (obj == null)
                return;

            GCHandle handle = GCHandle.Alloc(obj, GCHandleType.Pinned);

            byte[] zero = new byte[obj.Length*sizeof(char)];
            for (int i=0;i<obj.Length*sizeof(char);i++)
            {
                zero[i] = 0;
            }
            Marshal.Copy(zero, 0, handle.AddrOfPinnedObject(), obj.Length*sizeof(char));

            handle.Free();
        }

        public static void SecureErase(byte[] obj)
        {
            if (obj == null)
                return;

            GCHandle handle = GCHandle.Alloc(obj, GCHandleType.Pinned);

            byte[] zero = new byte[obj.Length];
            for (int i = 0; i < obj.Length; i++)
            {
                zero[i] = 0;
            }
            Marshal.Copy(zero, 0, handle.AddrOfPinnedObject(), obj.Length);

            handle.Free();
        }

        public static void SecureErase(NoteManager mgr)
        {
            if (mgr == null)
                return;

            foreach (Notebook nb in mgr.Notebooks)
            {
                SecureErase(nb.Title);
                foreach (Section sec in nb.Sections)
                {
                    SecureErase(sec.Title);
                    foreach (Page pg in sec.Pages)
                    {
                        SecureErase(pg.Title);
                        SecureErase(pg.Content);
                    }
                    sec.Pages.Clear();
                }
                nb.Sections.Clear();
            }
            mgr.Notebooks.Clear();
        }
    }
}
