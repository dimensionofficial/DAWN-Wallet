using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HotBasePanel : MonoBehaviour
{
    private bool isTweening = false;
    public float showX = 0;
    public float hideX = -1000;

    public virtual void Open()
    {
        this.gameObject.SetActive(true);
        if (isTweening)
        {
            Come();
        }
    }

    private void Come()
    {
        transform.localPosition = new Vector3(hideX, transform.localPosition.y, transform.localPosition.z);
        transform.DOLocalMoveX(showX, 0.2F).SetEase(Ease.OutQuint);
    }

    private void BackTween()
    {
        transform.DOLocalMoveX(hideX, 0.2F).SetEase(Ease.OutQuint).OnComplete(() =>ClosedLater());
    }

    public virtual void Closed()
    {
        if (isTweening)
        {
            BackTween();
        }
        else
        {
            ClosedLater();
        }
    }

    private void ClosedLater()
    {
        OnClosedPanel();
        this.gameObject.SetActive(false);
    }

    public virtual void OnClosedPanel()
    {

    }
}
