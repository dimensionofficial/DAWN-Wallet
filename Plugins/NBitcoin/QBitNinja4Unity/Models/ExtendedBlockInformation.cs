using System;

namespace QBitNinja4Unity.Models
{
	[Serializable]
	public class ExtendedBlockInformation
	{
		public int		size;
		public int		strippedSize;
		public int		transactionCount;
		public int		blockSubsidy;
		public int		blockReward;

		static public QBitNinja.Client.Models.ExtendedBlockInformation Create(ExtendedBlockInformation block)
		{
			var extendedBlockInformation = new QBitNinja.Client.Models.ExtendedBlockInformation();

			extendedBlockInformation.Size				= block.size;
			extendedBlockInformation.StrippedSize		= block.strippedSize;
			extendedBlockInformation.TransactionCount	= block.transactionCount;
			extendedBlockInformation.BlockSubsidy		= block.blockSubsidy;
			extendedBlockInformation.BlockReward		= block.blockReward;

			return extendedBlockInformation;
		}
	}
}