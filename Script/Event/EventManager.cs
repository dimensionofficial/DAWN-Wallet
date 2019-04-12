using System.Collections.Generic;
public class EventManager : Singleton<EventManager>
{
    public delegate void OnEventCallBack(params object[] obj);

    private Dictionary<int, OnEventCallBack> m_callBackDict = new Dictionary<int, OnEventCallBack>();

    public void AddEventListener(EventID eventID, OnEventCallBack callBack)
    {
        if (!m_callBackDict.ContainsKey((int)eventID))
        {
            m_callBackDict.Add((int)eventID, callBack);
        }
        else
        {
            m_callBackDict[(int)eventID] -= callBack;
            m_callBackDict[(int)eventID] += callBack;
        }
    }

    public void RemoveEventListener(EventID eventID, OnEventCallBack callBack)
    {
        {
            if (!m_callBackDict.ContainsKey((int)eventID))
            {
                return;
            }
            else
            {
                m_callBackDict[(int)eventID] -= callBack;
                if (m_callBackDict[(int)eventID] == null || m_callBackDict[(int)eventID].GetInvocationList() == null)
                {
                    m_callBackDict.Remove((int)eventID);
                }
            }
        }
    }

    public void SendEvent(EventID eventID, params object[] obj)
    {
        if (m_callBackDict.ContainsKey((int)eventID))
        {
            if (m_callBackDict[(int)eventID] != null)
            {
                m_callBackDict[(int)eventID](obj);
            }
        }
    }

    public override void Dispose()
    {
        m_callBackDict.Clear();
    }
}