using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILauncher : UIBase<UILauncherWidget> {
    public InputField nicknameField => _widget.nicknameField;
    public Button settingsBtn => _widget.settingsBtn;
    public Button quickplayBtn => _widget.quickplayBtn;
    public Text connectingMessage => _widget.connectingMessage;

    public UILauncher(UILauncherWidget widget) : base(widget) {
        nicknameField.text = Settings.getInstance().nickname;
        quickplayBtn.onClick.AddListener(Launcher.LAUNCHER.Connect);
    }

    public void InitState() {
        connectingMessage.gameObject.SetActive(false);
    }

    public void ConnectState() {
        connectingMessage.text = "Connecting...";
        connectingMessage.gameObject.SetActive(true);
    }
}