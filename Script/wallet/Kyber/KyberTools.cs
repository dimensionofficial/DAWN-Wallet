using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.JsonRpc.UnityClient;
using System.Numerics;
using LitJson;
using UnityEngine.UI;
using Nethereum.Hex.HexTypes;
using ZXing;
using System.Threading;
using System.Text;
using System.Security.Cryptography;

public class KyberTools : MonoBehaviour
{
    // Use this for initialization "0x818e6fecd516ecc3849daf6845e3ec868087b755"  ZIL "0x05f4a42e251f2d52b8ed15e9fedaacfcef1fad27"
    //testAddress 0xbb9c28a16654a0cd510f5b0a250255c4a07211f2, 0x950b87923d52b09b1050abda589f91521e17e606, 0x2c018fc6c9bb2b7653136dc7c5b7b588f2d11986
    const string url = "http://127.0.0.1:8888/";
    public string ABI;
    public string tokenAddress = "0x818e6fecd516ecc3849daf6845e3ec868087b755";
    public string erc20ABI;
    //public string tokenAddress = "0x11542D7807DFb2B44937F756b9092c76e814F8eD";
    public List<TokenContract> supportedList = new List<TokenContract>();
    public TokenPrefab leftToken;
    public TokenPrefab rightToken;
    public GameObject selectedPage;
    public TokenPrefab tokenPrefab;
    public RectTransform prefabParent;
    public Text rateText;
    private float rectHeight;
    private Vector3 startPosition;
    private decimal rate;
    private BigInteger rateBigInt;
    public string walletAddress = "";

    public static KyberTools instance;
    public BigInteger gasTest = new BigInteger(500000);
    public BigInteger gaspriceTest = new BigInteger(3000000000);
    public MainPanel mainPanel;
    NewWalletManager walletManager;

    public Text walletText;
    public GameObject walletListPage;
    public WalletPrefab walletPrefab;
    public RectTransform walletParent;
    public GameObject exchangeMask;
    public Sprite selectedSprite;
    public Sprite emptySprite;
    public Sprite ethIcon;
    public InputField searchInput;
    public InputField leftInput;
    public InputField rightInput;
    public GameObject exchangeInfoPage;
    public Text paySymbol;
    public Text getSymbol;
    public Text paidText;
    public Text getText;
    public Text exchangeRateText;
    public Text paidAddressText;
    public Text gasText;
    public Text gasCNYText;
    public Slider gasPriceSlider;

    public GameObject qrCodePage;
    public Text countText;
    public Button lastButton;
    public Button nextButton;
    public Image codeImage;
    public RectTransform codeParent;
    public GameObject mainPage;
    public ScanBar scanBar;
    public GameObject scaningPage;
    public RawImage qrCameraImage;
    public GameObject goOnObject;
    public Text goOnText;
    public GameObject reScanObject;
    public GameObject confirmInfoPage;
    public Text confirmForToken;
    public Text confirmToToken;
    public Text confirmPaidText;
    public Text confirmGetText;
    public Text confirmRateText;
    public Text confirmAddressText;
    public Text confirmGasInETH;
    public Text confirmGasInCNY;
    public Button sendoutButton;
    public GameObject sendOverPage;
    public Text sendSuccessForToken;
    public Text sendSuccessToToken;
    public Text sendSuccessPaidText;
    public Text sendSuccessGetText;
    public Text sendSuccessRateText;
    public Text sendSuccessAddressText;
    public Text sendSuccessGasInETH;
    public Text sendSuccessGasInCNY;
    public Color successHistoryColor;
    public Color runningHistoryColor;
    public Color faildHistoryColor;
    public RectTransform kyberHistoryParent;
    public KyberHistoryPrefab kyberHistoryPrefab;
    public ApproveHistoryPrefab approveHistoryPrefab;
    public KyberHistroyInfoPage kyberHistroyInfoPage;
    public KyberHistroyInfoPage approveHistoryInfoPage;
    public GameObject warnNotEnoughMoney;

    public GameObject morePage;
    public GameObject rulesPage;
    public GameObject approvePage;
    public GameObject helpPage;
    public ApproveAuthPrefab approveAuthPrefab;
    public RectTransform approveAuthParent;
    public InputField searchApproveInput;
    public GameObject approveInfo;
    public Text approveSrc;
    public Text approveDes;
    public Slider approveSlider;
    public Text approveGasText;
    public Text approveGasCNYText;
    public GameObject approveConfirmPage;
    public Text approveConfirmSrc;
    public Text approveConfirmDes;
    public Text approveConfirmGas;
    public Text approveConfirmGasCNY;
    public GameObject approveSuccessPage;
    public Text approveSuccessSrc;
    public Text approveSuccessDes;
    public Text approveSuccessGas;
    public Text approveSuccessGasCNY;
    public GameObject approveWarnPage;
    public Button confirmButtonWarn;
    public Text tokenNameForApprove;
    public Button sendApproveOutButton;
    public GameObject firstKyberTips;
    public GameObject noDataObj;
    public Button addMoreButton;


