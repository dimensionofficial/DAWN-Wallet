using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class LockTimePageController : BaseState
    {

        BaseUI ui;
        string trans = string.Empty;
        string tempHandCode = "";

        public LockTimePageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("LockScreenA").gameObject.AddComponent<LockTimeUI>();
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
                    FlowManager.Instance.stateManager.BackToLastState();
                }
                else
                {
                    int i = int.Parse((string)o);
                    PlayerPrefs.SetInt(FlowManager.LOCKTIME, i);
                    if (i == 1000)
                    {
                        Screen.sleepTimeout = SleepTimeout.NeverSleep;
                    }
                    else
                    {
                        Screen.sleepTimeout = i;
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
