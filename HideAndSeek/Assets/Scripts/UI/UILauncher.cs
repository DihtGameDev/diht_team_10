using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILauncher : UIBase {
    public InputField nicknameField;
    public Button settingsBtn;
    public Button quickplayBtn;
    
    public UILauncher(GameObject canvasGO) : base(canvasGO) {
    }

    protected override void Init() {
        nicknameField = canvasGO.transform.Find("NicknameField").GetComponent<InputField>();
        settingsBtn = canvasGO.transform.Find("SettingsBtn").GetComponent<Button>();
        quickplayBtn = canvasGO.transform.Find("QuickPlayBtn").GetComponent<Button>();
    }
}
