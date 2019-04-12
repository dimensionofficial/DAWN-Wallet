using NBitcoin;

namespace ProgrammingBlockchainCodeExamples
{
	public class _BitcoinSecret : UnityEngine.MonoBehaviour
	{
		void Start()
		{
			Key privateKey			= new Key();									// generate a random private key
			var mainNetPrivateKey	= privateKey.GetBitcoinSecret(Network.Main); 	// get our private key for the mainnet
			var testNetPrivateKey	= privateKey.GetBitcoinSecret(Network.TestNet);	// get our private key for the testnet
			UnityEngine.Debug.Log(mainNetPrivateKey);								// L5B67zvrndS5c71EjkrTJZ99UaoVbMUAK58GKdQUfYCpAa6jypvn
            UnityEngine.Debug.Log(testNetPrivateKey);								// cVY5auviDh8LmYUW8AfafseD6p6uFoZrP7GjS3rzAerpRKE9Wmuz
		}
	}
}