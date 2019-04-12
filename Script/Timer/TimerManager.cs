using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

    public sealed class TimerManager : Singleton<TimerManager>
    {
        private List<Timer> m_LifeList = new List<Timer>();
        private List<Timer> m_suspendableList = new List<Timer>();
        private List<Timer> m_renderTimerList = new List<Timer>();

        private bool m_isPause = false;
        public bool IsPause
        {
            get { return m_isPause; }
            set { m_isPause = value; }
        }

        public TimerManager()
        {
            StaticUpdater.Instance.AddFixedUpdateCallBack(FixedAdvance);
            StaticUpdater.Instance.AddRenderUpdateCallBack(RenderAdavance);
        }

        private void PreSceneChange(params object[] obj)
        {
            m_LifeList.Clear();
            m_suspendableList.Clear();
        }

        public override void Dispose()
        {
            for (int i = 0; i < m_LifeList.Count; i++)
            {
                m_LifeList[i].Dispose();
            }

            for (int i = 0; i < m_suspendableList.Count; i++)
            {
                m_suspendableList[i].Dispose();
            }

            for (int i = 0; i < m_renderTimerList.Count; i++)
            {
                m_renderTimerList[i].Dispose();
            }

            m_LifeList.Clear();
            m_suspendableList.Clear();
            m_renderTimerList.Clear();
            m_isPause = false;
        }

        public void AddTimer(Timer timer)
        {
            if (timer != null)
            {
                if (timer.Type == Timer.TimerType.LifeCycle)
                {
                    m_LifeList.Add(timer);
                }
                else if (timer.Type == Timer.TimerType.Suspendable)
                {
                    m_suspendableList.Add(timer);
                }
                else if (timer.Type == Timer.TimerType.Render)
                {
                    m_renderTimerList.Add(timer);
                }
                timer.Begin();
            }
        }

        public Timer AddTimer(float duration, Timer.OnTimerCallBack callBack, Timer.TimerType type = Timer.TimerType.LifeCycle, uint maxHitCount = 1)
        {
            Timer timer = new Timer(duration, callBack, type, maxHitCount);
            AddTimer(timer);
            return timer;
        }

        public void DelayCall(float duration, Timer.OnTimerCallBack callBack)
        {
            AddTimer(duration, callBack);
        }

        public void RemoveTimer(Timer timer)
        {
            if (timer != null)
            {
                if (timer.Type == Timer.TimerType.LifeCycle)
                {
                    m_LifeList.Remove(timer);
                }
                else if (timer.Type == Timer.TimerType.Suspendable)
                {
                    m_suspendableList.Remove(timer);
                }
                else if (timer.Type == Timer.TimerType.Render)
                {
                    m_renderTimerList.Remove(timer);
                }
                timer.Dispose();
                timer = null;
            }
        }

        private void FixedAdvance()
        {
            AdvanceLifeTime();

            if (!IsPause)
            {
                AdvanceSuspendable();
            }
        }

        private void RenderAdavance()
        {
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < m_renderTimerList.Count; i++)
            {
                m_renderTimerList[i].RenderDeltaUpdate(deltaTime);
            }
        }

        private void AdvanceLifeTime()
        {
            float curTime = Time.realtimeSinceStartup;
            for (int i = 0; i < m_LifeList.Count; i++)
            {
                m_LifeList[i].FixedDeltaUpdate(curTime);
            }
        }

        private void AdvanceSuspendable()
        {
            float curTime = Time.realtimeSinceStartup;
            for (int i = 0; i < m_suspendableList.Count; i++)
            {
                m_suspendableList[i].FixedDeltaUpdate(curTime);
            }
        }

        /// <summary>
        /// Time in second
        /// </summary>
        /// <returns></returns>
        public float TimeSinceStartUp()
        {
            return Time.realtimeSinceStartup;
        }

        public long NowTick()
        {
            return DateTime.Now.Ticks;
        }

        public TimeSpan SecondToSpan(int second)
        {
            return new TimeSpan(0, 0, second);
        }
    }