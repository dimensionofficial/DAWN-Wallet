using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BtnWordItem : MonoBehaviour
{

    private Text text;
    private Button btn;
    public BtnWordItem targetItem;

    public Text Text
    {
        get
        {
            if (text == null)
            {
                Transform t = transform.Find("Text");

                if(t != null)
                    text = t.GetComponent<Text>();
            }
            return text;
        }
    }

    public Button Btn
    {
        get
        {
            if (btn == null)
            {
                btn = transform.GetComponent<Button>();
            }
            return btn;
        }
    }

    public void OnClick()
    {
        Debug.Log(123);
    }
}
