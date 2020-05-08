using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogCoverable : MonoBehaviour {
    private Renderer[] _renderers;

    void Start() {
        _renderers = GetComponentsInChildren<Renderer>();
        FieldOfView.OnTargetsVisibilityChange += FieldOfViewOnTargetsVisibilityChange;  // subscribe
    }

    void OnDestroy() {
        FieldOfView.OnTargetsVisibilityChange -= FieldOfViewOnTargetsVisibilityChange;  // unsubscrive
    }

    void FieldOfViewOnTargetsVisibilityChange(List<Transform> newTargets) {
        foreach (Renderer r in _renderers) {
            r.enabled = newTargets.Contains(transform);
        }
    }
}