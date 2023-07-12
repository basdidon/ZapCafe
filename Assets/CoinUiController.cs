using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinUiController : MonoBehaviour
{
    public TextMeshPro coin_txt;

    private void Awake()
    {
        //LevelManager.Instance.OnCoinChanged += newCoinValue => coin_txt.SetText(newCoinValue);
    }
}