    public Text debugInfo;
    private long tempGasPrice;
    private long gWei = 1000000000;
    private int currentPage;
    private int totalPage;
    private string[] mulitPic = null;
    private WebCamTexture webCamTexture;
    private bool isScanning;
    private Result result = null;
    public const int lastCount = 7;
    private Action<string> ScanEndBack;
    private string strinfo;
    private BigInteger ethMount;
    private bool isApprove;
    private System.DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    private bool isGetSupportedList;
    private bool isGetting;
    private bool gettingRate;
    private decimal balance;
    private bool isApproveAuth;
    private string approvingAddress;
    private int page = 1;
    private int recordCount = 10;
    private bool willClear = false;
    public Dictionary<string, List<KyberHistoryRecord>> kyberHistoryDIc = new Dictionary<string, List<KyberHistoryRecord>>();
    private void Awake()
    {
        instance = this;
        walletManager = NewWalletManager._Intance;
        TouchScreenKeyboard.hideInput = true;
    }
    private void OnEnable()
    {
        //walletText.text = "选择钱包";
        //walletAddress = "";
        //exchangeMask.SetActive(true);
        //ClearRect(kyberHistoryParent);
        rightInput.text = "";
        leftInput.text = "";
        leftInput.placeholder.GetComponent<Text>().text = "转出数量";
        BigInteger aa = BigInteger.Pow(10, 18);
        if(!isGetSupportedList)
        {
            GetSupportedToken();
        }
        //noDataObj.SetActive(false);
        if (walletAddress != "")
        {
            if(kyberHistoryParent.childCount > 1)
            {
                willClear = false;
            }
            HistoryManagerNew.Instance.Refresh(walletAddress, null, RefreshType.Token);
            HistoryManagerNew.Instance.Refresh(walletAddress, RefreshFinished, RefreshType.ETH);
        }
    }
    void Start()
    {
        //rectHeight = prefabParent.sizeDelta.y;
        //startPosition = prefabParent.localPosition;
        int isFirst = PlayerPrefs.GetInt("isFirstTimeKyber");
        if(isFirst == 0)
        {
            firstKyberTips.SetActive(true);
            PlayerPrefs.SetInt("isFirstTimeKyber",1);
        }
        // GetAllowance("ETH");
        KyberTokenService kyberTokenService = new KyberTokenService(ABI, tokenAddress);
        addMoreButton.gameObject.SetActive(false);
        //GetUserCapInWei(walletAddress);
        //Debug.Log(kyberTokenService.DecodeVariable<BigInteger>("trade", "000000000000000000000000000000000000000000000000080c92ed51ba0000"));
        //PlayerPrefs.SetString("ETHDefaultWallet","");
        string _walletAddress = PlayerPrefs.GetString("ETHDefaultWallet");
        if(_walletAddress != null && _walletAddress != "")
        {
            Debug.Log("存在默认ETH钱包：" + _walletAddress);
            WalletPrefab tempPrefab = new WalletPrefab();
            tempPrefab.walletName = walletManager.ethcoinAddresListInfo[_walletAddress];
            tempPrefab.address = _walletAddress;
            SelectWallet(tempPrefab);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool IsETH(string _address)
    {
        if (_address == "0xeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee")
            return true;
        else
            return false;
    }
    public void ShutDownKyberPage()
    {
        gameObject.SetActive(false);
    }
    public void GetMessageFromKyber(string op, Dictionary<string, string> dic = null, Action<string> Callback = null)
    {
        string urlTemp = url + op;
        if (dic != null)
        {
            urlTemp += "?";
            foreach (var item in dic)
            {
                urlTemp += item.Key;
                urlTemp += "=";
                urlTemp += item.Value;
                urlTemp += "&";
            }
            urlTemp = urlTemp.TrimEnd('&');
        }
        Debug.Log(urlTemp);
        StartCoroutine(SendUrl(urlTemp, Callback));
    }

    public void GetSupportedToken()
    {
        if(supportedList.Count > 0)
        {
            isGetSupportedList = true;
            GetExpectedRate2();
            return;
        }
        isGetting = true;
        //string urlTemp = "https://tracker.kyber.network/api/tokens/supported";
        //StartCoroutine(SendUrl(urlTemp, (s) =>
        // {
        //     JsonData _jd = JsonMapper.ToObject(s);

        //     if (_jd.Count > 0)
        //     {
        //         for (int i = 0; i < _jd.Count; i++)
        //         {
        //             JsonData js = _jd[i];
        //             TokenContract contract = new TokenContract();
        //             contract.symbol = js["symbol"].ToString();
        //             contract.cmcName = js["cmcName"].ToString();
        //             contract._name = js["name"].ToString();
        //             contract.decimals = int.Parse(js["decimals"].ToString());
        //             contract.contractAddress = js["contractAddress"].ToString().ToLower();
        //             GetTokenIconPath(contract.contractAddress, (icon) => { contract.iconPath = icon; });

        //             bool isContain = false;
        //             for (int j = 0; j < supportedList.Count; j++)
        //             {
        //                 if(supportedList[j].contractAddress == contract.contractAddress)
        //                 {
        //                     isContain = true;
        //                     break;
        //                 }
        //             }
        //             if(isContain == false)
        //             {
        //                 supportedList.Add(contract);
        //             }
        //         }
        //         supportedList.Sort((a, b) =>
        //         {
        //             if (a.symbol[0] > b.symbol[0])
        //                 return 1;
        //             else if (a.symbol[0] < b.symbol[0])
        //                 return -1;
        //             else return 0;

        //         });
        //         isGetSupportedList = true;
        //         //GetExpectedRate();
        //         GetExpectedRate2();
        //     }
        // }));

        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("sign", "bb432b049e9fa65baad8ca6cb8ff6fac"));
        StartCoroutine(GetNodeJsRequest("supported_tokens", ws, (string data) =>
        {
            if (data != null && data != "")
            {
                JsonData _jd = JsonMapper.ToObject(data);
                isGetting = false;
                for (int i = 0; i < _jd.Count; i++)
                {
                    JsonData js = _jd[i];
                    TokenContract contract = new TokenContract();
                    contract.symbol = js["symbol"].ToString();
                    contract.cmcName = js["cmcName"].ToString();
                    contract._name = js["name"].ToString();
                    contract.decimals = int.Parse(js["decimals"].ToString());
                    contract.contractAddress = js["contractAddress"].ToString().ToLower();
                    GetTokenIconPath(contract.contractAddress, (icon) => { contract.iconPath = icon; });

                    bool isContain = false;
                    for (int j = 0; j < supportedList.Count; j++)
                    {
                        if (supportedList[j].contractAddress == contract.contractAddress)
                        {
                            isContain = true;
                            break;
                        }
                    }
                    if (isContain == false)
                    {
                        supportedList.Add(contract);
                    }
                }

                supportedList.Sort((a, b) =>
                {
                    if (a.symbol[0] > b.symbol[0])
                        return 1;
                    else if (a.symbol[0] < b.symbol[0])
                        return -1;
                    else return 0;

                });
                isGetSupportedList = true;
                GetExpectedRate2();
            }
            else
            {
                isGetting = false;
                isGetSupportedList = false;
                PopupLine.Instance.Show("网络错误");
            }
            PanelManager._Instance._eosResourcesPanel.loadingPanel.SetActive(false);
        }));
    }


    private IEnumerator SendUrl(string url, Action<string> Callback = null)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.timeout = 10;
            HttpManager._Intance.loadingPanel.gameObject.SetActive(true);
            yield return www.SendWebRequest();
            if (www.error != null)
            {
                Debug.Log(www.error);
                isGetting = false;
                isGetSupportedList = false;
                HttpManager._Intance.loadingPanel.gameObject.SetActive(false);
            }
            else
            {
                if (www.responseCode == 200 && Callback != null)//200表示接受成功
                {
                    Callback(www.downloadHandler.text);
                    isGetting = false;
                    HttpManager._Intance.loadingPanel.gameObject.SetActive(false);
                }
            }
        }

    }

    private void CallBack(string s)
    {
        Debug.Log(s);
    }

    public void ShowSelectedPage(TokenPrefab _prefab)
    {
        if (isGetting)
        {
            PopupLine.Instance.Show("正在获取支持的币种");
            return;
        }
        else if (!isGetSupportedList)
        {
            PopupLine.Instance.Show("无法获取到可支持的币种");
            return;
        }
        selectedPage.SetActive(true);
        if (supportedList.Count <= 0)
            return;
        int count = 0;
        for (int i = 0; i < supportedList.Count; i++)
        {
            TokenContract temp = supportedList[i];

            if (_prefab.symbol == leftToken.symbol && temp.symbol == rightToken.symbol)
                continue;
            if (_prefab.symbol == rightToken.symbol && temp.symbol == leftToken.symbol)
                continue;

            TokenPrefab prefab = Instantiate(tokenPrefab);
            prefab.transform.SetParent(prefabParent);
            count++;
            prefabParent.sizeDelta = new Vector2(prefabParent.sizeDelta.x, 114 * count);
            prefab.transform.localScale = new Vector3(1, 1, 1);
            prefab.symbol = temp.symbol;
            prefab._name = temp._name;
            prefab.decimals = temp.decimals;
            prefab.contractAddress = temp.contractAddress;
            prefab.tokenName.text = temp.symbol;
            prefab.allowance = temp.allowance;
            prefab._button.onClick.AddListener(delegate () { SelecteToken(prefab, _prefab); });
            if (prefab.symbol == _prefab.symbol)
                prefab.isSelected.sprite = selectedSprite;
            prefab.balance.text = GetBalanceByAddress(prefab.contractAddress) + prefab.symbol;
            if (IsETH(prefab.contractAddress))
                prefab.tokenImage.sprite = ethIcon;
            else
                StartCoroutine(TextureUIAsset._Instance.LoadImage(prefab.tokenImage, temp.iconPath));
        }
    }

    string GetBalanceByAddress(string _address)
    {
        EthcoinInfoItem item = mainPanel.ethItemList[walletAddress];
        if (IsETH(_address))
            return item.coinInfo.money.ToString("0.00000000");
        else
        {
            for (int i = 0; i < item.eRC_TokenList.Count; i++)
            {
                if (item.eRC_TokenList[i].tokenService.tokenContractAddress == _address)
                    return item.eRC_TokenList[i].cout;
            }
        }
        return "0";
    }
    public TokenContract GetTokenInfoBySymbol(string _symbol)
    {
        for (int i = 0; i < supportedList.Count; i++)
        {
            if (supportedList[i].symbol == _symbol)
                return supportedList[i];
        }
        return null;
    }
    public TokenContract GetTokenInfoByAddress(string _address)
    {
        for (int i = 0; i < supportedList.Count; i++)
        {
            if (supportedList[i].contractAddress.ToLower() == _address.ToLower())
                return supportedList[i];
        }
        return null;
    }
    public void ShowSelectedPageLeft()
    {
        ShowSelectedPage(leftToken);
    }

    public void ShowSelectedPageRight()
    {
        ShowSelectedPage(rightToken);
    }
    public void SelecteToken(TokenPrefab prefab, TokenPrefab _prefab)
    {
        selectedPage.SetActive(false);
        ClearRect(prefabParent);
        searchInput.text = "";
        _prefab.symbol = prefab.symbol;
        _prefab.tokenName.text = prefab.symbol;
        _prefab.tokenImage.sprite = prefab.tokenImage.sprite;
        _prefab.contractAddress = prefab.contractAddress;
        _prefab.decimals = prefab.decimals;
        _prefab.allowance = prefab.allowance;
        rightInput.text = "";
        leftInput.text = "";
        leftInput.placeholder.GetComponent<Text>().text = "转出数量";
        GetExpectedRate2();
    }

    public void HideSelectedPage()
    {
        selectedPage.SetActive(false);
        ClearRect(prefabParent);
        searchInput.text = "";
    }

    public void ClearRect(RectTransform tran)
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < tran.childCount; i++)
        {
            if (tran.GetChild(i).gameObject != addMoreButton.gameObject)
                list.Add(tran.GetChild(i).gameObject);
        }
        //tran.DetachChildren();
        tran.anchoredPosition = Vector2.zero;
        tran.sizeDelta = new Vector2(0, rectHeight);
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i]);
        }
    }

    public void GetExpectedRate()
    {
        gettingRate = true;
        rateText.text = "<color=#9A9A9AFF>汇率</color>  " + "正在获取...";
        KyberTokenService kyberTokenService = new KyberTokenService(ABI, tokenAddress);
        TokenContract srcToken = GetTokenInfoBySymbol(leftToken.symbol);
        TokenContract desToken = GetTokenInfoBySymbol(rightToken.symbol);
        string srcAddress = srcToken.contractAddress;
        string desAddress = desToken.contractAddress;
        BigInteger integer = BigInteger.Pow(10, srcToken.decimals);
        StartCoroutine(kyberTokenService.GetExpectecRate((value) =>
        {
            rate = GetDecimalFromBigint(value); Debug.Log(value);
            rateText.text = "<color=#9A9A9AFF>汇率</color>  ≈" + rate.ToString("0.00000000");
            gettingRate = false;
            if (leftInput.text != "")
            {
                leftInput.onValueChanged.Invoke(leftInput.text);
            }
        }, srcAddress, desAddress, integer));
    }

    public void GetMinConversionRate()
    {
        KyberTokenService kyberTokenService = new KyberTokenService(ABI, tokenAddress);
        TokenContract srcToken = GetTokenInfoBySymbol(leftToken.symbol);
        TokenContract desToken = GetTokenInfoBySymbol("ETH");
        string srcAddress = srcToken.contractAddress;
        string desAddress = desToken.contractAddress;
        BigInteger integer = BigInteger.Pow(10, srcToken.decimals);
        StartCoroutine(kyberTokenService.GetExpectecRate((value) =>
        {
            rateBigInt = value;
        }, srcAddress, desAddress, integer));
    }
    public void GetExpectedRate2()
    {
        gettingRate = true;
        rateText.text = "<color=#9A9A9AFF>汇率</color>  " + "正在获取...";
        KyberTokenService kyberTokenService = new KyberTokenService(ABI, tokenAddress);
        TokenContract srcToken = GetTokenInfoBySymbol(leftToken.symbol);
        TokenContract desToken = GetTokenInfoBySymbol(rightToken.symbol);
        string srcAddress = srcToken.contractAddress;
        string desAddress = desToken.contractAddress;
        BigInteger integer = BigInteger.Pow(10, srcToken.decimals);
        GetMinConversionRate();
        StartCoroutine(kyberTokenService.GetExpectecRate2((value) =>
        {
            rate = GetDecimalFromBigint(value);
            rateText.text = "<color=#9A9A9AFF>汇率</color>  ≈" + rate.ToString("0.00000000");
            gettingRate = false;
            if (leftInput.text != "")
            {
                leftInput.onValueChanged.Invoke(leftInput.text);
            }
        }, srcAddress, desAddress, integer));
        
    }
    public void GetExpectedRate(string src, string des)
    {
        gettingRate = true;
        rateText.text = " <color=#9A9A9AFF>汇率</color>  " + "正在获取...";
        KyberTokenService kyberTokenService = new KyberTokenService(ABI, tokenAddress);
        TokenContract srcToken = GetTokenInfoBySymbol(src);
        TokenContract desToken = GetTokenInfoBySymbol(des);
        string srcAddress = srcToken.contractAddress;
        string desAddress = desToken.contractAddress;
        BigInteger integer = BigInteger.Pow(10, srcToken.decimals);
        StartCoroutine(kyberTokenService.GetExpectecRate((value) =>
        {
            rate = GetDecimalFromBigint(value); Debug.Log(value);
            rateText.text = "<color=#9A9A9AFF>汇率</color>  ≈" + rate.ToString("0.00000000");
            gettingRate = false;
            if (leftInput.text != "")
            {
                leftInput.onValueChanged.Invoke(leftInput.text);
            }
        }, srcAddress, desAddress, integer));
    }

    public void GetUserCapInWei(string address)
    {
        KyberTokenService kyberTokenService = new KyberTokenService(ABI, tokenAddress);
        StartCoroutine(kyberTokenService.GetUserCapInWei(address));
    }

    public static decimal GetDecimalFromBigint(BigInteger src)
    {
        decimal result = (decimal)src;
        for (int i = 0; i < 18; i++)
        {
            result = result / 10;
        };
        return result;
    }

    public static decimal GetDecimalFromBigDecimal(decimal src)
    {
        decimal result = src;
        for (int i = 0; i < 18; i++)
        {
            result = result / 10;
        };
        return result;
    }
    public TransactionInput CreatTransactionInput(string addressSrc, BigInteger srcAmount, string addressDes, string walletAddress)
    {
        BigInteger maxDestAmount = -1;
        BigInteger minConversionRate = rateBigInt;
        KyberTokenService kyberTokenService = new KyberTokenService(ABI, tokenAddress);
        return kyberTokenService.CreateTradeFundsTransactionInput(addressSrc,
            srcAmount, addressDes, walletAddress,
            maxDestAmount, minConversionRate);
    }

    public TransactionInput CreatTransactionInput(string addressSrc, BigInteger srcWeiPrice)
    {
        KyberTokenService erc20Service = new KyberTokenService(erc20ABI, addressSrc);
        return erc20Service.CreatApproveFundsTransactionInput(tokenAddress, srcWeiPrice);
    }
    public void GetAllowance(string addressSrc = "", Action<BigInteger> callBack = null)
    {
        BigInteger result = 0;
        KyberTokenService tempService = new KyberTokenService(erc20ABI, addressSrc);
        StartCoroutine(tempService.GetAllowance((value) =>
        {
            if (callBack != null)
                callBack(value);
        }, walletAddress, tokenAddress));
    }

    public void GetSupply(string addressSrc)
    {
        TokenContract tokenTemp = GetTokenInfoBySymbol(addressSrc);
        KyberTokenService tempService = new KyberTokenService(erc20ABI, tokenTemp.contractAddress);
        StartCoroutine(tempService.GetTotalSupply());
    }

    public void ShutDownWalletListPage()
    {
        walletListPage.SetActive(false);
    }
    public void ShowWalletListPage()
    {
        if (isGetting)
        {
            PopupLine.Instance.Show("正在获取支持的币种");
            return;
        }
        else if (!isGetSupportedList)
        {
            PopupLine.Instance.Show("无法获取到可支持的币种");
            return;
        }
        walletListPage.SetActive(true);
        ClearRect(walletParent);
        foreach (var item in walletManager.ethcoinAddresListInfo)
        {
            WalletPrefab prefab = Instantiate(walletPrefab);
            prefab.transform.SetParent(walletParent);
            prefab.transform.localScale = new Vector3(1, 1, 1);
            prefab.walletText.text = item.Value;
            prefab.walletName = item.Value;
            prefab.address = item.Key;
            if (walletAddress != null && walletAddress != "" && prefab.address.ToLower() == walletAddress)
                prefab.isSelected.sprite = selectedSprite;
        }
    }

    public void SelectWallet(WalletPrefab prefab)
    {
        ShutDownWalletListPage();
        if(walletAddress != "" &&  walletAddress == prefab.address)
        {
            HistoryManagerNew.Instance.Refresh(walletAddress, null, RefreshType.ETH);
            HistoryManagerNew.Instance.Refresh(walletAddress, null, RefreshType.Token);
            return;
        }
        walletAddress = prefab.address.ToLower() ;
        PlayerPrefs.SetString("ETHDefaultWallet", prefab.address);
        walletText.text = prefab.walletName;
        exchangeMask.SetActive(false);
        firstKyberTips.SetActive(false);
        willClear = true;
        HistoryManagerNew.Instance.Refresh(walletAddress, null, RefreshType.Token);
        HistoryManagerNew.Instance.Refresh(walletAddress, RefreshFinished, RefreshType.ETH);

        

        //获取可支持币的授权情况
        for (int i = 0; i < supportedList.Count; i++)
        {
            TokenContract contract = supportedList[i];
            if (!IsETH(contract.contractAddress))
            {
                GetAllowance(contract.contractAddress, (alow) => {
                    contract.allowance = alow;
                    if (contract.contractAddress == rightToken.contractAddress)
                    {
                        rightToken.allowance = alow;
                    }
                    else if (contract.contractAddress == leftToken.contractAddress)
                    {
                        leftToken.allowance = alow;
                    }
                });
            }
        }
    }
    private void RefreshFinished(bool result)
    {
        //获取本地历史记录
        List<KyberHistoryRecord> currentKyberHistory;
        if (!kyberHistoryDIc.ContainsKey(walletAddress))
        {
            currentKyberHistory = new List<KyberHistoryRecord>();
            kyberHistoryDIc.Add(walletAddress, currentKyberHistory);
        }
        else
            currentKyberHistory = kyberHistoryDIc[walletAddress];
        currentKyberHistory = GetLocalData(currentKyberHistory, walletAddress);//从本地获取历史记录

        GetWildData(currentKyberHistory, walletAddress);//从外部网络获取历史记录
    }
    public void GetTokenIconPath(string _tokenAddress, Action<string> callBack)
    {
        if (IsETH(_tokenAddress))
            return; http://47.96.131.169:8090/images/edu.png;http://47.96.131.169:8090/images/chat.png;
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("op", "gettokeninfo"));
        ws.Add(new KeyValuePair<string, string>("contactid", _tokenAddress));
        StartCoroutine(HttpManager._Intance.SendRequest(ws, delegate (Hashtable table)
        {
            ArrayList data = table["tokenList"] as ArrayList;
            if (data.Count > 0)
            {
                Hashtable temp = data[0] as Hashtable;
                if (callBack != null)
                {
                    callBack(temp["iconPath"].ToString());
                }
            }
            else
            {
                Debug.Log("未能查找到：" + _tokenAddress);
            }
        }));
    }

    public void SearchByTokenName()
    {
        string str = searchInput.text;
        int countInView = 0;
        if (str.Length == 0)
        {
            for (int i = 0; i < prefabParent.childCount; i++)
            {
                prefabParent.GetChild(i).gameObject.SetActive(true);
                countInView++;
            }
        }
        else
        {
            for (int i = 0; i < prefabParent.childCount; i++)
            {
                TokenPrefab prefab = prefabParent.GetChild(i).GetComponent<TokenPrefab>();
                if (prefab.symbol.IndexOf(str.ToUpper()) >= 0)
                {
                    prefab.gameObject.SetActive(true);
                    countInView++;
                }
                else
                {
                    prefab.gameObject.SetActive(false);
                }
            }
        }
        prefabParent.anchoredPosition = Vector2.zero;
        prefabParent.sizeDelta = new Vector2(0, countInView * 114f);
    }

    public void ShowLeftTokenBalance()
    {
        leftInput.placeholder.GetComponent<Text>().text = "余额：" + GetBalanceByAddress(leftToken.contractAddress);
        leftInput.onValueChanged.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.EditorAndRuntime);
    }
    public void EndEditLeftInput()
    {
        if (leftInput.text == "")
            leftInput.placeholder.GetComponent<Text>().text = "转出数量";
        rightInput.onValueChanged.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.EditorAndRuntime);
    }

    public void EndEditRightInput()
    {
        leftInput.onValueChanged.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.EditorAndRuntime);
    }
    public void OnValueChangeLeft()
    {
        warnNotEnoughMoney.SetActive(false);
        rightInput.onValueChanged.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
        leftInput.onValueChanged.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.EditorAndRuntime);
        if (leftInput.text == "")
        {
            rightInput.text = "";
            return;
        }
        rightInput.text = (decimal.Parse(leftInput.text) * rate).ToString("0.00000000");
        balance = decimal.Parse(GetBalanceByAddress(leftToken.contractAddress));
        if (decimal.Parse(leftInput.text) > balance)
        {
            warnNotEnoughMoney.SetActive(true);
        }
    }

    public void OnValueChangeRight()
    {
        warnNotEnoughMoney.SetActive(false);
        leftInput.onValueChanged.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
        rightInput.onValueChanged.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.EditorAndRuntime);
        if (rightInput.text == "")
        {
            leftInput.text = "";
            return;
        }
        leftInput.text = (decimal.Parse(rightInput.text) / rate).ToString("0.00000000");
        balance = decimal.Parse(GetBalanceByAddress(leftToken.contractAddress));
        if (decimal.Parse(leftInput.text) > balance)
        {
            warnNotEnoughMoney.SetActive(true);
        }
    }

    public void SpeedExchange()
    {
        if (ChectBalance())
        {
            ShowExchangeInfoPage();
        }
    }
    public bool ChectBalance()
    {
        if (leftInput.text == "")
        {
            PopupLine.Instance.Show("请输入转出数量，且小数点后不超过8位");
            return false;
        }
        decimal needPay;
        if (gettingRate)
        {
            PopupLine.Instance.Show("正在获取汇率");
            return false;
        }
        if (!decimal.TryParse(leftInput.text, out needPay))
        {
            PopupLine.Instance.Show("转出数量输入有误");
            return false;
        }
        decimal willGet;
        if (!decimal.TryParse(rightInput.text, out willGet))
        {
            PopupLine.Instance.Show("未能获取到收到数量");
            return false;
        }

        string balance = GetBalanceByAddress(leftToken.contractAddress);
        decimal ba = decimal.Parse(balance);
        decimal money = decimal.Parse(leftInput.text);
        if (money > ba)
        {
            PopupLine.Instance.Show("余额不足");
            return false;
        }
        TokenContract from = GetTokenContractByAddress(leftToken.contractAddress);
        List<KyberHistoryRecord> tempList = kyberHistoryDIc[walletAddress];
        if (tempList != null && tempList.Count > 0)
        {
            for (int j = 0; j < tempList.Count; j++)
            {
                if (tempList[j].to == from.contractAddress && tempList[j].isPending)
                {
                    PopupLine.Instance.Show("授权交易正在进行，请稍后");
                    return false;
                }
            }
        }
        if (!IsETH(from.contractAddress) && from.allowance >= 0)
        {

            ShowApproveWarnPage();
            tokenNameForApprove.text = leftToken.symbol;
            confirmButtonWarn.onClick.RemoveAllListeners();
            confirmButtonWarn.onClick.AddListener(delegate () { ShowApproveInfo(from.contractAddress); });
            return false;
        }

        return true;


    }

    public void ShowExchangeInfoPage()
    {
        exchangeInfoPage.SetActive(true);
        paySymbol.text = leftToken.symbol;
        getSymbol.text = rightToken.symbol;
        paidText.text = decimal.Parse(leftInput.text) + "  " + leftToken.symbol;
        getText.text = decimal.Parse(rightInput.text) + "  " + rightToken.symbol;
        exchangeRateText.text = rate.ToString("f8");
        paidAddressText.text = walletAddress.Substring(0, 14) + "..." + walletAddress.Substring(walletAddress.Length - 13, 13);
        tempGasPrice = HttpManager._Intance.ethGasPrice_int64;
        if (tempGasPrice < 10 * gWei)
        {
            gasPriceSlider.minValue = tempGasPrice / 2;
            gasPriceSlider.maxValue = tempGasPrice * 4;
        }
        else if (tempGasPrice >= 10 * gWei && tempGasPrice <= 30 * gWei)
        {
            gasPriceSlider.minValue = tempGasPrice / 2;
            gasPriceSlider.maxValue = tempGasPrice * 3;
        }
        else if (tempGasPrice > 30)
        {
            gasPriceSlider.minValue = tempGasPrice / 2;
            gasPriceSlider.maxValue = tempGasPrice * 2;
        }
        gasPriceSlider.value = tempGasPrice;
        decimal gasReal = GetDecimalFromBigDecimal(tempGasPrice * HttpManager._Intance.kyberGas_Limit);
        gasText.text = gasReal.ToString("f8") + "  eth";
        gasCNYText.text = "≈ " + (gasReal * HttpManager._Intance.eth_RMB).ToString("f2") + " CNY";

        lastButton.interactable = false;
        nextButton.interactable = false;
    }

    public void OnGasPriceChange()
    {
        decimal gasReal = GetDecimalFromBigDecimal((decimal)gasPriceSlider.value * HttpManager._Intance.kyberGas_Limit);
        gasText.text = gasReal.ToString("f8") + "  eth";
        gasCNYText.text = "≈ " + (gasReal * HttpManager._Intance.eth_RMB).ToString("f8");
    }
    public void HideExchangeInfoPage()
    {
        exchangeInfoPage.SetActive(false);
    }
    public void ShowQRCodePage()
    {
        TokenContract from = GetTokenInfoBySymbol(leftToken.symbol);
        TokenContract to = GetTokenInfoBySymbol(rightToken.symbol);
        
        isApproveAuth = false;
       
        decimal exchangeValue = decimal.Parse(leftInput.text);
        BigInteger srcAmount = Nethereum.Util.UnitConversion.Convert.ToWei(exchangeValue, leftToken.decimals);
        TransactionInput input = CreatTransactionInput(
            from.contractAddress, srcAmount,
            to.contractAddress, walletAddress
            );
        string tokenData = input.Data;
        ethMount = 0;
        if (IsETH(leftToken.contractAddress))
            ethMount = srcAmount;

        PanelManager._Instance.loadingPanel.SetActive(true);

        QRPayTools.CreateNoSignPayQRInfo_ETH(walletAddress, tokenAddress, ethMount, new BigInteger(HttpManager._Intance.kyberGas_Limit), new BigInteger(gasPriceSlider.value), this.gameObject, ETHInfo._url, tokenData, delegate (string qrinfo)
        {

            PanelManager._Instance.loadingPanel.SetActive(false);

            if (NewWalletManager._Intance.IsNeedColdWallet)
            {
                qrCodePage.SetActive(true);
                CreatQRImage(qrinfo);
            }
            else
            {
                PanelManager._Instance._pingCodeInputBox.OpenMe(qrinfo, PingCodeInputBox.SingType.Kyber, walletAddress, null);
            }
           
        });//代币兑换 src为ETH时，amount的值，需要与data中srcAmount的值对应
    }

    public void HideQRCodePage()
    {
        qrCodePage.SetActive(false);
        HideApproveWarnPage();
    }

    public void LastPage()
    {
        currentPage -= 1;
        ShowCodeImage(currentPage);
        countText.text = currentPage + "/" + totalPage;
        nextButton.GetComponentInChildren<Text>().text = "下一张";
        if (currentPage == 1)
            lastButton.interactable = false;
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(delegate () { NextPage(); });
    }
    public void NextPage()
    {
        currentPage += 1;
        ShowCodeImage(currentPage);
        lastButton.interactable = true;
        countText.text = currentPage + "/" + totalPage;
        if (currentPage == totalPage)
        {
            nextButton.GetComponentInChildren<Text>().text = "下一步";
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(delegate () { ShowScanPage(); });
        }

    }
    public void ShowCodeImage(int _index)
    {
        for (int i = 1; i < codeParent.childCount; i++)
        {
            if (i == _index)
                codeParent.GetChild(i).gameObject.SetActive(true);
            else
                codeParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void CreatQRImage(string info)
    {
        if (codeParent.childCount > 1)
        {
            for (int i = 1; i < codeParent.childCount; i++)
            {
                Destroy(codeParent.GetChild(i).gameObject);
            }
        }
        int oneCount = 400;

        int tempCount = info.Length / oneCount;

        if (info.Length % oneCount != 0)
        {
            tempCount += 1;
        }

        List<string> infos = new List<string>();
        string str = "";
        foreach (char a in info)
        {
            str += a;
            if (str.Length == oneCount)
            {
                infos.Add(str);
                str = "";
            }
        }
        infos.Add(str);
        currentPage = 1;
        totalPage = infos.Count;
        countText.text = currentPage + "/" + totalPage;
        QRCodeManager qrCodeManager = new QRCodeManager();
        for (int i = 1; i <= infos.Count; i++)
        {
            Image temp = Instantiate(codeImage);
            temp.transform.SetParent(codeParent, false);

            string _info = infos[i - 1];
            string one = i.ToString();
            if (i < 10)
            {
                one = "00" + i.ToString();
            }
            else if (i < 100)
            {
                one = "0" + i.ToString();
            }
            string two = infos.Count.ToString();
            if (infos.Count < 10)
            {
                two = "00" + infos.Count.ToString();
            }
            else if (infos.Count < 100)
            {
                two = "0" + infos.Count.ToString();
            }

            _info = _info + one + "/" + two;
            qrCodeManager.EncodeQRCode(temp, _info);
            if (i != 1)
                temp.gameObject.SetActive(false);
        }
        nextButton.interactable = true;
        lastButton.onClick.RemoveAllListeners();
        nextButton.onClick.RemoveAllListeners();
        lastButton.onClick.AddListener(delegate () { LastPage(); });
        nextButton.onClick.AddListener(delegate () { NextPage(); });
    }
    public void ShowScanPage()
    {
        // mainPanel.gameObject.SetActive(false);
        //mainPage.SetActive(false);
        scaningPage.SetActive(true);
        strinfo = "";
        OnScan(false);
        ScanEndBack = OnEndScan;
    }

    public void OnEndScan(string str)
    {
        qrCameraImage.gameObject.SetActive(false);
        ClosedCamera();
        strinfo = str;
        if (string.IsNullOrEmpty(str))
        {
            PopupLine.Instance.Show("扫描错误,请重新扫描");
        }
        else
        {
            scaningPage.SetActive(false);
            if (!isApproveAuth)
                ShowConfirmPage();
            else
            {
                ShowApproveConfirmPage();
            }
        }
        Debug.Log(str);
    }
    public void BackScanPanel()
    {
        //mainPanel.gameObject.SetActive(true);
        //mainPage.SetActive(true);
        scaningPage.SetActive(false);
        ClosedCamera();
    }

    void OnScan(bool isScanAddress)
    {
        mulitPic = null;

        if (webCamTexture == null)
        {
            webCamTexture = new WebCamTexture(Screen.width, Screen.height);
        }


        if (isScanning)
            StopCoroutine(Scanning(isScanAddress));

        if (webCamTexture != null)
        {
            webCamTexture.Play();
            qrCameraImage.texture = webCamTexture;

            qrCameraImage.gameObject.SetActive(true);

            StartCoroutine(Scanning(isScanAddress));
        }
    }

    IEnumerator Scanning(bool isScanAddress)
    {
        scanBar.StartDo();
        isScanning = true;
        IBarcodeReader iBR = new BarcodeReader();

        while (webCamTexture.width == 16)
            yield return null;

        result = iBR.Decode(webCamTexture.GetPixels32(), webCamTexture.width, webCamTexture.height);
        int dot = 0;

        float height = 1600F;
        float width = 900F;

        if (Camera.main.pixelHeight > height)
            height = Camera.main.pixelHeight;
        if (Camera.main.pixelWidth > width)
            width = Camera.main.pixelWidth;

        Vector2 scaleTo = new Vector2(webCamTexture.width, webCamTexture.height);
        float scale = height * 1.0F / webCamTexture.width;
        scaleTo *= scale;
        if (scaleTo.y < width)
        {
            scale = width / scaleTo.y;
            scaleTo *= scale;
        }
        qrCameraImage.rectTransform.sizeDelta = scaleTo;
        qrCameraImage.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        qrCameraImage.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        qrCameraImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        qrCameraImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaleTo.x);
        qrCameraImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaleTo.y);
#if UNITY_ANDROID   
        qrCameraImage.rectTransform.localEulerAngles = new Vector3(0, 0, -90);
#endif
#if UNITY_IPHONE
        qrCameraImage.rectTransform.localEulerAngles = new Vector3(0, 0, 90);
        qrCameraImage.rectTransform.localScale = new Vector3(-1, 1, 1);
#endif

        while (result == null)
        {
            if (dot % 2 == 0)
            {
                Color32[] colors = webCamTexture.GetPixels32();
                int w = webCamTexture.width;
                int h = webCamTexture.height;
                dot = 1;
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    result = iBR.Decode(colors, w, h);
                    dot = 0;
                }, null);
            }
            yield return new WaitForFixedUpdate();
        }

        try
        {
            string count = result.Text.Substring(result.Text.Length - lastCount, lastCount);
            string[] num = count.Split('/');
            int cur = int.Parse(num[0]);
            int tol = int.Parse(num[1]);
            if (mulitPic == null)
            {
                mulitPic = new string[tol];
            }
            if (mulitPic.Length != tol)
            {
                throw new System.Exception("error qr");
            }
            else
            {
                bool isReadScan = false;
                if (!string.IsNullOrEmpty(mulitPic[cur - 1]))
                {
                    isReadScan = true;
                }
                mulitPic[cur - 1] = result.Text.Substring(0, result.Text.Length - lastCount);
                if (cur == tol)
                {
                    string resultData = "";
                    foreach (var v in mulitPic)
                    {
                        resultData += v;
                    }

                    isScanning = false;
                    ClosedCamera();

                    if (ScanEndBack != null)
                    {
                        ScanEndBack(resultData);
                        ScanEndBack = null;
                    }
                }
                else
                {
                    goOnObject.gameObject.SetActive(true);
                    if (isReadScan)
                    {
                        goOnText.text = cur + " / " + tol + "扫描完成\r\n此二维码已扫过，请继续下一张";
                    }
                    else
                    {
                        goOnText.text = cur + " / " + tol + "扫描完成\r\n请扫描下一张二维码";
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            reScanObject.SetActive(true);
        }
    }
    public void ClosedCamera()
    {
        if (webCamTexture != null)
        {
            if (webCamTexture.isPlaying)
                webCamTexture.Stop();
            webCamTexture = null;
        }
    }

    public void OnClickReScanBtn()
    {
        reScanObject.SetActive(false);
        StartCoroutine(Scanning(false));
    }

    public void OnClickGoOnScan()
    {
        goOnObject.gameObject.SetActive(false);
        StartCoroutine(Scanning(false));
    }

    public void HideConfirmPage()
    {
        confirmInfoPage.SetActive(false);
    }
    public void ShowConfirmPage()
    {

        confirmInfoPage.SetActive(true);
        sendoutButton.interactable = true;
        confirmForToken.text = leftToken.symbol;
        confirmToToken.text = rightToken.symbol;
        confirmPaidText.text = paidText.text;
        confirmGetText.text = getText.text;
        confirmRateText.text = exchangeRateText.text;
        confirmAddressText.text = paidAddressText.text;
        confirmGasInETH.text = gasText.text;
        confirmGasInCNY.text = gasCNYText.text;
    }
    public void SendOut()
    {
        //HideConfirmPage();
        sendoutButton.interactable = false;
        sendApproveOutButton.interactable = false;
        StartCoroutine(SendETH());
    }

    private IEnumerator SendETH()
    {
        Hashtable myJsonData = new Hashtable();
        myJsonData["id"] = 1;
        myJsonData["jsonrpc"] = "2.0";
        myJsonData["method"] = "eth_sendRawTransaction";

        ArrayList arrayList = new ArrayList(); Debug.Log("提交");
        string oxStr = "0x" + strinfo;
        arrayList.Add(oxStr);
        myJsonData["params"] = arrayList;
        string rpcRequestJson = Json.jsonEncode(myJsonData);
        Debug.Log(rpcRequestJson);
        UnityWebRequest unityRequest = QRPayTools.GetUnityWebRequest(rpcRequestJson);
        yield return unityRequest.SendWebRequest();
        if (unityRequest.error != null)
        {
            PopupLine.Instance.Show("当前网络不可用，请检查网络配置");
        }
        else
        {
            string r = unityRequest.downloadHandler.text;
            Debug.Log(r);
            byte[] results = unityRequest.downloadHandler.data;
            string responseJson = Encoding.UTF8.GetString(results).ToString();
            Debug.Log(responseJson);
            Hashtable table = Json.jsonDecode(responseJson) as Hashtable;
            if (table.Contains("error"))
            {
                string str1 = Json.jsonEncode(table["error"]);

                Hashtable table1 = Json.jsonDecode(str1) as Hashtable;
                //                infoText.text = "Error:" + table1["message"].ToString();
                Debug.Log(table1["message"].ToString());

                string hashStr = "";
                if (table.ContainsKey("result"))
                {
                    hashStr = table["result"].ToString();
                }

                PopUpBox.Instance.Show(() =>
                {
                    ResetPages();
                }, null, "确定", "", "转账失败", "可能因以下原因造成：1.矿工费用设置过低。2.签名数据过期\r\n请重试");
            }
            else
            {
                if (!isApproveAuth)
                {
                    AddLocalContain(table["result"].ToString(), walletAddress, tokenAddress, ethMount, gasText.text, strinfo);
                    ShowSendOverPage();
                }
                else
                {
                    AddLocalContain(table["result"].ToString(), walletAddress, approvingAddress, ethMount, approveGasText.text, strinfo,GetTokenContractByAddress(approvingAddress).symbol,"1");
                    ShowApproveSuccessPage();
                }
            }
        }
    }
    public void ShowSendOverPage()
    {
        sendOverPage.SetActive(true);
        sendSuccessForToken.text = leftToken.symbol;
        sendSuccessToToken.text = rightToken.symbol;
        sendSuccessPaidText.text = paidText.text;
        sendSuccessGetText.text = getText.text;
        sendSuccessRateText.text = exchangeRateText.text;
        sendSuccessAddressText.text = paidAddressText.text;
        sendSuccessGasInETH.text = gasText.text;
        sendSuccessGasInCNY.text = gasCNYText.text;
    }
    public void Complete()
    {
        ResetPages();

        ShowKyberHistory(walletAddress);
        //显示本地交易记录
    }

    private void ResetPages()
    {
        walletListPage.SetActive(false);
        selectedPage.SetActive(false);
        exchangeInfoPage.SetActive(false);
        qrCodePage.SetActive(false);
        scaningPage.SetActive(false);
        confirmInfoPage.SetActive(false);
        sendOverPage.SetActive(false);
        strinfo = "";
        HideRulesPage();
        HideMorePage();
        HideApprovePage();
        HideApproveConfirmPage();
        HideApproveSuccessPage();
        HideApproveInfo();
    }

    public void AddLocalContain(string hash, string srcAddress, string destAddress, BigInteger ethAmount, string gasEth, string data, string tokenSymbol = "", string _isApprove = "0")
    {
        Hashtable hs = new Hashtable();
        TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
        long t = (long)cha.TotalSeconds;
        hs["timeStamp"] = t.ToString();
        hs["hash"] = hash;
        hs["from"] = srcAddress;
        hs["to"] = destAddress;
        hs["value"] = ethAmount.ToString();
        hs["gas"] = gasEth;
        hs["tokenSymbol"] = tokenSymbol;
        hs["contractAddress"] = "";
        hs["confixmations"] = 0 + "";
        hs["fullName"] = "";
        if(_isApprove == "0")
        {
            hs["exchangeFrom"] = leftToken.contractAddress;
            hs["exchangeTo"] = rightToken.contractAddress;
            hs["srcAmount"] = leftInput.text;
            hs["desAmount"] = rightInput.text;
        }
        else
        {
            hs["exchangeFrom"] = "";
            hs["exchangeTo"] ="";
            hs["srcAmount"] = "";
            hs["desAmount"] = "";
        }
        hs["isApprove"] = _isApprove;
        hs["txReceiptStatus"] = "0";
        string jsonValue = Json.jsonEncode(hs);
        string[] tempInfo = PlayerPrefsX.GetStringArray(walletAddress + "KyberHistroy");
        List<string> tempList;

        if (tempInfo.Length > 0)
        {
            tempList = new List<string>(tempInfo);
        }
        else
        {
            tempList = new List<string>();
        }
        tempList.Add(jsonValue);
        Debug.Log("代币发送本地保存 = " + jsonValue);
        PlayerPrefsX.SetStringArray(walletAddress + "KyberHistroy", tempList.ToArray());
    }

    protected List<KyberHistoryRecord> GetLocalData(List<KyberHistoryRecord> _rcordlist, string _walletAddress)
    {
        string[] temp = PlayerPrefsX.GetStringArray(_walletAddress + "KyberHistroy");
        if (temp.Length > 0)
        {
            List<string> tempList = new List<string>(temp);

            List<KyberHistoryRecord> tempNeedAdd = new List<KyberHistoryRecord>();

            for (int j = 0; j < temp.Length; j++)
            {
                Hashtable hs = Json.jsonDecode(temp[j]) as Hashtable;
                if (!ContainsHash(_rcordlist, hs["hash"].ToString()))//将本地记录加入到列表中
                {
                    KyberHistoryRecord tempHistory = new KyberHistoryRecord();
                    long time = long.Parse(hs["timeStamp"].ToString()); Debug.Log("创建时间 " + time);

                    TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
                    long t = (long)cha.TotalSeconds; 

                    long tempTime = t - time; Debug.Log("当前时间 " + t);

                    if (tempTime >= 60 * 60)
                    {
                        tempList.Remove(temp[j]);
                    }
                    else
                    {
                        tempHistory.hash = hs["hash"].ToString();
                        DateTime dt = startTime.AddSeconds(time).ToLocalTime();
                        tempHistory.timeStamp = dt.ToString("yyyy-MM-dd HH:mm:ss");
                        tempHistory.value = hs["value"].ToString();
                        tempHistory.gas = hs["gas"].ToString();
                        tempHistory.from = hs["from"].ToString();
                        tempHistory.to = hs["to"].ToString();

                        tempHistory.isPending = true;
                        //hs["exchangeFrom"] = leftToken.contractAddress;
                        //hs["exchangeTo"] = rightToken.contractAddress;
                        //hs["srcAmount"] = leftInput.text;
                        //hs["desAmount"] = rightInput.text;
                        //hs["isApprove"] = _isApprove;
                        tempHistory.exchangeFrom = hs["exchangeFrom"].ToString();
                        tempHistory.exchangeTo = hs["exchangeTo"].ToString();
                        decimal exchangeValue = 0;
                        decimal.TryParse(hs["srcAmount"].ToString(), out exchangeValue);
                        decimal getValue = 0;
                        decimal.TryParse(hs["desAmount"].ToString(), out getValue);
                        tempHistory.exchangeSrcAmount = Nethereum.Util.UnitConversion.Convert.ToWei(exchangeValue, leftToken.decimals);
                        tempHistory.exchangeDesAmount = Nethereum.Util.UnitConversion.Convert.ToWei(getValue, leftToken.decimals);
                        tempHistory.isApprove = hs["isApprove"].ToString() == "0" ? false : true;
                        tempHistory.txReceiptStatus = "0";
                        tempNeedAdd.Add(tempHistory);
                    }
                }
            }

            if (tempList.Count > 0)
                PlayerPrefsX.SetStringArray(_walletAddress + "KyberHistroy", tempList.ToArray());
            else
                PlayerPrefs.DeleteKey(_walletAddress + "KyberHistroy");

            for (int i = 0; i < tempNeedAdd.Count; i++)
            {
                _rcordlist.Add(tempNeedAdd[i]);
            }
        }

        return _rcordlist;
    }

    protected bool ContainsHash(List<KyberHistoryRecord> data, string hash)
    {
        foreach (var v in data)
        {
            if (v.hash == hash)
            {
                return true;
            }
        }
        return false;
    }

    private void GetWildData(List<KyberHistoryRecord> list, string _address)
    {
        // List<ETHHistoryRcord> ethHistoryList = mainPanel.ethItemList[walletAddress].historyRcordlist;
        List<ETHHistoryRcord> ethHistoryList = new List<ETHHistoryRcord>();
        page = 1;
        HistoryManagerNew.Instance.GetHistory(page, recordCount, walletAddress, delegate (List<ETHHistoryRcord> oo) {
            ethHistoryList = oo;
            if (ethHistoryList.Count > 0)
            {
                List<KyberHistoryRecord> needAdd = new List<KyberHistoryRecord>();
                for (int i = 0; i < ethHistoryList.Count; i++)
                {
                    ETHHistoryRcord tempRecord = ethHistoryList[i];
                    KyberHistoryRecord tempKyber = GetRecordFromList(list, tempRecord.hash);
                    if (tempKyber != null)
                    {
                        //检测数据是否有更新
                        if (tempKyber.isPending)//更新处于打包状态的数据
                        {
                            tempKyber.txReceiptStatus = tempRecord.txReceiptStatus;
                            tempKyber.gas = tempRecord.gas;
                            tempKyber.blockNumber = tempRecord.blockNumber;
                            tempKyber.isPending = false;
                            DeleteLocalData(_address, tempRecord.hash);
                        }
                    }
                    else
                    {
                        //创建新的数据
                        KyberHistoryRecord kyberHistory = new KyberHistoryRecord(tempRecord);
                        string dataStr = kyberHistory.input;
                        if (dataStr.Length < 50)
                            continue;
                        string methodId = dataStr.Substring(0, 10);
                        if (methodId.Equals("0xcb3c28c7"))//Trade
                        {
                            kyberHistory.isApprove = false;
                            string fromStr = "0x" + dataStr.Substring(34, 40);
                            string srcAmoutStr = dataStr.Substring(74, 64);
                            string toStr = "0x" + dataStr.Substring(162, 40);
                            string rateStr = dataStr.Substring(330, 64);
                            KyberTokenService tokenService = new KyberTokenService(ABI, tokenAddress);
                            kyberHistory.exchangeFrom = fromStr;
                            kyberHistory.exchangeTo = toStr;
                            kyberHistory.exchangeRate = tokenService.DecodeBigint("trade", rateStr);
                            kyberHistory.exchangeSrcAmount = tokenService.DecodeBigint("trade", srcAmoutStr);
                            needAdd.Add(kyberHistory);

                            //查询代币记录中是否有该记录的值
                        }
                        else if (methodId.Equals("0x095ea7b3"))//Approve
                        {
                            kyberHistory.isApprove = true;
                            needAdd.Add(kyberHistory);
                        }
                    }
                }
                for (int i = 0; i < needAdd.Count; i++)
                {
                    list.Add(needAdd[i]);
                }
                //显示历史记录
                if(willClear)
                {
                    ShowKyberHistory(walletAddress);
                }
                else
                {
                    RefreshPrefabInfo();
                }
            }

        },"", true, false);
    }

    bool IsContainHash(List<KyberHistoryRecord> list, string _hash)
    {
        foreach (var item in list)
        {
            if (item.hash == _hash)
                return true;
        }
        return false;
    }

    bool IsContainHash(List<ETHHistoryRcord> list, string _hash)
    {
        foreach (var item in list)
        {
            if (item.hash == _hash)
                return true;
        }
        return false;
    }
    KyberHistoryRecord GetRecordFromList(List<KyberHistoryRecord> list, string _hash)
    {
        foreach (var item in list)
        {
            if (item.hash == _hash)
                return item;
        }
        return null;
    }

    private void ShowKyberHistory(string _address)
    {
        if (_address == "")
            return;
        ClearRect(kyberHistoryParent);
        List<KyberHistoryRecord> tempList = kyberHistoryDIc[_address];
        tempList = GetLocalData(tempList, _address);
        tempList.Sort(
                delegate (KyberHistoryRecord p1, KyberHistoryRecord p2)
                {
                    DateTime time1 = DateTime.Parse(p1.timeStamp);
                    DateTime time2 = DateTime.Parse(p2.timeStamp);
                    return time1.CompareTo(time2);//升序
                });
        //debugInfo.text = tempList.Count.ToString();
        List<KyberHistoryPrefab> successPrefab = new List<KyberHistoryPrefab>();
        if (tempList.Count > 0)
        {
            noDataObj.SetActive(false);
            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                KyberHistoryRecord temp = tempList[i];
                
                if (!temp.isApprove)
                {
                    KyberHistoryPrefab prefab = Instantiate(kyberHistoryPrefab);
                    prefab.transform.SetParent(kyberHistoryParent);
                    prefab.transform.localScale = new Vector3(1, 1, 1);
                    kyberHistoryParent.sizeDelta += new Vector2(0, 171);


                    TokenContract srcToken = GetTokenContractByAddress(temp.exchangeFrom);
                    TokenContract desToken = GetTokenContractByAddress(temp.exchangeTo);
                    if (srcToken == null || desToken == null)
                    {
                        Debug.Log(temp.exchangeFrom);
                        return;
                    }
                    prefab.from.text = srcToken.symbol;
                    prefab.to.text = desToken.symbol;
                    decimal paidNumber = Nethereum.Util.UnitConversion.Convert.FromWei(temp.exchangeSrcAmount, srcToken.decimals);
                    prefab.srcAmount = paidNumber;
                    prefab.paid.text = paidNumber.ToString();
                    prefab.data.text = temp.timeStamp;
                    prefab.hax = temp.hash;
                    prefab.gas = temp.gas;
                    prefab.value = temp.value;
                    prefab.block = temp.blockNumber;
                    prefab.paidAddress = temp.from;
                    prefab.getAddress = temp.to;
                    prefab.decimals = temp.tokendecimals;
                    if (temp.isPending && temp.txReceiptStatus == "0")
                    {
                        prefab.stateText.text = "兑换中";
                        prefab.stateText.color = runningHistoryColor;
                        prefab.exchangeState = ExchangeState.Pending;
                        decimal getNumber = Nethereum.Util.UnitConversion.Convert.FromWei(temp.exchangeDesAmount, srcToken.decimals);
                        prefab.desAmount = getNumber;
                        prefab.get.text = getNumber.ToString();
                    }
                    else if (temp.txReceiptStatus == "1")
                    {
                        prefab.stateText.text = "交易成功";
                        prefab.stateText.color = successHistoryColor;
                        prefab.exchangeState = ExchangeState.Success;
                        successPrefab.Add(prefab);
                        //查询代币交易记录，获取成交额度
                        /* if(mainPanel.ethItemList.ContainsKey(walletAddress) && mainPanel.ethItemList[walletAddress].etherc20Dic.ContainsKey(desToken.contractAddress))
                         {
                             List<ETHHistoryRcord> tempErcList = mainPanel.ethItemList[walletAddress].etherc20Dic[desToken.contractAddress];
                             if (tempErcList != null && tempErcList.Count > 0 && IsContainHash(tempErcList, prefab.hax))
                             {
                                 for (int j = 0; j < tempErcList.Count; j++)
                                 {
                                     if (tempErcList[j].hash == prefab.hax)
                                     {
                                         prefab.desAmount = Nethereum.Util.UnitConversion.Convert.FromWei(BigInteger.Parse(tempErcList[j].value), desToken.decimals);
                                         prefab.get.text = prefab.desAmount.ToString("f8");
                                         break;
                                     }
                                 }
                             }
                         }*/
                    }
                    else
                    {
                        prefab.stateText.text = "交易失败";
                        prefab.stateText.color = faildHistoryColor;
                        prefab.exchangeState = ExchangeState.Faild;
                        prefab.get.text = "0";
                        prefab.desAmount = 0;
                    }
                    prefab.GetComponent<Button>().onClick.AddListener(delegate () { kyberHistroyInfoPage.Show(prefab); });
                }
                else
                {
                    TokenContract _token = GetTokenInfoByAddress(temp.to);
                    if (_token == null)
                    {
                        continue;
                    }
                    ApproveHistoryPrefab prefab = Instantiate(approveHistoryPrefab);
                    prefab.transform.SetParent(kyberHistoryParent);
                    prefab.transform.localScale = new Vector3(1, 1, 1);
                    kyberHistoryParent.sizeDelta += new Vector2(0, 171);
                    prefab.from = temp.from;
                    prefab.des = temp.to;
                    prefab.gas = temp.gas;
                    prefab.value = temp.value;
                    prefab.hash = temp.hash;
                    prefab.block = temp.blockNumber;
                    
                    prefab.tipsText.text = _token.symbol + " 授权合约";
                    prefab.date.text = temp.timeStamp;
                    if (temp.isPending && temp.txReceiptStatus == "0")
                    {
                        prefab.stateText.text = "授权中";
                        prefab.stateText.color = runningHistoryColor;
                        prefab.exchangeState = ExchangeState.Pending;
                    }
                    else if (temp.txReceiptStatus == "1")
                    {
                        prefab.stateText.text = "授权成功";
                        prefab.stateText.color = successHistoryColor;
                        prefab.exchangeState = ExchangeState.Success;
                    }
                    else
                    {
                        prefab.stateText.text = "授权失败";
                        prefab.stateText.color = faildHistoryColor;
                        prefab.exchangeState = ExchangeState.Faild;
                    }
                    prefab.GetComponent<Button>().onClick.AddListener(delegate () { approveHistoryInfoPage.Show(prefab); });
                }
            }
            kyberHistoryParent.sizeDelta += new Vector2(0, 100);
            addMoreButton.gameObject.SetActive(true);
            addMoreButton.transform.SetSiblingIndex(kyberHistoryParent.childCount);
            if (tempList.Count >= recordCount)
            {
                addMoreButton.GetComponentInChildren<Text>().text = "点击加载更多";
                addMoreButton.onClick.RemoveAllListeners();
                addMoreButton.onClick.AddListener(AddMore);
            }
            else
            {
                addMoreButton.GetComponentInChildren<Text>().text = "没有更多信息";
                addMoreButton.onClick.RemoveAllListeners();
            }
            GetTokenValue(successPrefab);
        }
        else
        {
            Debug.Log("未能获取到交易记录");
            noDataObj.SetActive(true);
        }
    }

    public string GetSymbolByAddress(string _address)
    {
        for (int i = 0; i < supportedList.Count; i++)
        {
            if (supportedList[i].contractAddress == _address)
                return supportedList[i].symbol;
        }
        return "";
    }

    public TokenContract GetTokenContractByAddress(string _address)
    {
        for (int i = 0; i < supportedList.Count; i++)
        {
            if (supportedList[i].contractAddress.ToLower() == _address)
                return supportedList[i];
        }
        return null;
    }


    public void DeleteLocalData(string _address, string _hash)
    {
        string[] temp = PlayerPrefsX.GetStringArray(_address + "KyberHistroy");
        if (temp.Length > 0)
        {
            List<string> tempList = new List<string>(temp);
            
            List<KyberHistoryRecord> tempNeedRemove = new List<KyberHistoryRecord>();

            for (int j = 0; j < temp.Length; j++)
            {
                Hashtable hs = Json.jsonDecode(temp[j]) as Hashtable;
                if (hs["hash"].ToString() == _hash)
                {
                    Debug.Log("删除" + _hash);
                    tempList.Remove(temp[j]);
                }
            }

            if (tempList.Count > 0)
                PlayerPrefsX.SetStringArray(_address + "KyberHistroy", tempList.ToArray());
            else
            {
                PlayerPrefs.DeleteKey(_address + "KyberHistroy");
            }
        }
    }

    public void ShowMorePage()
    {
        morePage.SetActive(true);
    }
    public void HideMorePage()
    {
        morePage.SetActive(false);
    }
    public void ShowRulesPage()
    {
        rulesPage.SetActive(true);
    }
    public void HideRulesPage()
    {
        rulesPage.SetActive(false);
    }

    public void ShowApprovePage()
    {
        if (isGetting)
        {
            PopupLine.Instance.Show("正在获取支持的币种");
            return;
        }
        else if (!isGetSupportedList)
        {
            PopupLine.Instance.Show("无法获取到可支持的币种");
            return;
        }
        if (walletAddress == "" || walletAddress == null)
        {
            PopupLine.Instance.Show("请先选择钱包");
            return;
        }
        approvePage.SetActive(true);
        ShowApproveList();
    }

    public void HideApprovePage()
    {
        approvePage.SetActive(false);
        ClearRect(approveAuthParent);
        searchApproveInput.text = "";
        HideMorePage();
        HideApproveWarnPage();
    }

    public void ShowHelpPage()
    {
        helpPage.SetActive(true);
    }
    public void HideHelpPage()
    {
        helpPage.SetActive(false);
    }

    public void ShowApproveList()
    {

        ClearRect(approveAuthParent);
        for (int i = 0; i < supportedList.Count; i++)
        {
            TokenContract tempContract = supportedList[i];
            ApproveAuthPrefab prefab = Instantiate(approveAuthPrefab);
            prefab.transform.SetParent(approveAuthParent);
            prefab.transform.localScale = new Vector3(1, 1, 1);
            approveAuthParent.sizeDelta = new Vector2(0, approveAuthParent.childCount * 130);
            prefab.symbol = tempContract.symbol;
            prefab._name = tempContract._name;
            prefab.decimals = tempContract.decimals;
            prefab.contractAddress = tempContract.contractAddress;
            prefab.tokenName.text = tempContract._name;
            prefab.tokenSymbol.text = tempContract.symbol;
            if (IsETH(prefab.contractAddress))
                prefab.tokenImage.sprite = ethIcon;
            else
                StartCoroutine(TextureUIAsset._Instance.LoadImage(prefab.tokenImage, tempContract.iconPath));
            if (tempContract.allowance >= 0) // 需查询当前是否正在授权进行中
            {
                prefab.ShowState(ApproveState.No);
                List<KyberHistoryRecord> tempList = kyberHistoryDIc[walletAddress];
                if (tempList != null && tempList.Count > 0)
                {
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        if (tempList[j].to == prefab.contractAddress && tempList[j].isPending)
                        {
                            prefab.ShowState(ApproveState.Pending);
                            break;
                        }
                    }
                }
            }
            else
            {
                prefab.ShowState(ApproveState.Success);
            }
        }
    }
    public void ShowApproveInfo(string prefabAddress)
    {
        List<KyberHistoryRecord> tempList = kyberHistoryDIc[walletAddress];
        if (tempList != null && tempList.Count > 0)
        {
            for (int j = 0; j < tempList.Count; j++)
            {
                if (tempList[j].to == prefabAddress && tempList[j].isPending)
                {
                    PopupLine.Instance.Show("授权交易正在进行，请稍后");
                    return;
                }
            }
        }
        approveInfo.SetActive(true);
        approvingAddress = prefabAddress;
        approveSrc.text = walletAddress.Substring(0,14) + "..." + walletAddress.Substring(walletAddress.Length - 13, 13);
        approveDes.text = prefabAddress.Substring(0, 14) + "..." + prefabAddress.Substring(prefabAddress.Length - 13, 13);
        tempGasPrice = HttpManager._Intance.ethGasPrice_int64;
        if (tempGasPrice < 10 * gWei)
        {
            approveSlider.minValue = tempGasPrice / 5;
            approveSlider.maxValue = tempGasPrice * 4;
        }
        else if (tempGasPrice >= 10 * gWei && tempGasPrice <= 30 * gWei)
        {
            approveSlider.minValue = tempGasPrice / 2;
            approveSlider.maxValue = tempGasPrice * 3;
        }
        else if (tempGasPrice > 30)
        {
            approveSlider.minValue = tempGasPrice / 2;
            approveSlider.maxValue = tempGasPrice * 2;
        }
        approveSlider.value = tempGasPrice;
        decimal gasReal = GetDecimalFromBigDecimal(tempGasPrice * HttpManager._Intance.kyberGas_Limit/3);
        approveGasText.text = gasReal.ToString("f8") + "  eth";
        approveGasCNYText.text = "≈ " + (gasReal * HttpManager._Intance.eth_RMB).ToString("f2") + " CNY";
    }

    public void OnApproveSliderValueChanged()
    {
        decimal gasReal = GetDecimalFromBigDecimal((decimal)approveSlider.value * HttpManager._Intance.kyberGas_Limit/3);
        approveGasText.text = gasReal.ToString("f8") + "  eth";
        approveGasCNYText.text = "≈ " + (gasReal * HttpManager._Intance.eth_RMB).ToString("f2") + " CNY";
    }
    public void HideApproveInfo()
    {
        approveInfo.SetActive(false);
    }

    public void SearchAuthByTokenName()
    {
        string str = searchApproveInput.text;
        int countInView = 0;
        if (str.Length == 0)
        {
            for (int i = 0; i < approveAuthParent.childCount; i++)
            {
                approveAuthParent.GetChild(i).gameObject.SetActive(true);
                countInView++;
            }
        }
        else
        {
            for (int i = 0; i < approveAuthParent.childCount; i++)
            {
                ApproveAuthPrefab prefab = approveAuthParent.GetChild(i).GetComponent<ApproveAuthPrefab>();
                if (prefab.symbol.IndexOf(str.ToUpper()) >= 0)
                {
                    prefab.gameObject.SetActive(true);
                    countInView++;
                }
                else
                {
                    prefab.gameObject.SetActive(false);
                }
            }
        }
        approveAuthParent.anchoredPosition = Vector2.zero;
        approveAuthParent.sizeDelta = new Vector2(0, countInView * 114f);
    }

    public void ShowApproveQRPage()
    {
        Debug.Log("ShowApproveQRPage");
        isApproveAuth = true;
        qrCodePage.SetActive(true);
        TransactionInput input = CreatTransactionInput(approvingAddress, -1);
        string tokenData = input.Data;
        QRPayTools.CreateNoSignPayQRInfo_ETH(walletAddress, approvingAddress, 0, new BigInteger(HttpManager._Intance.kyberGas_Limit/3), new BigInteger(approveSlider.value), this.gameObject, ETHInfo._url, tokenData, delegate (string qrinfo)
        {
            CreatQRImage(qrinfo);
        });//代币兑换 src为ETH时，amount的值，需要与data中srcAmount的值对应
    }

    public void ShowApproveConfirmPage()
    {
        approveConfirmPage.SetActive(true);
        sendApproveOutButton.interactable = true;
        approveConfirmSrc.text = approveSrc.text;
        approveConfirmDes.text = approveDes.text;
        approveConfirmGas.text = approveGasText.text;
        approveConfirmGasCNY.text = approveGasCNYText.text;
    }

    public void HideApproveConfirmPage()
    {
        approveConfirmPage.SetActive(false);
    }

    public void ShowApproveSuccessPage()
    {
        approveSuccessPage.SetActive(true);
        approveSuccessSrc.text = approveSrc.text;
        approveSuccessDes.text = approveDes.text;
        approveSuccessGas.text = approveGasText.text;
        approveSuccessGasCNY.text = approveGasCNYText.text;
    }
    public void HideApproveSuccessPage()
    {
        approveSuccessPage.SetActive(false);
    }

    public void ShowApproveWarnPage()
    {
        approveWarnPage.SetActive(true);
    }
    public void HideApproveWarnPage()
    {
        approveWarnPage.SetActive(false);
    }

    public IEnumerator GetNodeJsRequest(string op, List<KeyValuePair<string, string>> parms, System.Action<string> callBack)
    {
        //http://47.96.131.169:8888/getgoodslist?sign=bb432b049e9fa65baad8ca6cb8ff6fac
        //http://47.96.131.169:8888/createorder?userid=26&address=1234567&hash=7893&goodid=1&sign=1dc86eb78e84ed92c9d24f53a2f16a4f;

        string url =HttpManager._Intance.url_nodeJs + op + "?";
        string singHx = "";
        foreach (var v in parms)
        {
            url += v.Key + "=" + v.Value + "&";
            singHx += v.Value;
        }
        //string m_node_tt = "ssydds129";
        //string signInfo = EncryptWithMD5(singHx + m_node_tt);
        //url += "sign=" + signInfo;

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.timeout = 10;

        yield return www.SendWebRequest();

        if (!www.isNetworkError)
        {
            if (www.error != null)
            {
                if (callBack != null)
                {
                    callBack(null);
                }
            }
            else
            {
                try
                {
                    string jsonString = GetUTF8String(www.downloadHandler.data);
                    callBack(jsonString);
                }
                catch
                {
                    if (callBack != null)
                    {
                        callBack(null);
                    }
                }
            }
        }
        else
        {
            if (callBack != null)
            {
                callBack(null);
            }
        }
    }

    private string EncryptWithMD5(string source)
    {
        byte[] sor = Encoding.UTF8.GetBytes(source);
        MD5 md5 = MD5.Create();
        byte[] result = md5.ComputeHash(sor);
        StringBuilder strbul = new StringBuilder(40);
        for (int i = 0; i < result.Length; i++)
        {
            strbul.Append(result[i].ToString("x2"));

        }
        return strbul.ToString().ToLower();
    }

    private string GetUTF8String(byte[] buffer)
    {
        if (buffer == null)
            return null;

        if (buffer.Length <= 3)
        {
            return Encoding.UTF8.GetString(buffer);
        }

        byte[] bomBuffer = new byte[] { 0xef, 0xbb, 0xbf };

        if (buffer[0] == bomBuffer[0]
            && buffer[1] == bomBuffer[1]
            && buffer[2] == bomBuffer[2])
        {
            return new UTF8Encoding(false).GetString(buffer, 3, buffer.Length - 3);
        }

        return Encoding.UTF8.GetString(buffer);
    }

    private void RefreshPrefabInfo()
    {

    }

    private void AddMore()
    {
        page++;
        List<ETHHistoryRcord> ethHistoryList = new List<ETHHistoryRcord>();
        HistoryManagerNew.Instance.GetHistory(page, recordCount, walletAddress, delegate (List<ETHHistoryRcord> oo) {
            ethHistoryList = oo;
            if (ethHistoryList.Count > 0)
            {
                List<KyberHistoryRecord> needAdd = new List<KyberHistoryRecord>();
                for (int i = 0; i < ethHistoryList.Count; i++)
                {
                    KyberHistoryRecord kyberHistory = new KyberHistoryRecord(ethHistoryList[i]);
                    string dataStr = kyberHistory.input;
                    if (dataStr.Length < 50)
                        continue;
                    string methodId = dataStr.Substring(0, 10);
                    if (methodId.Equals("0xcb3c28c7"))//Trade
                    {
                        kyberHistory.isApprove = false;
                        string fromStr = "0x" + dataStr.Substring(34, 40);
                        string srcAmoutStr = dataStr.Substring(74, 64);
                        string toStr = "0x" + dataStr.Substring(162, 40);
                        string rateStr = dataStr.Substring(330, 64);
                        KyberTokenService tokenService = new KyberTokenService(ABI, tokenAddress);
                        kyberHistory.exchangeFrom = fromStr;
                        kyberHistory.exchangeTo = toStr;
                        kyberHistory.exchangeRate = tokenService.DecodeBigint("trade", rateStr);
                        kyberHistory.exchangeSrcAmount = tokenService.DecodeBigint("trade", srcAmoutStr);
                        needAdd.Add(kyberHistory);

                        //查询代币记录中是否有该记录的值
                    }
                    else if (methodId.Equals("0x095ea7b3"))//Approve
                    {
                        kyberHistory.isApprove = true;
                        needAdd.Add(kyberHistory);
                    }
                }
                AddMoreHistory(needAdd);
            }

        }, "", true, false);
    }

    private void AddMoreHistory(List<KyberHistoryRecord> tempList)
    {
        tempList.Sort(
               delegate (KyberHistoryRecord p1, KyberHistoryRecord p2)
               {
                   DateTime time1 = DateTime.Parse(p1.timeStamp);
                   DateTime time2 = DateTime.Parse(p2.timeStamp);
                   return time1.CompareTo(time2);//升序
                });
        //debugInfo.text = tempList.Count.ToString();
        List<KyberHistoryPrefab> successPrefab = new List<KyberHistoryPrefab>();
        if (tempList.Count > 0)
        {
            noDataObj.SetActive(false);
            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                KyberHistoryRecord temp = tempList[i];

                if (!temp.isApprove)
                {
                    KyberHistoryPrefab prefab = Instantiate(kyberHistoryPrefab);
                    prefab.transform.SetParent(kyberHistoryParent);
                    prefab.transform.localScale = new Vector3(1, 1, 1);
                    kyberHistoryParent.sizeDelta += new Vector2(0, 171);


                    TokenContract srcToken = GetTokenContractByAddress(temp.exchangeFrom);
                    TokenContract desToken = GetTokenContractByAddress(temp.exchangeTo);
                    if (srcToken == null || desToken == null)
                    {
                        Debug.Log(temp.exchangeFrom);
                        return;
                    }
                    prefab.from.text = srcToken.symbol;
                    prefab.to.text = desToken.symbol;
                    decimal paidNumber = Nethereum.Util.UnitConversion.Convert.FromWei(temp.exchangeSrcAmount, srcToken.decimals);
                    prefab.srcAmount = paidNumber;
                    prefab.paid.text = paidNumber.ToString();
                    prefab.data.text = temp.timeStamp;
                    prefab.hax = temp.hash;
                    prefab.gas = temp.gas;
                    prefab.value = temp.value;
                    prefab.block = temp.blockNumber;
                    prefab.paidAddress = temp.from;
                    prefab.getAddress = temp.to;
                    prefab.decimals = temp.tokendecimals;
                    if (temp.isPending && temp.txReceiptStatus == "0")
                    {
                        prefab.stateText.text = "兑换中";
                        prefab.stateText.color = runningHistoryColor;
                        prefab.exchangeState = ExchangeState.Pending;
                        decimal getNumber = Nethereum.Util.UnitConversion.Convert.FromWei(temp.exchangeDesAmount, srcToken.decimals);
                        prefab.desAmount = getNumber;
                        prefab.get.text = getNumber.ToString();
                    }
                    else if (temp.txReceiptStatus == "1")
                    {
                        prefab.stateText.text = "交易成功";
                        prefab.stateText.color = successHistoryColor;
                        prefab.exchangeState = ExchangeState.Success;
                        successPrefab.Add(prefab);
                        //查询代币交易记录，获取成交额度
                        /* if (mainPanel.ethItemList.ContainsKey(walletAddress) && mainPanel.ethItemList[walletAddress].etherc20Dic.ContainsKey(desToken.contractAddress))
                         {
                             List<ETHHistoryRcord> tempErcList = mainPanel.ethItemList[walletAddress].etherc20Dic[desToken.contractAddress];
                             if (tempErcList != null && tempErcList.Count > 0 && IsContainHash(tempErcList, prefab.hax))
                             {
                                 for (int j = 0; j < tempErcList.Count; j++)
                                 {
                                     if (tempErcList[j].hash == prefab.hax)
                                     {
                                         prefab.desAmount = Nethereum.Util.UnitConversion.Convert.FromWei(BigInteger.Parse(tempErcList[j].value), desToken.decimals);
                                         prefab.get.text = prefab.desAmount.ToString("f8");
                                         break;
                                     }
                                 }
                             }
                         }*/
                    }
                    else
                    {
                        prefab.stateText.text = "交易失败";
                        prefab.stateText.color = faildHistoryColor;
                        prefab.exchangeState = ExchangeState.Faild;
                        prefab.get.text = "0";
                        prefab.desAmount = 0;
                    }
                    prefab.GetComponent<Button>().onClick.AddListener(delegate () { kyberHistroyInfoPage.Show(prefab); });
                }
                else
                {
                    TokenContract _token = GetTokenInfoByAddress(temp.to);
                    if (_token == null)
                    {
                        continue;
                    }
                    ApproveHistoryPrefab prefab = Instantiate(approveHistoryPrefab);
                    prefab.transform.SetParent(kyberHistoryParent);
                    prefab.transform.localScale = new Vector3(1, 1, 1);
                    kyberHistoryParent.sizeDelta += new Vector2(0, 171);
                    prefab.from = temp.from;
                    prefab.des = temp.to;
                    prefab.gas = temp.gas;
                    prefab.value = temp.value;
                    prefab.hash = temp.hash;
                    prefab.block = temp.blockNumber;

                    prefab.tipsText.text = _token.symbol + " 授权合约";
                    prefab.date.text = temp.timeStamp;
                    if (temp.isPending && temp.txReceiptStatus == "0")
                    {
                        prefab.stateText.text = "授权中";
                        prefab.stateText.color = runningHistoryColor;
                        prefab.exchangeState = ExchangeState.Pending;
                    }
                    else if (temp.txReceiptStatus == "1")
                    {
                        prefab.stateText.text = "授权成功";
                        prefab.stateText.color = successHistoryColor;
                        prefab.exchangeState = ExchangeState.Success;
                    }
                    else
                    {
                        prefab.stateText.text = "授权失败";
                        prefab.stateText.color = faildHistoryColor;
                        prefab.exchangeState = ExchangeState.Faild;
                    }
                    prefab.GetComponent<Button>().onClick.AddListener(delegate () { approveHistoryInfoPage.Show(prefab); });
                }
            }
            addMoreButton.gameObject.SetActive(true);
            addMoreButton.transform.SetSiblingIndex(kyberHistoryParent.childCount);
            if (tempList.Count >= recordCount)
            {
                addMoreButton.GetComponentInChildren<Text>().text = "点击加载更多";
                addMoreButton.onClick.RemoveAllListeners();
                addMoreButton.onClick.AddListener(AddMore);
            }
            else
            {
                addMoreButton.GetComponentInChildren<Text>().text = "没有更多信息";
                addMoreButton.onClick.RemoveAllListeners();
            }
            GetTokenValue(successPrefab);

        }
        else
        {
            addMoreButton.GetComponentInChildren<Text>().text = "没有更多信息";
            addMoreButton.onClick.RemoveAllListeners();
        }
    }


    private void GetTokenValue(List<KyberHistoryPrefab> list)
    {
        string msg = "'";
        for (int i = 0; i < list.Count; i++)
        {
            if(i == list.Count -1)
            {
                msg += list[i].hax + "'";
            }
            else
            {
                msg += list[i].hax + "','";
            }
        }
        string query = "select * FROM " + SqliteHelperA.SQL_TABLE_NAME +  " where " + SqliteHelperA.SQL_COL_HASH +" in ("  + msg + ")";
        HistoryManagerNew.Instance.RunCustomQuery(walletAddress, query, (r) =>
        {
            while (r.Read())
            {
                string _hash = r.GetValue(r.GetOrdinal(SqliteHelperA.SQL_COL_HASH)).ToString();
                string _value = r.GetValue(r.GetOrdinal(SqliteHelperA.SQL_COL_VALUE)).ToString();
                string _to = r.GetValue(r.GetOrdinal(SqliteHelperA.SQL_COL_TO)).ToString();
                for (int i = 0; i < list.Count; i++)
                {
                    TokenContract destToken = GetTokenInfoBySymbol(list[i].to.text);
                    if (list[i].hax == _hash)
                    {
                        if (walletAddress.ToLower() == _to)
                        {
                            list[i].desAmount = Nethereum.Util.UnitConversion.Convert.FromWei(BigInteger.Parse(_value), destToken.decimals);
                            list[i].get.text = list[i].desAmount.ToString("f8");
                            break;
                        }
                        else
                        {
                      //      Debug.Log(walletAddress + "|" + _to);
                        }
                    }
                }
            }
        },true);
    }
}

