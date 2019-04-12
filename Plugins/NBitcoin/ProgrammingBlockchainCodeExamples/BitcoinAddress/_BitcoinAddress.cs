using NBitcoin;

namespace ProgrammingBlockchainCodeExamples
{
	public class _BitcoinAddress : UnityEngine.MonoBehaviour
	{
		void Start()
		{
			Key privateKey		= new Key();								// generate a random private key

			PubKey publicKey	= privateKey.PubKey;
			UnityEngine.Debug.Log(publicKey);								// 0251036303164f6c458e9f7abecb4e55e5ce9ec2b2f1d06d633c9653a07976560c

			UnityEngine.Debug.Log(publicKey.GetAddress(Network.Main));		// 1PUYsjwfNmX64wS368ZR5FMouTtUmvtmTY
			UnityEngine.Debug.Log(publicKey.GetAddress(Network.TestNet));	// n3zWAo2eBnxLr3ueohXnuAa8mTVBhxmPhq

			var publicKeyHash	= publicKey.Hash;
			UnityEngine.Debug.Log(publicKeyHash);							// f6889b21b5540353a29ed18c45ea0031280c42cf
			var mainNetAddress	= publicKeyHash.GetAddress(Network.Main);
			var testNetAddress	= publicKeyHash.GetAddress(Network.TestNet);

			UnityEngine.Debug.Log(mainNetAddress);							// 1PUYsjwfNmX64wS368ZR5FMouTtUmvtmTY
			UnityEngine.Debug.Log(testNetAddress);							// n3zWAo2eBnxLr3ueohXnuAa8mTVBhxmPhq
		}
	}
}