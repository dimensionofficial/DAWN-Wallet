//----------------------------------------------
//Use StaticUpdater to fixed-update non-MonoBehaviour classes' function.
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class StaticUpdater : Singleton<StaticUpdater>
{
    public delegate void UpdateCallBack();
    public delegate IEnumerator CoroutineCallBack();
    private List<UpdateCallBack> m_fixedCallBack = new List<UpdateCallBack>();
    private List<UpdateCallBack> m_renderCallBack = new List<UpdateCallBack>();
    private List<UpdateCallBack> m_lateCallBack = new List<UpdateCallBack>();

    private static MonoUpdater m_updater;

    //         [RuntimeInitializeOnLoadMethod]
    //         private static void Main()
    //         {
    //             object obj = StaticUpdater.Instance;
    //         }

    public StaticUpdater()
    {

        if (m_updater == null || m_updater.gameObject == null)
        {
            GameObject go = new GameObject("staticUpdater");
            m_updater = go.AddComponent<MonoUpdater>();
            m_updater.owner = this;
        }
    }

    public void AddFixedUpdateCallBack(UpdateCallBack callBack)
    {
        m_fixedCallBack.Add(callBack);
    }

    public void RemoveFixedUpdateCallBack(UpdateCallBack callBack)
    {
        m_fixedCallBack.Remove(callBack);
    }

    public void AddRenderUpdateCallBack(UpdateCallBack callBack)
    {
        m_renderCallBack.Add(callBack);
    }

    public void RemoveRenderUpdateCallBack(UpdateCallBack callBack)
    {
        m_renderCallBack.Remove(callBack);
    }

    public void AddLateUpdateCallBack(UpdateCallBack callBack)
    {
        m_lateCallBack.Add(callBack);
    }

    public void RemoveLateUpdateCallBack(UpdateCallBack callBack)
    {
        m_lateCallBack.Remove(callBack);
    }

    public Coroutine StartCoroutine(CoroutineCallBack callBack)
    {
        return m_updater.StartCoroutine(callBack());
    }

    public Coroutine StartCoroutine(IEnumerator enumerator)
    {
        return m_updater.StartCoroutine(enumerator);
    }

    public void StopCoroutine(Coroutine coroutine)
    {
        m_updater.StopCoroutine(coroutine);
    }

    private sealed class MonoUpdater : MonoBehaviour
    {
        private StaticUpdater m_owner;

        public StaticUpdater owner
        {
            set
            {
                m_owner = value;
            }
        }

        void Awake()
        {
            UtilTools.SetDontDestroyOnLoad(gameObject);
            //DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            for (int i = 0; i < m_owner.m_renderCallBack.Count; i++)
            {
                if (m_owner == null)
                {
                    return;
                }
                m_owner.m_renderCallBack[i]();
            }
        }

        void FixedUpdate()
        {
            if (m_owner == null)
            {
                return;
            }
            for (int i = 0; i < m_owner.m_fixedCallBack.Count; i++)
            {
                m_owner.m_fixedCallBack[i]();
            }
        }

        void LateUpdate()
        {
            if (m_owner == null)
            {
                return;
            }
            for (int i = 0; i < m_owner.m_lateCallBack.Count; i++)
            {
                m_owner.m_lateCallBack[i]();
            }
        }
    }
}//StaticUpdater