﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitLauncherScreen : AbstractScreen {
    public InputField nicknameField;

    public Button settingsBtn;
    public Button quickplayBtn;
    public Button setAbilities;

    public Text connectingMessage;

    protected void Start() {
        nicknameField.text = Settings.getInstance().nickname;

        quickplayBtn.onClick.AddListener(Launcher.LAUNCHER.Connect);
        setAbilities.onClick.AddListener(() => {
            Launcher.LAUNCHER.ChangeScreen(UILauncher.Screen.SET_ABILITIES);
        });
    }

    public void ConnectingState() {
        connectingMessage.gameObject.SetActive(true);
        connectingMessage.text = "Connecting...";
    }

    public override void OnEnableScreen() {
    }

    public override void OnDisableScreen() {
    }
}
