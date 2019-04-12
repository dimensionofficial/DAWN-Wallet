using UnityEngine;

namespace Utility
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T m_instance;

        private static object m_lock = new object();

        public static T Instance
        {
            get
            {
                lock (m_lock)
                {
                    if (m_instance == null)
                    {
                        m_instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                " - there should never be more than 1 singleton!" +
                                " Reopenning the scene might fix it." + typeof(T));
                            return m_instance;
                        }

                        if (m_instance == null)
                        {
                            //Debug.Log (typeof(T));
                            GameObject singleton = new GameObject();
                            m_instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();
                        }
                    }

                    return m_instance;
                }
            }
        }

        void OnApplicationQuit()
        {
            DestroyImmediate(gameObject);
        }
    }
}
