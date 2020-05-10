using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimationPlayer : MonoBehaviour {
    private Animator _animator;
    private Vector3 _prevPos;

    private int _animIsMovingId;

    protected void Start() {
        _animator = GetComponentInChildren<Animator>();

        _prevPos = transform.position;
        _animIsMovingId = Animator.StringToHash("IsMoving");
    }

    protected void Update() {
        float __deltaMagnitude = (transform.position - _prevPos).magnitude;
        _animator.SetBool(_animIsMovingId, __deltaMagnitude != 0f);
        _prevPos = transform.position;
    }
}
