using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using  NaughtyAttributes;
using UnityEngine.UI;

public class UpgardesManager : Singleton<UpgardesManager>
{
    #region prefs

    public int Coins
    {
        get { return PlayerPrefs.GetInt("Coins", 0); }
        set { PlayerPrefs.SetInt("Coins", value); }
    }
    
    int GetUpgardeLevel(string str)
    {
        // first upgrade is dafault and is always unlocked
        return PlayerPrefs.GetInt(str, 0);
    }

    void SetUpgardeLevel(string str, int val)
    {
        PlayerPrefs.SetInt(str, val);
    }
    #endregion

    #region vars

    public Upgrade[] _upgrades;
    [SerializeField] private TextMeshProUGUI coinsText;

    [Foldout("Upgrade Anim")]
    [SerializeField]
    private GameObject animPrefab;
    [Foldout("Upgrade Anim")]
    [SerializeField] private float animRadius, animOffset;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Init();
        SetCoinsText();
    }

    #region others

    public int GetCurrentUpgradeValue(int index)
    {
        Upgrade upgrade = _upgrades[index];
        return upgrade.upgradeValues[GetUpgardeLevel(upgrade.prefString)]; 
    }
    void Init()
    {
        for (int i = 0; i < _upgrades.Length; i++)
        {
            SetUpgradeData(i);
        }
    }

    void SetUpgradeData(int i)
    {
        Upgrade upgrade;

        upgrade = _upgrades[i];
        upgrade.titleText.text = upgrade.name;
        int upgradeLevel = GetUpgardeLevel(upgrade.prefString);
        
        // if upgrade is at max
        if (upgradeLevel == upgrade.upgradeValues.Length - 1)
        {
            upgrade.canvasGroup.interactable = false;
            upgrade.priceText.text = "";
            upgrade.upgradeLevel.text =  "Max\nLVL";

        }
        else
        {
            upgrade.upgradeLevel.text = (upgradeLevel+1) + "\nLVL";

            if (Coins > upgrade.upgradePrices[upgradeLevel])
            {
                upgrade.canvasGroup.interactable = true;
                // +1 shows the price of next upgrade
                upgrade.priceText.text = upgrade.upgradePrices[upgradeLevel + 1].ToString();
            }
            else
            {
                bool adIsAvailable = false;
                upgrade.canvasGroup.interactable = false;
                upgrade.adBtn.SetActive(adIsAvailable);
            }
        }
    }

    public void BuyUpgrade(int i)
    {
        Upgrade upgrade;
        upgrade = _upgrades[i];
        int upgradeLevel = GetUpgardeLevel(upgrade.prefString);
        Coins -= upgrade.upgradePrices[upgradeLevel];
        SetCoinsText();

        UpgradeAnim(upgrade.canvasGroup.transform);
        
        if (upgradeLevel < upgrade.upgradeValues.Length - 1)
            SetUpgardeLevel(upgrade.prefString, upgradeLevel + 1);
        else
            upgrade.canvasGroup.interactable = false;

        if(upgrade.onPurchase!=null)
            upgrade.onPurchase.Invoke();
        
        SetUpgradeData(i);
    }

    public void SetCoinsText()
    {
        coinsText.text = Coins.ToString();
    }
    
    [Button("Set 10000 Coins")]
    void SetCoins()
    {
         Coins = 10000;
    }
    
    [Button("Clear playerprefs")]
    void clearplayerprefs()
    {
        PlayerPrefs.DeleteAll();
    }

    void SetHundredUpgradeLevels()
    {
            for (int j = 0; j < 100; j++)
            {
                _upgrades[1].upgradeValues[j] = j * 10;
                _upgrades[1].upgradePrices[j] = j * 100;
            }
        
    }
    
    #endregion
    public void UpgradeAnim(Transform animParent)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 positionToSpawn = Random.insideUnitCircle * animRadius;
            GameObject objInstantiated = Instantiate(animPrefab, animParent);
            objInstantiated.transform.localPosition = positionToSpawn;
            objInstantiated.GetComponent<Image>().color = new Color(1, 1, 1, Random.Range(0f, 1f));
            float randomScale = Random.Range(.5f, 1f);
            objInstantiated.SetActive(true);
            objInstantiated.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
            objInstantiated.transform.DOLocalMoveY(animOffset, Random.Range(.5f, 1f)).SetEase(Ease.Linear).SetRelative(true).onComplete = () => objInstantiated.SetActive(false);
            // prefabsReference.Add(objInstantiated);
        }
    }
}
[System.Serializable]
public class Upgrade
{
    public string name;
    
    [Tooltip("On which level the upgrade is")]
    public TextMeshProUGUI upgradeLevel, priceText, titleText;
    
    [Tooltip("If user has no coins we'll show ad button")]
    public GameObject adBtn;

    public int[] upgradePrices;
    
    public int[] upgradeValues;

    public string prefString;

    public CanvasGroup canvasGroup;

    public UnityEvent onPurchase;
}