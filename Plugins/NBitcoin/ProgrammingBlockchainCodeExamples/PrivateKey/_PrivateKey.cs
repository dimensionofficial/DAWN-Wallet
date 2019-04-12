using NBitcoin;

namespace ProgrammingBlockchainCodeExamples
{
	public class _PrivateKey : UnityEngine.MonoBehaviour
	{
		void Start()
		{
			Key privateKey						= new Key();									// generate a random private key
			BitcoinSecret mainNetPrivateKey		= privateKey.GetBitcoinSecret(Network.Main);	// get our private key for the mainnet
			BitcoinSecret testNetPrivateKey		= privateKey.GetBitcoinSecret(Network.TestNet);	// get our private key for the testnet
			UnityEngine.Debug.Log(mainNetPrivateKey);											// L5B67zvrndS5c71EjkrTJZ99UaoVbMUAK58GKdQUfYCpAa6jypvn
			UnityEngine.Debug.Log(testNetPrivateKey);											// cVY5auviDh8LmYUW8AfafseD6p6uFoZrP7GjS3rzAerpRKE9Wmuz

			bool WifIsBitcoinSecret				= mainNetPrivateKey == privateKey.GetWif(Network.Main);
			UnityEngine.Debug.Log(WifIsBitcoinSecret);											// True
			            
			BitcoinSecret bitcoinPrivateKey		= privateKey.GetWif(Network.Main);				// L5B67zvrndS5c71EjkrTJZ99UaoVbMUAK58GKdQUfYCpAa6jypvn
			Key samePrivateKey					= bitcoinPrivateKey.PrivateKey;

			PubKey publicKey					= privateKey.PubKey;
			BitcoinPubKeyAddress bitcoinPubicKey= publicKey.GetAddress(Network.Main);			// 1PUYsjwfNmX64wS368ZR5FMouTtUmvtmTY
			//PubKey samePublicKey = bitcoinPubicKey.ItIsNotPossible;
		}
	}
}
