using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Seeker : PlayerController {

    private Animator _animator;
    private Vector3 _prevPos;

    private int _animIsMovingId;

    protected new void Start() {
        base.Start();
        _animator = GetComponentInChildren<Animator>();

        _prevPos = transform.position;
        _animIsMovingId = Animator.StringToHash("IsMoving");
    }

    protected new void Update() {
        float __deltaMagnitude = (transform.position - _prevPos).magnitude;
        _animator.SetBool(_animIsMovingId, __deltaMagnitude != 0f);
        _prevPos = transform.position;
    }

    protected void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == Constants.HIDEMAN_TAG) {
            if (PhotonNetwork.IsMasterClient) {
                PhotonView __pv = other.gameObject.GetComponent<PhotonView>();
                __pv.RPC("KillHideman", RpcTarget.All, "" + __pv.ViewID, PhotonNetwork.NickName); // we send all users that user with this viewId is die
            }
        }
    }

    public new void StartMovement(PlayerData playerData) {
        base.StartMovement(playerData);
    }
}
