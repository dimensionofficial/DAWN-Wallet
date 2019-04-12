using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public virtual void Open()
    {
        this.gameObject.SetActive(true);
    }
    public virtual void Hide()
    {
        OnHide();
        this.gameObject.SetActive(false);
    }

    public virtual void OnHide()
    {

    }
}
