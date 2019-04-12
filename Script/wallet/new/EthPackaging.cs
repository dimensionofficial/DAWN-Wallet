using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DG.Tweening;



public class EthPackaging : MonoBehaviour
{
    public Image m_sprite;

    private void Show(int p)
    {
        Debug.Log(p);
        float endValue = p * 1.0F / 12;
        DOTween.To(() => m_sprite.fillAmount, x => m_sprite.fillAmount = x, endValue, 0.5F);
        if (endValue >= 1)
        {
  //          PackagEnd();
        }
    }

    //public void PackagEnd()
    //{
    //    EthTokenManager._Intance.StopCoroutine(EthTokenManager._Intance.Packaging(Show));
    //}

    //public void StartPackag()
    //{
    //    m_sprite.fillAmount = 0;
    //    EthTokenManager._Intance.StartCoroutine(EthTokenManager._Intance.Packaging(Show));
    //}
}
