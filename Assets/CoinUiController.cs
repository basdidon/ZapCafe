using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinUiController : MonoBehaviour
{
    public TMP_Text coin_txt;

    private void Awake()
    {
        if (coin_txt == null)
            Debug.LogError("coin_txt is null");

        coin_txt.SetText("0");
        LevelManager.Instance.OnCoinChanged += newCoinValue => coin_txt.SetText(newCoinValue.ToString());
    }
}
