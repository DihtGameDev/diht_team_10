using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkeletonAbilitiesScreen : AbstractSoulsIndicatorScreen {
    public Button backBtn;

    [Header("Set ability cards")]
    public Button speedupCardBtn;
    public Button invisibilityCardBtn;

    [Header("Set ability states for cards")]
    public GameObject needsBuyStateSpeedup;
    public GameObject activeStateSpeedup;

    public GameObject needsBuyStateInvisibility;
    public GameObject activeStateInvisibility;

    [HideInInspector]
    public static event System.Action OnChangingAbility;

    [Header("Set ability prices text")]
    public Text speedupPriceText;
    public Text invisibilityPriceText;

    protected override void Start() {
        base.Start();

        backBtn.onClick.AddListener(() => {
            Launcher.instance.ChangeScreen(UILauncher.Screen.ABILITIES);
        });

        speedupCardBtn.onClick.AddListener(() => {
            OnCardClick(AbilityType.SURGE);
        });

        invisibilityCardBtn.onClick.AddListener(() => {
            OnCardClick(AbilityType.INVISIBILITY);
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
            FirebaseController.instance.BuyAbilityAsync(Misc.GetAbilityTagByType(abilityType, false));

        StartCoroutine(Misc.WaitWhile(
            () => { return readyState.isReady == false; },
            () => { StartCoroutine(Misc.WaitAndDo(2f, () => { ResetAllStates(); })); }
        ));
    }

    private void SetPricesToCards() {
        invisibilityPriceText.text = Settings.getInstance().firebaseGameData.H_InvisiblePrice.ToString() + " x";
        speedupPriceText.text = Settings.getInstance().firebaseGameData.H_SurgePrice.ToString() + " x";
    }

    private bool CheckIfAvailableAndSet(AbilityType abilityType) {
        string abilityTag = Misc.GetAbilityTagByType(abilityType, false);
        bool abilityIsAvailable = Settings.getInstance().firebaseGameData.abilityTags.Contains(abilityTag);

        if (abilityIsAvailable) {
            Settings.getInstance().hidemanAbility = abilityTag;
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
        needsBuyStateInvisibility.SetActive(true);
        activeStateInvisibility.SetActive(false);

        needsBuyStateSpeedup.SetActive(true);
        activeStateSpeedup.SetActive(false);
    }

    private void SetCardStates() {
        AbilityType activeAbilityType = Misc.GetAbilityTypeByTag(Settings.getInstance().hidemanAbility);
        GetActiveIndicatorFromAbilityType(activeAbilityType).SetActive(true);

        string[] availableAbilities = Settings.getInstance().firebaseGameData.abilityTags.buffer;
        foreach (string abilityTag in availableAbilities) {
            AbilityType abilityType = Misc.GetAbilityTypeByTag(abilityTag); // sorry for this, a will fix this later
            if (abilityType != AbilityType.NULL && abilityTag[0] == 'H') {
                GetNeedsBuyStateFromAbilityType(abilityType).SetActive(false);
            } 
        }
    }

    private GameObject GetActiveIndicatorFromAbilityType(AbilityType abilityType) {
        switch (abilityType) {
            case AbilityType.SURGE: {
                return activeStateSpeedup;
            }
            case AbilityType.INVISIBILITY: {
                return activeStateInvisibility;
            }
        }

        return activeStateSpeedup;
    }

    private GameObject GetNeedsBuyStateFromAbilityType(AbilityType abilityType) {
        switch (abilityType) {
            case AbilityType.INVISIBILITY: {
                return needsBuyStateInvisibility;
            }
            case AbilityType.SURGE: {
                return needsBuyStateSpeedup;
            }
        }

        return needsBuyStateSpeedup;
    }
}
