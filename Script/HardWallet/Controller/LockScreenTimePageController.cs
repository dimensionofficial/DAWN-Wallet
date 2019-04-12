using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class LockScreenTimePageController : BaseState
    {

        BaseUI ui;
        string trans = string.Empty;
        string tempHandCode = "";

        public LockScreenTimePageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("LockScreen").gameObject.AddComponent<LockScreenTimeUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            trans = string.Empty;
            string mode = (string)FlowManager.Instance.GetGlobalParams(FlowManager.GlobalData_SettingMode);
            ui.Show(mode);
            ui.events += (o) => {
                if ((string)o == "backBtn")
                {
                    FlowManager.Instance.stateManager.BackToLastState();
                }
                else
                {
                    if (mode == FlowManager.Instance.language.GetWord(Words.亮度调节))
                    {
                        int i = int.Parse((string)o);
                        float brighness = Mathf.Lerp(0.1f, 1f, ((float)i - 15f) / 45f);
                        FlowManager.Instance.SetApplicationBrightnessTo(brighness);
                        PlayerPrefs.SetFloat(FlowManager.SCREENLIGHT, brighness);
                    }
                    else
                    {
                        int i = int.Parse((string)o);
                        PlayerPrefs.SetInt(FlowManager.SCREENLIGHT, i);
                        if (i == 60)
                        {
                            Screen.sleepTimeout = SleepTimeout.NeverSleep;
                        }
                        else
                        {
                            Screen.sleepTimeout = i;
                        }
                    }
                }
            };
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
