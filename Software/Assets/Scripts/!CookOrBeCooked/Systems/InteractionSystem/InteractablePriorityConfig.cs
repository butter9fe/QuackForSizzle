using UnityEngine;

namespace CookOrBeCooked.Systems.InteractionSystem
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "ScriptableObjects/kopiforge/Interaction/InteractablePriorityConfig", fileName = "SO_InteractablePriorityConfig", order = 0)]
    public class InteractablePriorityConfig : ScriptableObject
    {
        [Tooltip("Layers to check for interactables")]
        public LayerMask InteractablesMask;
        [Tooltip("Bias towards Interactables directly in front of the player")]
        public float AngleBias = 0.8f;
        [Tooltip("Bias towards Interactables that are closer to the player")]
        public float DistanceBias = 0.2f;
        [Tooltip("Bias towards the player's Interactable of the previous frame")]
        public float PreviousInteractableBias = 0.1f;
    }
}
