using System;
using System.Timers;
using UnityEngine;

namespace XMediator.Core.Util
{
    internal class XMediatorDebouncer<T>
    {
        private const int DEFAULT_TIME_SPAN = 1000;
        
        private readonly Timer timer;
        private T lastValue;
        private bool debounceCalls;

        public XMediatorDebouncer(int delayInMilliseconds = DEFAULT_TIME_SPAN)
        {
            timer = new Timer(delayInMilliseconds);
            timer.Elapsed += OnTimerElapsed;
        }

        public T Invoke(Func<T> action)
        {
            if (debounceCalls) return lastValue;

            debounceCalls = true;
            timer.Start();
            lastValue = action.Invoke();
            return lastValue;
        }

        public void Dispose()
        {
            timer.Dispose();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            debounceCalls = false;
        }
    }
}