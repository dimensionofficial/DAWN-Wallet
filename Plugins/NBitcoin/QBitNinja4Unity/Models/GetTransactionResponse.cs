using System;
using NBitcoin;

namespace QBitNinja4Unity.Models
{
	[Serializable]
	public class BlockInformation
	{
		public string			blockId;
		public string			blockHeader;
		public int				height;
		public int				confirmations;
		public string			medianTimePast;
		public string			blockTime;

		static public QBitNinja.Client.Models.BlockInformation Create(BlockInformation block)
		{
			if(block==null || block.blockHeader == null)
				return null;

			var blockInformation			= new QBitNinja.Client.Models.BlockInformation(new BlockHeader(NBitcoin.DataEncoders.Encoders.Hex.DecodeData(block.blockHeader)));

			blockInformation.Height			= block.height;
			blockInformation.Confirmations	= block.confirmations;
			blockInformation.MedianTimePast	= DateTimeOffset.Parse(block.medianTimePast);

			return blockInformation;
		}
	}

	[Serializable]
	public class CoinJson
	{
		public string			transactionId;
		public uint				index;
		public long				value;
		public string			scriptPubKey;
		public string			redeemScript;

		static ICoin Create(CoinJson coin)
		{
			var transactionId	= uint256.Parse(coin.transactionId);
			var scriptPubKey	= new Script(NBitcoin.DataEncoders.Encoders.Hex.DecodeData(coin.scriptPubKey));
			var redeemScript	= coin.redeemScript.Length == 0 ? null: new Script(NBitcoin.DataEncoders.Encoders.Hex.DecodeData(coin.redeemScript));
			var iCoin			= redeemScript == null ?
					                new Coin(new OutPoint(transactionId, coin.index), new TxOut(coin.value, scriptPubKey)) :
					                new ScriptCoin(new OutPoint(transactionId, coin.index), new TxOut(coin.value, scriptPubKey), redeemScript);

			return iCoin;
		}

		static public ICoin[] Create(CoinJson[] iCoins)
		{
			var coins = new ICoin[iCoins.Length];

			for (int i = 0; i < coins.Length; i++)
				coins[i] = Create(iCoins[i]);

			return coins;
		}
	}

	[Serializable]
	public class GetTransactionResponse
	{
		public string				transaction;
		public string				transactionId;
		public bool					isCoinbase;
		public BlockInformation		block;
		public CoinJson[]			spentCoins;
		public CoinJson[]			receivedCoins;
		public string				firstSeen;
		public int					fees;

		public QBitNinja.Client.Models.GetTransactionResponse Result()
		{
			//UnityEngine.Debug.Log(UnityEngine.JsonUtility.ToJson(this,true));

			var result					= new QBitNinja.Client.Models.GetTransactionResponse();

			result.Transaction			= new Transaction(NBitcoin.DataEncoders.Encoders.Hex.DecodeData(transaction));
			result.TransactionId		= uint256.Parse(transactionId);
			result.IsCoinbase			= isCoinbase;
			result.Block				= BlockInformation.Create(block);
			result.SpentCoins.AddRange(CoinJson.Create(spentCoins));
			result.ReceivedCoins.AddRange(CoinJson.Create(receivedCoins));
			result.FirstSeen			= DateTimeOffset.Parse(firstSeen);
			result.Fees					= fees;

			return result;
		}
	}
}