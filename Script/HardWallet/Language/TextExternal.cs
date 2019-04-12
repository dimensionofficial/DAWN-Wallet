using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace HardwareWallet
{
    public class TextExternal : MonoBehaviour {
        public Words word;
        void OnEnable()
        {
            if (FlowManager.Instance != null && FlowManager.Instance.language != null)
            {
                this.GetComponent<Text>().text = FlowManager.Instance.language.GetWord(word);
            }
        }
    }
}
