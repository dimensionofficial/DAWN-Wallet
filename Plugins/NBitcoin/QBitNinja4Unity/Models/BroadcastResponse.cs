using System;
using NBitcoin;

namespace QBitNinja4Unity.Models
{
	[Serializable]
	public class BroadcastError
	{
		public byte		errorCode;
		public string	reason;
	}

	[Serializable]
	public class BroadcastResponse
	{
		public bool				success;
		public BroadcastError	error;

		public QBitNinja.Client.Models.BroadcastResponse Result()
		{
			//UnityEngine.Debug.Log(UnityEngine.JsonUtility.ToJson(this,true));

			var result	= new QBitNinja.Client.Models.BroadcastResponse();

			result.Success = success;

			if (!success)
			{
				result.Error			= new QBitNinja.Client.Models.BroadcastError();
				result.Error.ErrorCode	= (NBitcoin.Protocol.RejectCode)error.errorCode;
				result.Error.Reason		= error.reason;
			}

			return result;
		}
	}
}
