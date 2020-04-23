using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBase<TWidget> where TWidget : MonoBehaviour {
    protected TWidget _widget;

    public UIBase(TWidget widget) {
        _widget = widget;
    }
}
