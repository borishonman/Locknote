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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Security.Keystore;
using Java.Security;
using Android.Support.V4.Hardware.Fingerprint;
using Javax.Crypto;
using Java.Security.Spec;
using Javax.Crypto.Spec;

using Locknote.Helpers;

namespace Locknote.Droid
{
    public class CryptoObjectHelper
    {
        // This can be key name you want. Should be unique for the app.
        static readonly string KEY_NAME = "com.amxdesigns.locknote.fingerprint";

        // We always use this keystore on Android.
        static readonly string KEYSTORE_NAME = "AndroidKeyStore";

        // Should be no need to change these values.
        static readonly string KEY_ALGORITHM = KeyProperties.KeyAlgorithmAes;
        static readonly string BLOCK_MODE = KeyProperties.BlockModeCbc;
        static readonly string ENCRYPTION_PADDING = KeyProperties.EncryptionPaddingPkcs7;
        static readonly string TRANSFORMATION = KEY_ALGORITHM + "/" +
                                                BLOCK_MODE + "/" +
                                                ENCRYPTION_PADDING;
        readonly KeyStore _keystore;

        public CryptoObjectHelper()
        {
            _keystore = KeyStore.GetInstance(KEYSTORE_NAME);
            _keystore.Load(null);
        }

        public FingerprintManagerCompat.CryptoObject BuildCryptoObject(CipherMode mode)
        {
            Cipher cipher = CreateCipher(mode);
            return new FingerprintManagerCompat.CryptoObject(cipher);
        }

        public Cipher CreateCipher(CipherMode mode, bool retry = true)
        {
            IKey key = GetKey();
            Cipher cipher = Cipher.GetInstance(TRANSFORMATION);
            try
            {
                if (mode == CipherMode.EncryptMode)
                {
                    cipher.Init(mode, key);
                    (new ConfigAndroid()).EncryptedPasswordIV = cipher.GetIV();
                }
                else if (mode == CipherMode.DecryptMode)
                {
                    byte[] IV = (new ConfigAndroid()).EncryptedPasswordIV;
                    cipher.Init(mode, key, new IvParameterSpec(IV));
                }
            }
            catch (KeyPermanentlyInvalidatedException e)
            {
                _keystore.DeleteEntry(KEY_NAME);
                if (retry)
                {
                    CreateCipher(mode, false);
                }
                else
                {
                    throw new Exception("Could not create the cipher for fingerprint authentication.", e);
                }
            }
            return cipher;
        }

        IKey GetKey()
        {
            IKey secretKey;
            if (!_keystore.IsKeyEntry(KEY_NAME))
            {
                CreateKey();
            }

            secretKey = _keystore.GetKey(KEY_NAME, null);
            return secretKey;
        }

        void CreateKey()
        {
            KeyGenerator keyGen = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, KEYSTORE_NAME);
            KeyGenParameterSpec keyGenSpec =
                new KeyGenParameterSpec.Builder(KEY_NAME, KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                    .SetBlockModes(BLOCK_MODE)
                    .SetEncryptionPaddings(ENCRYPTION_PADDING)
                    .SetUserAuthenticationRequired(true)
                    .Build();
            keyGen.Init(keyGenSpec);
            keyGen.GenerateKey();
        }
    }
}