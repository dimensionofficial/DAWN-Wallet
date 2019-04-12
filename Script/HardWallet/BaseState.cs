using UnityEngine;
using System.Collections;
namespace HardwareWallet
{
    public class BaseState
    {

        public string stateName;
        public Transform Owner;

        public BaseState()
        {

        }

        public virtual void OnEnter()
        {

        }

        public virtual void OnTick()
        {

        }

        public virtual string CheckTransation()
        {
            return string.Empty;
        }

        public virtual void OnExit() { }
    }
}
