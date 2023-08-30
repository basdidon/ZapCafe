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
            CoinChangedEvent?.Invoke(Coin);
        }
    }

    [SerializeField] int level = 1;
    public int Level
    {
        get => level;
        private set
        {
            level = value;
            
            if(maxExp.Length == Level - 1)
            {
                OnMaxLevelEvent?.Invoke(Level);
                Debug.Log("onMaxLevel");
            }
            else
            {
                LevelChangedEvent?.Invoke(Level, maxExp[Level - 1]);
            }
            
        }
    }

    readonly int[] maxExp = new[] { 100, 250, 800, 1500 };
    public int MaxExpLvl_1 => maxExp[0];

    [SerializeField] int exp = 0;
    public int Exp
    {
        get => exp;
        set
        {
            exp = value;

            if(maxExp.Length == Level-1) // on max level
                return;

            if(Level-1 < maxExp.Length && exp >= maxExp[Level - 1])
            {
                exp -= maxExp[Level - 1];
                Level++;
                if (maxExp.Length == Level - 1) // on max level at first time
                    return;
            }
            ExpChangedEvent?.Invoke(exp);
            Debug.Log("exp changed");
        }
    }

    public delegate void IntChangedEventHandler(int newInt);
    // Event
    public event IntChangedEventHandler CoinChangedEvent;
    public event IntChangedEventHandler ExpChangedEvent;
    public event IntChangedEventHandler OnMaxLevelEvent;

    public delegate void LevelChangedEventHandler(int newExp,int maxExp);
    public event LevelChangedEventHandler LevelChangedEvent;

    

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

    public bool TrySpend(int value)
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
