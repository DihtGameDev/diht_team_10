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
        GameManager.instance.uiGame.PrintInChatAndClear(killerNickname + " захватил тело");
    }
}
