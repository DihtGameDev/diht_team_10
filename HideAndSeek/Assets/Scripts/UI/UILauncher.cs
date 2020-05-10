using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILauncher : UIBase<UILauncherWidget> {
    public InitLauncherScreen initScreen => _widget.initScreen;
    public AbilitiesScreen abilitiesScreen => _widget.abilitiesScreen;
    /*  public InputField nicknameField => _widget.nicknameField;
      public Button settingsBtn => _widget.settingsBtn;
      public Button quickplayBtn => _widget.quickplayBtn;
      public Text connectingMessage => _widget.connectingMessage;
      public Button setAbilities => _widget.setAbilities;*/

    public AbstractScreen prevScreen;

    public enum Screen {
        NULL, INIT, CONNECTING, SET_ABILITIES
    }

    public UILauncher(UILauncherWidget widget) : base(widget) {
        prevScreen = initScreen;
        initScreen.gameObject.SetActive(false);
        abilitiesScreen.gameObject.SetActive(false);
    }

    public void SetScreen(Screen state) {
        switch (state) {
            case Screen.INIT: {
                SwitchScreen(initScreen);
                break;
            }
            case Screen.SET_ABILITIES: {
                SwitchScreen(abilitiesScreen);
                break;
            }
        }
    }

    private void SwitchScreen(AbstractScreen screen) {
        prevScreen.gameObject.SetActive(false);
        screen.gameObject.SetActive(true);
        prevScreen = screen;
    }

    /*
    public void InitState() {
        nicknameField.gameObject.SetActive(true);
        settingsBtn.gameObject.SetActive(true);
        quickplayBtn.gameObject.SetActive(true);
        
        setAbilities.gameObject.SetActive(true);
    }

    public void ConnectState() {
        nicknameField.gameObject.SetActive(false);
        settingsBtn.gameObject.SetActive(false);
        quickplayBtn.gameObject.SetActive(false);
        
        setAbilities.gameObject.SetActive(false);
    }*/
}