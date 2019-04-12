using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HardwareWallet
{
    public class Loading : MonoBehaviour
    {
        public static Loading Instace;
        [SerializeField]
        Image image;
        [SerializeField]
        List<Sprite> sps = new List<Sprite>();
        int index = 0;

        void Awake()
        {
            Instace = this;
            this.gameObject.SetActive(false);
            for (int i = 1; i <= 35; i++)
            {
                Sprite s = Resources.Load<Sprite>("Ani/wallet" + i.ToString().PadLeft(4, '0'));
                sps.Add(s);
            }
        }

        public void Show()
        {
            this.gameObject.SetActive(true);
            index = 0;
            StartCoroutine(Run());
        }

        IEnumerator Run()
        {
            while (true)
            {
                image.sprite = sps[index];
                yield return new WaitForSeconds(0.08f);
                index++;
                if (index >= sps.Count)
                {
                    index = 0;
                }
            }
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}