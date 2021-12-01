using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelection : Singleton<CharacterSelection>
{
    #region prefs
    
    public int CurrentCharacter
    {
        get { return PlayerPrefs.GetInt("CurrentCharacter", 0); }
        set { PlayerPrefs.SetInt("CurrentCharacter", value); }
    }
    public int GetCharacrterState(int i)
    {
        return PlayerPrefs.GetInt("Char" + i, 0);
    }
    
    void SetCharacrterState(int i, int val)
    {
         PlayerPrefs.SetInt("Char" + i, val);
    }
    
    public int CurrentPriceIndex
    {
        get { return PlayerPrefs.GetInt("CurrentPriceIndex", 0); }
        set { PlayerPrefs.SetInt("CurrentPriceIndex", value); }
    }
    
    public int UnlockAllCharacters
    {
        get { return PlayerPrefs.GetInt("UnlockAllCharacters", 0); }
        set { PlayerPrefs.SetInt("UnlockAllCharacters", value); }
    }
    #endregion
    
    #region vars

    [SerializeField] Transform[] buttons;
    [SerializeField] int[] _prices;

    [SerializeField] private Button buyBtn;
    [SerializeField] private TextMeshProUGUI priceText;

    [SerializeField] private UpgardesManager _upgardesManager;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("Char0"))
        {
            SetCharacrterState(0, 1);
        }
        Init();
    }
    #region others

    void Init()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (GetCharacrterState(i) == 1)
            {
                buttons[i].GetChild(3).gameObject.SetActive(false);
            }
        }

        SetCharBtnUI();

        if (UnlockAllCharacters == 0)
            if (AllCharactersUnlocked())
                UnlockAllCharacters = 1;

        if (UnlockAllCharacters == 1)
        {
            priceText.text = "ALL\nUNLOCKED";
            buyBtn.interactable = false;
            return;
        }
        else
            SetPurchasingUI();
        
    }

    private GameObject curCharBtn;
    void SetCharBtnUI()
    {
        if(curCharBtn)
            curCharBtn.transform.GetChild(1).gameObject.SetActive(false);
        
        curCharBtn = buttons[CurrentCharacter].gameObject;
        curCharBtn.transform.GetChild(1).gameObject.SetActive(true);
    }
    bool AllCharactersUnlocked()
    {
        int lockedCount = 0;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (GetCharacrterState(i) == 0)
            {
                lockedCount++;
            }
        }

        return lockedCount == 0;
    }
    
    void SetPurchasingUI()
    {
        int price = _prices[CurrentPriceIndex];

        priceText.text = "OPEN\n" + price;

        if (_upgardesManager.Coins >= price)
        {
            buyBtn.interactable = true;
        }
        else
        {
            buyBtn.interactable = false;
        }
    }

    public void OnAdButton()
    {
        // show ad
    }

    public void AdShownGiveReward()
    {
        _upgardesManager.Coins += 500;
        _upgardesManager.SetCoinsText();
    }
    
    public void OnBuyButton()
    {
        _upgardesManager.Coins -= _prices[CurrentPriceIndex];
        _upgardesManager.SetCoinsText();
        CurrentPriceIndex++;

        int unlockIndex = RandomLockedCharacter();
        SetCharacrterState(unlockIndex, 1);
        Init();
    }
    int RandomLockedCharacter()
    {
        while (true)
        {
            int idx = Random.Range(0, buttons.Length);
            if (GetCharacrterState(idx) == 0)
            {
                return idx;
            }
        }
    }
    
    public void OnClickBtn(int i)
    {
        if (GetCharacrterState(i) == 1)
        {
            CurrentCharacter = i;
             PlayerController.Instance.spawner.Init();
             SetCharBtnUI();
        }
    }
    
    [ContextMenu("UnlockAll")]
    void UnlockAll()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            SetCharacrterState(i, 1);
        }
    }
    #endregion

}