//合约类===============================================================================================================
public class KyberTokenService
{
    public string tokenContractAddress;
    public Contract contract;
    public TokenInfo TokenInfo = new TokenInfo();

    public KyberTokenService(string ABI, string tokenAddress)
    {
        tokenContractAddress = tokenAddress;
        this.contract = new Contract(null, ABI, tokenContractAddress);
    }

    public CallInput CreateCallInput(string variableName, params object[] p)
    {
        var function = contract.GetFunction(variableName);
        return function.CreateCallInput(p);
    }

    public T DecodeVariable<T>(string variableName, string result)
    {
        var function = contract.GetFunction(variableName);
        try
        {
            return function.DecodeSimpleTypeOutput<T>(result); // this results in an error if BigInteger is 0
        }
        catch
        {
            return default(T);
        }
    }
    public IEnumerator GetTokenInfo(string method, params object[] p)
    {
        var currencyInfoRequest = new EthCallUnityRequest(ETHInfo._url);
        CallInput callInput = CreateCallInput(method, p);
        yield return currencyInfoRequest.SendRequest(callInput, BlockParameter.CreateLatest());
        BigInteger value = DecodeVariable<BigInteger>(method, currencyInfoRequest.Result);
        string str = value.ToString();
        Debug.Log(str.Insert(str.Length - 18, "."));
    }

