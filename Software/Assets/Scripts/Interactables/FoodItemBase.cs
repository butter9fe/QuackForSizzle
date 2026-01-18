using UnityEngine;
using CookOrBeCooked.Utility.ObjectPool;
using AYellowpaper.SerializedCollections;

public class FoodItemBase : MonoBehaviour, IDiscardableItem
{
    [SerializeField] private FoodType _currFoodType;
    [SerializeField]
    [SerializedDictionary("cook amt", "model")] 
    private SerializedDictionary<CookedAmt, GameObject> _foodTypeToModelDict = new SerializedDictionary<CookedAmt, GameObject>();

    public FoodType FoodType { get => _currFoodType; set => _currFoodType = value; }
    private CookedAmt _cookedAmt = CookedAmt.Raw;
    public CookedAmt CookedAmt { get => _cookedAmt; 
        set {
            _cookedAmt = value;
            foreach (var (key, model) in _foodTypeToModelDict)
            {
                model.SetActive(key == CookedAmt);
            }
        }
    }

    private void OnEnable()
    {
        CookedAmt = CookedAmt.Raw;
    }

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
