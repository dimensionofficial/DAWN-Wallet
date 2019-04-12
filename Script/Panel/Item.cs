using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour,IPointerClickHandler
{
    public GameManager.CoinType m_type;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.OnSelectItem(this);
    }
}
