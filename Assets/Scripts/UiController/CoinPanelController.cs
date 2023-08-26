using UnityEngine;
using UnityEngine.UIElements;

public class CoinPanelController : MonoBehaviour
{
    TextElement CoinText { get; set; }

    private void Awake()
    {
        if(TryGetComponent(out UIDocument uiDoc))
        {
            CoinText = uiDoc.rootVisualElement.Q<Label>("coin-txt");
        }

        CoinText.text = LevelManager.Instance.Coin.ToString();
        LevelManager.Instance.CoinChangedEvent += (newCoinValue) => CoinText.text = newCoinValue.ToString();
    }
}
