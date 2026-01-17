using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using QuackForSizzle.Player;

namespace CookOrBeCooked.Systems.InteractionSystem
{
    /// <summary>
    //// Base handler for entities that require interaction behaviour.
    //// Most likely used for players.
    /// 
    /// Example usage:
    /// Create a script named PlayerInteractionHandler, and inherit from this script.
    /// Override _entityTransform and provide it with the player's transform.
    /// Finally, listen to input events to do something with <see cref="_currInteractable"/>
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public abstract class InteractionHandlerBase : MonoBehaviour
    {
        #region Properties
        [SerializeField] private InteractablePriorityConfig _config;
        protected abstract Transform _entityTransform { get; }

        protected HashSet<InteractableObjectBase> _interactablesInRange = new HashSet<InteractableObjectBase>();
        protected InteractableObjectBase _currInteractable = null;

        protected Controller _thisPlayer;
        #endregion Properties

        #region LifeCycle Methods
        protected virtual void Awake()
        {
            _thisPlayer = GetComponentInParent<Controller>();
        }

        protected virtual void OnEnable()
        {
            _currInteractable = null;
        }

        protected virtual void Update()
        {
            // Execute once every frame
            UpdateInteractables();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            // Check if collider is in the desired LayerMask
            if ((_config.InteractablesMask & (1 << other.gameObject.layer)) != 0)
            {
                // Add to list if it's an interactable
                var interactable = other.GetComponent<InteractableObjectBase>();
                if (interactable != null)
                    _interactablesInRange.Add(interactable);

                ReliableOnTriggerExit.NotifyTriggerEnter(other, gameObject, OnTriggerExit);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            // Check if collider is in the desired LayerMask
            if ((_config.InteractablesMask & (1 << other.gameObject.layer)) != 0)
            {
                // Remove from list if it's an interactable
                var interactable = other.GetComponent<InteractableObjectBase>();
                if (interactable != null)
                    _interactablesInRange.Remove(interactable);

                ReliableOnTriggerExit.NotifyTriggerExit(other, gameObject);
            }
        }
        #endregion LifeCycle Methods

        #region Private Methods
        /// <summary>
        /// Handles the main logic for switching between selecting different interactable
        /// </summary>
        private void UpdateInteractables()
        {
            InteractableObjectBase newInteractable = GetNewInteractable();

            // Unselect old interactable
            if (_currInteractable != newInteractable)
                UnselectCurrentInteractable();

            // Select new interactable, if any
            if (newInteractable != null)
                newInteractable.SetIsSelected(true, _thisPlayer.PlayerNumber);

            _currInteractable = newInteractable;
        }

        /// <summary>
        /// Gets the interactable with the highest priority and closest distance
        /// </summary>
        private InteractableObjectBase GetNewInteractable()
        {
            // No interactables in range => Not selecting anything
            if (_interactablesInRange.Count == 0)
                return null;

            // Filter interactables to existing ones and able to interact
            var filteredInteractables = _interactablesInRange.Where(x => x != null && x.CanInteract);

            // No valid interactables!
            if (filteredInteractables.Count() == 0)
                return null;

            // The Aggregate function allows us to write custom comparison logic,
            // and returns the item with the highest value.
            // Ref: https://stackoverflow.com/a/3188804 (Linear time complexity)
            return filteredInteractables.Aggregate((obj1, obj2) =>
            {
            // If equal priority, return the object with the lowest distance
            if (obj1.Priority == obj2.Priority)
                    return GetWeightedDistance(obj1) < GetWeightedDistance(obj2) ? obj1 : obj2;

            // Else, return the object with the highest priority
            else
                    return obj1.Priority > obj2.Priority ? obj1 : obj2;
            });
        }

        protected virtual void UnselectCurrentInteractable()
        {
            // No interactable to unselect
            if (_currInteractable == null)
                return;

            // Unselect interactable, calling relevant unselect events
            _currInteractable.SetIsSelected(false);
        }

        /// <summary>
        /// The lower the distance, the better
        /// </summary>
        private float GetWeightedDistance(InteractableObjectBase interactable)
        {
            // Get vector from entity to the interactable object
            Vector3 interactableDir = interactable.transform.position - _entityTransform.position;

            // (1) Angle Bias
            // Angle btw entity's forward vector & entity's direction to obj
            float angle = Vector3.Angle(_entityTransform.forward, interactableDir);
            float angleWeight = _config.AngleBias * Mathf.Sin(Mathf.Deg2Rad * angle);

            // (2) Distance Bias
            // Dist btw entity & obj
            float distance = interactableDir.magnitude;
            float distanceWeight = _config.DistanceBias * (distance / 7f);

            // (3) Persist Previous Object Bias
            float prevInteractableBias = 0f;
            if (_currInteractable != null)
                prevInteractableBias = _config.PreviousInteractableBias * ((interactable == _currInteractable) ? 0f : 1f);

            return angleWeight + distanceWeight + prevInteractableBias;
        }
        #endregion Private Methods
    }
}
