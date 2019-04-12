using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace HardwareWallet
{
    public class StateManager
    {

        private Dictionary<string, BaseState> cacheStates = new Dictionary<string, BaseState>();

        private BaseState currentState;

        private BaseState lastState;

        private Transform myOwner;

        public Transform Owner
        {
            get { return myOwner; }
        }

        public StateManager(Transform owner)
        {
            myOwner = owner;
        }

        public string LastState
        {
            get
            {
                return lastState.stateName;
            }
        }

        /// <summary>
        /// 设置初始状态
        /// </summary>
        /// <param name="stateName">State name.</param>
        public void SetDefaultState(string stateName)
        {
            if (stateName != string.Empty || stateName != "")
            {
                if (cacheStates.ContainsKey(stateName))
                {
                    currentState = cacheStates[stateName];
                    currentState.OnEnter();
                }
            }
        }

        public void Update()
        {
            if (currentState == null)
                return;
            string result = currentState.CheckTransation();
            CrossFade(result);
            currentState.OnTick();
        }

        public void AddState(BaseState state)
        {
            if (cacheStates.ContainsKey(state.stateName))
            {
                Debug.LogError("same state already contains");
                return;
            }
            state.Owner = myOwner;
            cacheStates.Add(state.stateName, state);
        }

        public void RemoveState(string stateName)
        {
            if (cacheStates.ContainsKey(stateName))
            {
                cacheStates.Remove(stateName);
            }
            else
            {
                Debug.LogError("Remove failure");
            }
        }

        public void CrossFade(string name)
        {
            if (name != string.Empty || name != "")
            {
                if (cacheStates.ContainsKey(name))
                {
                    lastState = currentState;
                    currentState = cacheStates[name];
                    lastState.OnExit();
                    currentState.OnEnter();
                }
                else
                {
                    Debug.LogError("No such key in cacheStates");
                }
            }
        }

        public void BackToLastState()
        {
            CrossFade(lastState.stateName);
        }

        private string GetCurrentStateName()
        {
            return currentState.stateName;
        }

        public BaseState GetCurrentState()
        {
            return currentState;
        }

        public void ShutDown()
        {
            lastState = currentState;
            currentState = null;
        }
    }
}
