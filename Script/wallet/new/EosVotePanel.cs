using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using System.Net;

public class EosVotePanel : MonoBehaviour
{

    public RectTransform voteListParent;
    public RectTransform selectListParent;
    public TextAsset country;
    public VoteInfoPrefab voteInfoPrefab;
    private decimal total_producer_vote_weight = 1;
    private string more = "";
    internal Dictionary<string, string> countryDic = new Dictionary<string, string>();
    private List<VoteInfo> voteList = new List<VoteInfo>();
    private List<VoteInfo> searchList = new List<VoteInfo>();
    private Dictionary<string, VoteInfoPrefab> showingPrefabDic = new Dictionary<string, VoteInfoPrefab>();
    private Dictionary<string, VoteInfoPrefab> showingInSelectList = new Dictionary<string, VoteInfoPrefab>();
    EOSWalletInfoPanel infoPanel;
    public static EosVotePanel _instance;
    public Text selectInfo;
    public Text cancleInfo;
    public Color activeColor;
    public Color inactiveColor;
    public GameObject supperDotPage;
    public GameObject selectDotPage;
    public Text supperText;
    public GameObject supperLine;
    public Text seleteText;
    public GameObject selectLine;
    public InputField searchInput;
    public Button voteButton;
    public Button cancleButton;
    public GameObject voteConfirmTips;
    public List<string> selectedList = new List<string>();
    public List<string> cancleList = new List<string>();
    public List<string> votedList = new List<string>();
    public Text ticketText;
    private RectTransform targetSearchParent;
    private float rectHight;
    private float prefabHight;
    private float refreshTime;
    private bool isSearching;
    // Use this for initialization
    void Awake()
    {
        _instance = this;
        GetCountryList();
    }

    private void Start()
    {
        rectHight = voteListParent.position.y;
        prefabHight = voteInfoPrefab.GetComponent<RectTransform>().sizeDelta.y;
    }
    // Update is called once per frame
    void Update()
    {
        if(cancleList.Count == 0 || voteListParent.childCount == 0)
        {
            cancleButton.interactable = false;
        }
        else
        {
            cancleButton.interactable = true;
        }

        bool isTheSame = true;
        for (int i = 0; i < selectedList.Count; i++)
        {
            if (!infoPanel.eoswalletInfo.votedList.Contains(selectedList[i]))
            {
                isTheSame = false;
                break;
            }
        }
        if(selectedList.Count != infoPanel.eoswalletInfo.votedList.Count)
            isTheSame = false;
        if (isTheSame)
            voteButton.interactable = false;
        else
            voteButton.interactable = true;
    }

