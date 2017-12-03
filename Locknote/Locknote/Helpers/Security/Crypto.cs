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
using System.Text;
using System.Linq;

using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Digests;
using System.Threading;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Agreement.Kdf;
using Org.BouncyCastle.Asn1.Nist;

namespace Locknote.Helpers
{
    class Crypto
    {
        private const string RANDOM_ALGO = "SHA512PRNG";
        private const string ECDSA_CURVE = "brainpoolp512t1";

        public delegate void GenCompleteEventHandler(AsymmetricCipherKeyPair kp);

        public static byte[] AsymmetricEncrypt(byte[] data, ref AsymmetricCipherKeyPair keypair)
        {
            //create the key agreement
            ECDHBasicAgreement ag = new ECDHBasicAgreement();
            ag.Init(keypair.Private);

            //calculate the shared secret key
            BigInteger a = ag.CalculateAgreement(keypair.Public);
            byte[] secret = a.ToByteArray();

            //derive the symmetric encryption key
            ECDHKekGenerator topkek = new ECDHKekGenerator(DigestUtilities.GetDigest("SHA256"));
            topkek.Init(new DHKdfParameters(NistObjectIdentifiers.Aes, secret.Length, secret));
            byte[] symKey = new byte[DigestUtilities.GetDigest("SHA256").GetDigestSize()];
            topkek.GenerateBytes(symKey, 0, symKey.Length);

            //encrypt the data
            KeyParameter parm = ParameterUtilities.CreateKeyParameter("DES", symKey);
            IBufferedCipher cipher = CipherUtilities.GetCipher("DES/ECB/ISO7816_4PADDING");
            cipher.Init(true, parm);
            byte[] ret = null;
            try
            {
                ret = cipher.DoFinal(data);
            }
            catch (Exception e)
            {
                if (e != null)
                    return null;
            }

            //erase the keys
            Eraser.SecureErase(secret);
            Eraser.SecureErase(symKey);

            return ret;
        }

        public static byte[] AsymmetricDecrypt(byte[] data, ref AsymmetricCipherKeyPair keypair)
        {
            //create the key agreement
            ECDHBasicAgreement ag = new ECDHBasicAgreement();
            ag.Init(keypair.Private);

            //calculate the shared secret key
            BigInteger a = ag.CalculateAgreement(keypair.Public);
            byte[] secret = a.ToByteArray();

            //derive the symmetric encryption key
            ECDHKekGenerator topkek = new ECDHKekGenerator(DigestUtilities.GetDigest("SHA256"));
            topkek.Init(new DHKdfParameters(NistObjectIdentifiers.Aes, secret.Length, secret));
            byte[] symKey = new byte[DigestUtilities.GetDigest("SHA256").GetDigestSize()];
            topkek.GenerateBytes(symKey, 0, symKey.Length);

            //decrypt the data
            KeyParameter parm = ParameterUtilities.CreateKeyParameter("DES", symKey);
            IBufferedCipher cipher = CipherUtilities.GetCipher("DES/ECB/ISO7816_4PADDING");
            cipher.Init(false, parm);
            byte[] ret = null;
            try
            {
                ret = cipher.DoFinal(data);
            }
            catch (Exception e)
            {
                if (e != null)
                    return null;
            }

            //erase the keys
            Eraser.SecureErase(secret);
            Eraser.SecureErase(symKey);

            return ret;
        }

        public static string HashStringStr(string str)
        {
            byte[] raw = HashString(str);
            string ret = Hex.ToHexString(raw);
            Eraser.SecureErase(raw);
            return ret;
        }

        public static byte[] HashString(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            Sha256Digest digest = new Sha256Digest();
            digest.BlockUpdate(bytes, 0, bytes.Length);
            byte[] hashedBytes = new byte[digest.GetDigestSize()];
            digest.DoFinal(hashedBytes, 0);
            Eraser.SecureErase(bytes);

            return hashedBytes;
        }

