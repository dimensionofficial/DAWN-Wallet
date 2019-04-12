using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpBox : MonoBehaviour {

    public static PopUpBox Instance;

    [SerializeField]
    Image background;
    [SerializeField]
    Sprite type1;
    [SerializeField]
    Sprite type2;
    [SerializeField]
    Button okBtn;
    [SerializeField]
    Button cancelBtn;
    [SerializeField]
    Text title;
    [SerializeField]
    Text description;
    [SerializeField]
    Text ok;
    [SerializeField]
    Text cancel;

    System.Action onOK;
    System.Action onCancel;



    public void Show(System.Action onOK, System.Action onCancel, string ok, string cancel, string title, string content)
    {
        this.onOK = onOK;
        this.onCancel = onCancel;
        this.title.text = title;
        this.description.text = content;
        this.ok.text = ok;
        this.cancel.text = cancel;

        if (onCancel == null)
        {
            background.sprite = type1;
            cancelBtn.gameObject.SetActive(false);
        }
        else
        {
            background.sprite = type2;
            cancelBtn.gameObject.SetActive(true);
        }
        this.gameObject.SetActive(true);
    }

    void Awake()
    {
        Instance = this;
        okBtn.onClick.AddListener(()=>{

            this.gameObject.SetActive(false);
            if (onOK != null)
            {
                onOK();
            }
        });
        cancelBtn.onClick.AddListener(() => {

            this.gameObject.SetActive(false);
            if (onCancel != null)
            {
                onCancel();
            }
           
        });
        this.gameObject.SetActive(false);
    }
}
