using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateOrResume : MonoBehaviour
{

    public GameObject SelectPanel;
    public CreateWalletPanel CreatePanel;
    public ResumeWalletPanel ResumePanel;


    // Use this for initialization
    public void OpenMe()
    {
        gameObject.SetActive(true);
        SelectPanel.SetActive(true);
        CreatePanel.gameObject.SetActive(false);
        ResumePanel.gameObject.SetActive(false);

    }
    public void SelectCreate()
    {
        CreatePanel.OpenMe();
        ResumePanel.gameObject.SetActive(false);
    }

    public void SelectResume()
    {
        CreatePanel.gameObject.SetActive(false);
        ResumePanel.OpenMe();
    }

    public void SelectBack()
    {
        OpenMe();
    }

    public void Complete()
    {
        this.gameObject.SetActive(false);
    }
}
