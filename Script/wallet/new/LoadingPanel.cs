using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class LoadingPanel : MonoBehaviour
{

    public Action disableCallBack;
    public List<Sprite> loadSprites = new List<Sprite>();
    public Image m_image;
    private float fps = 10.0f;
    private float time = 0;
    private int spriteIndex = 0;
    void OnEnable()
    {
        spriteIndex = 0;
        m_image.overrideSprite = loadSprites[spriteIndex];
    }

    void OnDisable()
    {
        if (disableCallBack != null)
            disableCallBack();
    }

    // Update is called once per frame
    void Update ()
    {
        time += Time.deltaTime;

        if (time >= 1.0f / fps)
        {
            spriteIndex++;
            time = 0;
            if (spriteIndex > loadSprites.Count - 1)
            {
                spriteIndex = 0;
            }
            m_image.overrideSprite = loadSprites[spriteIndex];
        }
    }
}
