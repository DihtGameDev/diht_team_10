using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SkeletonRPC : MonoBehaviour {
    [PunRPC]
    void DestroyLyingSkeleton(string killerNickname) {
        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.Destroy(gameObject);
        }
        GameManager.GAME_MANAGER.uiGame.PrintInChat(killerNickname + " захватил тело");
    }
}
