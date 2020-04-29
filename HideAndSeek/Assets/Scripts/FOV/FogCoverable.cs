using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogCoverable : MonoBehaviour {
    Renderer[] renderers;

    void Start() {
        renderers = GetComponentsInChildren<Renderer>();
        FieldOfView.OnTargetsVisibilityChange += FieldOfViewOnTargetsVisibilityChange;  // subscribe
    }

    void OnDestroy() {
        FieldOfView.OnTargetsVisibilityChange -= FieldOfViewOnTargetsVisibilityChange;
    }

    void FieldOfViewOnTargetsVisibilityChange(List<Transform> newTargets) {
        foreach (Renderer r in renderers) {
            r.enabled = newTargets.Contains(transform);
        }
    }
}