using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Hideman : PlayerController {

    /* protected void OnCollisionEnter(Collision collision) {
         if (collision.gameObject.tag == Constants.PLAYER_TAG) {
             print("click-clack");
             PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
             print(pc.Type());
         }
     }*/

    [PunRPC]
    void DestroyObject(string viewId) {
        foreach (var __player in GameObject.FindGameObjectsWithTag(Constants.HIDEMAN_TAG)) {
            if (__player.GetComponent<PhotonView>().ViewID == System.Int32.Parse(viewId)) {
                //          _nicknameManager.DeletePlayer(player);
                Destroy(__player);
            }
        }
    }
}
