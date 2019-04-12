using UnityEngine;
using System.Collections;
using Nethereum.Hex.HexConvertors.Extensions;
namespace HardwareWallet
{
    public class SNCodePageController : BaseState
    {

        BaseUI ui;
        string trans = string.Empty;
        string tempHandCode = "";

        public SNCodePageController()
        {
            if (ui == null)
            {
                ui = FlowManager.Instance.transform.Find("SNCodePanel").gameObject.AddComponent<SNCodePageUI>();
            }
            ui.Ini();
        }

        public override void OnEnter()
        {
            trans = string.Empty;
            Battery.Instance.SetInfo(FlowManager.Instance.language.GetWord(Words.钱包命名));
            ui.Show(null);
            ui.events += (o) => {
                if ((string)o == "backBtn")
                {
                    trans = "SelectLanguagePageController";
                }
                else if ((string)o == "create")
                {
                    trans = "MainPageController";
                }
                else
                {
                    PlayerPrefs.SetString(FlowManager.SNCODE, (string)o);
                    string registData = FlowManager.Instance.GetGlobalParams(FlowManager.GlobalData_TempCode).ToString();
                    FlowManager.Instance.SetGlobalParams(FlowManager.GlobalData_TempCode, "");
                    FlowManager.Instance.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(Run(registData));
                }
            };
        }

        IEnumerator Run(string o)
        {
            Loading.Instace.Show();
            //ui.Hide();
            yield return new WaitForSeconds(2.5f);
            string data = (string)o;
            string[] datas = data.Split('%');
            string pincode = datas[1];
            string bip = datas[0];
            string handcode = datas[2];
            SaveAndLoad.Save(FlowManager.BIPKEYWORDS,
                System.Text.UTF8Encoding.UTF8.GetBytes(bip),
                System.Text.UTF8Encoding.UTF8.GetBytes(pincode));
            SaveAndLoad.Save(FlowManager.PINCODEVERYFI,
                System.Text.UTF8Encoding.UTF8.GetBytes(FlowManager.PINCODEVERYFI),
                System.Text.UTF8Encoding.UTF8.GetBytes(pincode));
            PlayerPrefs.SetString(FlowManager.HandCODEVERYFI, handcode);
            
            FlowManager.Instance.masterPuKey = QRPayTools.GetMastPubKey(bip);
            Loading.Instace.Hide();
            o = "";
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
