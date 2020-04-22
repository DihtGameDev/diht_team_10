using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Seeker : PlayerController {

    private Animator _animator;
    private Vector3 _prevPos;

    protected new void Start() {
        base.Start();
        _animator = GetComponentInChildren<Animator>();

        _prevPos = transform.position;

        moveSpeed *= 2f; // seeker moves faster
    }

    protected new void Update() {
        float __deltaMagnitude = (transform.position - _prevPos).magnitude;
        _animator.SetBool("IsMoving", __deltaMagnitude != 0f);
        _prevPos = transform.position;
    }

    protected void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == Constants.HIDEMAN_TAG) {
            if (PhotonNetwork.IsMasterClient) {
                PhotonView pv = other.gameObject.GetComponent<PhotonView>();
                pv.RPC("KillHideman", RpcTarget.All, "" + pv.ViewID, PhotonNetwork.NickName); // we send all users that user with this viewId is die
            }
        }
    }
}
