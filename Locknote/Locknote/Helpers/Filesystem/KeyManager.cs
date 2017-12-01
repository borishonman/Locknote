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

using Org.BouncyCastle.Utilities.IO.Pem;
using System.Text;

namespace Locknote.Helpers
{
    class KeyManager
    {
        public static void SaveKeys(byte[] priv, byte[] pub)
        {
            //save the private key to file
            FileManager.WriteBytes(Encoding.ASCII.GetBytes(Crypto.GetPem(priv, true)), "private.key.pem");
            //save the public key to file
            FileManager.WriteBytes(Encoding.ASCII.GetBytes(Crypto.GetPem(pub, false)), "public.key.pem");
        }

        public static void LoadKeys(out byte[] priv, out byte[] pub)
        {
            byte[] privKeyRaw;
            byte[] pubKeyRaw;
            string privKeyPem;
            string pubKeyPem;

            //initialize the byte arrays
            priv = null;
            pub = null;

            //read the raw bytes from the file
            privKeyRaw = FileManager.ReadBytes("private.key.pem");
            pubKeyRaw = FileManager.ReadBytes("public.key.pem");

            if (privKeyRaw == null || pubKeyRaw == null)
                return;

            //get the string from the file
            privKeyPem = Encoding.ASCII.GetString(privKeyRaw);
            pubKeyPem = Encoding.ASCII.GetString(pubKeyRaw);

            //get the key byte arrays from the PEM data
            System.IO.TextReader privStream = new System.IO.StringReader(privKeyPem);
            System.IO.TextReader pubStream = new System.IO.StringReader(pubKeyPem);
            PemReader rdrPriv = new PemReader(privStream);
            PemReader rdrPub = new PemReader(pubStream);
            PemObject pemPriv = rdrPriv.ReadPemObject();
            PemObject pemPub = rdrPub.ReadPemObject();
            priv = (pemPriv.Type == "LOCKNOTE PRIVATE KEY") ? pemPriv.Content : null;
            pub = (pemPub.Type == "LOCKNOTE PUBLIC KEY") ? pemPub.Content : null;
        }
    }
}
