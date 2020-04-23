using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private Joystick _moveJoystick;

    private CharacterController _controller;

    public PlayerData playerData;

    protected void Awake() {
    }

    protected void Start() {
        _controller = GetComponent<CharacterController>();
    }

    public void StartMovement(PlayerData playerData) { // =(
        this.playerData = playerData;
        StartCoroutine("TrySetJoystick");
    }

    private IEnumerator TrySetJoystick() {
        do {
            print("I'll try to set joystick");
            _moveJoystick = GameObject.Find("Canvas")
                        .GetComponent<Canvas>()
                        .GetComponentInChildren<Joystick>();
            yield return new WaitForSeconds(0.5f);
        } while (_moveJoystick == null);
        StartCoroutine("Move");
    }
    
    private IEnumerator Move() {
        while (true) {
            Vector3 __dir = Vector3.zero;
            __dir.x = _moveJoystick.Direction.x;
            __dir.z = _moveJoystick.Direction.y;
            _controller.Move(__dir * playerData.moveSpeed * Time.deltaTime);

            if (_moveJoystick.Direction != Vector2.zero) {
                float __angle = Mathf.Rad2Deg * Mathf.Atan(_moveJoystick.Direction.x / _moveJoystick.Direction.y);
                if (_moveJoystick.Direction.y < 0) {
                    __angle -= 180f;
                }

                transform.eulerAngles = new Vector3(0,
                                                    Mathf.LerpAngle(transform.eulerAngles.y, __angle, playerData.lerpAngleCoeff),
                                                    0);
            }

            yield return null;
        }
    }
}
