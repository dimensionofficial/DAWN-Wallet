using NBitcoin;

namespace ProgrammingBlockchainCodeExamples
{
	public class _BlockHeaderResponse : UnityEngine.MonoBehaviour
	{
		void Start()
		{
			var id = new uint256("0000000000000033f78469696b80110effbc73691d2ce40e2c5ef116e6f48623");
			var blockFeature = new QBitNinja.Client.Models.BlockFeature(id);
			QBitNinja4Unity.QBitNinjaClient.BlockHeader(blockFeature, Network.TestNet, BlockHeaderResponse);
		}

		void BlockHeaderResponse(QBitNinja.Client.Models.WhatIsBlockHeader result, Network network)
		{
			UnityEngine.Debug.Log(result.Hash);
			UnityEngine.Debug.Log(result.Previous);
			UnityEngine.Debug.Log(result.Time);
			UnityEngine.Debug.Log(result.Nonce);
			UnityEngine.Debug.Log(result.HashMerkelRoot);
			UnityEngine.Debug.Log(result.Version);
			UnityEngine.Debug.Log(result.Bits);
			UnityEngine.Debug.Log(result.Difficulty);
		}
	}
}