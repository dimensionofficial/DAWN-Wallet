using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckWebView : MonoBehaviour
{
    public GameObject target;

    private UniWebView m_webView;
    public UniWebView webView
    {
        get
        {
            if (m_webView == null)
                m_webView = GetComponent<UniWebView>();

            return m_webView;
        }
    }

    void Update ()
    {
        if (target.activeInHierarchy == false)
        {
            if (webView != null)
            {
                webView.Hide();
            }
        }
	}
}
