using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Controller : MonoBehaviour {

    [SerializeField]
    private Joystick _moveJoystick;

    private CharacterController _controller;

    private float additionalMoveSpeed;

    public PlayerData playerData;

    public float movementAngle = 90f;

    protected void Awake() {
    }

    protected void Start() {
        _controller = GetComponent<CharacterController>();
    }

    public void StartMovement(PlayerData playerData) { // =(
        this.playerData = playerData;
        StartCoroutine(TrySetJoystick(0.5f));
    }

    private IEnumerator TrySetJoystick(float delay) {
        do {
            _moveJoystick = GameObject.Find("Canvas")
                        .GetComponentInChildren<Joystick>();
            yield return new WaitForSeconds(delay);
        } while (_moveJoystick == null);
        StartCoroutine("Move");
    }
    
    private IEnumerator Move() {
        while (true) {
            if (_moveJoystick.Direction != Vector2.zero) {
                float __angle = Mathf.Rad2Deg * Mathf.Atan(_moveJoystick.Direction.x / _moveJoystick.Direction.y);
                if (_moveJoystick.Direction.y < 0) {
                    __angle -= 180f;
                }

                transform.eulerAngles = new Vector3(0,
                                                    Mathf.LerpAngle(transform.eulerAngles.y, __angle, Time.deltaTime * playerData.lerpAngleCoeff),
                                                    0);
                Vector3 __dir = Vector3.zero;
                __dir.x = _moveJoystick.Direction.x;
                __dir.z = _moveJoystick.Direction.y;
                _controller.Move(__dir * (playerData.moveSpeed + additionalMoveSpeed)* Time.deltaTime);
            }

            yield return null;
        }
    }

    public void SetAdditionalMoveSpeed(float additionalMoveSpeed) {
        this.additionalMoveSpeed = additionalMoveSpeed; 
    }
}
