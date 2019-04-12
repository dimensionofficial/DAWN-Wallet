using NBitcoin;

namespace ProgrammingBlockchainCodeExamples
{
	public class _GetBlockResponse : UnityEngine.MonoBehaviour
	{
		void Start()
		{
			var id = new uint256( "0000000000000033f78469696b80110effbc73691d2ce40e2c5ef116e6f48623");
			var blockFeature = new QBitNinja.Client.Models.BlockFeature(id);
			QBitNinja4Unity.QBitNinjaClient.GetBlock(blockFeature, Network.TestNet, GetBlockResponse);
		}

		void GetBlockResponse(QBitNinja.Client.Models.GetBlockResponse result, Network network)
		{
			UnityEngine.Debug.Log(result.AdditionalInformation.BlockId);
			UnityEngine.Debug.Log(result.AdditionalInformation.BlockHeader);
			UnityEngine.Debug.Log(result.AdditionalInformation.Height);
			UnityEngine.Debug.Log(result.AdditionalInformation.Confirmations);
			UnityEngine.Debug.Log(result.ExtendedInformation);
			UnityEngine.Debug.Log(NBitcoin.DataEncoders.Encoders.Hex.EncodeData(result.Block.ToBytes()));
			UnityEngine.Debug.Log(result.Block.GetHash());
		}
	}
}