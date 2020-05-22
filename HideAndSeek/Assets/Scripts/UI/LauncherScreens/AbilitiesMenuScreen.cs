using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesMenuScreen : AbstractSoulsIndicatorScreen {
    public Button playBtn;
    public Button backBtn;

    public Button ghostAbilitiesBtn;
    public Button skeletonAbilitiesBtn;

    public Text ghostAbilityNameText;
    public Text skeletonAbilityNameText;

    public Image ghostAbilityImg;
    public Image skeletonAbilityImg;

    [Header("ability icons")]
    public Sprite flareAbilitySprite;
    public Sprite surgeSprite;
    public Sprite invisibilitySprite;
    public Sprite radarSprite;

    protected override void Start() {
        base.Start();

        playBtn.onClick.AddListener(OnPlayClick);
        backBtn.onClick.AddListener(OnBackClick);

        ghostAbilitiesBtn.onClick.AddListener(() => {
            Launcher.instance.ChangeScreen(UILauncher.Screen.GHOST_ABILITIES);
        });
        skeletonAbilitiesBtn.onClick.AddListener(() => {
            Launcher.instance.ChangeScreen(UILauncher.Screen.SKELETON_ABILITIES);
        });

        SetAbilitiesIconsAndText();

        GhostAbilitiesScreen.OnChangingAbility += SetAbilitiesIconsAndText;
        SkeletonAbilitiesScreen.OnChangingAbility += SetAbilitiesIconsAndText;

        FirebaseController.instance.OnSettingSettingsFromFirebase += SetSoulsCount;
    }

    private void OnPlayClick() {
        Launcher.instance.Connect();
    }

    private void OnBackClick() {
        Launcher.instance.ChangeScreen(UILauncher.Screen.MAIN);
    }

    private void SetAbilitiesIconsAndText() {
        AbilityType ghostAbilityType = Misc.GetAbilityTypeByTag(Settings.getInstance().seekerAbility);
        AbilityType skeletonAbilityType = Misc.GetAbilityTypeByTag(Settings.getInstance().hidemanAbility);

        ghostAbilityImg.sprite = GetAbilitySprite(ghostAbilityType);
        skeletonAbilityImg.sprite = GetAbilitySprite(skeletonAbilityType);

        ghostAbilityNameText.text = GetAbilityName(ghostAbilityType);
        skeletonAbilityNameText.text = GetAbilityName(skeletonAbilityType);
    }

    private Sprite GetAbilitySprite(AbilityType abilityType) {
        switch (abilityType) {
            case AbilityType.FLARE: {
                return flareAbilitySprite;
            }
            case AbilityType.INVISIBILITY: {
                return invisibilitySprite;
            }
            case AbilityType.RADAR: {
                return radarSprite;
            }
            case AbilityType.SURGE: {
                return surgeSprite;
            }
        }

        return flareAbilitySprite;
    }

    private string GetAbilityName(AbilityType abilityType) {
        switch (abilityType) {
            case AbilityType.FLARE: {
                return "Flare";
            }
            case AbilityType.INVISIBILITY: {
                return "Invisibility";
            }
            case AbilityType.RADAR: {
                return "Radar";
            }
            case AbilityType.SURGE: {
                return "Speedup";
            }
        }

        return "Flare";
    }
}
