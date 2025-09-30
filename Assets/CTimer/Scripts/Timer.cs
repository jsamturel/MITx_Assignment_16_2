using System;
using UnityEngine;

namespace CTools.CTimer
{
    public class Timer : IDisposable
    {
        /// <summary>
        /// Id of the timer to store and retrieve from dictionary
        /// </summary>
        public string ID { get; private set; }
        /// <summary>
        /// Is this timer currently running
        /// </summary>
        public bool IsRunning { get; private set; } = false;
        /// <summary>
        /// Is this timer currently paused
        /// </summary>
        public bool IsPaused { get; private set; } = false;
        /// <summary>
        /// Is this timer killed
        /// </summary>
        public bool IsKilled { get; private set; } = false;
        /// <summary>
        /// Has this timer any link to a gameObject
        /// </summary>
        public bool HasLink { get; private set; } = false;

        /// <summary>
        /// Start time of the timer
        /// </summary>
        private float m_StartTime = 0f;
        /// <summary>
        /// Targeted duration of the timer
        /// </summary>
        private float m_Duration = 0f;

        /// <summary>
        /// Time, this timer paused
        /// </summary>
        private float m_PauseStartTime = 0f;
        /// <summary>
        /// Total time that this timer passed paused
        /// </summary>
        private float m_PausedTime = 0f;

        /// <summary>
        /// Should this timer use unscaled time
        /// </summary>
        private bool m_UseUnscaledTime = false;

        /// <summary>
        /// gameObject that this timer linked to
        /// </summary>
        private GameObject m_LinkedObject = null;

        public event Action OnTimerStart;
        public event Action OnTimerComplete;

        public event Action<Timer> OnBeforeTimerDisposed;
        public event Action<Timer, string> OnIdChanged;

        public Timer(object creator, float duration, bool useUnscaledTime = false)
        {
            if (creator is TimerManager)
            {
                this.m_UseUnscaledTime = useUnscaledTime;
                
                ResetTimer();

                this.m_Duration = duration;
            
                IsRunning = true;
                Continue();
            }
            else
            {
                throw new Exception($"Creating outside of the Time Manager is prohibited. Please use TimerManager.Timer() instead");
            }
        }

        public void Update()
        {
            if (IsKilled)
                return;
            
            if (HasLink)
            {
                if (m_LinkedObject == null)
                {
                    IsKilled = true;
                    Dispose();
                }
            }
            
            CheckComplete();
        }

        private void CheckComplete()
        {
            if (IsPaused)
                return;
            
            var startTime = m_StartTime + m_PausedTime;
            var currentTime = m_UseUnscaledTime ? Time.unscaledTime : Time.time;
            if (currentTime >= startTime + m_Duration)
            {
                OnTimerComplete?.Invoke();
                Kill();
            }
        }

        public Timer SetId(string id)
        {
            this.ID = id;

            OnIdChanged?.Invoke(this, this.ID);
            return this;
        }

        public Timer OnStart(Action onStart)
        {
            this.OnTimerStart = onStart;

            return this;
        }

        /// <summary>
        /// Subscribe to on complete method of the timer
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public Timer OnComplete(Action onComplete)
        {
            this.OnTimerComplete = onComplete;

            return this;
        }

        /// <summary>
        /// Set link gameObject to the timer
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public Timer SetLink(GameObject targetGameObject)
        {
            HasLink = true;
            m_LinkedObject = targetGameObject;
            return this;
        }

        /// <summary>
        /// Pauses the timer.
        /// </summary>
        /// <returns></returns>
        public Timer Pause()
        {
            if (IsPaused || IsKilled)
                return this;
            
            this.IsPaused = true;
            m_PauseStartTime = m_UseUnscaledTime ? Time.unscaledTime : Time.time;
            return this;
        }

        /// <summary>
        /// Continues the paused timer
        /// </summary>
        /// <returns></returns>
        public Timer Continue()
        {
            if (IsPaused == false || IsKilled)
                return this;

            m_PausedTime += (m_UseUnscaledTime ? Time.unscaledTime : Time.time) - m_PauseStartTime;
            IsPaused = false;
            return this;
        }

        /// <summary>
        /// Kills the timer
        /// </summary>
        /// <param name="complete"> Fires the onComplete method </param>
        public void Kill(bool complete = false)
        {
            if (IsKilled)
                return;

            IsKilled = true;

            if (complete)
            {
                OnTimerComplete?.Invoke();
            }
            
            ResetTimer();
            Dispose();
        }

        /// <summary>
        /// Resets the timer
        /// </summary>
        public void ResetTimer()
        {
            this.m_StartTime = m_UseUnscaledTime ? Time.unscaledTime : Time.time;
            this.m_PausedTime = 0f;
            this.m_PauseStartTime = 0f;
            this.m_LinkedObject = null;
        }
        
        public void Dispose()
        {
            OnBeforeTimerDisposed?.Invoke(this);
            
            OnTimerStart = null;
            OnTimerComplete = null;
            OnIdChanged = null;
            OnBeforeTimerDisposed = null;
        }
    }
}
