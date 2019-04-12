using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldManage : MonoBehaviour {

    public static InputFieldManage instance;
    private void Awake()
    {
        instance = this;
    }
    private InputFieldManage() { }
    public List<GameObject> allInputFields=new List<GameObject>();
    List<GameObject> curInputFields=new List<GameObject>();
    public void CloseInputFields()
    {
        curInputFields.Clear();
        for (int i = 0; i < allInputFields.Count; i++)
        {
            if (allInputFields[i].activeInHierarchy)
            {
                curInputFields.Add(allInputFields[i]);
                allInputFields[i].SetActive(false);
            }
        }
    }
    public void OpenInputFields()
    {
        for (int i = 0; i < curInputFields.Count; i++)
        {
            curInputFields[i].SetActive(true);
        }
    }
}
