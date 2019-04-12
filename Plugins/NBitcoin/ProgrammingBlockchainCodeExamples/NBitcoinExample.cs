using NBitcoin;
using UnityEngine;

// based on
// https://github.com/MetacoSA/NBitcoin
// https://blockchainprogramming.azurewebsites.net/

public class NBitcoinExample : MonoBehaviour {
	void Awake()
	{
		//Sample_01_BitcoinAddress();
		//Sample_02_ScriptPublicKey();
		//Sample_03_PrivateKey();
		//Sample_04_Transaction();

		Sample_05_SpendYourCoin();
	}
	//--------------------------------------------------------------------------------
	void Sample_01_BitcoinAddress()
	{
		Key				privateKey		= new Key();			// generate random private key
		PubKey			publicKey		= privateKey.PubKey;	// generate public key from private key
		KeyId			publicKeyHash	= publicKey.Hash;		// generate public key hash

		Debug.Log(publicKey + " <-------------- publicKey");
		Debug.Log(publicKeyHash + " <-------------- publicKeyHash");

		Debug.Log(publicKey.GetAddress(NBitcoin.Network.TestNet) + " <-------------- testNetAdress from public Key");
		Debug.Log(publicKeyHash.GetAddress(NBitcoin.Network.TestNet) + " <-------------- testNetAdress from public Key Hash");
	}
	void Sample_02_ScriptPublicKey()
	{
		// case 1
		KeyId			publicKeyHash	= new KeyId("14836dbe7f38c5ac3d49e8d790af808a4ee9edcf");
		BitcoinAddress	bitcoinAddress	= publicKeyHash.GetAddress(NBitcoin.Network.TestNet);

		Debug.Log(bitcoinAddress.ScriptPubKey + " <-------------- Bitcoin Address from public Key Hash");

		Script			paymentScript	= publicKeyHash.ScriptPubKey;
		BitcoinAddress	destination		= paymentScript.GetDestinationAddress(NBitcoin.Network.TestNet);

		Debug.Log(destination.ScriptPubKey + " <-------------- Bitcoin Address from public Key Hash");

		// case 2
		KeyId sameKeyHash				= (KeyId)paymentScript.GetDestination();
		BitcoinAddress sameAdress		= new BitcoinPubKeyAddress(sameKeyHash,NBitcoin.Network.TestNet);
		Debug.Log(sameAdress);

		Debug.Log(publicKeyHash);
		Debug.Log(sameKeyHash);
	}
	void Sample_03_PrivateKey()
	{
		Key				privateKey		= new Key();						// generate random private key
		BitcoinSecret	bitcoinSecret	= privateKey.GetBitcoinSecret(NBitcoin.Network.TestNet);

		Debug.Log(privateKey.PubKey.Hash);
		Debug.Log(bitcoinSecret.PubKey.Hash);

		BitcoinPubKeyAddress bitcoinPubKey = bitcoinSecret.PubKey.GetAddress(NBitcoin.Network.TestNet);
		Debug.Log(bitcoinPubKey.Hash);
	}
	//--------------------------------------------------------------------------------
	QBitNinja.Client.Models.GetTransactionResponse transactionResponse_04_01;
	void Sample_04_Transaction()
	{
		// http://api.qbit.ninja/transactions/f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94
		var transactionID	= uint256.Parse("f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94");
		QBitNinja4Unity.QBitNinjaClient.GetTransaction(transactionID, NBitcoin.Network.Main, Sample_04_01);
	}
	void Sample_04_01(QBitNinja.Client.Models.GetTransactionResponse transactionResponse, NBitcoin.Network network)
	{
		/*
		Debug.Log("----------------------------------------------------------------");

		Debug.Log("transactionId : " + transactionResponse.TransactionId);    // "f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94"
		Debug.Log("transactionId : " + transactionResponse.Transaction.GetHash());

		Debug.Log("----------------------------------------------------------------");
		var recivedCoins		= transactionResponse.ReceivedCoins;

		Debug.Log("RecivedCoins");
		foreach (var coin in recivedCoins)
		{
			Script			paymentScript		= coin.TxOut.ScriptPubKey;
			BitcoinAddress	destination			= paymentScript.GetDestinationAddress(network);
			Money			amount				= (Money)coin.Amount;

			Debug.Log(paymentScript);
			Debug.Log(destination);
			Debug.Log(amount.ToDecimal(MoneyUnit.BTC));
		}

		Debug.Log("----------------------------------------------------------------");
		Debug.Log("Out");
		TxOutList outPutList	= transactionResponse.Transaction.Outputs;
		foreach (TxOut output in outPutList)
		{
			Script			paymentScript		= output.ScriptPubKey;
			BitcoinAddress	destination			= paymentScript.GetDestinationAddress(network);
			Money			amount				= output.Value;

			Debug.Log(paymentScript);
			Debug.Log(destination);
			Debug.Log(amount.ToDecimal(MoneyUnit.BTC));
		}

		Debug.Log("----------------------------------------------------------------");
		Debug.Log("In");
		TxInList inList = transactionResponse.Transaction.Inputs;
		foreach (TxIn input in inList)
		{
			OutPoint		previousOutPoint	= input.PrevOut;
			Debug.Log(previousOutPoint.Hash);
			Debug.Log(previousOutPoint.N);
		}

		Debug.Log("----------------------------------------------------------------");
		*/

		transactionResponse_04_01		= transactionResponse;

		Money		twentyOneBTC		= new Money(21, MoneyUnit.BTC);
		var			scriptPubKey		= transactionResponse.Transaction.Outputs[0].ScriptPubKey;
		TxOut		txOut				= new TxOut(twentyOneBTC, scriptPubKey);

		OutPoint	firstOutPoint		= transactionResponse.ReceivedCoins[0].Outpoint;
		Debug.Log(firstOutPoint.Hash);
		Debug.Log(firstOutPoint.N);

		Debug.Log(transactionResponse.Transaction.Inputs.Count);

		OutPoint	firstPreviousPoint	= transactionResponse.Transaction.Inputs[0].PrevOut;
		QBitNinja4Unity.QBitNinjaClient.GetTransaction(firstPreviousPoint.Hash, network, Sample_04_02);
	}
	void Sample_04_02(QBitNinja.Client.Models.GetTransactionResponse transactionResponse, NBitcoin.Network network)
	{
		Debug.Log(transactionResponse.IsCoinbase);

		Money spentAmount = Money.Zero;
		foreach (var spentCoin in transactionResponse_04_01.SpentCoins)
		{
			spentAmount = (Money)spentCoin.Amount.Add(spentAmount);
		}

		Debug.Log(spentAmount.ToDecimal(MoneyUnit.BTC));

		var fee = transactionResponse_04_01.Transaction.GetFee(transactionResponse_04_01.SpentCoins.ToArray());
		Debug.Log(fee);
	}
	//--------------------------------------------------------------------------------
	void Sample_05_SpendYourCoin()
	{
		/*
		var network				= NBitcoin.Network.TestNet;
		var privateKey			= new Key();
		var bitcoinPrivateKey	= privateKey.GetWif(network);
		var address				= bitcoinPrivateKey.GetAddress();

		Debug.Log(bitcoinPrivateKey);	// cUwC2Dk7VvVyxF3jGyHdz5HTtxHYqHuQgWX1pnYvqckwCyUGStd3
		Debug.Log(address);				// n3x4Q89kqkHSV5d33LaRbnnoDdTuvDjY5D
		*/

		var bitcoinPrivateKey	= new BitcoinSecret("cUwC2Dk7VvVyxF3jGyHdz5HTtxHYqHuQgWX1pnYvqckwCyUGStd3");
		var network				= bitcoinPrivateKey.Network;
		var address				= bitcoinPrivateKey.GetAddress();

		Debug.Log(bitcoinPrivateKey);
		Debug.Log(address);

		// Get coin from https://testnet.manu.backend.hamburg/faucet

		var transactionID		= uint256.Parse("13cb292d07883f6a87b2db52c807e411e6330c9d7756535a7169a3ced8fe4385");
		QBitNinja4Unity.QBitNinjaClient.GetTransaction(transactionID, network, Sample_05_01);
	}
	void Sample_05_01(QBitNinja.Client.Models.GetTransactionResponse transactionResponse, NBitcoin.Network network)
	{
		Debug.Log(transactionResponse.TransactionId);
		Debug.Log(transactionResponse.Block.Confirmations);

		var bitcoinPrivateKey		= new BitcoinSecret("cUwC2Dk7VvVyxF3jGyHdz5HTtxHYqHuQgWX1pnYvqckwCyUGStd3");

		// To where?
		var			recivedCoins	= transactionResponse.ReceivedCoins;
		OutPoint	outPointToSpend	= null;
		foreach (var coin in recivedCoins)
		{
			if (coin.TxOut.ScriptPubKey == bitcoinPrivateKey.ScriptPubKey)
			{
				outPointToSpend = coin.Outpoint;
			}
		}

		if (outPointToSpend == null)
			Debug.Log("TxOut dosen't contain our ScriptPubKey");
		else
			Debug.Log("We want to spend "+(outPointToSpend.N + 1)+". outpoint :");

		var transaction = new Transaction();
		transaction.Inputs.Add(new TxIn() { PrevOut = outPointToSpend });

		// From Where ?

		var hallOfTheMakersAddress = new BitcoinPubKeyAddress("mwCwTceJvYV27KXBc3NJZys6CjsgsoeHmf");


		// How much?

		TxOut hallOfTheMakersTxOut = new TxOut()
		{
			Value			= new Money((decimal)0.5, MoneyUnit.BTC),
			ScriptPubKey	= hallOfTheMakersAddress.ScriptPubKey
		};

		TxOut changeBackTxOut = new TxOut()
		{
			Value			= new Money((decimal)0.4999, MoneyUnit.BTC),
			ScriptPubKey	= bitcoinPrivateKey.ScriptPubKey
		};

		transaction.Outputs.Add(hallOfTheMakersTxOut);
		transaction.Outputs.Add(changeBackTxOut);

		var hallOfTheMakersAmmount	= new Money(0.5m, MoneyUnit.BTC);
		var mineFee					= new Money(0.0001m, MoneyUnit.BTC);

		var	txInAmount				= transactionResponse.ReceivedCoins[(int)outPointToSpend.N].TxOut.Value;
		Money changeBackAmount		= txInAmount - hallOfTheMakersAmmount - mineFee;
	}
	//--------------------------------------------------------------------------------
}
