using NBitcoin;

namespace ProgrammingBlockchainCodeExamples
{
	public class _PaymentScript : UnityEngine.MonoBehaviour
	{
		void Start()
		{
			var publicKeyHash = new KeyId("14836dbe7f38c5ac3d49e8d790af808a4ee9edcf");

			var testNetAddress = publicKeyHash.GetAddress(Network.TestNet);
			var mainNetAddress = publicKeyHash.GetAddress(Network.Main);

			UnityEngine.Debug.Log(mainNetAddress.ScriptPubKey);				// OP_DUP OP_HASH160 14836dbe7f38c5ac3d49e8d790af808a4ee9edcf OP_EQUALVERIFY OP_CHECKSIG
			UnityEngine.Debug.Log(testNetAddress.ScriptPubKey);				// OP_DUP OP_HASH160 14836dbe7f38c5ac3d49e8d790af808a4ee9edcf OP_EQUALVERIFY OP_CHECKSIG

			var paymentScript = publicKeyHash.ScriptPubKey;
			var sameMainNetAddress = paymentScript.GetDestinationAddress(Network.Main);
			UnityEngine.Debug.Log(mainNetAddress == sameMainNetAddress);	// True


			var samePublicKeyHash = (KeyId)paymentScript.GetDestination();
			UnityEngine.Debug.Log(publicKeyHash == samePublicKeyHash);		// True
			var sameMainNetAddress2 = new BitcoinPubKeyAddress(samePublicKeyHash, Network.Main);
			UnityEngine.Debug.Log(mainNetAddress == sameMainNetAddress2);	// True
		}
	}
}
