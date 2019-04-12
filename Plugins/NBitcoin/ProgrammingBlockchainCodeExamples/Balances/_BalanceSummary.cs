using NBitcoin;

namespace ProgrammingBlockchainCodeExamples
{
public class _BalanceSummary : UnityEngine.MonoBehaviour
	{
		void Start()
		{
			//BitcoinSecret bitcoinPrivateKey = new BitcoinSecret("cUwC2Dk7VvVyxF3jGyHdz5HTtxHYqHuQgWX1pnYvqckwCyUGStd3");

            BitcoinPubKeyAddress bpa = new BitcoinPubKeyAddress("14wNVJBq9SeVRfhBPqkP4mVAFmag7F2Q2F");
            //print(bitcoinPrivateKey.GetAddress().ToString());
            //print(bitcoinPrivateKey.Network);
            QBitNinja4Unity.QBitNinjaClient.GetBalanceSummary(bpa, Network.Main, GetBalanceSummaryResponse);
		}

		void GetBalanceSummaryResponse(QBitNinja.Client.Models.BalanceSummary result, NBitcoin.Network network)
		{
			UnityEngine.Debug.Log(result.Spendable.Amount);
            UnityEngine.Debug.Log(result.Spendable.Received);
            UnityEngine.Debug.Log(result.Spendable.TransactionCount);

        }
	}
}