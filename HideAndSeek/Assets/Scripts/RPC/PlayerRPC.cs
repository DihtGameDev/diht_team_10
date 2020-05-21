using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerRPC : MonoBehaviour {

    private List<MeshRenderer> _renderers = new List<MeshRenderer>();

    [Header("Set in inspector")]
    public ParticleSystem surgePS; // setted in inspector
    public ParticleSystem invisiblePS; // setted in inspector

    protected void Start() {
        MeshRenderer[] allRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in allRenderers) {
            if (renderer.GetComponent<FieldOfView>() == null) { // we don't hide fov
                _renderers.Add(renderer);
            }
        }
    }

    [PunRPC]
    void DestroyThisPUNObject() {
    }

    [PunRPC]
    void KillHideman(string killerNickname, string victimName) {
        if (gameObject == GameManager.instance.mainPlayer) {  // this is mine hideman
            Camera.main.GetComponent<CameraController>().SetChasingObject(null);
            GameManager.instance.uiGame.StartRespawnTimer();
            GameManager.instance.Invoke("Respawn", Constants.RESPAWN_TIME);

            PhotonNetwork.Destroy(gameObject);
        }

        GameManager.instance.uiGame.PrintInChatAndClear(killerNickname + " убил " + victimName);

        if (PhotonNetwork.IsMasterClient) {
            GameManager.instance.SpawnLyingSkeletonsIfNeeds();
        }
    }

    [PunRPC]
    void BecomeInvisibleForTime(float timeSec) {
        Debugger.Log("become invisible for: " + name);
        foreach (var render in _renderers) {
            render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }

        StartCoroutine(BecomeVisible(timeSec));
    }

    private IEnumerator BecomeVisible(float delaySec) {
        yield return new WaitForSeconds(delaySec);
        Debugger.Log("Stop invisible: " + delaySec);
        foreach (var render in _renderers) {
            render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    [PunRPC]
    void UsedSurgeAbilityParticleSystem(float surgeDuration) {
        var main = surgePS.main;
        main.duration = surgeDuration * 0.7f;
        surgePS.Play();
    }
}