    public string DecodeString(string method, string str)
    {
        return DecodeVariable<string>(method, str);
    }

    public BigInteger DecodeBigint(string method, string str)
    {
        return DecodeVariable<BigInteger>(method, str);
    }

    public IEnumerator GetExpectecRate(Action<BigInteger> Callback = null, params object[] p)
    {
        string method = "getExpectedRate";
        var currencyInfoRequest = new EthCallUnityRequest(ETHInfo._url);
        CallInput callInput = CreateCallInput(method, p);
        yield return currencyInfoRequest.SendRequest(callInput, BlockParameter.CreateLatest());
        BigInteger value = DecodeVariable<BigInteger>(method, currencyInfoRequest.Result);
        if (Callback != null)
        {
            Callback(value);
        }
    }

    public IEnumerator GetExpectecRate2(Action<BigInteger> Callback = null, params object[] p)
    {
        string method = "getExpectedRate";
        CallInput callInput = CreateCallInput(method, p);
        string rpc_json = GetTokenRPC_Json(callInput.Data);
        UnityWebRequest unityRequest = QRPayTools.GetUnityWebRequest(rpc_json);
        yield return unityRequest.SendWebRequest();
        if (unityRequest.error != null)
        {
            Debug.Log(unityRequest.error);
        }
        else
        {
            byte[] results = unityRequest.downloadHandler.data;
            string responseJson = Encoding.UTF8.GetString(results).ToString();
            Hashtable ht = Json.jsonDecode(responseJson) as Hashtable;
            string reslt = ht["result"].ToString();
            BigInteger value = DecodeVariable<BigInteger>(method, reslt);
            if (Callback != null)
            {
                Callback(value);
            }
        }
    }

