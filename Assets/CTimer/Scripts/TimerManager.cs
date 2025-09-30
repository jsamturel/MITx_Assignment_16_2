using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CTools.CTimer
{
    public class TimerManager : MonoBehaviour
    {
        public static TimerManager Instance { get; private set; }

        public static WorldTime WorldTime
        {
            get;
            private set;
        }

        public static UserTimeData UserTimeData
        {
            get;
            private set;
        }

        private static Dictionary<string, Timer> m_TimerIdPairs = new Dictionary<string, Timer>();
        private static List<Timer> m_DisposableTimers = new List<Timer>();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            WorldTime = new WorldTime();
            UserTimeData = new UserTimeData(WorldTime);
        }

        /// <summary>
        /// Creates new timer
        /// </summary>
        /// <param name="duration"> Duration of the timer </param>
        /// <param name="useUnscaledTime"> Should this timer use unscaled time </param>
        /// <returns> new Timer </returns>
        public static Timer Timer(float duration, bool useUnscaledTime = false)
        {
            var timer = new Timer(Instance, duration, useUnscaledTime);
            var id = GetNewID();
            timer.SetId(id);
            
            m_TimerIdPairs.Add(id, timer);

            timer.OnBeforeTimerDisposed += Timer_OnBeforeTimerDisposed;
            timer.OnIdChanged += Timer_OnIdChanged;
            
            return timer;
        }

        private void Update()
        {
            foreach (var timerID in m_TimerIdPairs)
            {
                timerID.Value.Update();
            }

            for (int i = 0; i < m_DisposableTimers.Count; i++)
            {
                var timerToDispose = m_DisposableTimers[i];
                if(m_TimerIdPairs.TryGetValue(timerToDispose.ID, out Timer timer))
                {
                    m_TimerIdPairs.Remove(timer.ID);
                }
            }
        }

        private static void Timer_OnBeforeTimerDisposed(Timer timer)
        {
            m_DisposableTimers.Add(timer);
            
            if (timer != null)
            {
                timer.OnBeforeTimerDisposed -= Timer_OnBeforeTimerDisposed;
            }
        }

        private static string GetNewID()
        {
            var id = Guid.NewGuid().ToString();
            
            if (m_TimerIdPairs.ContainsKey(id))
                return GetNewID();
            
            return id;
        }

        private static void Timer_OnIdChanged(Timer timer, string newID)
        {
            m_DisposableTimers.Add(timer);
            m_TimerIdPairs.Add(newID, timer);
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Creates menu item
        /// </summary>
        [MenuItem("Tools/Create Timer Manager")]
        private static void CreateTimerManager()
        {
            var timerManager = GameObject.FindObjectOfType<TimerManager>();
            if (timerManager == null)
            {
                var go = new GameObject("TimerManager", typeof(TimerManager));
                
                go.transform.SetSiblingIndex(0);
            }
        }
#endif
    }
}

