using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LocalContract
{
    public string tokenAddress;
    public string tokenSymbol;
}

public class PlayerLocalDataManager : Singleton<PlayerLocalDataManager>
{
   

    /// <summary>
    /// 以太坊ERC20智能合约地址
    /// </summary>
    public const string CONTRACT_ADDRESS = "EthContractAddress";
    /// <summary>
    /// 以太坊ERC20代币名称缩写
    /// </summary>
    public const string CONTRACT_NAME = "EthContractSymbol";


    /// <summary>
    /// 获取以太坊ERC20合约
    /// </summary>
    /// <returns></returns>
    public List<LocalContract> GetLocalContract()
    {
        List<LocalContract> tempContract = new List<LocalContract>();
       
        string[] tempAddress = PlayerPrefsX.GetStringArray(CONTRACT_ADDRESS);
        if (tempAddress.Length > 0)
        {
            string[] tempSymbol = PlayerPrefsX.GetStringArray(CONTRACT_NAME);
            for (int i = 0; i < tempAddress.Length; i++)
            {
                LocalContract lc = new LocalContract();
                lc.tokenAddress = tempAddress[i];
                lc.tokenSymbol = tempSymbol[i];
                tempContract.Add(lc);
            }

        }

        return tempContract;
    }

    /// <summary>
    /// 保存以太坊ERC20合约
    /// </summary>
    /// <param name="contractAddresss"></param>
    /// <param name="tokenSymbol"></param>
    public void SetLocalContractAddress(string contractAddresss, string tokenSymbol)
    {
        string[] conAdd = PlayerPrefsX.GetStringArray(CONTRACT_ADDRESS);

        bool isAdd = true;
        for (int i = 0; i < conAdd.Length; i++)
        {
            if (conAdd[i].Equals(contractAddresss))
            {
                isAdd = false;
                break;
            }
        }

        if (isAdd)
        {
            List<string> tempList = new List<string>(conAdd);
            tempList.Add(contractAddresss);

            string[] symbol = PlayerPrefsX.GetStringArray(CONTRACT_NAME);
            List<string> tempNameList = new List<string>(symbol);
            tempNameList.Add(tokenSymbol);

            PlayerPrefsX.SetStringArray(CONTRACT_ADDRESS,tempList.ToArray());
            PlayerPrefsX.SetStringArray(CONTRACT_NAME, tempNameList.ToArray());
        }
    }

}
