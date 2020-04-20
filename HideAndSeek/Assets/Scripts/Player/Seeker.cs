using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Seeker : PlayerController {

    protected new void Start() {
        base.Start();

        moveSpeed *= 1.5f; // seeker moves faster
    }

    protected void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == Constants.HIDEMAN_TAG) {
            print("BOOM");

            if (PhotonNetwork.IsMasterClient) {
                PhotonView pv = collision.gameObject.GetComponent<PhotonView>();
                pv.RPC("KillHideman", RpcTarget.All, "" + pv.ViewID, PhotonNetwork.NickName);
            }
        }
    }
}
