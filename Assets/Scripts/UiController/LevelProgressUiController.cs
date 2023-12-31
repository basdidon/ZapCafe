using UnityEngine;
using UnityEngine.UIElements;

public class LevelProgressUiController : MonoBehaviour
{
    TextElement LevelProgressTxt { get; set; }
    ProgressBar LevelProgressBar { get; set; }

    private void Awake()
    {
        if(TryGetComponent(out UIDocument uiDoc))
        {
            LevelProgressTxt = uiDoc.rootVisualElement.Q<Label>("level-txt");
            LevelProgressBar = uiDoc.rootVisualElement.Q<ProgressBar>("level-progress");
        }

        LevelProgressTxt.text = LevelManager.Instance.Level.ToString();
        LevelProgressBar.highValue = LevelManager.Instance.MaxExpLvl_1;
        LevelProgressBar.value = LevelManager.Instance.Exp;

        LevelManager.Instance.LevelChangedEvent += (newLevel, newMaxExp) =>
        {
            LevelProgressTxt.text = newLevel.ToString();
            LevelProgressBar.highValue = newMaxExp;
        };

        LevelManager.Instance.ExpChangedEvent += (newExp) => {
            Debug.Log($"exp changed : {newExp}");
            LevelProgressBar.value = newExp;
        };

        LevelManager.Instance.OnMaxLevelEvent += (maxLevel) =>
        {
            LevelProgressTxt.text = maxLevel.ToString();
            // if highValue is 0 equal low value progressbar show at 0%
            // so we just set highValue and value with same number that more than 0
            LevelProgressBar.highValue = 100;
            LevelProgressBar.value = 100;
        };
    }
}
