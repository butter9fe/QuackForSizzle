using UnityEngine;
using System.Collections.Generic;
using System;

namespace CookOrBeCooked.Systems.EventSystem
{
    /// <summary>
    /// Handles the listening/triggering of events
    /// (1) Create a new <see cref="Enum"/> class containing types of events
    /// (2) Create a new script containing all the possible <see cref="EventArgs"/> if needed.
    /// (3) Provide them as the generics to this class as a <see cref="EventManagerBase{EventType, Args}"/>
    /// </summary>
    public class EventBus<EventType, Args> where EventType : Enum where Args : ArgsBase
    {
        #region Properties
        // Map of Event Listeners to Events
        private Dictionary<string, Action<Args>> _eventDictionary;

        // Interface for other scripts to interact with the Event Bus
        private static EventBusInterface<EventType, Args> _eventBusInterface;
        public static EventBusInterface<EventType, Args> Interface => _eventBusInterface;

        // Queue of Triggered Events in this frame
        private Queue<EventCommand> _eventCommands;
        public bool HasCommand { get { return _eventCommands.Count > 0; } }
        public void TriggerNextCommand()
        {
            if (HasCommand)
            {
                EventCommand cmd = _eventCommands.Dequeue();
                TriggerEventListeners(cmd);
            }
        }

        // Queue of Triggered Events in this frame
        private Queue<EventCommand> _eventCommands_Fixed;
        public bool HasCommand_Fixed { get { return _eventCommands_Fixed.Count > 0; } }
        public void TriggerNextCommand_Fixed()
        {
            if (HasCommand_Fixed)
            {
                EventCommand cmd = _eventCommands_Fixed.Dequeue();
                TriggerEventListeners(cmd);
            }
        }
        #endregion Properties

        public EventBus()
        {
            if (_eventDictionary == null)
            {
                _eventDictionary = new Dictionary<string, Action<Args>>();
            }

            _eventBusInterface = new EventBusInterface<EventType, Args>(this);
            _eventCommands = new Queue<EventCommand>();
            _eventCommands_Fixed = new Queue<EventCommand>();
        }

        #region Public Methods
        public void StartListening(string eventName, Action<Args> listener)
        {
            Action<Args> thisEvent;

            if (_eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent += listener;
                _eventDictionary[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                _eventDictionary.Add(eventName, thisEvent);
            }
        }

        public void StopListening(string eventName, Action<Args> listener)
        {
            Action<Args> thisEvent;
            if (_eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent -= listener;
                _eventDictionary[eventName] = thisEvent;

                //_eventDictionary[eventName] -= listener;
            }
        }

        public void QueueEvent(string eventName, Args message, bool isPhysics = false)
        {
            // Add to Queue
            if (isPhysics)
                _eventCommands_Fixed.Enqueue(new EventCommand(eventName, message));
            else
                _eventCommands.Enqueue(new EventCommand(eventName, message));

        }

        public void Reset()
        {
            _eventCommands.Clear();
            _eventCommands_Fixed.Clear();
        }

        private void TriggerEventListeners(EventCommand eventCommand)
        {
            // Invoke Event for all Listeners
            Action<Args> thisEvent = null;
            if (_eventDictionary.TryGetValue(eventCommand.eventName, out thisEvent))
            {
                if (thisEvent != null)
                    thisEvent.Invoke(eventCommand.message);
                //else
                //   Debug.LogError($"Error: {eventCommand.eventName} event is null");
            }
        }
        #endregion Public Methods

        public class EventCommand
        {
            public string eventName;
            public Args message;

            public EventCommand(string eventName, Args message)
            {
                this.eventName = eventName;
                this.message = message;
            }
        }
    }
}
