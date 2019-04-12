using System.Collections;
using System.Collections.Generic;
using NBitcoin.BouncyCastle.Math;
using Nethereum.Util;
using UnityEngine;
using UnityEngine.UI;
public class EosPaymentwalletItem : MonoBehaviour {

    public Color normlColor;
    public Color redColor;

    public Text walletNameText;
    public Text walletAddressText;
    public Text dstCountText;

    public GameObject selectMark;

    public EthcoinInfoItem walletItem;


    public decimal tokenCount;
    public string contractAddresss = "0x6691459761bf1aab7e07ac719d6e423ac2661fa5";

    public void Show(EthcoinInfoItem eth)
    {
        walletItem = eth;
        gameObject.SetActive(true);
        walletNameText.text = eth.coinInfo.walletname;
        string address = eth.coinInfo.address;
        string one = address.Substring(0, 10);
        string two = address.Substring(address.Length - 10, 10);
        walletAddressText.text = one + "..." + two;

        contractAddresss = PanelManager._Instance._eosRegisterPanel.paymentOrderInfo.contactAddress;
        dstCountText.text = "";

        StartCoroutine(GetTokenBalances(walletItem.coinInfo.address, contractAddresss));
    }

    public IEnumerator GetTokenBalances(string address, string conAddress)
    {
        List<KeyValuePair<string, string>> ws1 = new List<KeyValuePair<string, string>>();
        ws1.Add(new KeyValuePair<string, string>("address", address));
        ws1.Add(new KeyValuePair<string, string>("ercaddress", conAddress));
        yield return HttpManager._Intance.GetNodeJsRequest("getBalance_erc", ws1, (Hashtable data) =>
        {
            if (data != null)
            {
                decimal ethCount = decimal.Parse(walletItem.coinInfo.ethmoney);
                string v = NewWalletManager._Intance.ShowCount(null, ethCount);

                System.Numerics.BigInteger tokenBlance =  new System.Numerics.BigInteger(decimal.Parse(data["balance"].ToString()));
                tokenCount = UnitConversion.Convert.FromWei(tokenBlance, 18);

                string sy = PanelManager._Instance._eosRegisterPanel.paymentOrderInfo.symbol;

                dstCountText.text = v + " ETH ( " + tokenCount + " " + sy + " )";

                if (ethCount <= 0 || tokenCount < PanelManager._Instance._eosRegisterPanel.paymentOrderInfo.price)
                {
                    dstCountText.color = redColor;
                }
                else
                {
                    dstCountText.color = normlColor;
                }
            }
        });
    }

    public string GetTokenCount()
    {

        for (int i = 0; i < walletItem.eRC_TokenList.Count; i++)
        {
            if (walletItem.eRC_TokenList[i].tokenService.tokenContractAddress.Equals(contractAddresss))
            {
                return walletItem.eRC_TokenList[i].cout;
            }
        }
        return "0";
    }

    public void OnSelectMe()
    {
        PanelManager._Instance._eosOrderPanel.OnClickSelectPayItem(this, tokenCount);
    }
}
