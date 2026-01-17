using UnityEngine;

namespace QuackForSizzle.Player
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "ScriptableObjects/PlayerConfig", fileName = "SO_PlayerConfig", order = 1)]
    public class Config : ScriptableObject
    {
        [Foldout("Player Movement")]
        public float MovementAcceleration = 50f;
        public float MaxMovementSpeed = 4f;
        public float Drag = 30f;
    }
}
