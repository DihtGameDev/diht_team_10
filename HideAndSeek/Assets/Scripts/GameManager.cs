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

    [SerializeField]
    private GameObject _archPrefab;

    public static GameManager GAME_MANAGER;

    public ObjectsPool objectsPool = new ObjectsPool();

    public UIGame uiGame;

    public GameObject archObject;

    public bool isWinner = false;

    [HideInInspector]
    public GameObject mainPlayer;

    public NicknameManager nicknameManager;

    public PlayerData seekerPlayerData;
    public PlayerData hidemanPlayerData;

    protected void Awake() {
        GAME_MANAGER = this;
    }

    protected void Start() {
        uiGame = new UIGame(_uiGameWidget);

        uiGame.loadingScreenGO.SetActive(true);  //  to do using states
        uiGame.playerCounterText.gameObject.SetActive(false);
        uiGame.moveJoystick.gameObject.SetActive(false);
        nicknameManager = NicknameManager.GetInstance(_uiGameWidget.gameObject, _nicknameTextPrefab);

        if (!PhotonNetwork.IsConnected) {
            Debugger.Log("Something went wrong, while loading this scene");
            SceneManager.LoadScene("Launcher");
        }
        Invoke("StartGame", 1f);
    }

    protected void Update() {
        nicknameManager.UpdatePositions();
    }

    private void StartGame() {
        Debugger.Log("GameManager, StartGame begin");
          if (PhotonNetwork.IsMasterClient) {
              StartCoroutine(CheckAndSpawnSkeletons(Constants.MAX_SKELETONS_IN_SCENE, Constants.SPAWN_SKELETONS_DELAY));
          }

        Respawn();

        uiGame.loadingScreenGO.SetActive(false);
        uiGame.moveJoystick.gameObject.SetActive(true);
        uiGame.playerCounterText.gameObject.SetActive(true);

        uiGame.StartPlayerCounter();

        Debugger.Log("GameManager, StartGame end");
    }

    private void Respawn() {  // respawn after death
        Debugger.Log("GameManager, Respawn");
        CreatePlayer(PlayerType.SEEKER, Vector3.zero);

        if (archObject != null) {
            archObject.SetActive(false);
        }

        StartCoroutine(SetNicknames(PlayerType.SEEKER, 0.2f));
    }

    public IEnumerator BecomeSkeleton(Vector3 spawnPos) {
        Debugger.Log("GameManager, BecomeSkeleton");
        yield return new WaitForSeconds(0.3f);
        CreatePlayer(PlayerType.HIDEMAN, spawnPos);

        StartCoroutine(SetNicknames(PlayerType.HIDEMAN, 0.2f));

        if (archObject == null) {
            int randIndex = Random.Range(0, Constants.GRAVE_SPAWN_POSITIONS.Length);
            archObject = Instantiate(_archPrefab, Constants.GRAVE_SPAWN_POSITIONS[randIndex], Quaternion.identity);
        } else {
            archObject.SetActive(true);
        }

        yield return null;
    }

    private void SetFovSettings(FieldOfView fov, PlayerData playerData) {
        fov.viewRadius = playerData.viewRadius;
        fov.viewAngle = playerData.viewAngle;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        GameManager.GAME_MANAGER.uiGame.PrintInChat(newPlayer.NickName + " начал играть");

        StartCoroutine(SetNicknames(mainPlayer.tag == Constants.SEEKER_TAG ? PlayerType.SEEKER : PlayerType.HIDEMAN, 3f));
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
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

        if (PhotonNetwork.IsMasterClient) {
            StartCoroutine(CheckAndSpawnSkeletons(Constants.MAX_SKELETONS_IN_SCENE, Constants.SPAWN_SKELETONS_DELAY));
        }
    }

    public void CreatePlayer(PlayerType playerType, Vector3 spawnPos) {  // =(
        if (playerType == PlayerType.SEEKER) {
            mainPlayer = PhotonNetwork.Instantiate("SeekerGhost", spawnPos, Quaternion.identity, 0);
            FieldOfView fov = Object.Instantiate(_fovPrefab, mainPlayer.transform).GetComponent<FieldOfView>();
            Controller playerController = mainPlayer.AddComponent<Controller>();

            mainPlayer.AddComponent<Seeker>();
            playerController.StartMovement(seekerPlayerData);
            SetFovSettings(fov, seekerPlayerData);
        } else {
            mainPlayer = PhotonNetwork.Instantiate("Skeleton", spawnPos, Quaternion.identity, 0);
            FieldOfView fov = Object.Instantiate(_fovPrefab, mainPlayer.transform).GetComponent<FieldOfView>();
            Controller playerController = mainPlayer.AddComponent<Controller>();

            mainPlayer.AddComponent<Hideman>();
            playerController.StartMovement(hidemanPlayerData);
            SetFovSettings(fov, hidemanPlayerData);
        }

        Camera.main.GetComponent<CameraController>().SetChasingObject(mainPlayer);

        mainPlayer.name = "MyPlayer";
    }

    private IEnumerator SetNicknames(PlayerType myType, float delay) {
        yield return new WaitForSeconds(delay);

        var __allies = GameObject.FindGameObjectsWithTag(
            myType == PlayerType.SEEKER ?
            Constants.SEEKER_TAG :
            Constants.HIDEMAN_TAG
            );

        nicknameManager.AddAllPlayers(__allies);
    }

    public int PlayersInTheScene() {
        return
            PhotonNetwork.CurrentRoom.PlayerCount;
    }

    private IEnumerator CheckAndSpawnSkeletons(int maxSkeletons, float delay) {  // this function require that free graves count more than maxSkeletons
        yield return new WaitForSeconds(delay);
        Debugger.Log("GameManager, CheckAndSpawnSkeletons");

        if (!PhotonNetwork.IsMasterClient) {
            yield break;
        }

        int missingSkeletonsNum =
            maxSkeletons - GameObject.FindGameObjectsWithTag(Constants.SKELETON_TAG).Length + GameObject.FindGameObjectsWithTag(Constants.HIDEMAN_TAG).Length;

        if (missingSkeletonsNum == 0 || !PhotonNetwork.IsMasterClient) {
            yield return null;
        }

        GameObject[] graves = GameObject.FindGameObjectsWithTag(Constants.GRAVE_TAG);

        for (int i = 0; i < missingSkeletonsNum; ++i) {
            int randomIndex;
            do {
                randomIndex = Random.Range(0, graves.Length);
                if (graves[randomIndex] != null) {
                    PhotonNetwork.InstantiateSceneObject("LyingSkeleton", graves[randomIndex].transform.position, Quaternion.identity, 0);
                    graves[randomIndex].tag = Constants.OCCUPIED_GRAVE_TAG;
                }
            } while (graves[randomIndex] == null);  // searching free grave
            graves[randomIndex] = null;  // mark that this grave is occupied
        }

        yield return null;
    }

    public override void OnDisconnected(DisconnectCause cause) {
        Debugger.Log("GameManager, OnDisconnect");
        nicknameManager.Clear();
        SceneManager.LoadScene("Launcher");
    }

    public void Leave() {
        Debugger.Log("GameManager, Leave");

        PhotonView __pv = GetComponent<PhotonView>();
        __pv.RPC("OnPunPlayerLeave", RpcTarget.All, PhotonNetwork.NickName, isWinner);  // send all users, that i am leaving the game
        PhotonNetwork.SendAllOutgoingCommands();  // because rpc call has delay and after disconnect, message will not be sent
        PhotonNetwork.Disconnect();  // leaves the room and photon server 
    }

    [PunRPC]
    private void OnPunPlayerLeave(string nickname, bool isWinner) {
        uiGame.PrintInChat(nickname + (isWinner ? " освободился" : " покинул игру"));
    }
}
