using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EOSGlobalInfo
{
    /// <summary>
    /// 全网内存
    /// </summary>
    public decimal totalRam;
    /// <summary>
    /// 全网可用内存
    /// </summary>
    public decimal ramAvailabe;
    /// <summary>
    /// 内存价格 1kb 等于多少eos
    /// </summary>
    public decimal ramPriceEos;
    /// <summary>
    /// cpu价格
    /// </summary>
    public decimal cpuPriceEos;
    /// <summary>
    /// 网络价格
    /// </summary>
    public decimal netPriceEos;
}
[System.Serializable]
public class ResourcesMyslef
{
    public Text eosCountText;
    public InputField affirmText;

    public InputField cancelText;
    public Text cancelNumberText;

    public void Init()
    {
        affirmText.text = "";
        cancelText.text = "";
    }
}
[System.Serializable]
public class ResourcesOther
{
    public InputField accountText;

    public Text eosCountText;
    public InputField affirmText;
    public InputField cancelText;
    public Text eosCancelText;
    public void Init()
    {
        accountText.text = "";
        affirmText.text = "";
        cancelText.text = "";

        if(eosCancelText != null)
            eosCancelText.text = "";
    }
}

public class EosResourcesPanel : MonoBehaviour
{

    public Timer m_timer;

    public GameObject loadingPanel;

    public Image armImage;
    public Text armPrect;
    public Text arm_prectText;

    public Image cpuImage;
    public Text cpuPrect;
    public Text cpu_prectText;

    public Image networkImage;
    public Text networkPrect;
    public Text net_prectText;

    public GameObject delegatebwObject;
    public Text undelegatebwText;

    public EOSResourcesARMPanel resourcesARM;
    public EOSResourcesCPuPanel resourcesDBW;
    public EOSResourcesNetwork resourcesNetwork;
    // Use this for initialization
    public EOSWalletInfo eosInfo;

    public EOSResourcesOverPanel resourcesOverPanel;

    public float armPrectValue;
    public float cpuPrectValue;
    public float netPrectValue;

    public float armLimitValue;
    public float cpuLimitValue;
    public float netLimitValue;

    private int cpuTimeCount;
    private int netTimeCount;

    public EOSGlobalInfo eosGlobalInfo = new EOSGlobalInfo();
    public EOSResourcesBasePanel.ResourcesType currentType;

    public void Open(EOSWalletInfo eoswalletInfo)
    {
        eosInfo = eoswalletInfo;
        gameObject.SetActive(true);

        OnClickRAMBtn();

        GetEOSGlobal();

    }

    public void ReSet()
    {
        resourcesARM.reMyslef.Init();
        resourcesARM.reOther.Init();

        resourcesDBW.reMyslef.Init();
        resourcesDBW.reOther.Init();

        resourcesNetwork.reMyslef.Init();
        resourcesNetwork.reOther.Init();
    }

    public void GetEOSGlobal()
    {
        HttpManager._Intance.StartCoroutine(HttpManager._Intance.EOSSendRequest("getGlobalState", null, false, delegate(Hashtable ha)
        {
            if (ha != null && ha.ContainsKey("error"))
            {
                PopUpBox.Instance.Show(null, null, "确定", "", "错误提示", ha["error"].ToString());
                Debug.Log("Eos 网络错误：" + ha["error"]);
                return;
            }

            try
            {
                eosGlobalInfo.totalRam = decimal.Parse(ha["totalRam"].ToString());
                eosGlobalInfo.ramAvailabe = decimal.Parse(ha["ramAvailabe"].ToString());
                eosGlobalInfo.ramPriceEos = decimal.Parse(ha["ramPriceEos"].ToString());
                eosGlobalInfo.cpuPriceEos = decimal.Parse(ha["cpuPriceEos"].ToString());
                eosGlobalInfo.netPriceEos = decimal.Parse(ha["netPriceEos"].ToString());
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }

        }, null, false));
    }

    void Start()
    {
        EventManager.Instance.AddEventListener(EventID.UpdateEOSBalance, RefreshPrect);
    }

    void OnDestory()
    {
        EventManager.Instance.RemoveEventListener(EventID.UpdateEOSBalance, RefreshPrect);
    }

    private void RefreshPrect(params object[] obj)
    {
        if (PanelManager._Instance._eosWalletInfoPanel.currentItem != null)
        {
            EosItem iten = PanelManager._Instance._eosWalletInfoPanel.currentItem as EosItem;

            if (eosInfo != null && iten.eosWalletInfo.adminAddress == eosInfo.adminAddress)
            {
                ShowResourcesPrect();
                resourcesARM.RefreshInfo();
                resourcesDBW.RefreshInfo();
                resourcesNetwork.RefreshInfo();
            }
        }
    }

