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
using Android.Support.V4.Hardware.Fingerprint;

using Locknote.Helpers;
using Android.Support.V4.Content;
using Android;
using Javax.Crypto;
using Java.Lang;

using Locknote.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(Fingerprint))]
namespace Locknote.Droid
{
    class Fingerprint : FingerprintManagerCompat.AuthenticationCallback, IFingerprint
    {
        private bool m_ready;
        EventHandler m_evt;
        FingerprintManagerCompat m_fingerprintManager;
        static readonly byte[] SECRET_BYTES = { 3, 9, 6, 4, 5, 63, 1, 1, 7 };
        Android.Support.V4.OS.CancellationSignal cancellationSignal;
        CipherMode m_mode;
        byte[] m_data;

        public Fingerprint()
        {
            m_ready = false;
            cancellationSignal = null;
            m_mode = CipherMode.EncryptMode;
        }

        public void InitReader()
        {
            m_fingerprintManager = FingerprintManagerCompat.From(Application.Context);
            //make sure hardware is available
            if (!m_fingerprintManager.IsHardwareDetected)
                return;
            m_ready = true;
        }

        public bool IsReady()
        {
            return m_ready;
        }

        public void CancelFingerprint()
        {
            if (cancellationSignal != null)
                cancellationSignal.Cancel();
        }

        public void GetFingerprint(EventHandler evt, bool encrypt, byte[] data)
        {
            //make sure the device is secured
            KeyguardManager keyguardManager = (KeyguardManager)Application.Context.GetSystemService(Context.KeyguardService);
            if (!keyguardManager.IsKeyguardSecure)
                return;
            //make sure fingerprints are enrolled
            if (!m_fingerprintManager.HasEnrolledFingerprints)
                return;
            //check permissions
            Android.Content.PM.Permission permissionResult = ContextCompat.CheckSelfPermission(Application.Context, Manifest.Permission.UseFingerprint);
            if (permissionResult != Android.Content.PM.Permission.Granted)
                return;

            m_evt = evt;
            m_data = data;
            m_mode = encrypt ? CipherMode.EncryptMode : CipherMode.DecryptMode;

            CryptoObjectHelper cryptoHelper = new CryptoObjectHelper();
            cancellationSignal = new Android.Support.V4.OS.CancellationSignal();

            //prompt user to scan their fingerprint
            m_fingerprintManager.Authenticate(cryptoHelper.BuildCryptoObject(m_mode), 0, cancellationSignal, this, null);
        }

        public override void OnAuthenticationSucceeded(FingerprintManagerCompat.AuthenticationResult result)
        {
            if (result.CryptoObject.Cipher != null)
            {
                try
                {
                    if (m_mode == CipherMode.EncryptMode)
                    {
                        byte[] encPasswd = result.CryptoObject.Cipher.DoFinal(m_data);
                        m_evt(encPasswd, new EventArgs());
                    }
                    else if (m_mode == CipherMode.DecryptMode)
                    {
                        byte[] decPasswd = result.CryptoObject.Cipher.DoFinal(m_data);
                        m_evt(decPasswd, new EventArgs());
                    }          
                }
                catch (BadPaddingException bpe)
                {
                    // Can't really trust the results.
                }
                catch (IllegalBlockSizeException ibse)
                {
                    // Can't really trust the results.
                }
            }
            else
            {
                // No cipher used
            }
        }
        public override void OnAuthenticationError(int errMsgId, ICharSequence errString)
        {
            // Report the error to the user. Note that if the user canceled the scan,
            // this method will be called and the errMsgId will be FingerprintState.ErrorCanceled.
        }
        public override void OnAuthenticationFailed()
        {
            // Tell the user that the fingerprint was not recognized.
        }
        public override void OnAuthenticationHelp(int helpMsgId, ICharSequence helpString)
        {
            // Notify the user that the scan failed and display the provided hint.
        }
    }
}