    private void GetCountryList()
    {
        string countryStr = country.text;
        string[] strAtrr = countryStr.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < strAtrr.Length; i++)
        {
            string[] tempArr = strAtrr[i].Split('|');
            if (tempArr.Length > 0)
                countryDic.Add(tempArr[0], tempArr[1]);
        }
    }
    public void HideVotePage()
    {
        //ClearRect(voteListParent);
        //ClearRect(selectListParent);
        //showingPrefabDic.Clear();
        gameObject.SetActive(false);
    }

    public void ShowVotePage()
    {
        //List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        //ws.Add(new KeyValuePair<string, string>("op", "listproducers"));
        infoPanel = PanelManager._Instance._eosWalletInfoPanel;
        

        string cpu_weight = infoPanel.eoswalletInfo.self_delegated_bandwidth.cpu_weight.TrimEnd("EOS".ToCharArray());
        string net_weight = infoPanel.eoswalletInfo.self_delegated_bandwidth.net_weight.TrimEnd("EOS".ToCharArray());
        ticketText.text = "已抵押 " +  (float.Parse(cpu_weight) + float.Parse(net_weight)).ToString() + "票";

        gameObject.SetActive(true);
        if (voteList.Count == 0)
        {
            refreshTime = Time.time;
            StartCoroutine(HttpManager._Intance.EOSSendRequest("listproducers", null, true, VoteInfoCallBack, null, true));
        }
        else if(Time.time - refreshTime > 600)
        {
            refreshTime = Time.time;//超过10分钟刷新
            StartCoroutine(HttpManager._Intance.EOSSendRequest("listproducers", null, true, VoteInfoCallBack, null, true));Debug.Log("涮新");
        }
        else
        {
            RefreshVoteInfo();
        }
    }
    private void VoteInfoCallBack(Hashtable h)
    {
        ShowSupperList();
        total_producer_vote_weight = decimal.Parse(h["total_producer_vote_weight"].ToString());
        more = h["more"].ToString();
        ArrayList hashList = h["rows"] as ArrayList;

        if (hashList.Count > 0)
        {
            if (voteList.Count == 0)
            {
                for (int i = 0; i < hashList.Count; i++)
                {
                    Hashtable hash = hashList[i] as Hashtable;
                    VoteInfo voteInfo = new VoteInfo();
                    voteInfo.location = int.Parse(hash["location"].ToString());
                    voteInfo.total_votes = decimal.Parse(hash["total_votes"].ToString());
                    voteInfo.is_active = int.Parse(hash["is_active"].ToString()) == 0 ? false : true;
                    voteInfo.producer_key = hash["producer_key"].ToString();
                    voteInfo.url = hash["url"].ToString();
                    voteInfo.last_claim_time = long.Parse(hash["last_claim_time"].ToString());
                    voteInfo.owner = hash["owner"].ToString();
                    voteList.Add(voteInfo);
                }
                VoteInfo voteInfo2 = new VoteInfo();
                voteInfo2.location = 0;
                voteInfo2.total_votes = 53932256203360464;
                voteInfo2.is_active = true;
                voteInfo2.producer_key = "EOS7JR4vL3P2qgPwfMagZDqAtRLjB38qQCe6xw87gefVV73x85bsw";
                voteInfo2.url = "http://www.eosjrr.io/";
                voteInfo2.last_claim_time = 1535090522000000;
                voteInfo2.owner = "jrrcryptoeos";
                voteList.Add(voteInfo2);
                //StartCoroutine(ShowVoteList());
                votedList = infoPanel.eoswalletInfo.votedList;
                selectedList = new List<string>(votedList);
                selectInfo.text = selectedList.Count + " / 30";
                cancleInfo.text = 0 + " / " + votedList.Count;
                ScrollViewTool.instance.StartInit(voteList);
                InitVotedList();
            }
            else
            {
                List<VoteInfo> tempList = new List<VoteInfo>();
                for (int i = 0; i < hashList.Count; i++)
                {
                    Hashtable hash = hashList[i] as Hashtable;
                    VoteInfo voteInfo = new VoteInfo();
                    voteInfo.location = int.Parse(hash["location"].ToString());
                    voteInfo.total_votes = decimal.Parse(hash["total_votes"].ToString());
                    voteInfo.is_active = int.Parse(hash["is_active"].ToString()) == 0 ? false : true;
                    voteInfo.producer_key = hash["producer_key"].ToString();
                    voteInfo.url = hash["url"].ToString();
                    voteInfo.last_claim_time = long.Parse(hash["last_claim_time"].ToString());
                    voteInfo.owner = hash["owner"].ToString();
                    tempList.Add(voteInfo);
                }
                VoteInfo voteInfo2 = new VoteInfo();
                voteInfo2.location = 0;
                voteInfo2.total_votes = 53932256203360464;
                voteInfo2.is_active = true;
                voteInfo2.producer_key = "EOS7JR4vL3P2qgPwfMagZDqAtRLjB38qQCe6xw87gefVV73x85bsw";
                voteInfo2.url = "http://www.eosjrr.io/";
                voteInfo2.last_claim_time = 1535090522000000;
                voteInfo2.owner = "jrrcryptoeos";
                tempList.Add(voteInfo2);

                //更新原有信息
                for (int i = 0; i < tempList.Count; i++)
                {
                    VoteInfo old = null;
                    for (int j = 0; j < voteList.Count; j++)
                    {
                        if(tempList[i].owner == voteList[j].owner)
                        {
                            old = voteList[j];
                            break;
                        }
                    }
                    if(old != null) // 更新
                    {
                        tempList[i].dotName = old.dotName;
                        tempList[i].place = old.place;
                        tempList[i].iconPath = old.iconPath;
                    }
                }
                voteList = tempList;
            }
        }
    }

    public void ShowVotePageMore()
    {
        //List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        //ws.Add(new KeyValuePair<string, string>("op", "listproducers"));
        StartCoroutine(HttpManager._Intance.EOSSendRequest("listproducers", null, true, VoteInfoCallBack));
    }
    public IEnumerator ShowVoteList()
    {
        //ClearRect(voteListParent);
        //ClearRect(selectListParent);
        ShowSupperList();
        //showingPrefabDic.Clear();
        //selectedList.Clear();
        cancleList.Clear();
        votedList = infoPanel.eoswalletInfo.votedList;
        selectedList = new List<string>(votedList);
        selectInfo.text = selectedList.Count + " / 30";
        cancleInfo.text = 0 + " / " + votedList.Count;
        ScrollViewTool.instance.Refresh(voteList);
        RefreshVotedList();

        yield break;
        for (int i = 0; i < voteList.Count; i++)
        {

            VoteInfo temp = voteList[i];
            if (!showingPrefabDic.ContainsKey(temp.owner))
            {
                VoteInfoPrefab tempInfo = Instantiate(voteInfoPrefab);
                tempInfo.transform.SetParent(voteListParent);
                tempInfo.transform.localScale = new Vector3(1, 1, 1);
                tempInfo.transform.localPosition = new Vector3(0, -i * prefabHight, 0);
                tempInfo.GetComponent<RectTransform>().sizeDelta = new Vector2(0, prefabHight);
                voteListParent.sizeDelta = new Vector2(0, voteListParent.childCount * 135);

                tempInfo.owner = temp.owner;
                tempInfo.dotOwner.text = "(" + temp.owner + ")";
                tempInfo.url = temp.url;
                tempInfo.place = countryDic.ContainsKey(temp.location.ToString("000")) ? countryDic[temp.location.ToString("000")] : "未知";
                tempInfo.rankingNumber = (i + 1);
                tempInfo.voteNumber = temp.total_votes;
                tempInfo.producer_key = temp.producer_key;
                tempInfo.location = temp.location;
                //InitNameAndIcon(tempInfo, temp);
                tempInfo.selectButton.onClick.AddListener(delegate ()
                {
                    AddOwner(tempInfo);
                });
                if (votedList.Contains(temp.owner))
                {

                    VoteInfoPrefab tempVoted = Instantiate(tempInfo);
                    tempVoted.voteNumber = temp.total_votes;
                    tempVoted.transform.SetParent(selectListParent);
                    tempVoted.transform.localScale = new Vector3(1, 1, 1);
                    selectListParent.sizeDelta = new Vector2(0, selectListParent.childCount * 135);
                    tempVoted.selectButton.onClick.RemoveAllListeners();
                    tempVoted.selectButton.onClick.AddListener(delegate ()
                    {
                        AddToCancleList(tempVoted);
                    });

                    AddOwner(tempInfo);
                    showingInSelectList.Add(tempVoted.owner,tempVoted);
                }
                //if ((i -2) * prefabHight > rectHight)
                //    break;
                showingPrefabDic.Add(tempInfo.owner, tempInfo);
            }
            else//更新信息，是否投票
            {
                //Debug.Log("更新信息");
                VoteInfoPrefab prefab = showingPrefabDic[temp.owner];
                if (votedList.Contains(temp.owner) && prefab.sprite1.gameObject.activeInHierarchy)//把未投票的，改为投票
                {
                    AddOwner(prefab);
                    //Debug.Log("把未投票的，改为投票");
                    //如果已投列表不存在则添加
                    
                }
                else if (!votedList.Contains(temp.owner) && prefab.sprite2.gameObject.activeInHierarchy)//把投票的，改为未投票
                {
                    AddOwner(prefab);
                    //Debug.Log("把投票的，改为未投票");
                    //如果已投列表中存在则删除
                    
                }

                if(votedList.Contains(temp.owner))
                {
                    bool isExit = false;
                    for (int j = 0; j < selectListParent.childCount; j++)
                    {
                        VoteInfoPrefab voteTemp = selectListParent.GetChild(j).GetComponent<VoteInfoPrefab>();
                        if (voteTemp.owner == temp.owner)
                        {
                            isExit = true;
                            break;
                        }
                    }
                    if (!isExit)
                    {
                        VoteInfoPrefab tempVoted = Instantiate(prefab);
                        tempVoted.transform.SetParent(selectListParent);
                        tempVoted.transform.localScale = new Vector3(1, 1, 1);
                        selectListParent.sizeDelta = new Vector2(0, selectListParent.childCount * 135);
                        tempVoted.selectButton.onClick.RemoveAllListeners();
                        tempVoted.selectButton.onClick.AddListener(delegate ()
                        {
                            AddToCancleList(tempVoted);
                        });
                        showingInSelectList.Add(tempVoted.owner, tempVoted);
                    }
                }
            }
            //yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        selectInfo.text = selectedList.Count + " / 30";
        cancleInfo.text = cancleList.Count + " / " + votedList.Count;
        for (int j = 0; j < selectListParent.childCount; j++)
        {
            VoteInfoPrefab voteTemp = selectListParent.GetChild(j).GetComponent<VoteInfoPrefab>();
            voteTemp.sprite1.gameObject.SetActive(true);
            voteTemp.sprite2.gameObject.SetActive(false);
            if (!votedList.Contains(voteTemp.owner))
            {
                Destroy(voteTemp.gameObject);
                if (showingInSelectList.ContainsKey(voteTemp.owner))
                    showingInSelectList.Remove(voteTemp.owner);
                break;
            }
        }
    }

    private void InitNameAndIcon(VoteInfoPrefab tempInfo, VoteInfo temp)
    {
        tempInfo.init = true;
        if (temp.iconPath == "" || temp.iconPath == null)
        {
            GetTokenIconPath(temp.owner, (_iconpath, _dotName, _location) =>
            {
                temp.iconPath = _iconpath;
                temp.dotName = _dotName;
                tempInfo.dotName.text = _dotName;
                tempInfo.ranking.text = "排名：" + tempInfo.rankingNumber + " | " + "区域：" + tempInfo.place + " | " + "权重：" + (tempInfo.voteNumber / total_producer_vote_weight * 100).ToString("0.00") + "%";
                RectTransform _rect = tempInfo.dotName.GetComponent<RectTransform>();
                _rect.sizeDelta = new Vector2(TextLength(tempInfo.dotName) + 10, _rect.sizeDelta.y);
                 StartCoroutine(TextureUIAsset._Instance.LoadImage(tempInfo.icon, temp.iconPath));
            });
        }
        else
        {
            tempInfo.dotName.text = temp.dotName;
            tempInfo.ranking.text = "排名：" + tempInfo.rankingNumber + " | " + "区域：" + tempInfo.place + " | " + "权重：" + (tempInfo.voteNumber / total_producer_vote_weight * 100).ToString("0.00") + "%";
            RectTransform _rect = tempInfo.dotName.GetComponent<RectTransform>();
            _rect.sizeDelta = new Vector2(TextLength(tempInfo.dotName) + 10, _rect.sizeDelta.y);
            StartCoroutine(TextureUIAsset._Instance.LoadImage(tempInfo.icon, temp.iconPath));
        }
    }

    public IEnumerator InitNameAndIcon(VoteInfoPrefab tempInfo, string _owner)
    {
        yield return new WaitForEndOfFrame();
        if(tempInfo.info.iconPath != null && tempInfo.info.iconPath != "")
        {
            tempInfo.place = tempInfo.info.place;
            tempInfo.dotName.text = tempInfo.info.dotName;
            tempInfo.ranking.text = "排名：" + tempInfo.rankingNumber + " | " + "区域：" + tempInfo.place + " | " + "权重：" + (tempInfo.voteNumber / total_producer_vote_weight * 100).ToString("0.00") + "%";
            RectTransform _rect = tempInfo.dotName.GetComponent<RectTransform>();
            _rect.sizeDelta = new Vector2(TextLength(tempInfo.dotName) + 10, _rect.sizeDelta.y);
            StartCoroutine(TextureUIAsset._Instance.LoadImage(tempInfo.icon, tempInfo.info.iconPath));
        }
        else
        {
            GetTokenIconPath(_owner, (_iconpath, _dotName, _location) =>
            {
                VoteInfo temp = null;
                for (int i = 0; i < voteList.Count; i++)
                {
                    if (voteList[i].owner == _owner)
                    {
                        temp = voteList[i];
                        break;
                    }
                }
                if (temp != null)
                {
                    temp.iconPath = _iconpath;
                    temp.dotName = _dotName;
                    temp.place = NCRtoString(_location);
                    tempInfo.dotName.text = _dotName;
                    tempInfo.place = temp.place;
                    tempInfo.ranking.text = "排名：" + tempInfo.rankingNumber + " | " + "区域：" + tempInfo.place + " | " + "权重：" + (tempInfo.voteNumber / total_producer_vote_weight * 100).ToString("0.00") + "%";
                    RectTransform _rect = tempInfo.dotName.GetComponent<RectTransform>();
                    _rect.sizeDelta = new Vector2(TextLength(tempInfo.dotName) + 10, _rect.sizeDelta.y);
                    StartCoroutine(TextureUIAsset._Instance.LoadImage(tempInfo.icon, temp.iconPath));
                    tempInfo.init = true;
                }
            });
        }
    }
    public void AddOwner(VoteInfoPrefab prefab)
    {
        if(selectedList.Count >= 30)
        {
            PopupLine.Instance.Show("超出限制，最多可选30个节点");
        }
        if (prefab.sprite1.gameObject.activeInHierarchy)
        {
            if (!selectedList.Contains(prefab.owner))
                selectedList.Add(prefab.owner);
            selectInfo.text = selectedList.Count + " / 30";
            selectedList.Sort();
            prefab.sprite1.gameObject.SetActive(false);
            prefab.sprite2.gameObject.SetActive(true);
        }
        else
        {
            if (selectedList.Contains(prefab.owner))
                selectedList.Remove(prefab.owner);
            selectInfo.text = selectedList.Count + " / 30";
            prefab.sprite1.gameObject.SetActive(true);
            prefab.sprite2.gameObject.SetActive(false);
        }
    }

    public void DeleteOwner(string _owner)
    {
     
    }

    public void AddToCancleList(VoteInfoPrefab prefab) 
    {
        if (prefab.sprite1.gameObject.activeInHierarchy)
        {
            if (!cancleList.Contains(prefab.owner))
                cancleList.Add(prefab.owner);
            cancleInfo.text = cancleList.Count + " / " + votedList.Count;
            prefab.sprite1.gameObject.SetActive(false);
            prefab.sprite2.gameObject.SetActive(true);
        }
        else
        {
            if (cancleList.Contains(prefab.owner))
                cancleList.Remove(prefab.owner);
            cancleInfo.text = cancleList.Count + " / " + votedList.Count;
            prefab.sprite1.gameObject.SetActive(true);
            prefab.sprite2.gameObject.SetActive(false);
        }
         
    }
    public void RemoveCancleList(string _owner)
    {
        if (cancleList.Contains(_owner))
            cancleList.Remove(_owner);
        cancleInfo.text = cancleList.Count + " / " + votedList.Count;
    }
    public void ClearRect(RectTransform tran)
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < tran.childCount; i++)
        {
            list.Add(tran.GetChild(i).gameObject);
        }
       
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i]);
        }
