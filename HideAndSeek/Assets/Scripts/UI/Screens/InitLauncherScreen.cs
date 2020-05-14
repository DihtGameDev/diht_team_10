using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitLauncherScreen : AbstractScreen {
    public InputField nicknameField;
    public Button settingsBtn;
    public Button quickplayBtn;
    public Text connectingMessage;
    public Button setAbilities;

    protected void Start() {
        nicknameField.text = Settings.getInstance().nickname;
        quickplayBtn.onClick.AddListener(Launcher.LAUNCHER.Connect);
        setAbilities.onClick.AddListener(delegate {
            Launcher.LAUNCHER.ChangeScreen(UILauncher.Screen.SET_ABILITIES);
        });
        settingsBtn.onClick.AddListener(() => {
            FirebaseController.instance.GetValue();
        });
    }

    public void ConnectingState() {
        connectingMessage.gameObject.SetActive(true);
        connectingMessage.text = "Connecting...";
    }

    public override void OnEnable() {
    }

    public override void OnDisable() {
    }
}
