using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class BackupPageController : BaseState
    {

        BaseUI ui;
        string trans = "";

        public BackupPageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("BackupPanel").gameObject.AddComponent<BackupUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            trans = "";
            PinCodeUI.Instace.Show((pincode) =>
            {
                IWalletDataBase walletBase = WalletDataFactory.CreateWalletData(FlowManager.Instance.walletType);
                string signedData = walletBase.GetBipWords(pincode);
                if (string.IsNullOrEmpty(signedData))
                {
                    PopBox.Instance.ShowMsg(FlowManager.Instance.language.GetWord(Words.内部错误));
                    FlowManager.Instance.stateManager.BackToLastState();
                }
                else
                {
                    ui.Show(signedData);
                    ui.events += (o) =>
                    {
                        string evt = (string)o;
                        switch (evt)
                        {
                            case "backBtn":
                                FlowManager.Instance.stateManager.BackToLastState();
                                break;
                            case "nextBtn":
                                //跳转backupcheck
                                FlowManager.Instance.SetGlobalParams(FlowManager.GlobalData_TempBip, signedData);
                                trans = "BackupCheckPageController";
                                break;
                        }
                    };
                }
            }, FlowManager.Instance.language.GetWord(Words.确认密码), ()=> {
                FlowManager.Instance.stateManager.BackToLastState();
            });
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
        }
    }
}
