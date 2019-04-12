using System.Text;
using System;
using NBitcoin;

namespace ProgrammingBlockchainCodeExamples
{
	public class _SpendYourCoins : UnityEngine.MonoBehaviour
	{
		BitcoinSecret	bitcoinPrivateKey;
		Transaction		transaction;

		void Start()
		{
			#region CREATE NEW PRIVKEY
			//var network = Network.TestNet;
			//Key privateKey = new Key();
			//var bitcoinPrivateKey = privateKey.GetWif(network);
			#endregion

			#region IMPORT PRIVKEY
			bitcoinPrivateKey = new BitcoinSecret("cUwC2Dk7VvVyxF3jGyHdz5HTtxHYqHuQgWX1pnYvqckwCyUGStd3");
			var network = bitcoinPrivateKey.Network;
			#endregion

			UnityEngine.Debug.Log(network);             // n3x4Q89kqkHSV5d33LaRbnnoDdTuvDjY5D

            var address = bitcoinPrivateKey.GetAddress();

			UnityEngine.Debug.Log(bitcoinPrivateKey);   // cUwC2Dk7VvVyxF3jGyHdz5HTtxHYqHuQgWX1pnYvqckwCyUGStd3
			UnityEngine.Debug.Log(address);             // n3x4Q89kqkHSV5d33LaRbnnoDdTuvDjY5D

            var transactionId = uint256.Parse("13cb292d07883f6a87b2db52c807e411e6330c9d7756535a7169a3ced8fe4385");
            UnityEngine.Debug.Log("transactionId = " + transactionId);
            QBitNinja4Unity.QBitNinjaClient.GetTransaction(transactionId, network, TransactionResponse);
		}

		void TransactionResponse(QBitNinja.Client.Models.GetTransactionResponse transactionResponse, Network network)
		{
			UnityEngine.Debug.Log(transactionResponse.TransactionId); // 13cb292d07883f6a87b2db52c807e411e6330c9d7756535a7169a3ced8fe4385
            UnityEngine.Debug.Log(transactionResponse.Block.Confirmations);

            var receivedCoins = transactionResponse.ReceivedCoins;
            OutPoint outPointToSpend = null;
            foreach (var coin in receivedCoins)
            {
                if (coin.TxOut.ScriptPubKey == bitcoinPrivateKey.ScriptPubKey)
                {
                    outPointToSpend = coin.Outpoint;
                }
            }
            if(outPointToSpend == null)
                throw new Exception("TxOut doesn't contain our ScriptPubKey");
			UnityEngine.Debug.Log(string.Format("We want to spend {0}. outpoint:", outPointToSpend.N + 1));

            transaction = new Transaction();
			transaction.Inputs.Add(new TxIn()
			{
				PrevOut = outPointToSpend
			});

            //var hallOfTheMakersAddress = new BitcoinPubKeyAddress("1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB");
            //var hallOfTheMakersAddress = new BitcoinPubKeyAddress("mwCwTceJvYV27KXBc3NJZys6CjsgsoeHmf");
            var hallOfTheMakersAddress = new BitcoinPubKeyAddress("mpXxQe9sPbHHqgvmhViLUEnc96Kp5hgCvT");

             // How much you want to TO
             var hallOfTheMakersAmount = new Money((decimal)0.5, MoneyUnit.BTC);
			/* At the time of writing the mining fee is 0.05usd
			 * Depending on the market price and
			 * On the currently advised mining fee,
			 * You may consider to increase or decrease it
			*/
			var minerFee = new Money((decimal)0.0001, MoneyUnit.BTC);
            // How much you want to spend FROM
            var txInAmount = (Money)receivedCoins[(int)outPointToSpend.N].Amount;
            UnityEngine.Debug.Log(txInAmount);
            Money changeBackAmount = txInAmount - hallOfTheMakersAmount - minerFee;
            

            TxOut hallOfTheMakersTxOut = new TxOut()
			{
				Value = hallOfTheMakersAmount,
				ScriptPubKey = hallOfTheMakersAddress.ScriptPubKey
			};

			TxOut changeBackTxOut = new TxOut()
			{
				Value = changeBackAmount,
				ScriptPubKey = bitcoinPrivateKey.ScriptPubKey
			};

			transaction.Outputs.Add(hallOfTheMakersTxOut);
			transaction.Outputs.Add(changeBackTxOut);

			var message = "nopara73 loves NBitcoin!";
			var bytes = Encoding.UTF8.GetBytes(message);
			transaction.Outputs.Add(new TxOut()
			{
				Value = Money.Zero,
                ScriptPubKey = TxNullDataTemplate.Instance.GenerateScriptPubKey(bytes)

			});

            //UnityEngine.Debug.Log(transaction);

            //var address = new BitcoinPubKeyAddress("mzK6Jy5mer3ABBxfHdcxXEChsn3mkv8qJv");
            //transaction.Inputs[0].ScriptSig = address.ScriptPubKey;

            // It is also OK:
            transaction.Inputs[0].ScriptSig =  bitcoinPrivateKey.ScriptPubKey;
            transaction.Sign(bitcoinPrivateKey, false);

			QBitNinja4Unity.QBitNinjaClient.Broadcast(transaction, network, BroadcastResponse);
		}
		void BroadcastResponse(QBitNinja.Client.Models.BroadcastResponse broadcastResponse, Network network)
		{
            if (!broadcastResponse.Success)
            {
                UnityEngine.Debug.Log(string.Format("ErrorCode: {0}", broadcastResponse.Error.ErrorCode));
                UnityEngine.Debug.Log("Error message: " + broadcastResponse.Error.Reason);
            }
            else
            {
                UnityEngine.Debug.Log("Success! You can check out the hash of the transaciton in any block explorer:");
                UnityEngine.Debug.Log(transaction.GetHash());
            }
			//using (var node = Node.ConnectToLocal(network)) //Connect to the node
            //{
            //    node.VersionHandshake(); //Say hello
            //                             //Advertize your transaction (send just the hash)
            //    node.SendMessage(new InvPayload(InventoryType.MSG_TX, transaction.GetHash()));
            //    //Send it
            //    node.SendMessage(new TxPayload(transaction));
            //    Thread.Sleep(500); //Wait a bit
            //}
		}
	}
}