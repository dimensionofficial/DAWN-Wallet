using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullRefreshControl : MonoBehaviour
{
    public RectTransform pullRefobj;
    public GameObject shuaxin;
    public GameObject shuaxinging;
    public Vector2 pos2;
    public float scroePosY;
    public float startPosY;
    public float pullrefobjHeight;
    public float currentY;

    public bool isRef = false;
	// Use this for initialization
	void Start ()
    {
        shuaxin.gameObject.SetActive(false);
        pullrefobjHeight = pullRefobj.sizeDelta.y;
        pos2 = new Vector2(Screen.width, Screen.height);
        scroePosY = pos2.y * 1.0F / 10;
        startPosY = transform.position.y;
        EventManager.Instance.AddEventListener(EventID.UpdateTotalbalance, HideObject);
    }

    private void HideObject(params object[] obj)
    {
        HideShuaXining();
        
    }

    void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener(EventID.UpdateTotalbalance, HideObject);
    }

    private void HideShuaXining()
    {
        shuaxinging.SetActive(false);
 //       pullRefobj.gameObject.SetActive(false);
        RemovedTimer();
    }

    private Timer pullTimer;

    private void RemovedTimer()
    {
        if (pullTimer != null)
            TimerManager.Instance.RemoveTimer(pullTimer);

        pullTimer = null;
    }

	// Update is called once per frame
	void Update ()
    {
        currentY = transform.position.y;

        if (transform.position.y < startPosY && isRef == false)
        {
            if (PanelManager._Instance._mainPanel.JudegeItem())
            {
                if (startPosY - transform.position.y >= scroePosY * 0.2F)
                {
                    isRef = true;
                }

                if (startPosY - transform.position.y >= scroePosY * 0.2F)
                {
                    shuaxin.gameObject.SetActive(true);
                    //pullRefobj.gameObject.SetActive(true);
                    shuaxinging.gameObject.SetActive(false);
                }
                else
                {
                    shuaxin.gameObject.SetActive(false);
                }
            }
        }

        if (isRef)
        {
            if (transform.position.y + pullrefobjHeight >= startPosY)
            {
                isRef = false;
                shuaxinging.gameObject.SetActive(true);
                shuaxin.gameObject.SetActive(false);
                PanelManager._Instance._mainPanel.Refresh();
                HttpManager._Intance.StartRequestHttp();
                RemovedTimer();
                pullTimer = TimerManager.Instance.AddTimer(5, HideShuaXining);
            }
        }
	}
}
