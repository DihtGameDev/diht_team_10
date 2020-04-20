using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    private Rigidbody _rb;

    [SerializeField]
    private float _lerpAngleCoeff = 0.08f;

    [SerializeField]
    private Joystick _moveJoystick;

    public float moveSpeed = 2f;

    protected void Awake() {
        StartCoroutine("TrySetJoystick");
    }

    protected void Start() {
        _rb = GetComponent<Rigidbody>();

     /*   if (_moveJoystick == null) {
            Debug.Log("Needs FixedJoystick on this scene");
            throw new UnassignedReferenceException();
        }*/
    }

    private IEnumerator TrySetJoystick() {
        do {
            _moveJoystick = GameObject.Find("Canvas")
                        .GetComponent<Canvas>()
                        .GetComponentInChildren<Joystick>();
            yield return new WaitForSeconds(0.5f);
        } while (_moveJoystick == null);
        StartCoroutine("Move");
    }

    private IEnumerator Move() {
        while (true) {
            Vector3 pos = transform.position;
            pos.x += _moveJoystick.Direction.x * moveSpeed * Time.deltaTime;
            pos.z += _moveJoystick.Direction.y * moveSpeed * Time.deltaTime;
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

            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;

            yield return null;
        }
    }
}
