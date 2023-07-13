using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] int coin;
    public int Coin
    {
        get => coin;
        set
        {
            coin = value;
            OnCoinChanged?.Invoke(Coin);
        }
    }

    public delegate void CoinChangedEvent(int newCoinValue);
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
}
