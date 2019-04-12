using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PincodeInputArea : MonoBehaviour {
    public GameObject zhujiTips;//提示助记词不正确
    public bool canGetText = false;

    List<InputField> inputAreas = new List<InputField>();

    private Text tips;
    private int currIndex;
    
    void Start()
    {
        tips = zhujiTips.GetComponent<Text>();
        if (inputAreas.Count == 0)
        {
            foreach (Transform child in this.transform)
            {
                inputAreas.Add(child.GetComponent<InputField>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < inputAreas.Count; i++)
        {
            if (i == inputAreas.Count - 1)
            {
                if (inputAreas[i].isFocused == true)
                {
                    inputAreas[i].onValueChanged.AddListener(
                        delegate
                        {
                            CompleteSelfPinCode();
                        });
                }
            }
            else
            {
                if (inputAreas[i].isFocused == true)
                {
                    currIndex = i;
                    inputAreas[i].onValueChanged.AddListener(
                        delegate
                        {
                            CompletePinCode();
                        });
                }
            }                
        }

        foreach(InputField child in inputAreas)
        {
            if(child.text.Length < 4)
            {
                canGetText = false;
            }
            else
            {
                canGetText = true;
            }
        }
    }

    private void ChangeTipsColor(Text txt)
    {
        txt.color = Color.red;
    }

    public string GetText()
    {
        string result = "";
        for (int i = 0; i < inputAreas.Count; i++)
        {
            result += inputAreas[i].GetComponentInChildren<Text>().text;
            if (i != inputAreas.Count - 1)
            {
                result += " ";
            }
        }
        return result;
    }


    private void CompletePinCode()
    {
        if(inputAreas[currIndex].text.Length == 4)
        {
            inputAreas[currIndex].DeactivateInputField();
            inputAreas[currIndex + 1].ActivateInputField();
        }
       
    }
    private void CompleteSelfPinCode()
    {
        if (inputAreas[currIndex].text.Length == 4)
        {
            inputAreas[currIndex].DeactivateInputField();          
        }
    }
}
