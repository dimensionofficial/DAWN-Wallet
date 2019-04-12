using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EOSResourcesBasePanel : MonoBehaviour
{
    public enum ResourcesType
    {
        None,
        ARM,
        CPU,
        NetWork,
    }
    public ResourcesType resourcesType;


    public enum ResouresReceiver
    {
        None,
        Myslef,
        Other,
    }
    public ResouresReceiver receiver = ResouresReceiver.None;

    public GameObject parentTitle;

    public ResourcesMyslef reMyslef = new ResourcesMyslef();
    public ResourcesOther reOther = new ResourcesOther();

    public GameObject myslefBtnMark;
    public GameObject myselfObject;

    public GameObject otherBtnMark;
    public GameObject otherObjct;

    public void Show()
    {
        gameObject.SetActive(true);
        parentTitle.SetActive(true);

        receiver = ResouresReceiver.Myslef;
        InitObjct();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        parentTitle.SetActive(false);
    }

    public void OnClickMyselfBtn()
    {
        receiver = ResouresReceiver.Myslef;
        InitObjct();
    }

    public void OnClickOtherBtn()
    {
        receiver = ResouresReceiver.Other;
        InitObjct();
    }

    public virtual void RefreshInfo()
    {

    }

    public virtual void InitObjct()
    {
        if (receiver == ResouresReceiver.Myslef)
        {
            reMyslef.Init();
            myselfObject.SetActive(true);
            otherObjct.SetActive(false);
            myslefBtnMark.SetActive(true);
            otherBtnMark.SetActive(false);
        }
        else if(receiver == ResouresReceiver.Other)
        {
            reOther.Init();
            myselfObject.SetActive(false);
            otherObjct.SetActive(true);
            myslefBtnMark.SetActive(false);
            otherBtnMark.SetActive(true);
        }

        RefreshInfo();
    }
}
