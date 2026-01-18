using UnityEngine;

public class CoinManager : CookOrBeCooked.Utility.Singleton<CoinManager>
{
    [SerializeField] private TMPro.TMP_Text coinText;
    private int _coins = 0;
    public int Coins { 
        get => _coins;
        set 
        {
            _coins = value;
            coinText.text = _coins.ToString("D3");
        }
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
    }

    public void ResetCoins()
    {
        Coins = 0;
    }
}
