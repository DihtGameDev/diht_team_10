using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class NicknameManager {
    public class PlayerInfo {
        public GameObject gameObject;
        public RectTransform nicknameRectTransform;
        public Text nicknameText;

        public PlayerInfo(GameObject go, RectTransform rt, Text nicknameText) {
            this.gameObject = go;
            this.nicknameRectTransform = rt;
            this.nicknameText = nicknameText;
        }
    }

    private LinkedList<PlayerInfo> _players = new LinkedList<PlayerInfo>();
    private RectTransform _canvasRect;

    public void InitPlayers(GameObject[] players, GameObject canvasGO, GameObject nicknamePrefab) {
        for (int i = 0; i < players.Length; ++i) {
            Text __nicknameText = GameObject.Instantiate(nicknamePrefab, canvasGO.transform).GetComponent<Text>();
            RectTransform __rt = __nicknameText.GetComponent<RectTransform>();
            __nicknameText.text = "bot_" + i;
            _players.AddLast(new PlayerInfo(players[i], __rt, __nicknameText));
        }

        _canvasRect = canvasGO.GetComponent<RectTransform>();
    }

    public void Update() {
        foreach (var player in _players) {
            Vector2 __viewportPosition = Camera.main.WorldToViewportPoint(player.gameObject.transform.position);
            Vector2 __worldObject_ScreenPosition = new Vector2(
                ((__viewportPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 0.5f)),
                ((__viewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f))
                );

            player.nicknameRectTransform.anchoredPosition = __worldObject_ScreenPosition;
        }
    }
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    private Rigidbody _rb;

    [SerializeField]
    private GameObject nicknameTextPrefab;

    [SerializeField]
    private float _lerpAngleCoeff = 0.08f;

    private Canvas _canvas;

    public float moveSpeed = 0.06f;

    public Joystick _moveJoystick;

    private NicknameManager _nicknameManager = new NicknameManager();

    List<RectTransform> _nicknamesRectTransforms = new List<RectTransform>();
    RectTransform _canvasRect;
    GameObject[] _players;

    private void Awake() {
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _moveJoystick = _canvas.GetComponentInChildren<Joystick>();
    }

    private void Start() {
        _rb = GetComponent<Rigidbody>();

        if (_moveJoystick == null) {
            Debug.Log("Needs FixedJoystick on this scene");
            throw new UnassignedReferenceException();
        }

        _nicknameManager.InitPlayers(
            GameObject.FindGameObjectsWithTag("Player"),
            _canvas.gameObject,
            nicknameTextPrefab
        );
    }

    private void Update() {
        print("Update: " + transform.position);
        _Move();
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        _nicknameManager.Update();
    }

    private void _Move() {
        Vector3 pos = transform.position;
        pos.x += _moveJoystick.Direction.x * moveSpeed;
        pos.z += _moveJoystick.Direction.y * moveSpeed;
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
