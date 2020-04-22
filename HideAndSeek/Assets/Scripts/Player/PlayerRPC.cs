using UnityEngine;
using Photon.Pun;

public class PlayerRPC : MonoBehaviour {
    protected void Start() {
        GameManager.GAME_MANAGER.uiGame.PrintInChat(GetComponent<PhotonView>().Owner.NickName + " начал играть");
    }

    [PunRPC]
    void KillHideman(string viewId, string killerNickname) {
        foreach (var player in GameObject.FindGameObjectsWithTag(Constants.HIDEMAN_TAG)) {
            PhotonView photonView = player.GetComponent<PhotonView>();
            if (photonView.ViewID == System.Int32.Parse(viewId)) {
                if (player == GameManager.GAME_MANAGER.mainPlayer) {
                    Camera.main.GetComponent<CameraController>().SetChasingObject(null);
                    GameManager.GAME_MANAGER.uiGame.StartRespawnTimer();
                    GameManager.GAME_MANAGER.Invoke("Respawn", Constants.RESPAWN_TIME);
                }

                GameManager.GAME_MANAGER.nicknameManager.DeletePlayer(player);
                Destroy(player);

                GameManager.GAME_MANAGER.uiGame.PrintInChat(killerNickname + " убил " + photonView.Owner.NickName);

                return;
            }
        }
    }
}
