using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NBitcoin;
using System;
using Nethereum.Hex.HexConvertors.Extensions;
namespace HardwareWallet
{
    public class FlowManager : MonoBehaviour
    {
        public static string BIPKEYWORDS = "bip";
        public static string PINCODEVERYFI = "pincode verify";
        public static string HandCODEVERYFI = "hand";
        public static string LOCKTIME = "lock time";
        public static string SCREENLIGHT = "screen light";
        public static string LOCKCOLDCOLD = "handcode cold";
        public static string LOCKCOLDCOUNT = "handcode cold Count";
        public static string SNCODE = "sn code";
        public static string LANGUWAGE = "language";
        public static string GlobalData_TempCode = "GlobalData_TempCode";
        public static string GlobalData_NeverShowBackupInfeture = "GlobalData_NeverShowBackupInfeture";
        public static string GlobalData_TempBip = "GlobalData_TempBip";
        public static string GlobalData_SettingMode = "GlobalData_SettingMode";
        public static string GlobalData_ScanFrom = "GlobalData_ScanFrom";
        public static string GlobalData_BackFrom = "GlobalData_BackUpFrom";
        public static FlowManager Instance;
        public StateManager stateManager;
        public WalletType walletType = WalletType.BTC;
        public List<WalletType> viewWalletTypeList = new List<WalletType>();
        public QRCodeManager qRCodeManager = new QRCodeManager();
        public NBitcoin.Network network = NBitcoin.Network.Main; 
        public Dictionary<string, object> globalParams = new Dictionary<string, object>();
        public List<Sprite> coinIcon = new List<Sprite>();
        public List<string> coinDes = new List<string>();
        public List<string> coinFullDes = new List<string>();
        public string version = "V1.0";
        private Dictionary<WalletType, string> publicKeys = new Dictionary<WalletType, string>();
        public ExtPubKey masterPuKey
        {
            get {
                string d = PlayerPrefs.GetString("masterKey", "");
                return new ExtPubKey(d.HexToByteArray());
            }
            set
            {
                PlayerPrefs.SetString("masterKey", value.ToBytes().ToHex());
            }
        }
        public Language language;
        public bool isDebug;

        void Awake()
        {
            language = LoadLanguage(PlayerPrefs.GetString(LANGUWAGE));
        }

        void Start()
        {
            EOSWalletData eos = new EOSWalletData();
            Debug.Log(eos.CreateAddressStringWithPrivateKey("b68062cc24a340b5ca7a73a25079698360e3afb7ad658aef7c4458f86e86bb0e"));
            Debug.Log(eos.CreateAddressStringWithPrivateKey("147ebbff10acc44937851fec3b5289f49a23ee8b1af0ed111469a9364e624ef6"));
            if (isDebug)
            {
                PlayerPrefs.DeleteAll();
            }
            if (PlayerPrefs.HasKey(LOCKTIME))
            {
                Screen.sleepTimeout = PlayerPrefs.GetInt(LOCKTIME);
            }
            else
            {
                Screen.sleepTimeout = 30;
            }
            if (PlayerPrefs.HasKey(SCREENLIGHT))
            {
                SetApplicationBrightnessTo(PlayerPrefs.GetFloat(SCREENLIGHT));
            }
            else
            {
                SetApplicationBrightnessTo(0.8f);
            }
            LoadWalletCoinType();
            LoadPublicKey();
            //PlayerPrefs.DeleteAll();
            //mainNetPrivateKey = new BitcoinSecret("cTfiAtwm9wwtihkZqXhqhGYnw7vNtCazN4hz8qCgWSku5uNdphRT");
            //changeScriptPubKey = mainNetPrivateKey.PubKey.GetAddress(network);
            Instance = this;
            stateManager = new StateManager(transform);
            stateManager.AddState(new WelComePageController() { stateName = "WelComePageController", Owner = this.transform });
            stateManager.AddState(new LoginPageController() { stateName = "LoginPageController", Owner = this.transform });
            stateManager.AddState(new MainPageAController() { stateName = "MainPageController", Owner = this.transform });
            stateManager.AddState(new ScanQRPageController() { stateName = "ScanQRPageController", Owner = this.transform });
            stateManager.AddState(new TransactionPreviewPageController() { stateName = "TransactionPreviewPageController", Owner = this.transform });
            stateManager.AddState(new TransactionCompletePageController() { stateName = "TransactionCompletePageController", Owner = this.transform });
            stateManager.AddState(new RegistPageController() { stateName = "RegistPageController", Owner = this.transform });
            stateManager.AddState(new BackupPageController() { stateName = "BackupPageController", Owner = this.transform });
            stateManager.AddState(new CoinAddressPageController() { stateName = "CoinAddressPageController", Owner = this.transform });
            stateManager.AddState(new CoinDetailsPageController() { stateName = "CoinDetailsPageController", Owner = this.transform });
            stateManager.AddState(new AddCoinViewAPageController() { stateName = "AddCoinViewPageController", Owner = this.transform });
            stateManager.AddState(new SelectLanguagePageController() { stateName = "SelectLanguagePageController", Owner = this.transform });
            stateManager.AddState(new SettingPageController() { stateName = "SettingPageController", Owner = this.transform });
            stateManager.AddState(new AddCoinViewPageController() { stateName = "CoinTypeManagerPageController", Owner = this.transform });
            stateManager.AddState(new BackupInfoPageController() { stateName = "BackupInfoPageController", Owner = this.transform });
            stateManager.AddState(new BackupCheckPageController() { stateName = "BackupCheckPageController", Owner = this.transform });
            stateManager.AddState(new LockScreenTimePageController() { stateName = "LockScreenTimePageController", Owner = this.transform });
            stateManager.AddState(new LockTimePageController() { stateName = "LockTimePageController", Owner = this.transform });
            stateManager.AddState(new StartSettingIniPageController() { stateName = "StartSettingIniPageController", Owner = this.transform });
            stateManager.AddState(new SNCodePageController() { stateName = "SNCodePageController", Owner = this.transform });
            stateManager.SetDefaultState("WelComePageController");
         
        }

