using UnityEngine;

public abstract class PluginMsgReceiver : MonoBehaviour
{
	private	int		nReceiverId;

	protected virtual void Start()
	{
		nReceiverId = PluginMsgHandler.getInst().RegisterAndGetReceiverId(this);
	}

	protected virtual void OnDestroy()
	{
		PluginMsgHandler.getInst().RemoveReceiver(nReceiverId);
	}

	protected JsonObject SendPluginMsg(JsonObject jsonMsg)
	{
		return PluginMsgHandler.getInst().SendMsgToPlugin(nReceiverId, jsonMsg);
	}

	public abstract void OnPluginMsgDirect(JsonObject jsonMsg);  
}