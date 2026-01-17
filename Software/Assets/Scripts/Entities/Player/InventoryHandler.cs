using UnityEngine;
using CookOrBeCooked.Systems.EventSystem;

namespace QuackForSizzle.Player
{
    /// <summary>
    /// Handles the picking up/placing of items
    /// </summary>
    public class InventoryHandler : CookOrBeCooked.Utility.Singleton<InventoryHandler>
    {
        #region Properties
        [Tooltip("Pivot to parent held inventory items to")]
        [SerializeField] private Transform _inventoryPivot;

        private GameObject _heldItem = null;
        public GameObject HeldItem => _heldItem;
        #endregion Properties

        #region LifeCycle Methods
        private void OnEnable()
        {
            EventManager.Interface.ListenToEvent(Events.PlayerEvent.SetInventory, Listen_SetInventory);
            EventManager.Interface.ListenToEvent(Events.PlayerEvent.DiscardInventory, Listen_DiscardInventory);
        }

        private void OnDisable()
        {
            EventManager.Interface.StopListeningToEvent(Events.PlayerEvent.SetInventory, Listen_SetInventory);
            EventManager.Interface.StopListeningToEvent(Events.PlayerEvent.DiscardInventory, Listen_DiscardInventory);
        }
        #endregion LifeCycle Methods

        #region Listeners
        private void Listen_SetInventory(ArgsBase e)
        {
            var args = e as EventArgs.Inventory;
            if (args == null)
                return;

            if (_heldItem != null)
            {
                Debug.LogWarning("Trying to set inventory item when there's already a held object! Ignoring call!");
                return;
            }

            _heldItem = args.Item;
            args.Item.transform.SetParent(_inventoryPivot, false);
        }

        private void Listen_DiscardInventory(ArgsBase e)
        {
            var item = GetInventoryItemOfType<IDiscardableItem>();
            if (item == null)
                return;

            item.OnItemDiscarded();
            _heldItem.transform.SetParent(null);
            _heldItem = null;
        }
        #endregion Listeners

        /// <summary>
        /// If held item is of type specified, return corresponding component
        /// Else, return null.
        /// </summary>
        /// <typeparam name="T">type, usually you'll provide an Interface to this</typeparam>
        /// <returns></returns>
        public T GetInventoryItemOfType<T>() where T : class
        {
            if (_heldItem == null)
                return null;

            _heldItem.TryGetComponent(out T component);
            return component;
        }
    }
}
