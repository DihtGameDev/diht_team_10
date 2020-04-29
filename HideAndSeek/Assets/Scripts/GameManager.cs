using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public enum PlayerType {
    SEEKER, HIDEMAN
}

public class GameManager : MonoBehaviourPunCallbacks {
    [SerializeField]
    private UIGameWidget _uiGameWidget;

    [SerializeField]
    private GameObject _nicknameTextPrefab;

    [SerializeField]
    private GameObject _fovPrefab;

    public static GameManager GAME_MANAGER;

    public GameObject sunObj;
    public UIGame uiGame;

    public GameObject mainPlayer;
    public PlayerType playerType;
    public NicknameManager nicknameManager;

    public PlayerData playerData;

    protected void Awake() {
        GAME_MANAGER = this;
    }

    protected void Start() {
        uiGame = new UIGame(_uiGameWidget);

        uiGame.moveJoystick.gameObject.SetActive(false);
        nicknameManager = NicknameManager.GetInstance(_uiGameWidget.gameObject, _nicknameTextPrefab);

        if (!PhotonNetwork.IsConnected) {
            Debug.Log("Something went wrong, while loading this scene");
            SceneManager.LoadScene("Launcher");
        }

        Invoke("StartGame", 1f);
    }

    protected void Update() {
        nicknameManager.Update();
    }

    private void StartGame() {
        Respawn();

        uiGame.loadingScreenGO.SetActive(false);
        uiGame.moveJoystick.gameObject.SetActive(true);

        SetNicknames();
    }

    private void Respawn() {
        playerType = ChooseTeam();
        CreatePlayer(playerType);

        if (playerType == PlayerType.SEEKER) { // i am seeker
            mainPlayer.AddComponent<Seeker>().StartMovement(playerData);
        } else {
            mainPlayer.AddComponent<Hideman>().StartMovement(playerData);
        }

        Object.Instantiate(_fovPrefab, mainPlayer.transform);
        Camera.main.GetComponent<CameraController>().SetChasingObject(mainPlayer);

        SetNicknames();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log("Player entered: " + newPlayer.NickName);

        Invoke("SetNicknames", 5f);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        Debug.Log("OnPlayerLeftRoom(): " + otherPlayer.NickName);
        if (PhotonNetwork.IsMasterClient) {
            Debug.Log("now I am master!");
        }

        foreach (var __player in GameObject.FindGameObjectsWithTag(Constants.HIDEMAN_TAG)) {
            if (__player.GetComponent<PhotonView>().Owner == null) {
                nicknameManager.DeletePlayer(__player);
            }
        }

        foreach (var __player in GameObject.FindGameObjectsWithTag(Constants.SEEKER_TAG)) {
            if (__player.GetComponent<PhotonView>().Owner == null) {
                nicknameManager.DeletePlayer(__player);
            }
        }
        
    }

    public void CreatePlayer(PlayerType playerType) {
        if (playerType == PlayerType.SEEKER) {
       //     sunObj.SetActive(false);
            mainPlayer = PhotonNetwork.Instantiate("SeekerGhost", new Vector3(1f, 0f, 1f), Quaternion.identity, 0);
        } else {
            mainPlayer = PhotonNetwork.Instantiate("FixedHideman",
                new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f)),
                Quaternion.identity, 0);
        }

        mainPlayer.name = "MyPlayer";
    }

    public PlayerType ChooseTeam() {
        int __seekersCount = GameObject.FindGameObjectsWithTag(Constants.SEEKER_TAG).Length;

        if (__seekersCount == 0) {
            return PlayerType.SEEKER;
        }

        int __hidemansCount = GameObject.FindGameObjectsWithTag(Constants.HIDEMAN_TAG).Length;
        int __coeff = __hidemansCount / __seekersCount;

        return __coeff <= 2 ? PlayerType.HIDEMAN : PlayerType.SEEKER;
    }

    private void SetNicknames() {
        var __allies = GameObject.FindGameObjectsWithTag(
            mainPlayer.tag == Constants.SEEKER_TAG ?
            Constants.SEEKER_TAG :
            Constants.HIDEMAN_TAG
            );

        foreach (var __player in __allies) {
            nicknameManager.AddPlayer(__player, __player.GetComponent<PhotonView>().Owner.NickName);
        }
    }
}
