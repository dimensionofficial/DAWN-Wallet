using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SafeKeyBoard : MonoBehaviour {
    public System.Action<int> keyDown;
    public System.Action onOK;
    public System.Action onBack;
    float y;
    void Awake()
    {
        y = this.GetComponent<RectTransform>().anchoredPosition.y;
        this.gameObject.SetActive(false);
        for (int i = 0; i <= 9; i++)
        {
            int a = i;
            transform.Find(i.ToString()).GetComponent<Button>().onClick.AddListener(() => {
                if (keyDown != null)
                {
                    keyDown(a);
                }
            });
        }
        transform.Find("ok").GetComponent<Button>().onClick.AddListener(() => {
            if (onOK != null)
            {
                onOK();
            }
        });

        transform.Find("back").GetComponent<Button>().onClick.AddListener(() => {
            if (onBack != null)
            {
                onBack();
            }
        });
    }

    public void Show(System.Action<int> keyDown, System.Action onOK, System.Action onBack)
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1300);
        this.gameObject.SetActive(true);
        this.keyDown = keyDown;
        this.onOK = onOK;
        this.onBack = onBack;
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1300);
        float t = 0;
        while (t < 1)
        {
            GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Mathf.Lerp(-1300f, y, t));
            t += 0.07f;
            yield return new WaitForFixedUpdate();
        }
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        this.keyDown = null;
        this.onOK = null;
        this.onBack = null;
    }
}
