using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;

public class SelectRegionPanel : MonoBehaviour {
    public LoopListView2 mLoopListViewDay;
    private System.Action<string> OnConfirmCallback;
    string mCurSelectedDay = "";
    string[] region = new string[] { "埃及", "0020", "摩洛哥", "00210", "阿尔及利亚", "00213", "突尼斯", "00216", "利比亚", "00218", "冈比亚", "00220", "塞内加尔", "00221", "毛里塔尼亚", "00222", "马里", "00223", "几内亚", "00224", "科特迪瓦", "00225", "布基拉法索", "00226", "尼日尔", "00227", "多哥", "00228", "贝宁", "00229", "毛里求斯", "00230", "利比里亚", "00231", "塞拉利昂", "00232", "加纳", "00233", "尼日利亚", "00234", "乍得", "00235", "中非", "00236", "喀麦隆", "00237", "佛得角", "00238", "圣多美", "00239", "普林西比", "00239", "赤道几内亚", "00240", "加蓬", "00241", "刚果", "00242", "扎伊尔", "00243", "安哥拉", "00244", "几内亚比绍", "00245", "阿森松", "00247", "塞舌尔", "00248", "苏丹", "00249", "卢旺达", "00250", "埃塞俄比亚", "00251", "索马里", "00252", "吉布提", "00253", "肯尼亚", "00254", "坦桑尼亚", "00255", "乌干达", "00256", "布隆迪", "00257", "莫桑比克", "00258", "赞比亚", "00260", "马达加斯加", "00261", "留尼旺岛", "00262", "津巴布韦", "00263", "纳米比亚", "00264", "马拉维", "00265", "莱索托", "00266", "博茨瓦纳", "00267", "斯威士兰", "00268", "科摩罗", "00269", "南非", "0027", "圣赫勒拿", "00290", "阿鲁巴岛", "00297", "法罗群岛", "00298", "马来西亚", "0060", "印度尼西亚", "0062", "菲律宾", "0063", "新加坡", "0065", "泰国", "0066", "文莱", "00673", "日本", "0081", "韩国", "0082", "越南", "0084", "朝鲜", "00850", "香港", "00852", "澳门", "00853", "柬埔寨", "00855", "老挝", "00856", "中国", "0086", "台湾", "00886", "孟加拉国", "00880", "土耳其", "0090", "印度", "0091", "巴基斯坦", "0092", "阿富汗", "0093", "斯里兰卡", "0094", "缅甸", "0095", "马尔代夫", "00960", "黎巴嫩", "00961", "约旦", "00962", "叙利亚", "00963", "伊拉克", "00964", "科威特", "00965", "沙特阿拉伯", "00966", "阿曼", "00968", "以色列", "00972", "巴林", "00973", "卡塔尔", "00974", "不丹", "00975", "蒙古", "00976", "尼泊尔", "00977", "伊朗", "0098", "俄罗斯", "007", "希腊", "0030", "荷兰", "0031", "比利时", "0032", "法国", "0033", "西班牙", "0034", "直布罗陀", "00350", "葡萄牙", "00351", "卢森堡", "00352", "爱尔兰", "00353", "冰岛", "00354", "阿尔巴尼亚", "00355", "马耳他", "00356", "塞浦路斯", "00357", "芬兰", "00358", "保加利亚", "00359", "匈牙利", "00336", "德国", "0049", "南斯拉夫", "00338", "意大利", "0039", "圣马力诺", "00223", "梵蒂冈", "00396", "罗马尼亚", "0040", "瑞士", "0041", "列支敦士登", "004175", "奥地利", "0043", "英国", "0044", "丹麦", "0045", "瑞典", "0046", "挪威", "0047", "波兰", "0048", "美国", "001", "加拿大", "001", "中途岛", "001808", "夏威夷", "001808", "威克岛", "001808", "安圭拉岛", "001809", "维尔京群岛", "001809", "圣卢西亚", "001809", "波多黎各", "001809", "牙买加", "001809", "巴哈马", "001809", "巴巴多斯", "001809", "阿拉斯加", "001907", "格陵兰岛", "00299", "福克兰群岛", "00500", "伯利兹", "00501", "危地马拉", "00502", "萨尔瓦多", "00503", "洪都拉斯", "00504", "尼加拉瓜", "00505", "哥斯达黎加", "00506", "巴拿马", "00507", "海地", "00509", "秘鲁", "0051", "墨西哥", "0052", "古巴", "0053", "阿根廷", "0054", "巴西", "0055", "智利", "0056", "哥伦比亚", "0057", "委内瑞拉", "0058", "玻利维亚", "00591", "圭亚那", "00592", "厄瓜多尔", "00593", "法属圭亚那", "00594", "巴拉圭", "00595", "马提尼克", "00596", "苏里南", "00597", "乌拉圭", "00598", "澳大利亚", "0061", "新西兰", "0064", "关岛", "00671", "科科斯岛", "006722", "诺福克岛", "006723", "圣诞岛", "006724", "瑙鲁", "00674", "汤加", "00676", "所罗门群岛", "00677", "瓦努阿图", "00678", "斐济", "00679", "科克群岛", "00682", "纽埃岛", "00683", "东萨摩亚", "00684", "西萨摩亚", "00685", "基里巴斯", "00686", "图瓦卢", "00688" };
    public List<KeyValuePair<string, string>> regionList = new List<KeyValuePair<string, string>>();
    public string CurSelectedDay
    {
        get { return mCurSelectedDay; }
    }

