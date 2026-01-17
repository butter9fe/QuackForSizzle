using UnityEngine;
using CookOrBeCooked.Utility.ObjectPool;

public class FoodItemBase : MonoBehaviour, IDiscardableItem
{
    [SerializeField] private FoodType _currFoodType;

    public FoodType FoodType { get => _currFoodType; set => _currFoodType = value; }
    public CookedAmt CookedAmt { get; set; }

    public void OnItemDiscarded()
    {
        ObjectPoolManager.Instance.ReturnToPool(gameObject);
    }
}

public enum FoodType
{
    Steak,
    Pancake,
    Egg
}

public enum CookedAmt
{
    Raw,
    Intermediate,
    Cooked
}
