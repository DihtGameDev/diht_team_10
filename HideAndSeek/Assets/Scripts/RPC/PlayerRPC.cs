using UnityEngine;
using Photon.Pun;

public class PlayerRPC : MonoBehaviour {

    [PunRPC]
    void DestroyThisPUNObject() {
        GameManager.GAME_MANAGER.nicknameManager.DeletePlayer(gameObject);
    }

    [PunRPC]
    void KillHideman(string killerNickname, string victimName) {
        GameManager.GAME_MANAGER.nicknameManager.DeletePlayer(gameObject);

        if (gameObject == GameManager.GAME_MANAGER.mainPlayer) {  // this is mine hideman
            Camera.main.GetComponent<CameraController>().SetChasingObject(null);
            GameManager.GAME_MANAGER.uiGame.StartRespawnTimer();
            GameManager.GAME_MANAGER.Invoke("Respawn", Constants.RESPAWN_TIME);

            PhotonNetwork.Destroy(gameObject);
        }

        GameManager.GAME_MANAGER.uiGame.PrintInChat(killerNickname + " убил " + victimName);
    }
}
