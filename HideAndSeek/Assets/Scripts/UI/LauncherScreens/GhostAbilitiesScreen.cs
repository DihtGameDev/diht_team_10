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

    protected override void Start() {
        base.Start();

        backBtn.onClick.AddListener(() => {
            Launcher.instance.ChangeScreen(UILauncher.Screen.ABILITIES);
        });

        DisableActiveAndEnableNeedsForAllCardStates();
        SetCardStates();
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
            if (abilityType != AbilityType.NULL) {
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
