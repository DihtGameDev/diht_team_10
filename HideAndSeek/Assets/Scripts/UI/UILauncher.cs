using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILauncher {
    private GameObject _canvasGO;

    public InputField nicknameField;
    public Button settingsBtn;
    public Button quickplayBtn;

    public UILauncher(GameObject canvasGO) {
        _canvasGO = canvasGO;

        Init();
    }

    private void Init() {
        nicknameField = _canvasGO.transform.Find("NicknameField").GetComponent<InputField>();
        settingsBtn = _canvasGO.transform.Find("SettingsBtn").GetComponent<Button>();
        quickplayBtn = _canvasGO.transform.Find("QuickPlayBtn").GetComponent<Button>();
    }
}
