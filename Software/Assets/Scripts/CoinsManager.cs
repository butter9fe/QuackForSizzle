using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    private const string CoinsKey = "Coins";
    public int Coins { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Coins = PlayerPrefs.GetInt(CoinsKey, 0);
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        SaveCoins();
    }

    public void ResetCoins()
    {
        Coins = 0;
        SaveCoins();
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(CoinsKey, Coins);
        PlayerPrefs.Save();
    }
}
