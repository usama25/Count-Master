using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using JetBrains.Annotations;
using NaughtyAttributes;
public class UnlockCharactersOnWin : MonoBehaviour
{
    public float CurrentUnlockedValue
    {
        get { return PlayerPrefs.GetFloat("CurrentUnlockedValue", 0); }
        set { PlayerPrefs.SetFloat("CurrentUnlockedValue", value); }
    }
    
    public int CurrentCharacterToUnlock
    {
        get { return PlayerPrefs.GetInt("CurrentCharacterToUnlock", 0); }
        set { PlayerPrefs.SetInt("CurrentCharacterToUnlock", value); }
    }
    
    [SerializeField] private CharacterSelection _characterSelection;

    [SerializeField] private GameObject[] charImages;

    [SerializeField] private float fillUpPerLevel;

    [SerializeField] private GameObject adButton;
    
    [SerializeField] private Text percentagText;
    
    [Button()]
    // Start is called before the first frame update
    void Start()
    {
        if (_characterSelection.UnlockAllCharacters == 1)
            return;

        // if last character was unlocked externally (e.g character selection) then get next character
        if (_characterSelection.GetCharacrterState(CurrentCharacterToUnlock) == 1)
        {
            CurrentCharacterToUnlock = GetNextUnlockableCharacter();
            CurrentUnlockedValue = 0;
        }
        SetPercentageText(CurrentUnlockedValue);

        Invoke("FillAnimation", .5f);
    }

    private void SetPercentageText(float val)
    {
        percentagText.text = (val * 100).ToString("F0") + " %"; 
    }

    int GetNextUnlockableCharacter()
    {
        for (int i = 0; i < charImages.Length; i++)
        {
            if (_characterSelection.GetCharacrterState(i) == 0)
                return i;
        }

        // redundant
        return charImages.Length - 1;
    }

    public GameObject obj;
    void FillAnimation()
    {
         obj = charImages[CurrentCharacterToUnlock];
        obj.gameObject.SetActive(true);
        Image image = obj.transform.GetChild(0).GetComponent<Image>();
        image.fillAmount = CurrentUnlockedValue;
        CurrentUnlockedValue += fillUpPerLevel;
        Tween t = DOTween.To(() => image.fillAmount, x => image.fillAmount = x, CurrentUnlockedValue, 1)
            .SetEase(Ease.Linear);
            t.onComplete = ShowAdButton;
            t.onUpdate = ()=> SetPercentageText(image.fillAmount);
    }

    void ShowAdButton()
    {
        adButton.SetActive(CurrentUnlockedValue == 1);
    }
}
