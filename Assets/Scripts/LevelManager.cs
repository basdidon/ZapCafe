using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] float coin;
    public float Coin
    {
        get => coin;
        set
        {
            coin = value;
            OnCoinChanged?.Invoke(Coin);
        }
    }

    public delegate void CoinChangedEvent(float newCoinValue);
    public CoinChangedEvent OnCoinChanged;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public bool TrySpend(float value)
    {
        if(value > Coin)
        {
            Debug.LogWarning("Coin not enough.");
            return false;
        }
        else
        {
            Coin -= value;
            return true;
        }
    }
}
