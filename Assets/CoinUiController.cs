using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class CoinUiController : MonoBehaviour
{
    TextElement coinText;

    private void Awake()
    {
        if(TryGetComponent(out UIDocument uIDoc))
        {
            coinText = uIDoc.rootVisualElement.Q<Label>("CoinText");
        }

        coinText.text = LevelManager.Instance.Coin.ToString();
        LevelManager.Instance.OnCoinChanged += newCoinValue => coinText.text = newCoinValue.ToString();
    }
}
