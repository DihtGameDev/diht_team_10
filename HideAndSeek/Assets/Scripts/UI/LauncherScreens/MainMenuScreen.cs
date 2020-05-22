using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : AbstractScreen {
    public float hiTextTimeDuration = 3f;

    public Text hiText;
    public Button playBtn;
    public Button settingsBtn;

    protected void Start() {
        hiText.text = "Hi, Egorik";

        playBtn.onClick.AddListener(OnPlayClick);
        settingsBtn.onClick.AddListener(OnSettingsClick);
    }

    protected void OnEnable() {
        StartCoroutine(ShowHiText());
    }

    private void OnPlayClick() {
        Launcher.instance.ChangeScreen(UILauncher.Screen.ABILITIES);
    }

    private void OnSettingsClick() {
        // TODO
    }

    private IEnumerator ShowHiText() {
        float startTime = Time.time;

        while (Time.time - startTime < hiTextTimeDuration) {
            Color textColor = hiText.color;
            textColor.a = (Time.time - startTime) / hiTextTimeDuration;
            hiText.color = textColor;

            yield return null;
        }
    }


}