    public void Show(System.Action<string> callback, string showRegion = "")
    {
        if (regionList.Count == 0)
        {
            for (int i = 0; i < region.Length; i++)
            {
                regionList.Add(new KeyValuePair<string, string>(region[i + 1], region[i]));
                i++;
            }
            mLoopListViewDay.InitListView(-1, OnGetItemByIndexForDay);
            mLoopListViewDay.mOnSnapNearestChanged = OnDaySnapTargetChanged;
        }
        this.OnConfirmCallback = callback;
        this.gameObject.SetActive(true);
        if (!string.IsNullOrEmpty(showRegion))
        {
            int tar = -1;
            for (int i = 0; i < regionList.Count; i++)
            {
                if (regionList[i].Key == "00" + showRegion.Substring(1))
                {
                    tar = i;
                }
            }
            if (tar != -1)
            {
                mLoopListViewDay.MovePanelToItemIndex(tar, 200);
                mLoopListViewDay.FinishSnapImmediately();
                OnDaySnapTargetChanged(mLoopListViewDay, mLoopListViewDay.GetShownItemByItemIndex(tar));
            }
        }
    }

    LoopListViewItem2 OnGetItemByIndexForDay(LoopListView2 listView, int index)
    {
        LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
        ListItem7 itemScript = item.GetComponent<ListItem7>();
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
            itemScript.Init();
        }
        int itemCount = regionList.Count;
        if (index >= 0)
        {
            index = index % itemCount;
        }
        else
        {
            index = itemCount + ((index + 1) % itemCount) - 1;
        }
        itemScript.Value = index;
        itemScript.mText.text = "+" + regionList[index].Key.Substring(2) + "(" + regionList[index].Value + ")";
        return item;
    }
    void OnDaySnapTargetChanged(LoopListView2 listView, LoopListViewItem2 item)
    {
        int index = listView.GetIndexInShownItemList(item);
        if (index < 0)
        {
            return;
        }
        ListItem7 itemScript = item.GetComponent<ListItem7>();
        OnListViewSnapTargetChanged(listView, index);
    }

    void OnListViewSnapTargetChanged(LoopListView2 listView, int targetIndex)
    {
        int count = listView.ShownItemCount;  
        for (int i = 0; i < count; ++i)
        {
            LoopListViewItem2 item2 = listView.GetShownItemByIndex(i); 
            ListItem7 itemScript = item2.GetComponent<ListItem7>();
            if (i == targetIndex)
            {
                itemScript.mText.color = Color.black;
                mCurSelectedDay = regionList[itemScript.Value].Key;
                itemScript.mText.fontSize = 60;
                if (OnConfirmCallback != null)
                {
                    OnConfirmCallback(CurSelectedDay);
                }
            }
            else
            {
                itemScript.mText.color = Color.gray;
                itemScript.mText.fontSize = 45;
            }
        }
    }

    public void OnConfirm()
    {
        this.gameObject.SetActive(false);
        if (OnConfirmCallback != null)
        {
            OnConfirmCallback(CurSelectedDay);
        }
    }

    public void OnCancel()
    {
        this.gameObject.SetActive(false);
    }
}
