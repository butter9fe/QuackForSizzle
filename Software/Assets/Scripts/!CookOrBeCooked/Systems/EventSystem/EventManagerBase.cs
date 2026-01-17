using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CookOrBeCooked.Systems.EventSystem
{
    /// <summary>
    /// EventManager singleton class. Users can subscribe to event names with a function signature that matches
    /// the EventCallback delegate, which is a function that takes in a EventArgs and return void. Users can 
    /// then check in the callback function if the EventArgs is of the correct event type.
    /// 
    /// Relevant functions: StartListening, StopListening, TriggerEvent
    /// 
    /// Assumes that event subscribers inherit from the MonoBehaviour script. You are recommended to unsubscribe (StopListening)
    /// to events once you're done, or OnDestroy, but if you forgot, there's a aux script EventSubscriber that does it for you.
    /// </summary>
    [DefaultExecutionOrder(-100)] 
    public abstract class EventManagerBase<EventType, Args> : CookOrBeCooked.Utility.Singleton<EventManagerBase<EventType, Args>>
        where EventType : Enum where Args : ArgsBase
    {
        private EventBus<EventType, Args> _eventBus;
        public static EventBusInterface<EventType, Args> Interface => EventBus<EventType, Args>.Interface;

        protected override void Awake()
        {
            base.Awake();
            _eventBus = new EventBus<EventType, Args>();

            // Not onSceneLoaded as some events (eg: SceneLoader) rely on it, activeSceneChanged is called before that
            SceneManager.activeSceneChanged += ChangedActiveScene;
        }

        private void ChangedActiveScene(Scene current, Scene next)
        {
            _eventBus.Reset();
        }

        private int frameCount = 0;
        private int frameCount_Fixed = 0;
        /// <summary>
        /// Trigger Events added to the Event Queue this frame. For non-Physics Events.
        /// </summary>
        private void LateUpdate()
        {
            frameCount++;
            //Debug.Log($"Frame {frameCount}");

            while (_eventBus.HasCommand)
                _eventBus.TriggerNextCommand();
        }


        /// <summary>
        /// Trigger Events added to the Event Queue this frame. For Physics Events (i.e. Events triggered from Physics interactions or involving Rigidbodies and Colliders).
        /// </summary>
        private void FixedUpdate()
        {
            frameCount_Fixed++;
            //Debug.Log($"Frame {frameCount_Fixed}");

            while (_eventBus.HasCommand_Fixed)
                _eventBus.TriggerNextCommand_Fixed();
        }
    }
}
