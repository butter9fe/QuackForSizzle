using UnityEngine;
using UnityEngine.UI;

public class CoinsDisplay : MonoBehaviour
{
    private Text coinText;

    private void Awake()
    {
        coinText = GetComponent<Text>();
    }

    private void Update()
    {
        coinText.text = CoinManager.Instance.Coins.ToString();
    }
}
