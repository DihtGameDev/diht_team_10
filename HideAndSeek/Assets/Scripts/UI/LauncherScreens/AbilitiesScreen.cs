using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AbilitiesScreen : AbstractScreen {
    public Dropdown hidemanDropdown;
    public Dropdown seekerDropwdown;

    public Button backBtn;
    public Button signInBtn;
    public Button signUpBtn;

    public InputField emailField;
    public InputField passwordField;

    public Text coinsText;

    protected void Start() {
        FirebaseController.instance.OnGettingFirebaseData += UpdateCoinsFromFirebaseGameData;

        hidemanDropdown.onValueChanged.AddListener((value) => {
                SetAbilities();
        });
        seekerDropwdown.onValueChanged.AddListener((value) => {
            SetAbilities();
        });
        backBtn.onClick.AddListener(() => {
            Launcher.LAUNCHER.ChangeScreen(UILauncher.Screen.INIT);
        });

        signUpBtn.onClick.AddListener(OnSignUpClicked);
        signInBtn.onClick.AddListener(OnSignInClicked);
    }

    public void Destroy() {
        FirebaseController.instance.OnGettingFirebaseData -= UpdateCoinsFromFirebaseGameData;
    }

    public override void OnEnableScreen() {
    }

    public override void OnDisableScreen() {
    }

    private void SetAbilities() {
        Settings settingsInstance = Settings.getInstance();
        settingsInstance.hidemanAbility = hidemanDropdown.options[hidemanDropdown.value].text;  // заглушка, чтобы потестить
        settingsInstance.seekerAbility = seekerDropwdown.options[seekerDropwdown.value].text;
        settingsInstance.save();

        ReadyState readyState = FirebaseController.instance.CheckAndResetAbilities(); // if we have cheaters
        DialogManager.instance.ShowDialog(DialogType.LOADING, () => { return readyState.isReady == false; });
    }

    private void OnSignUpClicked() {
        AuthState authState =
                FirebaseController.instance.auth.SignUpNewUser(emailField.text, passwordField.text,
                    (user) => {
                        FirebaseController.instance.MoveFirebaseGameDataToNewFirebaseUserId(Settings.getInstance().firebaseUserId, user.UserId);
                        Settings.getInstance().firebaseUserId = user.UserId;
                        Settings.getInstance().email = emailField.text;
                        Settings.getInstance().save();
                        FirebaseController.instance.SetFirebaseGameDataToPlayerPrefs(user.UserId);
                    },
                    (errorType) => {
                        Debugger.Log(FirebaseController.instance.auth.MessageFromErrorType(errorType));
                    }
                );
    }

    private void OnSignInClicked() {
        AuthState authState =
            FirebaseController.instance.auth.SignInExistingUser(emailField.text, passwordField.text,
                (user) => {
                    Settings.getInstance().firebaseUserId = user.UserId;
                    Settings.getInstance().email = emailField.text;
                    Settings.getInstance().save();
                    FirebaseController.instance.SetFirebaseGameDataToPlayerPrefs(user.UserId);
                },
                (errorType) => {
                    Debugger.Log(FirebaseController.instance.auth.MessageFromErrorType(errorType));
                }
            );
        DialogManager.instance.ShowDialog(DialogType.LOADING, () => { print(authState.state); return authState.state == AuthStates.NULL; });
    }

    public void UpdateCoinsFromFirebaseGameData(FirebaseGameData firebaseGameData) {
        coinsText.text = "coins: " + firebaseGameData.coins;
    }
}
