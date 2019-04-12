using System;
using NBitcoin;

namespace QBitNinja4Unity.Models
{
	[Serializable]
	public class BalanceModel
	{
		public string						continuation;
		public BalanceOperation[]			operations;
		public BalanceOperation[]			conflictedOperations;

		public QBitNinja.Client.Models.BalanceModel Result()
		{
			//UnityEngine.Debug.Log(UnityEngine.JsonUtility.ToJson(this,true));

			var result = new QBitNinja.Client.Models.BalanceModel();

			result.Continuation			= continuation;
			result.Operations			= new System.Collections.Generic.List<QBitNinja.Client.Models.BalanceOperation>(BalanceOperation.Create(operations));
			result.ConflictedOperations	= new System.Collections.Generic.List<QBitNinja.Client.Models.BalanceOperation>(BalanceOperation.Create(conflictedOperations));

			return result;
		}
	}

	[Serializable]
	public class BalanceOperation
	{

        private long amount;
		public int							confirmations;
		public int							height;
		public string						blockId;
		public string						transactionId;
		public CoinJson[]					receivedCoins;
		public CoinJson[]					spentCoins;
		public string						firstSeen;

        public long Amount
        {
            get
            {
                return amount;
            }

            set
            {
                amount = value;
            }
        }

        static public QBitNinja.Client.Models.BalanceOperation[] Create(BalanceOperation[] balanceOperations)
		{
			var result = new QBitNinja.Client.Models.BalanceOperation[balanceOperations.Length];

			for (int i = 0; i < balanceOperations.Length; i++)
			{
                result[i] = new QBitNinja.Client.Models.BalanceOperation();
                result[i].Amount = balanceOperations[i].Amount;
                result[i].Confirmations = balanceOperations[i].confirmations;
                result[i].Height = balanceOperations[i].height;
                result[i].BlockId = balanceOperations[i].blockId.Length == 0 ? null : uint256.Parse(balanceOperations[i].blockId);
                result[i].TransactionId = uint256.Parse(balanceOperations[i].transactionId);
                result[i].ReceivedCoins = new System.Collections.Generic.List<ICoin>(CoinJson.Create(balanceOperations[i].receivedCoins));
                result[i].SpentCoins = new System.Collections.Generic.List<ICoin>(CoinJson.Create(balanceOperations[i].spentCoins));
                result[i].FirstSeen = DateTimeOffset.Parse(balanceOperations[i].firstSeen); ;
            }

            return result;
		}
	}

	[Serializable]
	public class BalanceSummary
	{
		public BalanceSummaryDetails		unConfirmed;
		public BalanceSummaryDetails		confirmed;
		public BalanceSummaryDetails		spendable;
		public BalanceSummaryDetails		immature;
		public int							olderImmature;
		public string						cacheHit;

		public QBitNinja.Client.Models.BalanceSummary Result()
		{
			//UnityEngine.Debug.Log(UnityEngine.JsonUtility.ToJson(this,true));

			var result = new QBitNinja.Client.Models.BalanceSummary();

			result.UnConfirmed		= BalanceSummaryDetails.Create(unConfirmed);
			result.Confirmed		= BalanceSummaryDetails.Create(confirmed);
			result.Spendable		= BalanceSummaryDetails.Create(spendable);
			result.Immature			= BalanceSummaryDetails.Create(immature);
			result.OlderImmature	= olderImmature;

			if(cacheHit!=null&&cacheHit.Length>0)
				result.CacheHit	= (QBitNinja.Client.Models.CacheHit) Enum.Parse(typeof(QBitNinja.Client.Models.CacheHit),cacheHit,true);

			return result;
		}
	}

	[Serializable]
	public class BalanceSummaryDetails
	{
		public int							transactionCount;
		public long							amount;
		public long received;
		public AssetBalanceSummaryDetails[]	assets;

		static public QBitNinja.Client.Models.BalanceSummaryDetails Create(BalanceSummaryDetails details)
		{
			var result = new QBitNinja.Client.Models.BalanceSummaryDetails();

			result.TransactionCount = details.transactionCount;
			result.Amount			= details.amount;
			result.Received			= details.received;
			result.Assets			= AssetBalanceSummaryDetails.Create(details.assets);

			return result;
		}
	}

	[Serializable]
	public class AssetBalanceSummaryDetails
	{
		public string						asset;
		public long							quantity;
		public long							received;

		static public QBitNinja.Client.Models.AssetBalanceSummaryDetails[] Create(AssetBalanceSummaryDetails[] assets)
		{
			var result = new QBitNinja.Client.Models.AssetBalanceSummaryDetails[assets.Length];

			for (int i = 0; i < assets.Length; i++)
			{
				result[i].Asset		= new NBitcoin.OpenAsset.BitcoinAssetId(assets[i].asset);
				result[i].Quantity	= assets[i].quantity;
				result[i].Received	= assets[i].received;
			}

			return result;
		}
	}

    [Serializable]
    public class Fees
    {
        public int fastestFee;
        public int halfHourFee;
        public int hourFee;
    }
}
