using System;
using NBitcoin;

namespace QBitNinja4Unity.Models
{
	[Serializable]
	public class WhatIsBlockHeader
	{
		public string		version;
		public string		hash;
		public string		previous;
		public string		time;
		public uint			nonce;
		public string		hashMerkelRoot;
		public string		bits;
		public double		difficulty;

		public QBitNinja.Client.Models.WhatIsBlockHeader Result()
		{
			//UnityEngine.Debug.Log(UnityEngine.JsonUtility.ToJson(this,true));

			var blockHeader			= new BlockHeader();
			blockHeader.Bits		= new Target(uint256.Parse("1000000000000000000000000000000000000000000000000000000000000000"));
			var result				= new QBitNinja.Client.Models.WhatIsBlockHeader(blockHeader);

			result.Hash				= uint256.Parse(hash);
			result.Previous			= uint256.Parse(previous);
            result.Time				= DateTimeOffset.Parse(time);
            result.Nonce			= nonce;
            result.HashMerkelRoot	= uint256.Parse(hashMerkelRoot);
            result.Version			= version;
            result.Bits				= bits;
            result.Difficulty		= difficulty;

			return result;
		}
	}
}