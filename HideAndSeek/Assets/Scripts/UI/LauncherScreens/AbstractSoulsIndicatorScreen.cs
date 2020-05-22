using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractSoulsIndicatorScreen : AbstractScreen {
    public Text soulsIndicatorText;

    protected virtual void Start() {
        SetSoulsCount();
    }

    protected void SetSoulsCount() {
        soulsIndicatorText.text = Settings.getInstance().firebaseGameData.coins.ToString() + " x";
    }
}
