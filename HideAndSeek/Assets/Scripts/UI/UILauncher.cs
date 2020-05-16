using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILauncher : UIBase<UILauncherWidget> {
    public InitLauncherScreen initScreen => _widget.initScreen;
    public AbilitiesScreen abilitiesScreen => _widget.abilitiesScreen;

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
        prevScreen.OnDisableScreen();
        prevScreen.gameObject.SetActive(false);

        screen.OnEnableScreen();
        screen.gameObject.SetActive(true);

        prevScreen = screen;
    }
}