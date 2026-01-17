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

        private void Awake()
        {
            _inventoryHandler = GetComponentInChildren<InventoryHandler>();
        }
    }
}
