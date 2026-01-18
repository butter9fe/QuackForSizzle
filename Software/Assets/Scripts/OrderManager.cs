using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using QuackForSizzle.Player;
using QuackForSizzle.Player.EventArgs;
using QuackForSizzle.Player.Events;
using UnityEngine;

public class OrderManager : CookOrBeCooked.Utility.Singleton<OrderManager>
{
    [SerializeField]
    [SerializedDictionary("FoodType", "Order Card Prefab")]
    private SerializedDictionary<FoodType, GameObject> _orderCardPrefabs;

    [SerializeField] private Transform _orderCardParent;
    private List<System.Tuple<FoodType, GameObject>> _orders = new();

    private void Update()
    {
        if (_orders.Count < 2)
        {
            AddRandomOrder();
        }
    }

    private void AddRandomOrder()
    {
        var randomFoodType = (FoodType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(FoodType)).Length);
        var orderCard = Instantiate(_orderCardPrefabs[randomFoodType], _orderCardParent);
        _orders.Add(new System.Tuple<FoodType, GameObject>(randomFoodType, orderCard));
    }

    private void ClearOrder(System.Tuple<FoodType, GameObject> order)
    {
        Destroy(order.Item2);
        _orders.Remove(order);
    }

    public bool CheckCanClearOrder(FoodItemBase foodItem, PlayerNumber player)
    {
        if (foodItem.CookedAmt != CookedAmt.Cooked)
            return false;

        var matchingOrder = _orders.Find(x => x.Item1 == foodItem.FoodType);
        if (matchingOrder.Item2 == null)
        {
            return false;
        }
        else
        {
            ClearOrder(matchingOrder);

            // Also remove foodItem from player
            EventManager.Interface.TriggerEvent(PlayerEvent.DiscardInventory, new PlayerArgsBase(player));
            return true;
        }
    }
}
