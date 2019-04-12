using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HardwareWallet
{
    public class RebootUI : MonoBehaviour
    {

        public static RebootUI Instance;
        [SerializeField]
        Button backgroundBtn;
        [SerializeField]
        Button rebootBtn;
        [SerializeField]
        Button shutdownBtn;

        void Awake()
        {
            Instance = this;
            Hide();
            backgroundBtn.onClick.AddListener(() =>
            {
                Hide();
            });
            rebootBtn.onClick.AddListener(() =>
            {
                FlowManager.Instance.Reboot_imp();
            });
            shutdownBtn.onClick.AddListener(() =>
            {
                FlowManager.Instance.ShutDown_imp();
            });
        }

        public void Show()
        {
            this.gameObject.SetActive(true);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
        }
    }
}
