using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class BackupCheckPageController : BaseState
    {

        BaseUI ui;
        string trans = "";

        public BackupCheckPageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("BackupCheckPanel").gameObject.AddComponent<BackupCheckUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            trans = "";
            object bip = FlowManager.Instance.GetGlobalParams(FlowManager.GlobalData_TempBip);
            if (bip == null || bip.GetType() != typeof(string) || !QRPayTools.VerifyBip((string)bip))
            {
                PopBox.Instance.ShowMsg(FlowManager.Instance.language.GetWord(Words.内部错误), () => {
                    FlowManager.Instance.stateManager.BackToLastState();
                });
                return;
            }
            ui.Show(bip);
            ui.events += (o) => {
                string evt = (string)o;
                switch (evt)
                {
                    case "backBtn":
                        trans = "BackupInfoPageController";
                        break;
                    case "nextBtn":
                        if (FlowManager.Instance.GetGlobalParams(FlowManager.GlobalData_NeverShowBackupInfeture) != null && 
                            (string)FlowManager.Instance.GetGlobalParams(FlowManager.GlobalData_NeverShowBackupInfeture) == "true")
                        {
                            FlowManager.Instance.SetBackup();
                        }
                        trans = FlowManager.Instance.GetGlobalParams(FlowManager.GlobalData_BackFrom).ToString();
                        break;
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
            trans = "";
            FlowManager.Instance.SetGlobalParams(FlowManager.GlobalData_TempBip, "");
        }
    }
}