    public string GetTokenRPC_Json(string data)
    {
        Hashtable myJsonData = new Hashtable();
        myJsonData["id"] = 1;
        myJsonData["jsonrpc"] = "2.0";
        myJsonData["method"] = "eth_call";
        ArrayList arrayList = new ArrayList();
        Hashtable json1 = new Hashtable();
        json1["data"] = data;
        json1["to"] = tokenContractAddress;
        arrayList.Add(json1);
        arrayList.Add("latest");
        myJsonData["params"] = arrayList;
        return Json.jsonEncode(myJsonData);
    }


    public IEnumerator GetAllowance(Action<BigInteger> Callback = null, params object[] p)
    {
        string method = "allowance";
        CallInput callInput = CreateCallInput(method, p);
        string rpc_json = GetTokenRPC_Json(callInput.Data);
        UnityWebRequest unityRequest = QRPayTools.GetUnityWebRequest(rpc_json);
        yield return unityRequest.SendWebRequest();
        if (unityRequest.error != null)
        {
            Debug.Log(unityRequest.error);
        }
        else
        {
            byte[] results = unityRequest.downloadHandler.data;
            string responseJson = Encoding.UTF8.GetString(results).ToString();
            Hashtable ht = Json.jsonDecode(responseJson) as Hashtable;
            string reslt = ht["result"].ToString();
            BigInteger value = DecodeVariable<BigInteger>(method, reslt);
            if (Callback != null)
            {
                Callback(value);
            }
        }
    }

