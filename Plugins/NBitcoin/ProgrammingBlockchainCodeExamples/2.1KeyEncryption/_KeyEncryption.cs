using NBitcoin;

namespace ProgrammingBlockchainCodeExamples
{
	public class _KeyEncryption : UnityEngine.MonoBehaviour
	{
		void Start()
		{
			var privateKey			= new Key();
			var bitcoinPrivateKey	= privateKey.GetWif(Network.Main);
			UnityEngine.Debug.Log(bitcoinPrivateKey);			// L1tZPQt7HHj5V49YtYAMSbAmwN9zRjajgXQt9gGtXhNZbcwbZk2r
			BitcoinEncryptedSecret encryptedBitcoinPrivateKey = bitcoinPrivateKey.Encrypt("password");
			UnityEngine.Debug.Log(encryptedBitcoinPrivateKey);	// 6PYKYQQgx947Be41aHGypBhK6TA5Xhi9TdPBkatV3fHbbKrdDoBoXFCyLK
			var decryptedBitcoinPrivateKey = encryptedBitcoinPrivateKey.GetSecret("password");
			UnityEngine.Debug.Log(decryptedBitcoinPrivateKey);	// L1tZPQt7HHj5V49YtYAMSbAmwN9zRjajgXQt9gGtXhNZbcwbZk2r
		}
	}
}