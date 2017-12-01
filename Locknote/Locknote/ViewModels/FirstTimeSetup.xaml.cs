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

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Locknote.Helpers;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1.X509;
using System.IO;

namespace Locknote.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FirstTimeSetup : ContentPage
	{
        public event EventHandler OnSetupComplete;

        private byte[] m_seed;
        private AsymmetricCipherKeyPair m_keypair;

		public FirstTimeSetup()
		{
			InitializeComponent ();

            //create the password entry view
            PasswordEntryView pep = new PasswordEntryView();
            pep.OnSave += new EventHandler((o, e) =>
            {
                //get the public key ASN1
                SubjectPublicKeyInfo pubki = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(m_keypair.Public);
                //get the private key ASN1
                PrivateKeyInfo privki = PrivateKeyInfoFactory.CreatePrivateKeyInfo(m_keypair.Private);

                //encrypt the private key
                byte[] encPrivKey = Crypto.EncryptKey(privki.GetDerEncoded(), pep.Text);
                m_keypair = null;

                //delete the old notebooks
                if (Directory.Exists(NoteManager.GetNotebookDir()))
                    Directory.Delete(NoteManager.GetNotebookDir(), true);

                //save the keys to file
                KeyManager.SaveKeys(encPrivKey, pubki.GetDerEncoded());

                //erase the data
                Eraser.SecureErase(encPrivKey);

                //trigger the setup complete event
                if (OnSetupComplete != null)
                    OnSetupComplete(this, new EventArgs());
            });

            //create the activity indicator layout
            StackLayout actLayout = new StackLayout() { VerticalOptions=LayoutOptions.CenterAndExpand };
            ActivityIndicator actInd = new ActivityIndicator(); ;
            actLayout.Children.Add(actInd);
            actLayout.Children.Add(new Label() { Text = "Generating key pair", TextColor=Color.DarkGray, HorizontalTextAlignment=TextAlignment.Center });

            TapRandomizer tapRnd = new TapRandomizer();
            tapRnd.OnRandomized += new EventHandler((o, e) =>
            {
                m_seed = tapRnd.Seed;
                //show wait animation
                actInd.IsRunning = true;
                this.Content = actLayout;
                //generate the key pair
                Crypto.StartGenerateKeypair(m_seed, new Crypto.GenCompleteEventHandler((keypair) =>
                {
                    m_keypair = keypair;
                    //hide wait animation
                    actInd.IsRunning = false;
                    //show the password entry page
                    this.Content = pep;
                }));
            });

            this.Content = tapRnd;
		}
	}
}