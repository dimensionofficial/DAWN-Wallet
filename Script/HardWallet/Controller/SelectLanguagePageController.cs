using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class SelectLanguagePageController : BaseState
    {

        BaseUI ui;
        string trans = "";
        public static bool isIni = false;
        public SelectLanguagePageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("SelectLanguagePanel").gameObject.AddComponent<SelectLanguageUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            trans = "";
            ui.Show(null);
            bool isiniIn = isIni;
            isIni = false;
            Battery.Instance.SetInfo(FlowManager.Instance.language.GetWord(Words.DracoWallet));
            ui.events += (o) => {
                string l = (string)o;
                switch (l)
                {
                    case "jp":
                        FlowManager.Instance.SetLanguage("3");
                        break;
                    case "spchinese":
                        FlowManager.Instance.SetLanguage("0");
                        break;
                    case "chinese":
                        FlowManager.Instance.SetLanguage("1");
                        break;
                    case "english":
                        FlowManager.Instance.SetLanguage("2");
                        break;
                }
                if (isiniIn)
                {
                    trans = "StartSettingIniPageController";
                }
                else
                {
                    FlowManager.Instance.stateManager.BackToLastState();
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
