using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AbilitiesScreen : AbstractScreen {
    public Dropdown hidemanDropdown;
    public Dropdown seekerDropwdown;
    public Button backBtn;

    protected void Start() {
        hidemanDropdown.onValueChanged.AddListener(delegate {
                SetAbilities();
            });
        seekerDropwdown.onValueChanged.AddListener(delegate {
            SetAbilities();
        });
        backBtn.onClick.AddListener(delegate {
            Launcher.LAUNCHER.ChangeScreen(UILauncher.Screen.INIT);
        });
    }

    private void SetAbilities() {
        Settings settingsInstance = Settings.getInstance();
        settingsInstance.hidemanAbility = hidemanDropdown.options[hidemanDropdown.value].text;  // заглушка, чтобы потестить
        settingsInstance.seekerAbility = seekerDropwdown.options[seekerDropwdown.value].text;
        settingsInstance.save();
    }

    public override void OnEnable() {
    }

    public override void OnDisable() {
    }
}
