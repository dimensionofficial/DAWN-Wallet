using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace HardwareWallet
{
    public class BaseUI : MonoBehaviour
    {
        public bool smooth = false;
        public bool listenBack = false;
        public virtual void Ini()
        {
            this.gameObject.SetActive(false);
        }

        public virtual void Show(object param)
        {
            this.gameObject.SetActive(true);
            if (smooth)
            {
                StartCoroutine(Move());
            }

        }

        public virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && listenBack && events != null)
            {
                events("backBtn");
            }
        }

        public virtual void Hide()
        {
            events = null;
            GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            this.gameObject.SetActive(false);
        }

        IEnumerator Move()
        {
            GetComponent<RectTransform>().anchoredPosition = new Vector2(-1300, 0);
            float t = 0;
            while (t < 1)
            {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(-1300f, 0f, t), 0);
                t += 0.07f;
                yield return new WaitForFixedUpdate();
            }
            GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }

        public System.Action<object> events;
    }
}