    public bool JudgeResourceLimit()
    {
        if (armLimitValue >= 0.99 || cpuLimitValue >= 0.99 || netLimitValue >= 0.99)
        {
            return false;
        }

        return true;
    }

    void OnDisable()
    {
        if (m_timer != null)
        {
            TimerManager.Instance.RemoveTimer(m_timer);
            m_timer = null;
        }
    }

    public void ShowResourcesPrect()//EOSResourcesBasePanel.ResourcesType typ
    {
        delegatebwObject.SetActive(false);

        armLimitValue = float.Parse((eosInfo.eosRAM.used * 1 / eosInfo.eosRAM.max).ToString());
        cpuLimitValue = float.Parse((eosInfo.eosCPU.used * 1 / eosInfo.eosCPU.max).ToString()); ;
        netLimitValue = float.Parse((eosInfo.eosNetwork.used * 1 / eosInfo.eosNetwork.max).ToString());

        if (currentType == EOSResourcesBasePanel.ResourcesType.ARM)
        {
            
            if (m_timer != null)
            {
                TimerManager.Instance.RemoveTimer(m_timer);
                m_timer = null;
            }

            cpuPrectValue = float.Parse((eosInfo.eosRAM.used * 1 / eosInfo.eosRAM.max).ToString());

            armPrectValue = 1 - cpuPrectValue;
            arm_prectText.text = "可用(" + (armPrectValue * 100).ToString("f2") + "%)";
            armPrect.text = (eosInfo.eosRAM.max - eosInfo.eosRAM.used).ToString("f2") + "KB/" + eosInfo.eosRAM.max.ToString("f2") + "KB";

           
            cpuImage.fillAmount = cpuPrectValue;

            cpuPrect.text = eosInfo.eosRAM.used.ToString("f2") + "KB/" + eosInfo.eosRAM.max.ToString("f2") + "KB";
            cpu_prectText.text = "占用(" + (cpuPrectValue * 100).ToString("f2") + "%)";

            netPrectValue = 1 -  float.Parse((eosGlobalInfo.ramAvailabe * 1 / eosGlobalInfo.totalRam).ToString());
            networkImage.fillAmount = netPrectValue;
            decimal temp = eosGlobalInfo.totalRam - eosGlobalInfo.ramAvailabe;

            networkPrect.text = temp.ToString("f2") + "GB/" + eosGlobalInfo.totalRam.ToString("f2") + "GB";
            net_prectText.text = "全网占用(" + (netPrectValue * 100).ToString("f2") + "%)";
        }
        else if (currentType == EOSResourcesBasePanel.ResourcesType.CPU)
        {
            
            armPrectValue = float.Parse((eosInfo.eosCPU.available * 1 / eosInfo.eosCPU.max).ToString());
            arm_prectText.text = "可用(" + (armPrectValue * 100).ToString("f2") + "%)";
            armPrect.text = eosInfo.eosCPU.available.ToString("f2") + "ms/" + eosInfo.eosCPU.max.ToString("f2") + "ms";
            armImage.fillAmount = armPrectValue;

            cpuPrectValue = float.Parse((eosInfo.eosCPU.used * 1 / eosInfo.eosCPU.max).ToString());
            cpuImage.fillAmount = cpuPrectValue;

            cpuPrect.text = eosInfo.eosCPU.used.ToString("f2") + "ms/" + eosInfo.eosCPU.max.ToString("f2") + "ms";
            cpu_prectText.text = "占用(" + (cpuPrectValue * 100).ToString("f2") + "%)";


            netPrectValue = 0;
            cpuTimeCount = 0;
            if (eosInfo.refund_request.cpu_amount > 0)
            {
                
                cpuTimeCount = GetTimeStr(eosInfo.refund_request.request_time);

                if (m_timer != null)
                {
                    TimerManager.Instance.RemoveTimer(m_timer);
                   
                }
                m_timer = TimerManager.Instance.AddTimer(1, delegate ()
                {
                    cpuTimeCount--;
                    ShowTimeCount(cpuTimeCount, eosInfo.refund_request.cpu_amount);
                }, Timer.TimerType.LifeCycle, uint.MaxValue);
            }

            ShowTimeCount(cpuTimeCount, eosInfo.refund_request.cpu_amount);
            
            net_prectText.text = "赎回中(剩余时间)";
        }
        else if (currentType == EOSResourcesBasePanel.ResourcesType.NetWork)
        {
            
            armPrectValue = float.Parse((eosInfo.eosNetwork.available * 1 / eosInfo.eosNetwork.max).ToString());
            arm_prectText.text = "可用(" + (armPrectValue * 100).ToString("f2") + "%)";
            armPrect.text = eosInfo.eosNetwork.available.ToString("f2") + "KB/" + eosInfo.eosNetwork.max.ToString("f2") + "KB";
            armImage.fillAmount = armPrectValue;

            cpuPrectValue = float.Parse((eosInfo.eosNetwork.used * 1 / eosInfo.eosNetwork.max).ToString());
            cpuImage.fillAmount = cpuPrectValue;

            cpuPrect.text = eosInfo.eosNetwork.used.ToString("f2") + "KB/" + eosInfo.eosNetwork.max.ToString("f2") + "KB";
            cpu_prectText.text = "占用(" + (cpuPrectValue * 100).ToString("f2") + "%)";

            netPrectValue = 0;
            netTimeCount = 0;

            if (eosInfo.refund_request.net_amount > 0)
            {
                netTimeCount = GetTimeStr(eosInfo.refund_request.request_time);
                if (m_timer != null)
                {
                    TimerManager.Instance.RemoveTimer(m_timer);
                }
                m_timer = TimerManager.Instance.AddTimer(1, delegate ()
                {
                    netTimeCount--;
                    ShowTimeCount(netTimeCount, eosInfo.refund_request.net_amount);
                }, Timer.TimerType.LifeCycle, uint.MaxValue);
            }

            ShowTimeCount(netTimeCount, eosInfo.refund_request.net_amount);

            net_prectText.text = "赎回中(剩余时间)";
        }

    }

