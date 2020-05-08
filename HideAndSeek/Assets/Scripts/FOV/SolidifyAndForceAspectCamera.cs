using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidifyAndForceAspectCamera : MonoBehaviour {
    public Shader flatShader;
    public float aspect = 1;

    [SerializeField]
    private Camera _camera;

	void OnEnable () {
        _camera.aspect = aspect;
        _camera.SetReplacementShader(flatShader, "");
    }
}