        void Update()
        {
            stateManager.Update();
        }
        #region public Method

        public Language LoadLanguage(string o)
        {
            switch (o)
            {
                case "0":
                    return new SimpleChinese();
                case "1":
                    return new TWChinese();
                case "2":
                    return new English();
                case "3":
                    return new Janpanese();
                default:
                    return new SimpleChinese();
            }
        }

        public void SetLanguage(string o)
        {
            PlayerPrefs.SetString(LANGUWAGE, o);
            switch (o)
            {
                case "0":
                    language = new SimpleChinese();
                    break;
                case "1":
                    language = new TWChinese();
                    break;
                case "2":
                    language = new English();
                    break;
                case "3":
                    language = new Janpanese();
                    break;
                default:
                    language = new SimpleChinese();
                    break;
            }
        }

        public void AddViewWalletCoinType(WalletType type)
        {
            if (!viewWalletTypeList.Contains(type))
            {
                viewWalletTypeList.Add(type);
            }
            SaveWalletCoinType();
        }

        public void DeleteViewWalletCoinType(WalletType type)
        {
            if (viewWalletTypeList.Contains(type))
            {
                viewWalletTypeList.Remove(type);
            }
            SaveWalletCoinType();
        }

        private void SaveWalletCoinType()
        {
            ArrayList a = new ArrayList();
            foreach (var v in viewWalletTypeList)
            {
                a.Add((int)v);
            }
            string json = Json.jsonEncode(a);
            PlayerPrefs.SetString("WalletTypeView", json);
        }

        private void LoadWalletCoinType()
        {
            viewWalletTypeList.Clear();
            viewWalletTypeList.Add(WalletType.EOS);
            viewWalletTypeList.Add(WalletType.ETH);
            viewWalletTypeList.Add(WalletType.BTC);
            return;
            if (PlayerPrefs.HasKey("WalletTypeView"))
            {
                try
                {
                    string json = PlayerPrefs.GetString("WalletTypeView");
                    ArrayList a = Json.jsonDecode(json) as ArrayList;
                    foreach (var v in a)
                    {
                        viewWalletTypeList.Add((WalletType)int.Parse(v.ToString()));
                    }
                }
                catch
                {

                }
            }
            
        }

        private void LoadPublicKey()
        {
            foreach (WalletType type in Enum.GetValues(typeof(WalletType)))
            {
                if (PlayerPrefs.HasKey(type.ToString()) && !publicKeys.ContainsKey(type))
                {
                    publicKeys.Add(type, PlayerPrefs.GetString(type.ToString()));
                    //Debug.Log(PlayerPrefs.GetString(type.ToString()));
                }
            }
        }

        public void SavePublicKey(WalletType type, string data) 
        {
            if (!publicKeys.ContainsKey(type))
            {
                publicKeys.Add(type, data);
            }
            else
            {
                publicKeys[type] = data;
            }
            PlayerPrefs.SetString(type.ToString(), data);
        }

        public string GetPublicKey(WalletType type)
        {
            if (publicKeys.ContainsKey(type))
            {
                return publicKeys[type];
            }
            else
            {
                return null;
            }
        }

        public string GetMasterPublicKey()
        {
            ExtPubKey pubkey = new ExtPubKey(masterPuKey.ToBytes());
            return pubkey.ToBytes().ToHex();
        }