//        tran.DetachChildren();
        tran.anchoredPosition = Vector2.zero;
        tran.sizeDelta = Vector2.zero;
    }

    public void GetTokenIconPath(string _owner, Action<string, string,string> callBack)
    {
        List<KeyValuePair<string, string>> ws = new List<KeyValuePair<string, string>>();
        ws.Add(new KeyValuePair<string, string>("op", "geteostokeninfo"));
        ws.Add(new KeyValuePair<string, string>("contactid", _owner));
        StartCoroutine(HttpManager._Intance.SendRequest(ws, delegate (Hashtable table)
        {
            ArrayList data = table["tokenList"] as ArrayList;
            if (data.Count > 0)
            {
                Hashtable temp = data[0] as Hashtable;
                if (callBack != null)
                {
                    callBack(temp["iconPath"].ToString(), temp["tokenName"].ToString(),temp["location"].ToString());
                }
            }
            else
            {
                Debug.Log("未能查找到：" + _owner);
            }
        },null, false));
    }

    private float TextLength(Text tx)
    {
        float totalLength = 0;
        Font myFont = tx.font;  //chatText is my Text component
        myFont.RequestCharactersInTexture(tx.text, tx.fontSize, tx.fontStyle);
        CharacterInfo characterInfo = new CharacterInfo();

        char[] arr = tx.text.ToCharArray();

        foreach (char c in arr)
        {
            myFont.GetCharacterInfo(c, out characterInfo, tx.fontSize);

            totalLength += characterInfo.advance;
        }
        return totalLength;
    }

    public void ShowSupperList()
    {
        supperDotPage.SetActive(true);
        selectDotPage.SetActive(false);
        supperLine.SetActive(true);
        selectLine.SetActive(false);

        supperText.color = activeColor;
        seleteText.color = inactiveColor;
        targetSearchParent = voteListParent;
        searchInput.text = "";
        //for (int i = 0; i < targetSearchParent.childCount; i++)
        //{
        //    targetSearchParent.GetChild(i).gameObject.SetActive(true);
        //}
        if (isSearching)
        {
            ScrollViewTool.instance.Refresh(voteList);
            isSearching = false;
        }
    }
    public void ShowSelectList()
    {

        supperDotPage.SetActive(false);
        selectDotPage.SetActive(true);
        supperLine.SetActive(false);
        selectLine.SetActive(true);
        supperText.color = inactiveColor;
        seleteText.color = activeColor;
        //if(selectListParent.childCount > 0)
        //{
        //    for (int i = 0; i < selectListParent.childCount; i++)
        //    {
        //        VoteInfoPrefab prefab = selectListParent.GetChild(i).GetComponent<VoteInfoPrefab>();
        //        VoteInfoPrefab showing = showingPrefabDic[prefab.owner];
        //        if(showing != null)
        //        {
        //            prefab.icon.sprite = showing.icon.sprite;
        //            prefab.dotName.text = showing.dotName.text;
        //        }
        //    }
        //}
        targetSearchParent = selectListParent;

        searchInput.text = "";
        for (int i = 0; i < targetSearchParent.childCount; i++)
        {
            targetSearchParent.GetChild(i).gameObject.SetActive(true);
        }
        selectListParent.sizeDelta = new Vector2(0, selectListParent.childCount * 135);
    }

    public void Vote()
    {
        if(infoPanel.eoswalletInfo.votedList.Count == 0)
        {
            EOSWalletInfo eosWalletInfo = infoPanel.eoswalletInfo;
            if (eosWalletInfo.eosCPU.used >= eosWalletInfo.eosCPU.max)
            {
                PopupLine.Instance.Show("CPU资源不足，无法进行投票");
                return;
            }
            EosGetSingInfoPanel.Instance.Voteproducer(eosWalletInfo.adminAddress, EOSSendScanSing.LastPanel.voteproducer, infoPanel.eoswalletInfo.account, selectedList, RefreshVoteInfo);
        }
        else
        {
            voteConfirmTips.SetActive(true);
        }
    }

    public void ConfirmVote()
    {
        EOSWalletInfo eosWalletInfo = infoPanel.eoswalletInfo;
        if (eosWalletInfo.eosCPU.used >= eosWalletInfo.eosCPU.max)
        {
            PopupLine.Instance.Show("CPU资源不足，无法进行投票");
            return;
        }
        EosGetSingInfoPanel.Instance.Voteproducer(eosWalletInfo.adminAddress, EOSSendScanSing.LastPanel.voteproducer, infoPanel.eoswalletInfo.account, selectedList, RefreshVoteInfo);
        voteConfirmTips.SetActive(false);
    }
    public void RefuseVote()
    {
        voteConfirmTips.SetActive(false);
    }

    private void RefreshVoteInfo()
    {
        EOSWalletInfo eosWalletInfo = infoPanel.eoswalletInfo;
        EosGetSingInfoPanel.Instance.GetAccount(eosWalletInfo.account, delegate (Hashtable t)
        {
            Hashtable tempInfo = t["voter_info"] as Hashtable;
            if (tempInfo != null)
            {
                ArrayList tempArr = tempInfo["producers"] as ArrayList;
                eosWalletInfo.votedList.Clear();
                if (tempArr.Count > 0)
                {
                    for (int i = 0; i < tempArr.Count; i++)
                    {
                        string temoStr = tempArr[i].ToString();
                        if (!eosWalletInfo.votedList.Contains(temoStr))
                            eosWalletInfo.votedList.Add(temoStr);
                    }
                }
            }
            StartCoroutine(ShowVoteList());

        }, null, true);
    }
    public void CancleVote()
    {
        EOSWalletInfo eosWalletInfo = infoPanel.eoswalletInfo;
        if (eosWalletInfo.eosCPU.used >= eosWalletInfo.eosCPU.max)
        {
            PopupLine.Instance.Show("CPU资源不足，无法进行投票");
            return;
        }
        List<string> tempList = new List<string>(selectedList);

        for (int i = 0; i < cancleList.Count; i++)
        {
            tempList.Remove(cancleList[i]);
        }
        tempList.Sort();

        EosGetSingInfoPanel.Instance.Voteproducer(eosWalletInfo.adminAddress, EOSSendScanSing.LastPanel.voteproducer, infoPanel.eoswalletInfo.account, tempList, RefreshVoteInfo);
    }

    public void SearchDot()
    {
        string str = searchInput.text;
        if(targetSearchParent == selectListParent)
        {
            int countInView = 0;
            if (str.Length == 0)
            {
                for (int i = 0; i < targetSearchParent.childCount; i++)
                {
                    targetSearchParent.GetChild(i).gameObject.SetActive(true);
                    countInView++;
                }
            }
            else
            {
                for (int i = 0; i < targetSearchParent.childCount; i++)
                {
                    VoteInfoPrefab prefab = targetSearchParent.GetChild(i).GetComponent<VoteInfoPrefab>();
                    if (prefab.owner.IndexOf(str.ToLower()) >= 0)
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
            targetSearchParent.anchoredPosition = Vector2.zero;
            targetSearchParent.sizeDelta = new Vector2(0, countInView * 135);
        }
        else if(targetSearchParent == voteListParent)
        {
            if(str.Length == 0)
            {
                searchList = new List<VoteInfo>();
                for (int i = 0; i < voteList.Count; i++)
                {
                    searchList.Add(voteList[i]);
                }
                ScrollViewTool.instance.Refresh(searchList);
                isSearching = false;
            }
            else
            {
                searchList = new List<VoteInfo>();
                for (int i = 0; i < voteList.Count; i++)
                {
                    if(voteList[i].owner.IndexOf(str.ToLower()) >= 0)
                    {
                        searchList.Add(voteList[i]);
                    }
                }
                ScrollViewTool.instance.Refresh(searchList);
                isSearching = true;
            }
        }
       
    }

    public void ShowResourcePage()
    {
        PanelManager._Instance._eosResourcesPanel.Open(infoPanel.eoswalletInfo);
    }

    bool IsContain(List<VoteInfo> _list,VoteInfo _info)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            if(_list[i].owner == _info.owner)
            {
                return true;
            }
        }
        return false;
    }

    public void MoveDown(VoteInfoPrefab prefab,RectTransform rect)
    {
        prefab.transform.SetSiblingIndex(rect.childCount - 1);
    }

    //解析html的NCR编码方法
    public string NCRtoString(string htmltext)
    {
        if (htmltext == "0")
            return "未知";
        string result = "";
        try
        {
            //RegexHelper.GetMatchStr(htmltext, "<body>(.*?)</body>", out htmltext);
            //htmltext = htmltext.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "");
            //htmltext = Regex.Replace(htmltext, "<[^>]*>", "");
            htmltext = htmltext.Replace("&#x", "\\u").Replace(";", "");
            string[] strlist = htmltext.Replace("\\", "").Split('u');
            for (int i = 1; i < strlist.Length; i++)
            {
                if (strlist[i].Length != 4)
                {
                    strlist[i] = strlist[i].Substring(0, 4);
                }
                //将unicode字符转为10进制整数，然后转为char中文字符
                result += (char)int.Parse(strlist[i], System.Globalization.NumberStyles.HexNumber);
            }
        }
        catch (Exception)
        {
            return "解析html的NCR编码方法异常";
        }
        return result;
    }

    public bool IsVoted(string _owner)
    {
        for (int i = 0; i < votedList.Count; i++)
        {
            if (votedList[i] == _owner)
                return true;
        }
        return false;
    }

    public void InitVotedList()
    {
        List<Transform> needRemove = new List<Transform>();
        for (int i = 0; i < selectListParent.childCount; i++)
        {
            needRemove.Add(selectListParent.GetChild(i));
        }
        for (int i = 0; i < needRemove.Count; i++)
        {
            DestroyImmediate(needRemove[i].gameObject);
        }
        selectListParent.anchoredPosition = new Vector2(0, 0);
        for (int i = 0; i < votedList.Count; i++)
        {
            int _index = -1;
            for (int j = 0; j < voteList.Count; j++)
            {
                if(votedList[i] == voteList[j].owner)
                {
                    _index = j;
                    break;
                }
            }
            if (_index > -1)
            {
                VoteInfoPrefab tempVoted = Instantiate(voteInfoPrefab);
                tempVoted.transform.SetParent(selectListParent);
                tempVoted.transform.localScale = new Vector3(1, 1, 1);
                selectListParent.sizeDelta = new Vector2(0, selectListParent.childCount * 135);
                tempVoted.InitVoted(_index);
                tempVoted.selectButton.onClick.RemoveAllListeners();
                tempVoted.selectButton.onClick.AddListener(delegate ()
                {
                    AddToCancleList(tempVoted);
                });
                StartCoroutine(InitNameAndIcon(tempVoted, tempVoted.owner));

            }
        }
    }
    public void RefreshVotedList()
    {
        selectListParent.anchoredPosition = new Vector2(0, 0);
        //如果已经有prefab，设置为未选中；
        for (int i = 0; i < votedList.Count; i++)
        {
            int _index = -1;
            for (int j = 0; j < voteList.Count; j++)
            {
                if (votedList[i] == voteList[j].owner)
                {
                    _index = j;
                    break;
                }
            }
            bool isHave = false;
            for (int j = 0; j < selectListParent.childCount; j++)
            {
                VoteInfoPrefab prefab = selectListParent.GetChild(j).GetComponent<VoteInfoPrefab>();
                if (prefab.owner == votedList[i])
                {
                    isHave = true;
                    prefab.sprite1.gameObject.SetActive(true);
                    prefab.sprite2.gameObject.SetActive(false);
                    prefab.rankingNumber = _index + 1;
                    prefab.voteNumber = voteList[_index].total_votes;
                    prefab.ranking.text = "排名：" + prefab.rankingNumber + " | " + "区域：" + prefab.place + " | " + "权重：" + (prefab.voteNumber / total_producer_vote_weight * 100).ToString("0.00") + "%";
                    break;
                }
            }
            //没有的话，重新创建prefab
            if(isHave == false)
            {
                if (_index > -1)
                {
                    VoteInfoPrefab tempVoted = Instantiate(voteInfoPrefab);
                    tempVoted.transform.SetParent(selectListParent);
                    tempVoted.transform.localScale = new Vector3(1, 1, 1);
                    selectListParent.sizeDelta = new Vector2(0, selectListParent.childCount * 135);
                    tempVoted.InitVoted(_index);
                    tempVoted.selectButton.onClick.RemoveAllListeners();
                    tempVoted.selectButton.onClick.AddListener(delegate ()
                    {
                        AddToCancleList(tempVoted);
                    });
                    StartCoroutine(InitNameAndIcon(tempVoted, tempVoted.owner));

                }
            }
        }
        //多余的需要删除
        List<Transform> tempList = new List<Transform>();
        for (int i = 0; i < selectListParent.childCount; i++)
        {
            VoteInfoPrefab prefab = selectListParent.GetChild(i).GetComponent<VoteInfoPrefab>();
            bool needDelete = true;
            for (int j = 0; j < votedList.Count; j++)
            {
                if(prefab.owner == votedList[j])
                {
                    needDelete = false;
                    break;
                }
            }
            if(needDelete == true)
            {
                tempList.Add(prefab.transform);
            }
        }
        for (int i = 0; i < tempList.Count; i++)
        {
            Destroy(tempList[i].gameObject);
        }
    }
}
public class VoteInfo
{
    public int location;
    public decimal total_votes;
    public bool is_active;
    public string producer_key;
    public string url;
    public long last_claim_time;
    public int unpai_blocks;
    public string owner;
    public string iconPath;
    public string dotName;
    public string place;
}


