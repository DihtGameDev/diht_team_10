using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Hideman : MonoBehaviour {
    void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.gameObject.tag == Constants.ARCH_TAG) {  // i am winner
            GameManager.instance.isWinner = true;
            GameManager.instance.Leave();
        }
    }
}
