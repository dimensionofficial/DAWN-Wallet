using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarControl : MonoBehaviour {
    public GameObject loginObj;
    public RectTransform coinPage;
    public Image imageOffset;
    public Color colorStatusBar;
    public Color colorStatusBar2;
    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    private void Update()
    {
        if (!loginObj.activeInHierarchy)
        {
            imageOffset.color = colorStatusBar;
            if(coinPage.gameObject.activeInHierarchy && coinPage.localPosition.x == 0)
                imageOffset.color = colorStatusBar2;
        }
        else if (loginObj.activeInHierarchy)
            imageOffset.color = Color.white;
    }
}
