using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace HardwareWallet
{
    public class Battery : MonoBehaviour
    {
        public static Battery Instance;
        [SerializeField]
        RectTransform battery;
        [SerializeField]
        Text info;

        void Awake()
        {
            Instance = this;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            battery.sizeDelta = new Vector2(SystemInfo.batteryLevel * 78f, 30.67f);
            if (SystemInfo.batteryStatus == BatteryStatus.Charging)
            {
                battery.GetComponent<Image>().color = Color.green;
            }
            else
            {
                battery.GetComponent<Image>().color = Color.white;
            }
        }

        public void SetInfo(string i)
        {
            info.text = i;
        }
    }
}