    private int GetTimeStr(long overTime)
    {
        if (overTime <= 0)
            return 0;

        DateTime nowTime = DateTime.Now;
        //DateTime nowTime = Convert.ToDateTime("2018-09-06 10:33:19");
        DateTime dttd = new DateTime(overTime);

        TimeSpan ts = dttd - nowTime;

        int timeCount = ((ts.Days * 24) + ts.Hours) * 3600 + ts.Minutes * 60 + ts.Seconds;

        if (timeCount > 0)
        {
            return timeCount;
        }

        return 0;
    }

    public void ShowTimeCount(int timeCount, decimal eosAmount)
    {
        if (timeCount > 0)
        {
            int hour = timeCount / 3600;
            int minut = (timeCount - hour * 3600) / 60;
            int second = timeCount - hour * 3600 - minut * 60;

            string h = hour.ToString();
            if (hour < 10)
            {
                h = "0" + hour;
            }
            string m = minut.ToString();
            if (minut < 10)
            {
                m = "0" + minut;
            }
            string s = second.ToString();
            if (second < 10)
            {
                s = "0" + second;
            }
            networkPrect.text = h + ":" + m + ":" + s;

            float fillAmount = timeCount * 1.0F / (3 * 24 * 60 * 60);
            networkImage.fillAmount = fillAmount;


            int tempDay = hour / 24;
            int tempHours = hour % 24;
            string tempH = tempHours.ToString();
            if (tempHours < 10)
            {
                tempH = "0" + tempHours;
            }
            delegatebwObject.SetActive(true);
            //0.15EOS正在赎回中,预计赎回时间剩余00天00小时00分
            undelegatebwText.text = eosAmount + " EOS正在赎回中,预计赎回时间剩余" + "0" + tempDay + "天" + tempH + "小时" + m + "分";
        }
        else
        {
            delegatebwObject.SetActive(false);
            networkImage.fillAmount = 0;
            networkPrect.text = "00:00:00";
            if (m_timer != null)
            {
                TimerManager.Instance.RemoveTimer(m_timer);
                m_timer = null;
            }

        }
    }

    public void OnClickRAMBtn()
    {
        currentType = EOSResourcesBasePanel.ResourcesType.ARM;

        resourcesARM.Show();
        resourcesDBW.Hide();
        resourcesNetwork.Hide();

        ShowResourcesPrect();
    }
    public void OnClickCPUBtn()
    {
        currentType = EOSResourcesBasePanel.ResourcesType.CPU;

        resourcesARM.Hide();
        resourcesDBW.Show();
        resourcesNetwork.Hide();

        ShowResourcesPrect();
    }
    public void OnClicNetworkBtn()
    {
        currentType = EOSResourcesBasePanel.ResourcesType.NetWork;

        resourcesARM.Hide();
        resourcesDBW.Hide();
        resourcesNetwork.Show();

        ShowResourcesPrect();
    }
}
