using NBitcoin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USDTtest : HistoryManager
{
    public USDTtest instance;
    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start () {
        //GetUSDTSignatureHash("76a914ffab1e7a6972adaa1b637ce86a50ab8886a4a64088ac");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// 获取USDTInput
    /// </summary>
    public  void GetUSDTInput()
    {
        Debug.Log("-----------GetBTCGasFee---------");
        //HistoryManager.Instance.GetUSDTInput("1QJrJUgAtFJYKLPAj8b8TnCUMy8XPn5DKr", "1", GetUsdtSig);
    }
    
    public void PostInputSig()
    {
        Debug.Log("-----------PostInputSig---------");
        //HistoryManager.Instance.PostUSDTTx("1QJrJUgAtFJYKLPAj8b8TnCUMy8XPn5DKr", "1QJrJUgAtFJYKLPAj8b8TnCUMy8XPn5DKr", "1", inputVos, PostUsdtSig);
    }
    List<InputVo> inputVos = new List<InputVo>();
    public void GetUsdtSig(List<InputVo> inputs)
    {
       
        Debug.Log("-----------GetUsdtSig---------");
        List<string> ScriptPuks = new List<string>();
        for (int i = 0; i < inputs.Count; i++)
        {
            Debug.Log(inputs[i].scriptPubKey+"--------------------");
            inputs[i].signature = "3044022029a46053a3c17b331d2741e1418ff884dc13367f74f8df182df59ded6fd380580220465b60ddbf0d04d3fb5be982390e6fc5e36247060293d4e84ca8719c2ef7c097";
            inputs[i].pubKey = "0260adc39c28922d883151aafaab83c2305a203f5d866217f0eb9bd83e77e97e65";
            ScriptPuks.Add(inputs[i].scriptPubKey);
        }
        inputVos = inputs;
        List<string> usdtSigs = new List<string>();
        foreach (var v in ScriptPuks)
        {   Debug.Log("===="+v);
        }
    }
    public void PostUsdtSig(string inputs)
    {
        Debug.Log("PostUsdtSig");
        Debug.Log(inputs);
    }
    #region USDT
    public uint256 GetUSDTSignatureHash(string scriptCode, SigHash sigHash = SigHash.All, HashVersion sigversion = HashVersion.Original)
    {
        uint256 signatureHash = new uint256();
        Script script = new Script(scriptCode);
        Debug.Log(script);
        return signatureHash;
    }

    #endregion
}
