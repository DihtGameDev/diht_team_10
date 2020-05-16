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
        signInBtn.onClick.AddListener(delegate {
            Debugger.Log("SignIn");
            AuthState authState =
                FirebaseController.instance.auth.SignInExistingUser(emailField.text, passwordField.text,
                    (user) => {
                        Settings.getInstance().firebaseUserId = user.UserId;
                        Settings.getInstance().save();
                        FirebaseController.instance.SetFirebaseGameDataToPlayerPrefs(user.UserId);
                    },
                    (errorType) => {
                        Debugger.Log(FirebaseController.instance.auth.MessageFromErrorType(errorType));
                    }
                );
        });
        signUpBtn.onClick.AddListener(delegate {
            Debugger.Log("SignUp");
            AuthState authState =
                FirebaseController.instance.auth.SignUpNewUser(emailField.text, passwordField.text,
                    (user) => {
                        FirebaseController.instance.MoveFirebaseGameDataToNewFirebaseUserId(Settings.getInstance().firebaseUserId, user.UserId);
                        Settings.getInstance().firebaseUserId = user.UserId;
                        Settings.getInstance().save();
                        FirebaseController.instance.SetFirebaseGameDataToPlayerPrefs(user.UserId);
                    },
                    (errorType) => {
                        Debugger.Log(FirebaseController.instance.auth.MessageFromErrorType(errorType));
                    }
                );
        });
    }

    private void SetAbilities() {
        Settings settingsInstance = Settings.getInstance();
        settingsInstance.hidemanAbility = hidemanDropdown.options[hidemanDropdown.value].text;  // заглушка, чтобы потестить
        settingsInstance.seekerAbility = seekerDropwdown.options[seekerDropwdown.value].text;
        settingsInstance.save();

        FirebaseController.instance.CheckAndResetAbilities(); // if we have cheaters
    }

    public override void OnEnable() {
    }

    public override void OnDisable() {
    }
}
