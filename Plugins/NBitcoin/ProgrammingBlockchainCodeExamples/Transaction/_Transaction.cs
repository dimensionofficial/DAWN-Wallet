using System.Collections.Generic;
using System.Linq;
using NBitcoin;

namespace ProgrammingBlockchainCodeExamples
{
	public class _Transaction : UnityEngine.MonoBehaviour
	{
		List<ICoin> spentCoins;
		List<ICoin> receivedCoins;

		void Start()
		{
			// Parse transaction id to NBitcoin.uint256 so the client can eat it
			var transactionId = uint256.Parse("f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94");
			// Query the transaction
			QBitNinja4Unity.QBitNinjaClient.GetTransaction(transactionId, Network.Main, TransactionResponse);
		}

		void TransactionResponse(QBitNinja.Client.Models.GetTransactionResponse transactionResponse, Network network)
		{
			Transaction transaction = transactionResponse.Transaction;

			UnityEngine.Debug.Log(transactionResponse.TransactionId); // f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94
			UnityEngine.Debug.Log(transaction.GetHash()); // f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94

			// RECEIVED COINS
			receivedCoins = transactionResponse.ReceivedCoins;
			foreach (Coin coin in receivedCoins)
			{
				Money amount = coin.Amount;

				UnityEngine.Debug.Log(amount.ToDecimal(MoneyUnit.BTC));
				var paymentScript = coin.ScriptPubKey;
				UnityEngine.Debug.Log(paymentScript);  // It's the ScriptPubKey
				var address = paymentScript.GetDestinationAddress(network);
				UnityEngine.Debug.Log(address);
			}

			// RECEIVED COINS
			var outputs = transaction.Outputs;
			foreach (TxOut output in outputs)
			{
				Coin coin = new Coin(transaction, output);
				Money amount = coin.Amount;

				UnityEngine.Debug.Log(amount.ToDecimal(MoneyUnit.BTC));
				var paymentScript = coin.GetScriptCode();
				UnityEngine.Debug.Log(paymentScript);  // It's the ScriptPubKey
				var address = paymentScript.GetDestinationAddress(network);
				UnityEngine.Debug.Log(address);
			}

			// RECEIVED COINS
			foreach (TxOut output in outputs)
			{
				Money amount = output.Value;

				UnityEngine.Debug.Log(amount.ToDecimal(MoneyUnit.BTC));
				var paymentScript = output.ScriptPubKey;
				UnityEngine.Debug.Log(paymentScript);  // It's the ScriptPubKey
				var address = paymentScript.GetDestinationAddress(network);
				UnityEngine.Debug.Log(address);
			}

			// SPENT COINS
			spentCoins = transactionResponse.SpentCoins;
			foreach (Coin coin in spentCoins)
			{
				Money amount = coin.Amount;

				UnityEngine.Debug.Log(amount.ToDecimal(MoneyUnit.BTC));
				var paymentScript = coin.ScriptPubKey;
				UnityEngine.Debug.Log(paymentScript);  // It's the ScriptPubKey
				var address = paymentScript.GetDestinationAddress(network);
				UnityEngine.Debug.Log(address);
			}

			// SPENT COINS
			foreach (Coin coin in spentCoins)
			{
				TxOut previousOutput = coin.TxOut;
				Money amount = previousOutput.Value;

				UnityEngine.Debug.Log(amount.ToDecimal(MoneyUnit.BTC));
				var paymentScript = previousOutput.ScriptPubKey;
				UnityEngine.Debug.Log(paymentScript);  // It's the ScriptPubKey
				var address = paymentScript.GetDestinationAddress(network);
				UnityEngine.Debug.Log(address);
			}


			var fee = transaction.GetFee(spentCoins.ToArray());
			UnityEngine.Debug.Log(fee);

			var inputs = transaction.Inputs;
			foreach (TxIn input in inputs)
			{
				OutPoint previousOutpoint = input.PrevOut;
				UnityEngine.Debug.Log(previousOutpoint.Hash); // hash of prev tx
				UnityEngine.Debug.Log(previousOutpoint.N); // idx of out from prev tx, that has been spent in the current tx
			}

			// Let's create a txout with 21 bitcoin from the first ScriptPubKey in our current transaction
			Money twentyOneBtc = new Money(21, MoneyUnit.BTC);
			var scriptPubKey = transaction.Outputs.First().ScriptPubKey;
			TxOut txOut = new TxOut(twentyOneBtc, scriptPubKey);

			OutPoint firstOutPoint = spentCoins.First().Outpoint;
			UnityEngine.Debug.Log(firstOutPoint.Hash); // 4788c5ef8ffd0463422bcafdfab240f5bf0be690482ceccde79c51cfce209edd
			UnityEngine.Debug.Log(firstOutPoint.N); // 0

			UnityEngine.Debug.Log(transaction.Inputs.Count); // 9

			OutPoint firstPreviousOutPoint = transaction.Inputs.First().PrevOut;


			QBitNinja4Unity.QBitNinjaClient.GetTransaction(firstPreviousOutPoint.Hash, network, FirstPreviousTransactionResponse);
		}

