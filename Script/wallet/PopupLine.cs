using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupLine : MonoBehaviour {
    public static PopupLine Instance;
    [SerializeField]
    GameObject prefab;
    // Use this for initialization
    void Awake () {
        Instance = this;
    }

    public void Show(string text)
    {
        GameObject o = GameObject.Instantiate(prefab, this.transform);
        o.gameObject.SetActive(true);
        o.GetComponentInChildren<Text>().text = text;
        StartCoroutine(Run(o.GetComponentInChildren<Text>(), o.GetComponentInChildren<Image>()));
    }

    IEnumerator Run(Text t, Image i)
    {
        float tamp = 2.5F;
        while (tamp > 0)
        {
            t.color = new Color(1, 1, 1, tamp);
            i.color = new Color(1, 1, 1, tamp);
            i.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(
                new Vector2(0, -200), new Vector2(0, -271), tamp
                );
            tamp -= 0.02f;
            yield return new WaitForFixedUpdate();
        }
        Destroy(i.gameObject);
    }
}
