using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILauncher : MonoBehaviour {
    private GameObject _canvasGO;

    private InputField _nicknameField;
    private Button _settingsBtn;
    private Button _quickplayBtn;

    protected void Start() {
        _canvasGO = GameObject.Find("Canvas");

        _nicknameField = _canvasGO.transform.Find("NicknameField").GetComponent<InputField>();
        _settingsBtn = _canvasGO.transform.Find("SettingsBtn").GetComponent<Button>();
        _quickplayBtn = _canvasGO.transform.Find("QuickPlayBtn").GetComponent<Button>();

        _settingsBtn.onClick.AddListener(OnSettingsClicked);
        _quickplayBtn.onClick.AddListener(OnQuckplayClicked);
    }

    private void OnSettingsClicked() {
        // TODO
        print("settings button clicked");
    }

    private void OnQuckplayClicked() {
        // TODO
        print("on quick play clicked");

    }
}