		void FirstPreviousTransactionResponse(QBitNinja.Client.Models.GetTransactionResponse firstPreviousTransactionResponse, Network network)
		{
			UnityEngine.Debug.Log(firstPreviousTransactionResponse.IsCoinbase); // False
            Transaction firstPreviousTransaction = firstPreviousTransactionResponse.Transaction;

			//while (firstPreviousTransactionResponse.IsCoinbase == false)
			//{
			//    UnityEngine.Debug.Log(firstPreviousTransaction.GetHash());

			//    firstPreviousOutPoint = firstPreviousTransaction.Inputs.First().PrevOut;
			//    firstPreviousTransactionResponse = client.GetTransaction(firstPreviousOutPoint.Hash).Result;
			//    firstPreviousTransaction = firstPreviousTransactionResponse.Transaction;
			//}

			Money spentAmount = Money.Zero;
            foreach (var spentCoin in spentCoins)
            {
                spentAmount = (Money)spentCoin.Amount.Add(spentAmount);
            }
            UnityEngine.Debug.Log(spentAmount.ToDecimal(MoneyUnit.BTC)); // 13.19703492

            Money receivedAmount = Money.Zero;
            foreach (var receivedCoin in receivedCoins)
            {
                receivedAmount = (Money)receivedCoin.Amount.Add(receivedAmount);
            }
            UnityEngine.Debug.Log(receivedAmount.ToDecimal(MoneyUnit.BTC)); // 13.19683492

            UnityEngine.Debug.Log((spentAmount - receivedAmount).ToDecimal(MoneyUnit.BTC));

            UnityEngine.Debug.Log(spentAmount.ToDecimal(MoneyUnit.BTC)-receivedAmount.ToDecimal(MoneyUnit.BTC));

            //var inputs = transaction.Inputs;
            //foreach (TxIn input in inputs)
            //{
            //    uint256 previousTransactionId = input.PrevOut.Hash;
            //    GetTransactionResponse previousTransactionResponse = client.GetTransaction(previousTransactionId).Result;

            //    NBitcoin.Transaction previousTransaction = previousTransactionResponse.Transaction;

            //    var previousTransactionOutputs = previousTransaction.Outputs;
            //    foreach (TxOut previousTransactionOutput in previousTransactionOutputs)
            //    {
            //        Money amount = previousTransactionOutput.Value;

            //        UnityEngine.Debug.Log(amount.ToDecimal(MoneyUnit.BTC));
            //        var paymentScript = previousTransactionOutput.ScriptPubKey;
            //        UnityEngine.Debug.Log(paymentScript);  // It's the ScriptPubKey
            //        var address = paymentScript.GetDestinationAddress(network);
            //        UnityEngine.Debug.Log(address);
            //        UnityEngine.Debug.Log();
            //    }
            //}
		}
	}
}
