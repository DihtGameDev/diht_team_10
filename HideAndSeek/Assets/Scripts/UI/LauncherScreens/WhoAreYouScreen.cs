using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhoAreYouScreen : AbstractScreen {
    public float whoAreYouTextTimeDuration = 3f;

    public Button doneBtn;
    public Text doneText;
    public InputField nicknameField;
    public Text whoAreYouText;

    protected void Start() {
        doneBtn.onClick.AddListener(OnDoneClick);
    }

    private void OnDoneClick() {
        Settings.getInstance().nickname = nicknameField.text;
        Settings.getInstance().save();
        Launcher.instance.ChangeScreen(UILauncher.Screen.MAIN);
    }

    protected void OnEnable() {
        StartCoroutine(DoneBtnActivator());
        StartCoroutine(ShowWhoAreYouText());
    }

    private IEnumerator DoneBtnActivator(float waitDelay=0.5f) {
        while (true) {
            if (nicknameField.text == "") {
                doneText.color = Color.gray;
                doneBtn.interactable = false;
            } else {
                doneText.color = Color.white;
                doneBtn.interactable = true;
            }

            yield return new WaitForSeconds(waitDelay);
        }
    }

    private IEnumerator ShowWhoAreYouText() {
        float startTime = Time.time;

        while (Time.time - startTime < whoAreYouTextTimeDuration) {
            Color textColor = whoAreYouText.color;
            textColor.a = (Time.time - startTime) / whoAreYouTextTimeDuration;
            whoAreYouText.color = textColor;

            yield return null;
        }
    }
}
