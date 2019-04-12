using System;
public class Singleton<T> : IDisposable where T : new()
{
    private static T m_instance;
    static object m_lock = new object();
    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                lock (m_lock)
                {
                    if (m_instance == null)
                    {

                        m_instance = new T();
                    }
                }
            }
            return m_instance;
        }
        set { m_instance = value; }
    }

    /// <summary>
    /// 销毁单例
    /// </summary>
    public virtual void Dispose()
    {
        Instance = (T)(object)null;
    }
}
