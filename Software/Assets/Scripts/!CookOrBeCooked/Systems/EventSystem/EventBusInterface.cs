using UnityEngine;
using System.Collections.Generic;
using System;

namespace CookOrBeCooked.Systems.EventSystem
{
    public class EventBusInterface<EventType, Args> where EventType : Enum where Args : ArgsBase
    {
        private EventBus<EventType, Args> _eventBus;

        /*
         * NEW: Event string names Lookup tables to prevent generating string GC at runtime
         * TODO: This could be generated at compile time instead
         */
        private static bool _hasInitLUT = false;
        private static Dictionary<EventType, string> _eventLUT = new();

        public EventBusInterface(EventBus<EventType, Args> eventBus)
        {
            _eventBus = eventBus;
            InitEventNamesLUTs();
        }

        private void InitEventNamesLUTs()
        {
            if (_hasInitLUT)
                return;

            _hasInitLUT = true;

            foreach (EventType name in Enum.GetValues(typeof(EventType)))
                _eventLUT.Add(name, name.ToString());
        }

        /// <summary>
        /// Public callbacks for listening to events
        /// </summary>
        public void ListenToEvent(EventType eventType, Action<Args> listener)
        {
            _eventBus.StartListening(_eventLUT[eventType], listener);
        }

        /// <summary>
        /// Public callbacks for stopping listening to events
        /// </summary>
        public void StopListeningToEvent(EventType eventType, Action<Args> listener)
        {
            _eventBus.StopListening(_eventLUT[eventType], listener);
        }

        /// <summary>
        /// Public callbacks for triggering events
        /// </summary>
        public void TriggerEvent(EventType eventType, Args message, bool isPhysics = false)
        {
            _eventBus.QueueEvent(_eventLUT[eventType], message, isPhysics);
        }
    }
}
