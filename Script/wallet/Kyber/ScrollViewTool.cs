using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewTool : MonoBehaviour {
    public RectTransform parentRect;
    public VoteInfoPrefab voteInfoPrefab;
    public ScrollRect scroll;
    public RectTransform top;
    public RectTransform down;
    public static ScrollViewTool instance;
    public int currentIndex;
    public int maxIndex = 50 ;
    public int initCount;
    public float topPosition;
    public float downPosition;
    public float startDistance;
    private float rectHeight;
    public float prefabHeight;
    List<VoteInfo> voteListNow;
    bool freshing;
    private void Awake()
    {
        instance = this;
    }
    void Start () {
        //rectHeight = scroll.viewport.rect.height;
        //topPosition = top.position.y;
        //downPosition = down.position.y;
        //prefabHeight = testPrefab.GetComponent<RectTransform>().sizeDelta.y;
        //for (int i = 0; i < maxIndex; i++)
        //{
        //    TestPrefab prefab = Instantiate(testPrefab);
        //    prefab.Init(i);
        //    prefab.transform.SetParent(parentRect);
        //    RectTransform rectTrans = prefab.GetComponent<RectTransform>();
        //    rectTrans.localScale = new Vector3(1, 1, 1);
        //    rectTrans.anchoredPosition = new Vector3(0, -i * prefabHeight, 0);
        //    rectTrans.sizeDelta = new Vector2(0, prefabHeight);
        //    parentRect.sizeDelta =new Vector2(0, (i + 1) * prefabHeight);
        //    if (i * prefabHeight > rectHeight)
        //    {
        //        currentIndex = i;
        //        initCount = i + 1;
        //        break;
        //    }
        //}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MoveDown(VoteInfoPrefab prefab)
    {
        if (freshing)
            return;
        if (currentIndex + 1 >= maxIndex)
            return;
        //Debug.Log("moveDown");
        currentIndex++;
        prefab.Init(currentIndex);
        RectTransform rectTrans = prefab.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = new Vector3(0, -currentIndex * prefabHeight, 0);
        parentRect.sizeDelta = new Vector2(0, (currentIndex + 1) * prefabHeight);
    }

    public void MoveUp(VoteInfoPrefab prefab)
    {
        if (freshing)
            return;
        if (currentIndex - 1 < initCount - 1)
            return;
        //Debug.Log("moveUp");
        currentIndex--;
        int myIndex = currentIndex - initCount + 1;
        prefab.Init(myIndex);
        RectTransform rectTrans = prefab.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = new Vector3(0, -myIndex * prefabHeight, 0);
    }

    public void StartInit(List<VoteInfo> voteList)
    {
        rectHeight = scroll.viewport.rect.height;
        topPosition = top.position.y;
        downPosition = down.position.y;
        prefabHeight = voteInfoPrefab.GetComponent<RectTransform>().sizeDelta.y; 
        voteListNow = voteList;
        maxIndex = voteList.Count;
        for (int i = 0; i < maxIndex; i++)
        {
            VoteInfoPrefab prefab = Instantiate(voteInfoPrefab);
            prefab.Init(i);
            prefab.transform.SetParent(parentRect);
            RectTransform rectTrans = prefab.GetComponent<RectTransform>();
            rectTrans.localScale = new Vector3(1, 1, 1);
            rectTrans.anchoredPosition = new Vector3(0, -i * prefabHeight, 0);
            rectTrans.sizeDelta = new Vector2(0, prefabHeight);
            parentRect.sizeDelta = new Vector2(0, (i + 1) * prefabHeight);
            if (i * prefabHeight > rectHeight)
            {
                currentIndex = i;
                initCount = i + 1;
                break;
            }
        }
        startDistance = parentRect.rect.height - rectHeight;
    }

    public VoteInfo GetVoteInfo(int _index)
    {
        return voteListNow[_index];
    }

    public void Refresh(List<VoteInfo> voteList)
    {
        parentRect.anchoredPosition = new Vector2(0, 0);
        List<Transform> needReomve = new List<Transform>();
        for (int i = 0; i < parentRect.childCount; i++)
        {
            needReomve.Add(parentRect.GetChild(i));
        }
        for (int i = 0; i < needReomve.Count; i++)
        {
            Destroy(needReomve[i].gameObject);
        }
        StartInit(voteList);
    }
}
