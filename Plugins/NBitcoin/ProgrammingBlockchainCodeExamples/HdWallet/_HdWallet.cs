using NBitcoin;

namespace ProgrammingBlockchainCodeExamples
{
	public class _HdWallet : UnityEngine.MonoBehaviour
	{
        void Init()
        {
            Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            ExtKey hdRoot = mnemo.DeriveExtKey();

            ExtKey btc = hdRoot.Derive(new KeyPath("m/44'/0'/0'/1/1"));
            UnityEngine.Debug.Log("btc Key  : " + btc.ScriptPubKey.ToBytes());

            ExtKey eth = hdRoot.Derive(new KeyPath("m/44'/60'/0'/2/1"));
            UnityEngine.Debug.Log("eth Key  : " + eth.ScriptPubKey.GetDestinationAddress(Network.Main));


            mnemo = new Mnemonic("minute put grant neglect anxiety case globe win famous correct turn link", Wordlist.English);
            hdRoot = mnemo.DeriveExtKey("my password");
            UnityEngine.Debug.Log("hdRoot  : " + hdRoot);

        }

		void Start()
		{
            Init();
            return;
			ExtKey masterKey	= new ExtKey();
			UnityEngine.Debug.Log("Master key : " + masterKey.ToString(Network.Main));
			for (int i = 0; i< 5; i++)
			{
			    ExtKey key = masterKey.Derive((uint)i);
				UnityEngine.Debug.Log("Key " + i + " : " + key.ToString(Network.Main));
			}

			ExtPubKey masterPubKey = masterKey.Neuter();
			for (int i = 0; i< 5; i++)
			{
			    ExtPubKey pubkey = masterPubKey.Derive((uint)i);
				UnityEngine.Debug.Log("PubKey " + i + " : " + pubkey.ToString(Network.Main));
			}

			masterKey			= new ExtKey();
			masterPubKey		= masterKey.Neuter();

			//The payment server generate pubkey1
			ExtPubKey pubkey1	= masterPubKey.Derive((uint)1);

			//You get the private key of pubkey1
			ExtKey key1			= masterKey.Derive((uint)1);

			//Check it is legit
			UnityEngine.Debug.Log("Generated address : " + pubkey1.PubKey.GetAddress(Network.Main));
			UnityEngine.Debug.Log("Expected address : " + key1.PrivateKey.PubKey.GetAddress(Network.Main));

			ExtKey parent		= new ExtKey();
			ExtKey child11		= parent.Derive(1).Derive(1);

			// OR
			parent				= new ExtKey();
			child11				= parent.Derive(new KeyPath("1/1"));
            
            

			ExtKey ceoKey		= new ExtKey();
			UnityEngine.Debug.Log("CEO: " + ceoKey.ToString(Network.Main));
			ExtKey accountingKey = ceoKey.Derive(0, hardened: true);

			ExtPubKey ceoPubkey	= ceoKey.Neuter();

			//ExtKey ceoKeyRecovered = accountingKey.GetParentExtKey(ceoPubkey); //Crash

			var nonHardened		= new KeyPath("1/2/3");
			var hardened		= new KeyPath("1/2/3'");

			ceoKey				= new ExtKey();
			string accounting	= "1'";
			int customerId		= 5;
			int paymentId		= 50;
			KeyPath path		= new KeyPath(accounting + "/" + customerId + "/" + paymentId);
			//Path : "1'/5/50"
			ExtKey paymentKey	= ceoKey.Derive(path);

			Mnemonic mnemo		= new Mnemonic(Wordlist.English, WordCount.Twelve);
			ExtKey hdRoot		= mnemo.DeriveExtKey("my password");
			UnityEngine.Debug.Log(mnemo);

			mnemo				= new Mnemonic("minute put grant neglect anxiety case globe win famous correct turn link", Wordlist.English);

            
            
            hdRoot				= mnemo.DeriveExtKey("my password");
		}
	}
}
