using NBitcoin;

namespace ProgrammingBlockchainCodeExamples
{
	public class _Balances : UnityEngine.MonoBehaviour
	{
		void Start()
		{
			BitcoinSecret bitcoinPrivateKey = new BitcoinSecret("cUwC2Dk7VvVyxF3jGyHdz5HTtxHYqHuQgWX1pnYvqckwCyUGStd3");
            BitcoinPubKeyAddress bpa = new BitcoinPubKeyAddress("18rnfoQgGo1HqvVQaAN4QnxjYE7Sez9eca");
            QBitNinja4Unity.QBitNinjaClient.GetBalance(bpa, Network.Main, GetBalanceResponse);
		}

		void GetBalanceResponse(QBitNinja.Client.Models.BalanceModel result, Network network)
		{
			//UnityEngine.Debug.Log(result.Continuation);
		}
	}
}