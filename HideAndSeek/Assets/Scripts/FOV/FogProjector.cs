using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogProjector : MonoBehaviour {
    public RenderTexture fogTexture;
    public Shader blurShader;

    [Range(1, 4)]
    public int upsample = 2;

    public float blur = 1;
    public float blendSpeed = 1;

    [SerializeField]
    private Projector _projector;

    private RenderTexture _projecTexture;
    private RenderTexture _oldTexture;
    private Material _blurMaterial;

    private float _blend;
    private int _blendNameId;

    void OnEnable() {
        _blurMaterial = new Material(blurShader);
        _blurMaterial.SetVector("_Parameter", new Vector4(blur, -blur, 0, 0));

        _projecTexture = new RenderTexture(
                            fogTexture.width * upsample,
                            fogTexture.height * upsample,
                            0,
                            fogTexture.format) {filterMode = FilterMode.Bilinear};

        _oldTexture = new RenderTexture(
                         fogTexture.width * upsample,
                         fogTexture.height * upsample,
                         0,
                         fogTexture.format) {filterMode = FilterMode.Bilinear};

        _projector.material.SetTexture("_FogTex", _projecTexture);
        _projector.material.SetTexture("_OldFogTex", _oldTexture);
        _blendNameId = Shader.PropertyToID("_Blend");
        _blend = 1;
        _projector.material.SetFloat(_blendNameId, _blend);
        Graphics.Blit(fogTexture, _projecTexture);
        UpdateFog();
    }

    public void UpdateFog() {
        Graphics.Blit(_projecTexture, _oldTexture);
        Graphics.Blit(fogTexture, _projecTexture);

        RenderTexture temp = RenderTexture.GetTemporary(
            _projecTexture.width,
            _projecTexture.height,
            0,
            _projecTexture.format
        );

        temp.filterMode = FilterMode.Bilinear;

        Graphics.Blit(_projecTexture, temp, _blurMaterial, 1);
        Graphics.Blit(temp, _projecTexture, _blurMaterial, 2);

        StartCoroutine(Blend());

        RenderTexture.ReleaseTemporary(temp);
    }

    IEnumerator Blend() {
        _blend = 0;
        _projector.material.SetFloat(_blendNameId, _blend);
        while (_blend < 1) {
            _blend = Mathf.MoveTowards(_blend, 1, blendSpeed * Time.deltaTime);
            _projector.material.SetFloat(_blendNameId, _blend);
            yield return null;
        }
    }
}