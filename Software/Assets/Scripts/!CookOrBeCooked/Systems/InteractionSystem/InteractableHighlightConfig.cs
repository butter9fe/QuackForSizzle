using UnityEngine;

namespace CookOrBeCooked.Systems.InteractionSystem
{
    /// <summary>
    /// Configuration for highlights for <see cref="InteractableObjectBase"/>.
    /// This is a ScriptableObject such that we can have multiple "presets" in the case of different types of objects, and allows for easier configuration
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(menuName = "ScriptableObjects/kopiforge/Interaction/InteractableHighlightConfig", fileName = "SO_InteractableHighlightConfig", order = 3)]
    public class InteractableHighlightConfig : ScriptableObject
    {
        public Color HighlightColor = new Color(0.6f, 0.6f, 0.6f, 0.5f);
        public float HighlightIntensity = 0.5f;
    }
}
