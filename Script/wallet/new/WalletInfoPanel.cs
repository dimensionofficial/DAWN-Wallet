using Nethereum.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalletInfoPanel : MonoBehaviour
{
    public MultiJSData multiJSData;
    public Text addressText;
    public CoinManagerPanel coinManagerPanel;
    public GameObject remindObject;

    public CoinInfoItemBase currentItem;

    public Text walletName;
    public SendCoinPanel sendCoinPanel;
    public ReceiveCoinPanel receiveCoinPanel;
    public Image m_Image;
    public Text countText;
    public Text rmbText;
    public Text typeText1;

    public GameObject ethTokenListObject;
    public EthTokenItem cloneObjet;
    public Transform lastObject;
    public Transform cloneParent;
    public Dictionary<string, EthTokenItem> ethTokenItemListDic = new Dictionary<string, EthTokenItem>();

    private Dictionary<string, string> haveedGetIconDic = new Dictionary<string, string>();

    public GameObject bottomBtns;

    [System.NonSerialized]
    public EthTokenItem ethselfItem;

    public EthTokenItem currentTokenItem;

    public GameObject delBtnObject;

   
    
    public HistroyRecord recordPage;
    //多签判断
    public bool isMultiAddres = false;
    public Image colorImage;
    public Color normlColor;
    public Color multiColor;

    public Image titleImage;
    public Sprite normlSprite;
    public Sprite multiSprite;

    public Image title2Image;

    public Sprite norml2Sprite;
    public Sprite multi2Sprite;

    void Start()
    {
        EventManager.Instance.AddEventListener(EventID.UpdateTotalbalance, UpdateMoney);
    }

    void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener(EventID.UpdateTotalbalance, UpdateMoney);
    }

    private void UpdateMoney(params object[] obj)
    {
        UpdateMoney();
    }

    public void OnClickCoinManagerBtn()
    {
        coinManagerPanel.inputText.text = "";
        coinManagerPanel.ShowMe(currentItem as EthcoinInfoItem);
    }

    public void UpdateMoney()
    {
        if (currentItem == null)
            return;

        decimal rmb = 0;
        NewWalletManager._Intance.ShowCount(countText, currentItem.coinInfo.money);

        if (currentItem.type == NewWalletManager.CoinType.BTC)
        {
            m_Image.overrideSprite = TextureUIAsset._Instance.btcIcon;
            
            countText.text = countText.text + " BTC";
            typeText1.text = "BTC";
            rmb = HttpManager._Intance.btc_RMB * currentItem.coinInfo.money;
        }
        else if (currentItem.type == NewWalletManager.CoinType.USDT)
        {
            m_Image.overrideSprite = TextureUIAsset._Instance.usdtIcon;

            countText.text = countText.text + " USDT";
            typeText1.text = "USDT";
            rmb = HttpManager._Intance.current_usdt * currentItem.coinInfo.money;

        }
        else if (currentItem.type == NewWalletManager.CoinType.ETH)
        {
            if (currentTokenItem != null)
            {
                m_Image.overrideSprite = currentTokenItem.image.overrideSprite;
                if (currentTokenItem.isToken)
                {
                    countText.text = currentTokenItem.tokenNumberText.text + " " + currentTokenItem.tokenNameText.text;
                    typeText1.text = currentTokenItem.tokenNameText.text;
                    if (currentTokenItem.erc_Token == null)
                    {
                        rmb = 0;
                    }
                    else
                    {
                        rmb = (currentTokenItem.erc_Token.tokenNumber * currentTokenItem.erc_Token.tokenService.TokenInfo.rmbValue);
                        if (rmb <= 0)
                        {
                            rmb = 0;
                        }
                    }
                }
                else
                {
                    m_Image.overrideSprite = TextureUIAsset._Instance.ethIcon;
                    countText.text = countText.text + " ETH";
                    typeText1.text = "ETH";
                    rmb = HttpManager._Intance.eth_RMB * (currentItem.coinInfo.money);
                }
            }
            else
            {
                m_Image.overrideSprite = TextureUIAsset._Instance.ethIcon;
                countText.text = countText.text + " ETH";
                typeText1.text = "ERC20";
                rmb = HttpManager._Intance.eth_RMB * (currentItem.coinInfo.money + currentItem.coinInfo.ethTokenMoney);
            }
        }
        else if (currentItem.type == NewWalletManager.CoinType.EOS)
        {
            m_Image.overrideSprite = TextureUIAsset._Instance.eosIcon;
            countText.text = countText.text + " EOS";
            typeText1.text = "EOS";
            rmb = 0;
        }
        NewWalletManager._Intance.ShowProperty(rmbText, rmb);
        //rmbText.text = rmb;
    }

    public void ShowMe(CoinInfoItemBase currentItem)
    {
        try
        {
            isMultiAddres = multiJSData.IsMultiSigAddress(currentItem.coinInfo.address);
        }
        catch (Exception)
        {
            isMultiAddres = false;
        }
        
        if (isMultiAddres)
        {
            colorImage.color = multiColor;
            titleImage.sprite = multiSprite;
            title2Image.sprite = multi2Sprite;
        }
        else
        {
            colorImage.color = normlColor;
            titleImage.sprite = normlSprite;
            title2Image.sprite = norml2Sprite;
        }
        currentItem.GetHistory();

        delBtnObject.SetActive(true);
        
        PanelManager._Instance.currentSubPage = PanelManager.SubPage.WalletInfo;
        //rmbText.text ="￥" currentItem.coinInfo.money * HttpManager._
        string adr = currentItem.coinInfo.address;

        string qianAdr = adr.Substring(0, 10); //adr.Substring(result.Text.Length - lastCount, lastCount);
        string houAdr = adr.Substring(adr.Length - 10, 10);

        addressText.text = qianAdr + "..." + houAdr;
        currentTokenItem = null;
        this.currentItem = currentItem;
        walletName.text = currentItem.coinname;
        gameObject.SetActive(true);
        NewWalletManager._Intance.currentCoinType = currentItem.type;
       
        UpdateMoney();

        if (currentItem.type == NewWalletManager.CoinType.ETH)
        {
            countText.gameObject.SetActive(false);
            PanelManager._Instance._mainPanel.bottomBtn.SetActive(true);

            EthcoinInfoItem ethItem = currentItem as EthcoinInfoItem;
            ethItem.GetTokenContractAddress();

            ShowETHandERC20();
        }
        else
        {
            countText.gameObject.SetActive(true);
            PanelManager._Instance._mainPanel.bottomBtn.SetActive(false);

            ethTokenListObject.gameObject.SetActive(false);
            bottomBtns.gameObject.SetActive(true);
        }
    //    recordPage.gameObject.SetActive(false);
        recordPage.transactionPanel.gameObject.SetActive(false);

        if (currentItem.type == NewWalletManager.CoinType.BTC)
        {
            recordPage.type = HistroyRecord.CurrentHistroyType.BTC;

            recordPage.currentAddress = currentItem.coinInfo.address;

            HistoryManagerNew.Instance.GetHistory(1, HistroyRecord.pageCount, currentItem.coinInfo.address, (o) =>
            {
                List<ETHHistoryRcord> rcordList = currentItem.AddLocalData(o,false, "BTC");
                recordPage.InitRcordItem(rcordList, "BTC");
            });
        }
        //--------?!?
        if (currentItem.type == NewWalletManager.CoinType.USDT)
        {
            recordPage.type = HistroyRecord.CurrentHistroyType.USDT;

            recordPage.currentAddress = currentItem.coinInfo.address;

            BTCGetHistory.Instance.GetUSDTHistory(recordPage.currentAddress, (o) =>
            {
                Debug.Log("USDT 记录 " + o.Count);
                List<ETHHistoryRcord> rcordList = currentItem.AddLocalData(o, false, "USDT");
                recordPage.InitRcordItem(rcordList, "USDT");
            }, (err) =>
            {

            },0,20);
        }
    }

    public void ShowMe(EthTokenItem tokenItem)
    {
        delBtnObject.SetActive(false);
        PanelManager._Instance._mainPanel.bottomBtn.SetActive(false);

        currentTokenItem = null;
        m_Image.overrideSprite = tokenItem.image.overrideSprite;
        currentTokenItem = tokenItem;
        ethTokenListObject.gameObject.SetActive(false);
        bottomBtns.gameObject.SetActive(true);

        decimal rmb = 0;
        countText.gameObject.SetActive(true);
        if (tokenItem.isToken)
        {
            tokenItem.image.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            NewWalletManager._Intance.ShowCount(countText, tokenItem.erc_Token.tokenNumber);
            countText.text = countText.text + " " + tokenItem.tokenNameText.text;
            typeText1.text = tokenItem.tokenNameText.text;

            rmb = (tokenItem.erc_Token.tokenNumber * tokenItem.erc_Token.tokenService.TokenInfo.rmbValue);

        }
        else
        {
 //           currentTokenItem = null;
            typeText1.text = "ETH";
            NewWalletManager._Intance.ShowCount(countText, currentItem.coinInfo.money);
            countText.text = countText.text + " ETH";
            rmb = (HttpManager._Intance.eth_RMB * currentItem.coinInfo.money);
        }

        NewWalletManager._Intance.ShowProperty(rmbText, rmb);
        //rmbText.text = rmb;
        //rmb = "≈￥" + (HttpManager._Intance.eth_RMB * currentItem.coinInfo.money).ToString("f2") + " CNY";
        //显示记录
        recordPage.ShowRecord(tokenItem);

    }

    private void ShowETHandERC20()
    {
        //if (NewWalletManager._Intance.currentCoinType == NewWalletManager.CoinType.ETH)
        //{
            ethTokenListObject.gameObject.SetActive(true);
            bottomBtns.gameObject.SetActive(false);

            if (ethselfItem == null)
            {
                GameObject tokenItem = GameObject.Instantiate(cloneObjet.gameObject);
                tokenItem.gameObject.SetActive(true);
                tokenItem.transform.SetParent(cloneParent, false);
                EthTokenItem tempTokenItem = tokenItem.GetComponent<EthTokenItem>();
                tempTokenItem.ETHAddEventListener();
                tempTokenItem.tokenNameText.fontStyle = FontStyle.Bold;
                tempTokenItem.ShowETH(currentItem.coinInfo);
                ethselfItem = tempTokenItem;
            }
            else
            {
                ethselfItem.ShowETH(currentItem.coinInfo);
            }

            EthcoinInfoItem ethItem = currentItem as EthcoinInfoItem;
            foreach (var k in ethTokenItemListDic)
            {
                k.Value.gameObject.SetActive(false);
            }
            StartCoroutine(GetShowTokens(ethItem));
 //           List<string> tokenAddressList = ethItem.GetTokenAddressKeyList();
 //           for (int i = 0; i < tokenAddressList.Count; i++)
 //           {
 //               EthTokenItem tempTokenItem;
 //               ERCContractInofo conInfo = ethItem.GetERContractInfo(tokenAddressList[i]);
 //               if (ethTokenItemListDic.ContainsKey(conInfo.contractAddress))
 //               {
 //                   tempTokenItem = ethTokenItemListDic[conInfo.contractAddress];
 //                   tempTokenItem.gameObject.SetActive(true);
 //               }
 //               else
 //               {
 //                   GameObject tokenItem = GameObject.Instantiate(cloneObjet.gameObject);
 //                   tokenItem.gameObject.SetActive(true);
 //                   tokenItem.transform.SetParent(cloneParent, false);
 //                   tempTokenItem = tokenItem.GetComponent<EthTokenItem>();
 //                 tempTokenItem.AddEventListener();
 //                   tempTokenItem.containsAddress = conInfo.contractAddress;
 //                   ethTokenItemListDic.Add(conInfo.contractAddress, tempTokenItem);
 //               }
 //               tempTokenItem.tokenNameText.fontStyle = FontStyle.Normal;
 //               tempTokenItem.ShowTokenInfo(conInfo, ethItem);
 //           }
 //       }
        lastObject.SetAsLastSibling();
    }

    private IEnumerator GetShowTokens(EthcoinInfoItem ethItem)
    {
        List<string> tokenAddressList = ethItem.GetTokenAddressKeyList();
        for (int i = 0; i < tokenAddressList.Count; i++)
        {
            ERCContractInofo conInfo = ethItem.GetERContractInfo(tokenAddressList[i]);

            if (!ethItem.isHideToken(conInfo.contractAddress))
            {
                if (string.IsNullOrEmpty(conInfo.iconPath))
                {
                    HttpManager._Intance.UpLoadContractAddress(conInfo.contractAddress, conInfo.symbol, conInfo.fullName, delegate (Hashtable h)
                    {
                        string iconPath = "";
                        if (int.Parse(h["error"].ToString()) > 0)
                        {
                            if (h["tokenIcon"] != null)
                                iconPath = h["tokenIcon"].ToString();
                        }
                        conInfo.iconPath = iconPath;

                        ethItem.SetTokenAddress(conInfo.contractAddress, conInfo.symbol, iconPath, conInfo.fullName, true);
                        ShowTokenInfo(ethItem, conInfo);
                    });
                    yield return new WaitForSeconds(0.05F);
                }
                else
                {
                    ShowTokenInfo(ethItem, conInfo);
                }
            }
        }
    }

    private void ShowTokenInfo(EthcoinInfoItem ethItem,  ERCContractInofo conInfo)
    {
        EthTokenItem tempTokenItem;

        if (ethTokenItemListDic.ContainsKey(conInfo.contractAddress))
        {
            tempTokenItem = ethTokenItemListDic[conInfo.contractAddress];
            tempTokenItem.gameObject.SetActive(true);
        }
        else
        {
            GameObject tokenItem = GameObject.Instantiate(cloneObjet.gameObject);
            tokenItem.gameObject.SetActive(true);
            tokenItem.transform.SetParent(cloneParent, false);
            tempTokenItem = tokenItem.GetComponent<EthTokenItem>();
            //tempTokenItem.AddEventListener();
            tempTokenItem.containsAddress = conInfo.contractAddress;
            ethTokenItemListDic.Add(conInfo.contractAddress, tempTokenItem);
        }
        tempTokenItem.tokenNameText.fontStyle = FontStyle.Normal;
        tempTokenItem.ShowTokenInfo(conInfo, ethItem);
        lastObject.SetAsLastSibling();
    }

    public void OnClickBackBtn()
    {
        if (currentItem != null && currentItem.type == NewWalletManager.CoinType.ETH && recordPage.gameObject.activeInHierarchy)
        {
            ShowMe(currentItem);
        }
        else
        {
            NewWalletManager._Intance.DoTweenBack(this.transform, -1000);
            PanelManager._Instance._mainPanel.bottomBtn.SetActive(true);
            currentItem = null;
        }
        recordPage.ActiveRecord(false);
        
    }

    public void OnClickSendBtn()
    {
        sendCoinPanel.SetETHTokenItem(currentTokenItem);
        sendCoinPanel.Open();
        NewWalletManager._Intance.DOTweenCome(sendCoinPanel.transform, -1000, 0);
    }

    public void OnClickReceiveBtn()
    {
        receiveCoinPanel.Open(this);
        NewWalletManager._Intance.DOTweenCome(receiveCoinPanel.transform, -1000, 0);
    }


    public void OnClickDelBtn()
    {
        if (NewWalletManager._Intance.IsNeedColdWallet)
        {
            remindObject.SetActive(true);
        }
        else
        {
            if (SeedKeyManager.Instance.IsBackUp() || !SeedKeyManager.Instance.isFirstSeedBip(currentItem.coinInfo.address))
            {
                remindObject.SetActive(true);
            }
            else
            {
                PopUpBox.Instance.Show(delegate ()
                {
                    PanelManager._Instance._backUpPrivateKeyPanel.OpenMe();
                }, delegate ()
                {
                    OnClickDelWallet();
                }, "马上备份", "不需要", "重要提示", "您尚未备份钱包,如无妥善备份,删除钱包后将无法找回钱包，请慎重处理该操作");
            }
        }
    }

    public void OnClickDelWallet()
    {
        if (multiJSData.RemoveAddress(currentItem.coinInfo.address))
        {
            multiJSData.SaveMultiWalletInfo();
        }
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("op", "deletAddress"));
        ws.Add(new KeyValuePair<string, string>("userName", NewWalletManager._Intance.userName));
        ws.Add(new KeyValuePair<string, string>("address", currentItem.coinInfo.address));
        ws.Add(new KeyValuePair<string, string>("type", ((int)(currentItem.type)).ToString()));
        StartCoroutine(HttpManager._Intance.SendRequest(ws, CallBack));
    }

    private void CallBack(Hashtable h)
    {
        int re = System.Convert.ToInt32(h["error"]);
        if (re == 0)
        {
            this.gameObject.SetActive(false);
            this.transform.localPosition = new Vector3(-1000F, this.transform.localPosition.y, this.transform.localPosition.z);
            PanelManager._Instance.currentSubPage = PanelManager.SubPage.Property;
            PanelManager._Instance._mainPanel.bottomBtn.SetActive(true);
            PanelManager._Instance._mainPanel.OnDeletWallet(currentItem);
            EventManager.Instance.SendEvent(EventID.UpdateTotalbalance);
            remindObject.SetActive(false);
        }
        else
        {
            remindObject.SetActive(false);
            PopupLine.Instance.Show("删除失败请重试");
        }
    }

    private void GetFailer(string str)
    {
        Debug.Log(str);
    }
}
