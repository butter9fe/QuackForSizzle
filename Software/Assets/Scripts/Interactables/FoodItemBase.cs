using UnityEngine;
using CookOrBeCooked.Utility.ObjectPool;

public class FoodItemBase : MonoBehaviour, IDiscardableItem
{
    public void OnItemDiscarded()
    {
        ObjectPoolManager.Instance.ReturnToPool(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
