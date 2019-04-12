using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
namespace HardwareWallet
{
    public class MoveOverListener : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IBeginDragHandler
    {
        public System.Action<MoveOverListener> OnMoveEnter;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (OnMoveEnter != null)
            {
                OnMoveEnter(this);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (OnMoveEnter != null)
            {
                OnMoveEnter(this);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (OnMoveEnter != null)
            {
                OnMoveEnter(this);
            }
        }
    }
}
