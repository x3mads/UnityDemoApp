using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMediator.Core.Util
{
    public class XMediatorMainThreadDispatcher : MonoBehaviour
    {

        private static XMediatorMainThreadDispatcher instance = null;
        
        private static List<Action> eventsQueue = new List<Action>();
        private static volatile bool isEventsQueueEmpty = true;

        internal static void Initialize()
        {
            if (IsInitialized()) return;

            GameObject gameObject = new GameObject("XMediatorMainThreadDispatcher");
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameObject);
            instance = gameObject.AddComponent<XMediatorMainThreadDispatcher>();
        }

        private static bool IsInitialized() => instance != null;

        public static void Enqueue(Action action)
        {
            lock (eventsQueue)
            {
                eventsQueue.Add(action);
                isEventsQueueEmpty = false;
            }
        }

        public void OnDisable()
        {
            instance = null;
        }

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Update()
        {
            if (isEventsQueueEmpty) return;

            List<Action> executableEvents = new List<Action>();
            lock (eventsQueue)
            {
                executableEvents.AddRange(eventsQueue);
                eventsQueue.Clear();
                isEventsQueueEmpty = true;
            }
            
            foreach (var action in executableEvents)
            {
                if (action.Target != null)
                {
                    action.Invoke();
                }
            }
        }
    }
}