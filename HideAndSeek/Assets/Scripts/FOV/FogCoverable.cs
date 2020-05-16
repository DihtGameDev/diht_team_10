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
        FieldOfView.OnTargetsVisibilityChange -= FieldOfViewOnTargetsVisibilityChange;  // unsubscribe
    }

    void FieldOfViewOnTargetsVisibilityChange(List<Transform> visibleTargets) {
        foreach (Renderer r in _renderers) {
            r.enabled = visibleTargets.Contains(transform);
        }
    }
}