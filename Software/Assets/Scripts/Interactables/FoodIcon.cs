using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEngine.UI;

public class FoodIcon : MonoBehaviour
{
    [SerializedDictionary("Food Type", "Sprite")]
    [SerializeField] 
    private SerializedDictionary<FoodType, SerializedDictionary<CookedAmt, Sprite>> _foodTypeToIconDict;

    [SerializeField]
    private Image _foodIcon;

    public void SetIcon(FoodType foodType, CookedAmt cookedAmt)
    {
        _foodIcon.sprite = _foodTypeToIconDict[foodType][cookedAmt];
    }
}
