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

    [HideInInspector]
    public bool isWinner = false;

    [HideInInspector]
    public GameObject mainPlayer;

    [HideInInspector]
    public NicknameManager nicknameManager;

    public PlayerData seekerPlayerData;
    public PlayerData hidemanPlayerData;

    public AbstractAbility ability;

    public AbilitiesDict seekerAbilitiesDict;  // setted in inspector
    public AbilitiesDict hidemanAbilitiesDict;  // setted in inspector

    public CameraController cameraController; // for setting chasing obj from anywhere

    [HideInInspector]
    public Controller playerController; // optimization for setting additional moveSpeed from anywhere
    [HideInInspector]
    public PhotonView photonViewMainPlayer; // --||--

    protected void Awake() {
        GAME_MANAGER = this;
    }

    protected void Start() {
        uiGame = new UIGame(_uiGameWidget);

        uiGame.LoadingState();

        nicknameManager = NicknameManager.GetInstance(_uiGameWidget.gameObject, _nicknameTextPrefab);

        if (!PhotonNetwork.IsConnected) {
            Debugger.Log("Something went wrong, while loading this scene. Loading Launcher scene");
            SceneManager.LoadScene("Launcher");
        }

        Invoke("StartGame", 1f);

        UpdateAnalytics();
    }

    protected void Update() {
        nicknameManager.UpdatePositions();
    }

    private void StartGame() {
          if (PhotonNetwork.IsMasterClient) {
              StartCoroutine(CheckAndSpawnSkeletons(Constants.MAX_SKELETONS_IN_SCENE, Constants.SPAWN_SKELETONS_DELAY));
          }

        Respawn();

        uiGame.PlayingState();
        uiGame.UpdatePlayerCounter();
    }

    private void Respawn() {  // respawn after death
        Debugger.Log("GameManager, Respawn");
        CreatePlayer(PlayerType.SEEKER, Vector3.zero);

        if (archObject != null) {
            archObject.SetActive(false);
        }

        StartCoroutine(SetNicknames(PlayerType.SEEKER, 0.2f));
    }

    private void UpdateAnalytics() {
        FirebaseController.instance.IncrementAnalyticsData(AnalyticsType.START_PLAY);
        StartCoroutine(Misc.WaitAndDo(
            120f, // 120 sec in 2 minutes
            () => {
                FirebaseController.instance.IncrementAnalyticsData(AnalyticsType.PLAYED_MORE_2_MINS);
            }));
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
        uiGame.UpdatePlayerCounter();
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

        uiGame.UpdatePlayerCounter();
    }

    public void CreatePlayer(PlayerType playerType, Vector3 spawnPos) {  // =(
        if (playerType == PlayerType.SEEKER) {
            mainPlayer = PhotonNetwork.Instantiate("SeekerGhost", spawnPos, Quaternion.identity, 0);
            FieldOfView fov = Object.Instantiate(_fovPrefab, mainPlayer.transform).GetComponent<FieldOfView>();
            playerController = mainPlayer.AddComponent<Controller>();

            mainPlayer.AddComponent<Seeker>();
            playerController.StartMovement(seekerPlayerData);
            SetFovSettings(fov, seekerPlayerData);

            ability = seekerAbilitiesDict.Get(Settings.getInstance().seekerAbility);
        } else {
            mainPlayer = PhotonNetwork.Instantiate("Skeleton", spawnPos, Quaternion.identity, 0);
            FieldOfView fov = Object.Instantiate(_fovPrefab, mainPlayer.transform).GetComponent<FieldOfView>();
            playerController = mainPlayer.AddComponent<Controller>();

            mainPlayer.AddComponent<Hideman>();
            playerController.StartMovement(hidemanPlayerData);
            SetFovSettings(fov, hidemanPlayerData);

            ability = hidemanAbilitiesDict.Get(Settings.getInstance().hidemanAbility);
        }


        //  otherwise the player starts blinking when there are several fov on the scene
        mainPlayer.layer = 0;
        mainPlayer.GetComponent<FogCoverable>().enabled = false;

        photonViewMainPlayer = mainPlayer.GetComponent<PhotonView>();

        cameraController.SetChasingObject(mainPlayer);

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

    public void UseAbility() {
        uiGame.abilityBtn.interactable = false;
        ability.UseAbility();
        StartCoroutine(UnlockAbility(ability.Cooldown()));
    }

    public IEnumerator UnlockAbility(float delay) {
        yield return new WaitForSeconds(delay);
        uiGame.abilityBtn.interactable = true;
    }

    [PunRPC]
    private void OnPunPlayerLeave(string nickname, bool isWinner) {
        uiGame.PrintInChat(nickname + (isWinner ? " освободился" : " покинул игру"));
    }
}
