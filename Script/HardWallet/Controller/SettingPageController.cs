using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class SettingPageController : BaseState
    {

        BaseUI ui;
        string trans = string.Empty;
        string tempHandCode = "";

        public SettingPageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("SettingPanel").gameObject.AddComponent<SettingPageUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            trans = string.Empty;
            ui.Show(null);
            ui.events += (o) => {
                if ((string)o == "backBtn")
                {
                    if (HandCodeUI.Instance.gameObject.activeSelf || PinCodeUI.Instace.gameObject.activeSelf)
                    {

                    }
                    else
                    {
                        trans = "MainPageController";
                    }
                }
                else if ((string)o == "coinType")
                {
                    trans = "CoinTypeManagerPageController";
                }
                else if ((string)o == "backUp")
                {
                    FlowManager.Instance.SetGlobalParams(FlowManager.GlobalData_BackFrom, "SettingPageController");
                    trans = "BackupInfoPageController";
                }
                else if ((string)o == "reset")
                {
                    PinCodeUI.Instace.Show((p) => {
                        FlowManager.Instance.Reset();
                        Application.LoadLevel(0);
                    }, FlowManager.Instance.language.GetWord(Words.恢复出厂设置),()=> { });
                }
                else if ((string)o == "handCode")
                {
                    ResetSetp1("");
                }
                else if ((string)o == "lighting")
                {
                    FlowManager.Instance.SetGlobalParams(FlowManager.GlobalData_SettingMode,
                        FlowManager.Instance.language.GetWord(Words.亮度调节));
                    trans = "LockScreenTimePageController";
                }
                else if ((string)o == "lockTime")
                {
                    trans = "LockTimePageController";
                }
                else if ((string)o == "language")
                {
                    trans = "SelectLanguagePageController";
                }
            };
        }

        void ResetSetp1(string info)
        {
            HandCodeUI.Instance.Show((s) => {
                if (s == PlayerPrefs.GetString(FlowManager.HandCODEVERYFI))
                {
                    ResetStep2("");
                    FlowManager.Instance.ClearHandCodeErrorCode();
                }
                else
                {
                    FlowManager.Instance.LockHandCodeAndPinCode();
                    ResetSetp1(FlowManager.Instance.language.GetWord(Words.密码错误));
                }
            }, 
            FlowManager.Instance.language.GetWord(Words.当前手势锁), info, ()=> { });
        }

        void ResetStep2(string info)
        {
            HandCodeUI.Instance.Show((s) => {
                tempHandCode = s;
                ResetStep3();
            },
            FlowManager.Instance.language.GetWord(Words.设置新手势锁), info,() => { });
        }

        void ResetStep3()
        {
            HandCodeUI.Instance.Show((s) => {
                if (tempHandCode == s)
                {
                    PlayerPrefs.SetString(FlowManager.HandCODEVERYFI, s);
                    PopBox.Instance.ShowMsg(FlowManager.Instance.language.GetWord(Words.手势锁修改成功), null, 
                        FlowManager.Instance.language.GetWord(Words.设置));
                }
                else
                {
                    ResetStep2(FlowManager.Instance.language.GetWord(Words.两次手势不相同));
                }
            },
            FlowManager.Instance.language.GetWord(Words.确认新手势锁), "", () => { });
        }

        public override void OnTick()
        {
            
        }

        public override string CheckTransation()
        {
            return trans;
        }

        public override void OnExit()
        {
            ui.Hide();
        }
    }
}
