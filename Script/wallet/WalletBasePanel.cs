using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WalletBasePanel : MonoBehaviour
{
    public virtual void Open()
    {
        this.gameObject.SetActive(true);
        Language();
    }
    public virtual void Hide()
    {
        OnHide();
        this.gameObject.SetActive(false);
    }

    public virtual void OnHide()
    {
        
    }

    public virtual void TweenMove(float startPos, float endPos)
    {
        transform.localPosition = new Vector3(startPos, transform.localPosition.y, transform.localPosition.z);
        transform.DOLocalMoveX(endPos, 0.2F).SetEase(Ease.InQuad);
    }

    public virtual void Language()
    {

    }

}
