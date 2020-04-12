using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : UIBase {
    public GameObject loadingScreenGO;
    public Joystick moveJoystick;

    public UIGame(GameObject canvasGO) : base(canvasGO) {
    }

    protected override void Init() {
        loadingScreenGO = canvasGO.transform.Find("LoadingScreen").gameObject;
        moveJoystick = canvasGO.GetComponentInChildren<Joystick>();
    }
}
