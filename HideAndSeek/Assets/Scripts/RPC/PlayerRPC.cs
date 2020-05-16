using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerRPC : MonoBehaviour {

    private List<MeshRenderer> _renderers = new List<MeshRenderer>();

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
        GameManager.instance.nicknameManager.DeletePlayer(gameObject);
    }

    [PunRPC]
    void KillHideman(string killerNickname, string victimName) {
        GameManager.instance.nicknameManager.DeletePlayer(gameObject);

        if (gameObject == GameManager.instance.mainPlayer) {  // this is mine hideman
            Camera.main.GetComponent<CameraController>().SetChasingObject(null);
            GameManager.instance.uiGame.StartRespawnTimer();
            GameManager.instance.Invoke("Respawn", Constants.RESPAWN_TIME);

            PhotonNetwork.Destroy(gameObject);
        }

        GameManager.instance.uiGame.PrintInChat(killerNickname + " убил " + victimName);
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
}
