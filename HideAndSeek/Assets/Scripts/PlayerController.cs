using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    private Rigidbody _rb;

    [SerializeField]
    private float _lerpAngleCoeff = 0.08f;

    public float moveSpeed = 0.03f;

    public Joystick _moveJoystick;

    private void Start() {
        _rb = GetComponent<Rigidbody>();
        _moveJoystick = GameObject.Find("Canvas").GetComponentInChildren<Joystick>();

        if (_moveJoystick == null) {
            print("Needs FixedJoystick on this scene");
            throw new UnassignedReferenceException();
        }
    }

    private void Update() {
        _Move();
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    private void _Move() {
        Vector3 pos = transform.position;
        pos.x += _moveJoystick.Direction.x * 0.03f;
        pos.z += _moveJoystick.Direction.y * 0.03f;
        transform.position = pos;
        if (_moveJoystick.Direction != Vector2.zero) {
            float angle = Mathf.Rad2Deg * Mathf.Atan(_moveJoystick.Direction.x / _moveJoystick.Direction.y);
            if (_moveJoystick.Direction.y < 0) {
                angle -= 180f;
            }

            transform.eulerAngles = new Vector3(0,
                                                Mathf.LerpAngle(transform.eulerAngles.y, angle + 180f, _lerpAngleCoeff),
                                                0);
        }
    }
}
