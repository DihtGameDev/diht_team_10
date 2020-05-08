using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour {
    [SerializeField]
    private Text _fpsText;

    protected void Update() {
        _fpsText.text = "" + (int)(1.0f / Time.smoothDeltaTime);
    }
}
