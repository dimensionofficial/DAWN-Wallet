using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteInfoPrefab : MonoBehaviour
{
    public Toggle toggle;
    public Image sprite1;
    public Image sprite2;
    public Image icon;
    public Text dotName;
    public Text dotOwner;
    public Text ranking;
    public string url;
    public string producer_key;
    public int location;
    public string owner;
    public Button selectButton;
    public int rankingNumber;
    public string place;
    public decimal voteNumber;
    public bool init;
    public VoteInfo info;
    private RectTransform rectTrans;
    public bool isInSelectedList;
    
    public void ShowWebView()
    {

    }

    private void Start()
    {
        rectTrans = GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        if (ranking.text == "")
            init = false;
    }
    //public void AddToVoteList()
    //{
    //    if(toggle.isOn == true)
    //    {
    //        EosVotePanel._instance.AddOwner(owner);
    //        Debug.Log("添加到投票列表");
    //    }
    //    else
    //    {
    //        EosVotePanel._instance.DeleteOwner(owner);
    //    }
    //}
    private void Update()
    {
        //if (transform.position.y - 270 > transform.parent.parent.position.y)
        //{
        //    //挪到下一层
        //    MoveDown();
        //}
        //if(init == false && transform.position.y >0)
        //{
        //    StartCoroutine(EosVotePanel._instance.InitNameAndIcon(this, owner));
        //}
        if(!isInSelectedList)
        {
            if (rectTrans.position.y - ScrollViewTool.instance.startDistance * 0.9f > ScrollViewTool.instance.topPosition)
            {
                ScrollViewTool.instance.MoveDown(this);
            }
            else if (rectTrans.position.y + ScrollViewTool.instance.startDistance - rectTrans.rect.height < ScrollViewTool.instance.downPosition)
            {
                ScrollViewTool.instance.MoveUp(this);
            }
        }
    }

    public void MoveDown()
    {
        transform.localPosition = new Vector3(0,-transform.parent.GetComponent<RectTransform>().sizeDelta.y,0);
        transform.parent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 135);
    }

    public void Init(int  _index)
    {
        info = ScrollViewTool.instance.GetVoteInfo(_index);

        owner = info.owner;
        dotOwner.text = "(" + info.owner + ")";
        url = info.url;
        place = EosVotePanel._instance.countryDic.ContainsKey(info.location.ToString("000")) ? EosVotePanel._instance.countryDic[info.location.ToString("000")] : "未知";
        rankingNumber = (_index + 1);
        voteNumber = info.total_votes;
        producer_key = info.producer_key;
        location = info.location;
        if(EosVotePanel._instance.selectedList.Contains(owner))
        {
            sprite1.gameObject.SetActive(false);
            sprite2.gameObject.SetActive(true);
        }
        else
        {
            sprite1.gameObject.SetActive(true);
            sprite2.gameObject.SetActive(false);
        }
        StartCoroutine(EosVotePanel._instance.InitNameAndIcon(this, owner));
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(delegate ()
        {
            EosVotePanel._instance.AddOwner(this);
        });
    }


    public void InitVoted(int _index)
    {
        info = ScrollViewTool.instance.GetVoteInfo(_index);
        isInSelectedList = true;
        owner = info.owner;
        dotOwner.text = "(" + info.owner + ")";
        url = info.url;
        place = EosVotePanel._instance.countryDic.ContainsKey(info.location.ToString("000")) ? EosVotePanel._instance.countryDic[info.location.ToString("000")] : "未知";
        rankingNumber = (_index + 1);
        voteNumber = info.total_votes;
        producer_key = info.producer_key;
        location = info.location;
    }
}