    public IEnumerator GetUserCapInWei(string address)
    {
        string method = "getExpectedRate";
        var currencyInfoRequest = new EthCallUnityRequest(ETHInfo._url);
        CallInput callInput = CreateCallInput(method, address);
        yield return currencyInfoRequest.SendRequest(callInput, BlockParameter.CreateLatest());
        BigInteger value = DecodeVariable<BigInteger>(method, currencyInfoRequest.Result);
    }

    public TransactionInput CreateTradeFundsTransactionInput(
       string addressSrc,
       BigInteger srcAmount,
       string addressDest,
       string walletAddress,
       BigInteger maxDestAmount,
       BigInteger minConversionRate,
       string walletId = "0")
    {
        var function = contract.GetFunction("trade");
        Debug.Log(String.Format("addressSrc:{0},srcAmount:{1},addressDest:{2},walletAddress:{3},maxDestAmount:{4},minConversionRate:{5},walletId:{6}",
            addressSrc, srcAmount, addressDest, walletAddress, maxDestAmount, minConversionRate, walletId));
        return function.CreateTransactionInput(addressSrc, addressSrc, srcAmount, addressDest, walletAddress, maxDestAmount, minConversionRate, walletId);
    }

    public TransactionInput CreatApproveFundsTransactionInput(
        string kyberAddress,
        BigInteger srcWeiPrice
        )
    {
        var function = contract.GetFunction("approve");
        return function.CreateTransactionInput(kyberAddress, kyberAddress, srcWeiPrice);
    }
    public IEnumerator GetTotalSupply()
    {
        string method = "totalSupply";
        var currencyInfoRequest = new EthCallUnityRequest(ETHInfo._url);
        CallInput callInput = CreateCallInput(method);
        yield return currencyInfoRequest.SendRequest(callInput, BlockParameter.CreateLatest());
        BigInteger value = DecodeVariable<BigInteger>(method, currencyInfoRequest.Result);
        Debug.Log(KyberTools.GetDecimalFromBigint(value));
    }

 

}

public class TokenContract
{
    public string symbol;
    public string cmcName;
    public string _name;
    public int decimals;
    public string contractAddress;
    public string iconPath;
    public BigInteger allowance;
}

public class KyberHistoryRecord : ETHHistoryRcord
{
    public string exchangeFrom;
    public string exchangeTo;
    public BigInteger exchangeSrcAmount;
    public BigInteger exchangeDesAmount;
    public BigInteger exchangeRate;
    public bool isApprove;

    public KyberHistoryRecord(ETHHistoryRcord temp)
    {
        tokendecimals = temp.tokendecimals;
        blockNumber = temp.blockNumber;
        timeStamp = temp.timeStamp;
        hash = temp.hash;
        from = temp.from;
        to = temp.to;
        value = temp.value;
        gas = temp.gas;
        isOverTime = temp.isOverTime;
        txReceiptStatus = temp.txReceiptStatus;
        input = temp.input;
        tokenSymbol = temp.tokenSymbol;
    }
    public KyberHistoryRecord()
    {
    }
}

public enum ExchangeState
{
    Faild,
    Pending,
    Success
}

public enum ApproveState
{
    No,
    Pending,
    Success
}
