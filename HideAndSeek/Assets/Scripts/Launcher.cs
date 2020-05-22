using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks {
    [SerializeField]
    private UILauncherWidget _uiLauncherWidget;

    private UILauncher _launcherUI;

    public static Launcher LAUNCHER;

    protected void Awake() {
        LAUNCHER = this;

        Debugger.Log("Launcher, Awake");
    }

    protected void Start() {
        // master will synchronize rooms
        PhotonNetwork.AutomaticallySyncScene = true;

        _launcherUI = new UILauncher(_uiLauncherWidget);
        _launcherUI.SetScreen(UILauncher.Screen.INIT);

        CheckAndSetUserIdWithFirebase();
    }

    protected void Update() {
    }

    public void Connect() {
        SaveNickname(_launcherUI.initScreen.nicknameField.text);
        _launcherUI.initScreen.ConnectingState();

        // connecting to a photon server
        if (PhotonNetwork.IsConnected) {  // if for some reason i'm alreader connected
            PhotonNetwork.JoinRoom("test");
        } else {  // first connecting
            PhotonNetwork.GameVersion = "1";
            PhotonNetwork.NickName = Settings.getInstance().nickname;
            PhotonNetwork.ConnectUsingSettings(); // connect to photon server
        }
    }

    private void CheckAndSetUserIdWithFirebase() {
        Settings settings = Settings.getInstance();
        
        if (settings.firebaseUserId == "") {
            settings.firebaseUserId = RandomStringGenerator.Generate(Constants.USER_ID_LENGTH);
            settings.save();
        }
        StartCoroutine(Misc.WaitWhile(
            () => { return FirebaseController.instance.isReady == false; },
            () => { FirebaseController.instance.SetFirebaseGameDataToPlayerPrefs(settings.firebaseUserId); }
        ));
    }

    private void SaveNickname(string nickname) {
        Settings.getInstance().nickname = nickname;
        Settings.getInstance().save();
    }

    public override void OnConnectedToMaster() {
        Debugger.Log("OnConnectedToMaster(), before JoinRandomRoom");

        PhotonNetwork.CreateRoom("test", new RoomOptions { MaxPlayers = 5 });
    }
    public override void OnDisconnected(DisconnectCause cause) {
        Debugger.Log("onDisconnected() with reason: " + cause);
    }

    public override void OnCreatedRoom() {
        Debugger.Log("OnCreatedRoom");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debugger.Log("OnCreateRoomFailed with message: " + message);
        PhotonNetwork.JoinRoom("test");
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debugger.Log("OnJoinRandomFailed(). mith message: " + message);
        Debugger.Log("Therefore we create new Room");
        // we failed to join a random room, maybe none exists or they are all full. Therefore we create a new room.
        PhotonNetwork.CreateRoom("test", new RoomOptions { MaxPlayers = 5 });
    }

    public override void OnJoinedRoom() {
        Debugger.Log("OnJoinedRoom(): " + PhotonNetwork.NickName);
        Debugger.Log("Launcher, Before LoadScene");
        if (PhotonNetwork.IsMasterClient) {  // if i am master, i load scene, otherwise, another master will do it for me
            ReadyState readyState = FirebaseController.instance.CheckAndResetAbilities();
            StartCoroutine(Misc.WaitWhile(
                () => { return readyState.isReady == false; },
                () => {
                    Debugger.Log("load the game scene");
                    SceneLoader.LoadSceneOnce(Constants.SceneName.GAME_SCENE);
                }
            ));
            DialogManager.instance.ShowDialog(DialogType.LOADING, () => { return readyState.isReady == false; });
        }
    }

    public void ChangeScreen(UILauncher.Screen screen) {
        _launcherUI.SetScreen(screen);
    }
}
