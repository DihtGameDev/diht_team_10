using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Seeker : BaseAnimationPlayer {
    void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.gameObject.tag == Constants.HIDEMAN_TAG) {
            print("HIT with: " + hit.gameObject.name);
            OnHidemanHitted(hit.gameObject);
        } else if (hit.gameObject.tag == Constants.SKELETON_TAG) {
            print("HIT with: " + hit.gameObject.name);
            OnSkeletonHitted(hit.gameObject);
        } else {
            return;
        }

        BecomeSkeleton();
    }

    private void OnHidemanHitted(GameObject hidemanObj) {
        print("hideman hitted");
        PhotonView __pv = hidemanObj.GetComponent<PhotonView>();  // send message on this photonview hit.gameObject object, i.e. same photon view
        __pv.RPC("KillHideman", RpcTarget.All, PhotonNetwork.NickName, __pv.Owner.NickName); // we send all users that user with this nickname killed by this user
    }

    private void OnSkeletonHitted(GameObject skeletonObj) {
        PhotonView __pv = skeletonObj.GetComponent<PhotonView>();
        __pv.RPC("DestroyLyingSkeleton", RpcTarget.All, PhotonNetwork.NickName);  // master will destroy skeleton, other print in chat about capture
    }

    private void BecomeSkeleton() {
        Camera.main.GetComponent<CameraController>().SetChasingObject(null);
        GameManager.GAME_MANAGER.StartCoroutine(GameManager.GAME_MANAGER.BecomeSkeleton(transform.position));
        GetComponent<PhotonView>().RPC("DestroyThisPUNObject", RpcTarget.All); // we send all players that this object they should destroy nickname for this object
        PhotonNetwork.SendAllOutgoingCommands();  // for instant sending previous message, otherwise players will reference to null
        PhotonNetwork.Destroy(gameObject);
    }
}
