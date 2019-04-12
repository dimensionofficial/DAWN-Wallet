using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveScreen : MonoBehaviour {
    // Use this for initialization
    private void Start()
    {
        int offet = NewWalletManager.statusBarOffset;
        if(offet > 0)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(0, -offet);
            rectTransform.anchoredPosition = new Vector2(0, -offet/2);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
    IEnumerator CaptureScreenshot()
    {
        //只在每一帧渲染完成后才读取屏幕信息
        yield return new WaitForEndOfFrame();

        Texture2D m_texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        // 读取Rect范围内的像素并存入纹理中
        m_texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        // 实际应用纹理
        m_texture.Apply();
        Color color = m_texture.GetPixel(1, 1);
    }
}
