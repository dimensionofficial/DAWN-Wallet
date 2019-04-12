using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputSeedWordBox : MonoBehaviour {

    private InputField m_Input;

    public InputField inputField
    {
        get
        {
            if (m_Input == null)
                m_Input = transform.GetComponent<InputField>();

            return m_Input;
        }
    }

    public void GetInputFocus()
    {
        inputField.ActivateInputField();
    }

    void Update()
    {
       
    }
}