        public static void StartGenerateKeypair(byte[] seed, GenCompleteEventHandler evt)
        {
            Thread genThread = new Thread(new ParameterizedThreadStart((o) =>
            {
                //create a random number generator using SHA512 algorithm
                SecureRandom rnd = SecureRandom.GetInstance(RANDOM_ALGO);
                rnd.SetSeed(seed);

                X9ECParameters curve = TeleTrusTNamedCurves.GetByName(ECDSA_CURVE);
                ECDomainParameters domain = new ECDomainParameters(curve.Curve, curve.G, curve.N);

                //create the parameters for initializing the key pair generator using the brainpoolp384t1 curve
                ECKeyGenerationParameters parms = new ECKeyGenerationParameters(domain, rnd);

                //create and initialize the key pair generator
                ECKeyPairGenerator gen = new ECKeyPairGenerator("ECDSA");
                gen.Init(parms);

                //generate the key pair
                AsymmetricCipherKeyPair kp = gen.GenerateKeyPair();

                //trigger the event
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    if (evt != null)
                        evt(kp);
                });
            }));
            genThread.Start();
        }

        public static byte[] EncryptKey(byte[] key, string password)
        {
            //hash the password using SHA256
            byte[] hashedPassword = HashString(password);

            return EncryptKey(key, hashedPassword);
        }

        public static byte[] EncryptKey(byte[] key, byte[] password)
        {
            //append a magic key to the key so we know if it was decrypted sucessfully
            byte[] concatenatedKey = (new byte[] { 39, 39, 39 }).Concat(key).ToArray();

            //create the cipher and parameter for the generator
            PaddedBufferedBlockCipher pbbc = new PaddedBufferedBlockCipher(new AesEngine());
            KeyParameter aesKey = new KeyParameter(password);
            pbbc.Init(true, aesKey);

            //create the output buffer
            byte[] encryptedKey = new byte[pbbc.GetOutputSize(concatenatedKey.Length)];
            //do the encryption
            int bytesWritten = pbbc.ProcessBytes(concatenatedKey, 0, concatenatedKey.Length, encryptedKey, 0);
            pbbc.DoFinal(encryptedKey, bytesWritten);

            //erase the unencrypted data
            Eraser.SecureErase(key);
            Eraser.SecureErase(password);
            Eraser.SecureErase(concatenatedKey);

            return encryptedKey;
        }

        public static byte[] DecryptKey(byte[] encKey, string password)
        {
            //hash the password using SHA256
            byte[] hashedPassword = HashString(password);

            return DecryptKey(encKey, hashedPassword);
        }

        public static byte[] DecryptKey(byte[] encKey, byte[] password)
        {
            //create the cipher and parameter for setting up the generator
            PaddedBufferedBlockCipher pbbc = new PaddedBufferedBlockCipher(new AesEngine());
            KeyParameter aesKey = new KeyParameter(password);
            pbbc.Init(false, aesKey);

            //create the output buffer
            byte[] decryptedKey = new byte[pbbc.GetOutputSize(encKey.Length)];
            //do the decryption
            int bytesWritten = pbbc.ProcessBytes(encKey, decryptedKey, 0);
            try
            {
                bytesWritten += pbbc.DoFinal(decryptedKey, bytesWritten);
            }
            catch (Exception e)
            { //any exception that occurs here means that the password was entered incorrectly
                if (e != null)
                    return null;
            }

            //resize the array to fit the actual size of the decrypted data
            byte[] resizedArray = new byte[bytesWritten];
            Array.Copy(decryptedKey, resizedArray, bytesWritten);
            Eraser.SecureErase(decryptedKey); //done with this, so free it

            //check for the magic key - if not present then the password was incorrect
            if (resizedArray[0] != 39 || resizedArray[1] != 39 || resizedArray[2] != 39)
                return null;

            //strip out the magic key
            byte[] finalKeyReversed = new byte[bytesWritten - 3];
            Array.Copy(resizedArray.Reverse().ToArray(), finalKeyReversed, bytesWritten - 3);
            Eraser.SecureErase(resizedArray); //done with this, so free it

            byte[] finalKey = new byte[bytesWritten - 3];
            Array.Copy(finalKeyReversed.Reverse().ToArray(), finalKey, bytesWritten - 3);
            Eraser.SecureErase(finalKeyReversed); //done with this, so free it

            return finalKey;
        }

        public static string GetPem(byte[] key, bool priv)
        {
            System.IO.StringWriter sw = new System.IO.StringWriter();
            PemWriter pWrt = new PemWriter(sw);

            PemObject o;
            
            if (priv)
                o = new PemObject("LOCKNOTE PRIVATE KEY", key);
            else
                o = new PemObject("LOCKNOTE PUBLIC KEY", key);

            pWrt.WriteObject(o);
            pWrt.Writer.Close();

            string pem = sw.GetStringBuilder().ToString();

            return pem;
        }
    }
}
