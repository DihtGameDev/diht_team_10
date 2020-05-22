using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkeletonAbilitiesScreen : AbstractSoulsIndicatorScreen {
    public Button backBtn;

    protected override void Start() {
        base.Start();

        backBtn.onClick.AddListener(() => {
            Launcher.instance.ChangeScreen(UILauncher.Screen.ABILITIES);
        });
    }
}