        /// <summary>
        /// 是否备份
        /// </summary>
        /// <returns></returns>
        public bool IsBackUp()
        {
            if (PlayerPrefs.HasKey("IsBackUp") && PlayerPrefs.GetString("IsBackUp") == "true")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置已备份
        /// </summary>
        public void SetBackup()
        {
            PlayerPrefs.SetString("IsBackUp", "true");
        }

        /// <summary>
        /// 设置全局变量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="o"></param>
        public void SetGlobalParams(string key, object o)
        {
            if (globalParams.ContainsKey(key))
            {
                globalParams[key] = o;
            }
            else
            {
                globalParams.Add(key, o);
            }
        }

        /// <summary>
        /// 获取全局变量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetGlobalParams(string key)
        {
            if (globalParams.ContainsKey(key))
            {
                return globalParams[key];
            }
            else
            {
                return null;
            }
        }
        #endregion

        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && stateManager != null)
            {
                if (!PlayerPrefs.HasKey(FlowManager.BIPKEYWORDS))
                {
                    //stateManager.CrossFade("WelComePageController");
                }
                else if(!HandCodeUI.Instance.gameObject.activeSelf)
                {
                    HandCodeUI.Instance.Show((s) => {
                        HandCodeCallback(s);
                    }, language.GetWord(Words.手势解锁));
                }
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus && stateManager != null)
            {
                if (!PlayerPrefs.HasKey(FlowManager.BIPKEYWORDS))
                {
                    //stateManager.CrossFade("WelComePageController");
                }
                else if (!HandCodeUI.Instance.gameObject.activeSelf)
                {
                    HandCodeUI.Instance.Show((s) => {
                        HandCodeCallback(s);
                    }, language.GetWord(Words.手势解锁));
                }
            }
        }

        public void HandCodeCallback(string s)
        {
            string handCode = PlayerPrefs.GetString(FlowManager.HandCODEVERYFI);
            if (s != handCode)
            {
                LockHandCodeAndPinCode();
                Handheld.Vibrate();
                HandCodeUI.Instance.Show((o) =>
                {
                    HandCodeCallback(o);
                }, language.GetWord(Words.手势解锁), language.GetWord(Words.密码错误));
            }
            else
            {
                ClearHandCodeErrorCode();
            }
        }

        /// <summary>
        /// 手势密码错误锁屏
        /// </summary>
        public void LockHandCodeAndPinCode()
        {
            int errorLevel = PlayerPrefs.GetInt(LOCKCOLDCOUNT, 0);
            errorLevel += 1;
            PlayerPrefs.SetInt(LOCKCOLDCOUNT, errorLevel);
            if (errorLevel > 10)
            {
                var errorLockTime = GetLockTimeByLevel(errorLevel - 10);
                PlayerPrefs.SetString(LOCKCOLDCOLD, DateTime.Now.AddSeconds(errorLockTime).ToString());
            }
        }

        /// <summary>
        /// 清楚错误手势锁定
        /// </summary>
        public void ClearHandCodeErrorCode()
        {
            PlayerPrefs.DeleteKey(LOCKCOLDCOLD);
            PlayerPrefs.DeleteKey(LOCKCOLDCOUNT);
        }

        /// <summary>
        /// 获取手势密码错误剩余惩罚时间
        /// </summary>
        /// <returns></returns>
        public int GetHandLockSec()
        {
            try
            {
                DateTime time = DateTime.Parse(PlayerPrefs.GetString(LOCKCOLDCOLD));
                if (DateTime.Now >= time)
                {
                    return 0;
                }
                else
                {
                    return (int)(time - DateTime.Now).TotalSeconds;
                }
            }
            catch(Exception e)
            {
                return 0;
            }
        }

        /// <summary>
        /// 错误密码惩罚时间
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public int GetLockTimeByLevel(int level)
        {
            switch (level)
            {
                case 1:
                    return 60;
                case 2:
                    return 60 * 5;
                case 3:
                    return 60 * 10;
                case 4:
                    return 60 * 15;
                case 5:
                    return 60 * 30;
                case 6:
                    return 60 * 45;
                case 7:
                    return 60 * 60;
                default:
                    return 60 * 60;
            }
        }

        /// <summary>
        /// 亮度调节
        /// </summary>
        /// <param name="Brightness"></param>
        public void SetApplicationBrightnessTo(float Brightness)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject Activity = null;
                Activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                Activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject Window = null, Attributes = null;
                    Window = Activity.Call<AndroidJavaObject>("getWindow");
                    Attributes = Window.Call<AndroidJavaObject>("getAttributes");
                    Attributes.Set("screenBrightness", Brightness);
                    Window.Call("setAttributes", Attributes);
                }));
            }
        }

        public void ShutDown()
        {
            RebootUI.Instance.Show();
        }

        public void ShutDown_imp()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "Close_Wallet");
                jo.Call("sendBroadcast", intent);
            }
        }

        public void Reboot_imp()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "Reboot_Wallet");
                jo.Call("sendBroadcast", intent);
            }
        }

        /// <summary>
        /// 重置钱包
        /// </summary>
        public void Reset()
        {
            PlayerPrefs.DeleteAll();
            globalParams.Clear();
            publicKeys.Clear();
        }
    }
    public enum WalletType
    {
        BTC = 0,
        ETH = 1,
        EOS = 2,
        Other = 3
    }
}
