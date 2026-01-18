using CookOrBeCooked.Systems.EventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QuackForSizzle.Player
{
    /// <summary>
    /// 
    /// </summary>
    public class Controller : MonoBehaviour
    {
        [SerializeField] public PlayerNumber PlayerNumber;

        private InventoryHandler _inventoryHandler;
        public InventoryHandler InventoryHandler => _inventoryHandler;

        public PlayerState PlayerState = PlayerState.Normal;

        private void Awake()
        {
            _inventoryHandler = GetComponentInChildren<InventoryHandler>();
        }

        private void OnEnable()
        {
            PlayerState = PlayerState.Normal;
        }
    }

    public enum PlayerState
    {
        Normal,
        Cooking,
        EndGame
    }
}
