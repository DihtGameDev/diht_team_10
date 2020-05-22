using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILauncher : UIBase<UILauncherWidget> {
    public MainMenuScreen mainMenuScreen => _widget.mainMenuScreen;
    public AbilitiesMenuScreen abilitiesMenuScreen => _widget.abilitiesMenuScreen;
    public GhostAbilitiesScreen ghostAbilitiesScreen => _widget.ghostAbilitiesScreen;
    public SkeletonAbilitiesScreen skeletonAbilitiesScreen => _widget.skeletonAbilitiesScreen;

    public AbstractScreen prevScreen;

    public enum Screen {
        NULL, MAIN, ABILITIES, GHOST_ABILITIES, SKELETON_ABILITIES
    }

    public UILauncher(UILauncherWidget widget) : base(widget) {
        prevScreen = null;

        mainMenuScreen.gameObject.SetActive(false);
        abilitiesMenuScreen.gameObject.SetActive(false);
        ghostAbilitiesScreen.gameObject.SetActive(false);
        skeletonAbilitiesScreen.gameObject.SetActive(false);
    }

    public void SetScreen(Screen state) {
        switch (state) {
            case Screen.MAIN: {
                Debug.Log("Switch main screen");
                SwitchScreen(mainMenuScreen);
                break;
            }
            case Screen.ABILITIES: {
                Debug.Log("Switch to abilities screen");
                SwitchScreen(abilitiesMenuScreen);
                break;
            }
            case Screen.GHOST_ABILITIES: {
                Debug.Log("Switch to ghost abilities screen");
                SwitchScreen(ghostAbilitiesScreen);
                break;
            }
            case Screen.SKELETON_ABILITIES: {
                Debug.Log("Switch to skeleton abilities screen");
                SwitchScreen(skeletonAbilitiesScreen);
                break;
            }
        }
    }

    private void SwitchScreen(AbstractScreen screen) {
        prevScreen?.gameObject.SetActive(false);
        screen.gameObject.SetActive(true);

        prevScreen = screen;
    }
}