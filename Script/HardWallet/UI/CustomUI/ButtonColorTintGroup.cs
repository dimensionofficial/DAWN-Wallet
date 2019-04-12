using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
namespace HardwareWallet
{
    public class ButtonColorTintGroup : Button
    {
        public List<Graphic> m_graphics = new List<Graphic>();
        public Graphic[] Graphics
        {
            get
            {
                m_graphics.Clear();
                foreach (var v in targetGraphic.transform.GetChild(0).GetComponentsInChildren<Graphic>())
                {
                    m_graphics.Add(v);
                }
                if (targetGraphic.transform.GetChild(0).GetComponent<Graphic>() != null)
                {
                    m_graphics.Add(targetGraphic.transform.GetChild(0).GetComponent<Graphic>());
                }
                return m_graphics.ToArray(); 
            }
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            Color color;
            switch (state)
            {
                case Selectable.SelectionState.Normal:
                    color = Color.white;
                    break;
                case Selectable.SelectionState.Highlighted:
                    color = new Color(77f/ 255f, 223f/ 255f, 143f/255f);
                    break;
                case Selectable.SelectionState.Pressed:
                    color = new Color(77f / 255f, 223f / 255f, 143f / 255f);
                    break;
                case Selectable.SelectionState.Disabled:
                    color = Color.gray;
                    break;
                default:
                    color = Color.black;
                    break;
            }
            foreach (var v in Graphics)
            {
                v.color = color;
            }
            base.DoStateTransition(state, instant);
        }
    }
}
