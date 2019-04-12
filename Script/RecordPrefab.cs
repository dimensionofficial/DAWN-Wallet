using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using Nethereum.Util;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RecordPrefab : MonoBehaviour
{

    public const int ethpackageHeight = 4;
    public const int packbagWith = 180;
    public const int normarWith = 140;

    public Color receiveColor;
    public Color myselfReceiveColor;
    public Color sendColor;

    public GameObject failure;
    public GameObject sendingObjct;
	public Image icon;
    public Text outOrIN;
    public Text date;
    public Text coin;
    public Text type;
    public Text fromText;
    public string time;
    public string hash;
    public string from;
    public string to;
    public string value;
    public string gas;
    public string confixmations;
    public string blockNumber;
    public string input;
    public string txReceiptStatus;
    public Button Open;
    public bool isFailure;
    public HistroyRecord.TabType tabtype;

    public Image m_sprite;
    public GameObject packagObject;
    private string myAddress;

    private bool isPacking = false;

    public void InitShow(ETHHistoryRcord historyRcord, bool isERC20, int m_decimal, string currentUnit)
    {
        bool needPack = false;
        if (!currentUnit.Equals("BTC") && !currentUnit.Equals("USDT"))
        {
            needPack = true;
        }

            CoinInfoItemBase currentItem = PanelManager._Instance._WalletInfoPanel.currentItem;
        myAddress = currentItem.coinInfo.address.ToLower();
        string fromAddress = historyRcord.from.ToLower();
        string toAddress = historyRcord.to.ToLower();

        time = historyRcord.timeStamp;
        hash = historyRcord.hash;
        from = historyRcord.from;
        to = historyRcord.to;
        isFailure = historyRcord.isOverTime;
        if (isERC20)
        {
            System.Numerics.BigInteger tempValue = System.Numerics.BigInteger.Parse(historyRcord.value);
            decimal customTokenBalance = UnitConversion.Convert.FromWei(tempValue, m_decimal);
            value = customTokenBalance.ToString();
        }
        else
        {
            value = historyRcord.value;
        }


        gas = historyRcord.gas;
        confixmations = historyRcord.confixmations;
        blockNumber = historyRcord.blockNumber;
        input = historyRcord.input;
        txReceiptStatus = historyRcord.txReceiptStatus;

        RectTransform rect = transform.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, normarWith);
        packagObject.SetActive(false);

        //try
        //{
            int a = int.Parse(historyRcord.confixmations);
            if (a > 0)
            {
                sendingObjct.SetActive(false);
                if (needPack && from.Equals(myAddress))
                {
                    PlayerPrefs.DeleteKey(from.ToLower() + hash.ToLower());
                }
            }
            else
            {
                sendingObjct.SetActive(false);

                if (historyRcord.isOverTime)
                {
                    failure.SetActive(true);
                    if (needPack && from.Equals(myAddress))
                    {
                        PlayerPrefs.DeleteKey(from.ToLower() + hash.ToLower());
                    }
                }
                else if(!PlayerPrefs.HasKey(myAddress.ToLower() + hash.ToLower() + "Over"))
                {
                    sendingObjct.SetActive(true);
                    failure.SetActive(false);

                    if (needPack && from.ToLower().Equals(myAddress.ToLower()))
                    {
                        packagObject.SetActive(true);
                        rect.sizeDelta = new Vector2(rect.sizeDelta.x, packbagWith);
                        if (PlayerPrefs.HasKey(myAddress.ToLower() + hash.ToLower() + "Packing"))
                        {
                            float tempValue = PlayerPrefs.GetFloat(myAddress.ToLower() + hash.ToLower() + "Packing");
                            m_sprite.fillAmount = tempValue;
                        }
                        else
                        {
                            m_sprite.fillAmount = 0;
                        }
                        StartPackag();
                    }
                }
            }

        if (fromAddress == myAddress && toAddress == myAddress)
        {
            tabtype = HistroyRecord.TabType.SelfType;
            icon.overrideSprite = TextureUIAsset._Instance.myselMark;
            outOrIN.text = "自收";
            fromText.text = "来自: " + currentItem.coinInfo.walletname;
            outOrIN.color = myselfReceiveColor;
        }
        else if (fromAddress == myAddress)
        {
            tabtype = HistroyRecord.TabType.SendType;
            icon.overrideSprite = TextureUIAsset._Instance.sendMark;
            outOrIN.text = "转出";
            string one = toAddress.Substring(0, 10);
            string two = toAddress.Substring(toAddress.Length - 5, 5);
            fromText.text = "发到: " + one + "..." + two;
            outOrIN.color = sendColor;
        }
        else if (toAddress == myAddress)
        {
            tabtype = HistroyRecord.TabType.RecordType;
            icon.overrideSprite = TextureUIAsset._Instance.getMark;
            outOrIN.text = "转入";
            string one = fromAddress.Substring(0, 10);
            string two = fromAddress.Substring(fromAddress.Length - 5, 5);
            fromText.text = "来自: " + one + "..." + two;
            outOrIN.color = receiveColor;
        }
        date.text = time;
        coin.text = value;

        type.text = currentUnit;


    }
    private bool isDestroy = false;

    void OnDestroy()
    {
        if (isPacking)
        {
            PlayerPrefs.SetFloat(myAddress.ToLower() + hash.ToLower() + "Packing",m_sprite.fillAmount);
        }
        isDestroy = true;
    }

    private void ShowPackaging(int p)
    {
        if (isDestroy)
            return;

        float endValue = p * 1.0F / ethpackageHeight;

        if (PlayerPrefs.HasKey(myAddress.ToLower() + hash.ToLower() + "Over"))
        {
            endValue = 1;
        }

        if (endValue > m_sprite.fillAmount)
        {
            if (endValue >= 1)
            {
                DOTween.To(() => m_sprite.fillAmount, x => m_sprite.fillAmount = x, endValue, 0.5F).OnComplete(()=> PackagEnd());
            }
            else
            {
                DOTween.To(() => m_sprite.fillAmount, x => m_sprite.fillAmount = x, endValue, 0.5F);
            }
           
        }
    }

    public void PackagEnd()
    {
        if (!isDestroy)
        {
            RectTransform rect = transform.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 140);
            packagObject.SetActive(false);
            sendingObjct.SetActive(false);
        }
        PlayerPrefs.SetInt(myAddress.ToLower() + hash.ToLower() + "Over", 1);
        isPacking = false;
        EthTokenManager._Intance.StopCoroutine(EthTokenManager._Intance.Packaging(from, hash, ShowPackaging));
        CancelInvoke("SendAccount");
    }

    public void StartPackag()
    {
        isPacking = true;
        EthTokenManager._Intance.StartCoroutine(EthTokenManager._Intance.Packaging(from, hash, ShowPackaging));
        InvokeRepeating("SendAccount", 1, 1);
    }

    public void SendAccount()
    {
        StartCoroutine(TransactionByHash());
    }

    private IEnumerator TransactionByHash()
    {
        WaitForSeconds wfs = new WaitForSeconds(1.0F);

        if (!NewWalletManager._Intance.isTestSend)
        {
            Hashtable myJsonData = new Hashtable();
            myJsonData["id"] = 1;
            myJsonData["jsonrpc"] = "2.0";
            myJsonData["method"] = "eth_getTransactionByHash";

            ArrayList arrayList = new ArrayList();
            arrayList.Add(hash);
            myJsonData["params"] = arrayList;
            string rpcRequestJson = Json.jsonEncode(myJsonData);


            
            byte[] requestBytes = Encoding.UTF8.GetBytes(rpcRequestJson);
            //https://mainnet.infura.io
            UnityWebRequest unityRequest = new UnityWebRequest("https://mainnet.infura.io", "POST");
            UploadHandlerRaw uploadHandler = new UploadHandlerRaw(requestBytes);
            unityRequest.SetRequestHeader("Content-Type", "application/json");
            uploadHandler.contentType = "application/json";
            unityRequest.uploadHandler = uploadHandler;
            unityRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return unityRequest.SendWebRequest();
            if (unityRequest.error != null)
            {
                Debug.Log("Error:" + unityRequest.error);
            }
            else
            {
                string r = unityRequest.downloadHandler.text;
                Hashtable hs = Json.jsonDecode(r) as Hashtable;
                Hashtable res = hs["result"] as Hashtable;
                if (res.ContainsKey("blockNumber") && res["blockNumber"] != null)
                {
                    string blockNumber = res["blockNumber"].ToString();
                    int bn = Convert.ToInt32(blockNumber, 16);
                    PlayerPrefs.SetInt(myAddress.ToLower() + hash.ToLower() + "Over", bn);
                    PlayerPrefs.DeleteKey(myAddress.ToLower() + hash.ToLower() + "Packing");
                    PlayerPrefs.DeleteKey(myAddress.ToLower() + hash.ToLower() + "BlockNumber");
                    ShowPackaging(ethpackageHeight);
                }
            }
        }

       
        yield return wfs;

    }
}
