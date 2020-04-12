using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class UIBase {
    protected GameObject canvasGO;

    public UIBase(GameObject canvasGO) {
        this.canvasGO = canvasGO;

        Init();
    }

    abstract protected void Init();
}
