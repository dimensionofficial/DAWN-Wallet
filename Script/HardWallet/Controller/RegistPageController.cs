using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class RegistPageController : BaseState
    {

        BaseUI ui;
        bool trans = false;

        public RegistPageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("RegistPanel").gameObject.AddComponent<RegistPageUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            trans = false;
            ui.Show(null);
            ui.events += (o) => {
                FlowManager.Instance.SetGlobalParams(FlowManager.GlobalData_TempCode, (string)o);
                trans = true;
            };
        }

        public override void OnTick()
        {

        }

        public override string CheckTransation()
        {
            if (trans)
            {
                trans = false;
                return "SNCodePageController";
            }
            return string.Empty;
        }

        public override void OnExit()
        {
            ui.Hide();
        }
    }
}
