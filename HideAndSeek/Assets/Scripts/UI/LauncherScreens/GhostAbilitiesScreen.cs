using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostAbilitiesScreen : AbstractSoulsIndicatorScreen {

    public Button backBtn;

    [Header("Set ability cards")]
    public Button flareCardBtn;
    public Button invisibilityCardBtn;
    public Button speedupCardBtn;
    public Button radarCardBtn;

    [Header("Set ability states for cards")]
    public GameObject needsBuyStateFlare;
    public GameObject activeStateFlare;

    public GameObject needsBuyStateInvisibility;
    public GameObject activeStateInvisibility;

    public GameObject needsBuyStateSpeedup;
    public GameObject activeStateSpeedup;

    public GameObject needsBuyStateRadar;
    public GameObject activeStateRadar;

    [HideInInspector]
    public static event System.Action OnChangingAbility;

    [Header("Set ability prices text")]
    public Text flarePriceText;
    public Text invisibilityPriceText;
    public Text speedupPriceText;
    public Text radarPriceText;

    protected override void Start() {
        base.Start();

        backBtn.onClick.AddListener(() => {
            Launcher.instance.ChangeScreen(UILauncher.Screen.ABILITIES);
        });

        flareCardBtn.onClick.AddListener(() => {
            OnCardClick(AbilityType.FLARE);
        });

        invisibilityCardBtn.onClick.AddListener(() => {
            OnCardClick(AbilityType.INVISIBILITY);
        });

        speedupCardBtn.onClick.AddListener(() => {
            OnCardClick(AbilityType.SURGE);
        });

        radarCardBtn.onClick.AddListener(() => {
            OnCardClick(AbilityType.RADAR);
        });

        SetPricesToCards();
        ResetAllStates();
    }

    private void OnCardClick(AbilityType abilityType) {
        bool abilityIsAvailable = CheckIfAvailableAndSet(abilityType);

        if (!abilityIsAvailable) {
            BuyAbility(abilityType);
        } else {
            OnChangingAbility?.Invoke();
        }
    }

    private void BuyAbility(AbilityType abilityType) {
        ReadyState readyState =
            FirebaseController.instance.BuyAbilityAsync(Misc.GetAbilityTagByType(abilityType, true));

        StartCoroutine(Misc.WaitWhile(
            () => { return readyState.isReady == false; },
            () => { StartCoroutine(Misc.WaitAndDo(2f, () => { ResetAllStates(); })); }
        ));
    }

    private void SetPricesToCards() {
        flarePriceText.text = Settings.getInstance().firebaseGameData.S_FlarePrice.ToString() + " x";
        invisibilityPriceText.text = Settings.getInstance().firebaseGameData.S_InvisiblePrice.ToString() + " x";
        speedupPriceText.text = Settings.getInstance().firebaseGameData.S_SurgePrice.ToString() + " x";
        radarPriceText.text = Settings.getInstance().firebaseGameData.S_RadarPrice.ToString() + " x";
    }

    private bool CheckIfAvailableAndSet(AbilityType abilityType) {
        string abilityTag = Misc.GetAbilityTagByType(abilityType, true);
        bool abilityIsAvailable = Settings.getInstance().firebaseGameData.abilityTags.Contains(abilityTag);

        if (abilityIsAvailable) {
            Settings.getInstance().seekerAbility = abilityTag;
            Settings.getInstance().save();

            ResetAllStates();
        }

        return abilityIsAvailable;
    }

    private void ResetAllStates() {
        DisableActiveAndEnableNeedsForAllCardStates();
        SetCardStates();
        SetSoulsCount();
    }

    private void DisableActiveAndEnableNeedsForAllCardStates() {
        needsBuyStateFlare.SetActive(true);
        activeStateFlare.SetActive(false);

        needsBuyStateInvisibility.SetActive(true);
        activeStateInvisibility.SetActive(false);

        needsBuyStateSpeedup.SetActive(true);
        activeStateSpeedup.SetActive(false);

        needsBuyStateRadar.SetActive(true);
        activeStateRadar.SetActive(false);
    }

    private void SetCardStates() {
        AbilityType activeAbilityType = Misc.GetAbilityTypeByTag(Settings.getInstance().seekerAbility);
        GetActiveIndicatorFromAbilityType(activeAbilityType).SetActive(true);

        string[] availableAbilities = Settings.getInstance().firebaseGameData.abilityTags.buffer;
        foreach (string abilityTag in availableAbilities) {
            AbilityType abilityType = Misc.GetAbilityTypeByTag(abilityTag);
            if (abilityType != AbilityType.NULL && abilityTag[0] == 'S') {
                GetNeedsBuyStateFromAbilityType(abilityType).SetActive(false);
            }
        }
    }

    private GameObject GetActiveIndicatorFromAbilityType(AbilityType abilityType) {
        switch (abilityType) {
            case AbilityType.FLARE: {
                return activeStateFlare;
            }
            case AbilityType.INVISIBILITY: {
                return activeStateInvisibility;
            }
            case AbilityType.RADAR: {
                return activeStateRadar;
            }
            case AbilityType.SURGE: {
                return activeStateSpeedup;
            }
        }

        return activeStateFlare;
    }

    private GameObject GetNeedsBuyStateFromAbilityType(AbilityType abilityType) {
        switch (abilityType) {
            case AbilityType.FLARE: {
                return needsBuyStateFlare;
            }
            case AbilityType.INVISIBILITY: {
                return needsBuyStateInvisibility;
            }
            case AbilityType.RADAR: {
                return needsBuyStateRadar;
            }
            case AbilityType.SURGE: {
                return needsBuyStateSpeedup;
            }
        }

        return needsBuyStateFlare;
    }
}
