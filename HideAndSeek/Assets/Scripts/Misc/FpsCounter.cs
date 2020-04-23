using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour {
    private Text _fpsText;

    protected void Start() {
        _fpsText = GetComponent<Text>();
    }

    protected void Update() {
        _fpsText.text = "" + (int)(1.0f / Time.smoothDeltaTime);
    }
}
