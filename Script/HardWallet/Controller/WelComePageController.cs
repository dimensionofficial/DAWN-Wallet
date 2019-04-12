using UnityEngine;
using System.Collections;
using System;
namespace HardwareWallet
{
    public class WelComePageController : BaseState
    {

        BaseUI ui;
        string trans = "";
        DateTime timer;
        public WelComePageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("WelComePanel").gameObject.AddComponent<WelComePageUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            ui.Show(null);
            trans = "";
            timer = DateTime.Now;
            if (PlayerPrefs.HasKey(FlowManager.BIPKEYWORDS))
            {
                HandCodeUI.Instance.Show((s) =>
                {
                    HandCodeCallback(s);
                }, FlowManager.Instance.language.GetWord(Words.手势解锁));
            }
            else
            {
                SelectLanguagePageController.isIni = true;
                trans = "SelectLanguagePageController";
            }
        }

        public void HandCodeCallback(string s)
        {
            string handCode = PlayerPrefs.GetString(FlowManager.HandCODEVERYFI);
            if (s != handCode)
            {
                FlowManager.Instance.LockHandCodeAndPinCode();
                Handheld.Vibrate();
                HandCodeUI.Instance.Show((o) =>
                {
                    HandCodeCallback(o);
                }, FlowManager.Instance.language.GetWord(Words.手势解锁), 
                FlowManager.Instance.language.GetWord(Words.密码错误));
            }
            else
            {
                FlowManager.Instance.ClearHandCodeErrorCode();
                trans = "MainPageController";
            }
        }

        public override void OnTick()
        {
            /*
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0) || (DateTime.Now - timer).TotalSeconds >= 2)
            {
                if (PlayerPrefs.HasKey(FlowManager.BIPKEYWORDS))
                {
                    trans = "MainPageController";
                }
                else

                {
                    trans = "SelectLanguagePageController";
                }
            }
            */
            
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
