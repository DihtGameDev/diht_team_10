using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks {
    [SerializeField]
    private UILauncherWidget _uiLauncherWidget;

    private UILauncher _launcherUI;

    private bool _connectedToRoom = false;

    protected void Awake() {
        // master will synchronize rooms
        PhotonNetwork.AutomaticallySyncScene = true;

        _launcherUI = new UILauncher(_uiLauncherWidget);
        _launcherUI.nicknameField.text = Settings.getInstance().nickname;
        _launcherUI.quickplayBtn.onClick.AddListener(Connect);
        _launcherUI.connectingMessage.gameObject.SetActive(false);
    }

    protected void Update() {
        /*if (connectedToRoom) {
            ui.connectingMessage.text = "Waiting for players (" + PhotonNetwork.PlayerList.Length + "/2)";
        }*/
    }

    public void Connect() {
        Settings.getInstance().nickname = _launcherUI.nicknameField.text;
        Settings.save();

        _launcherUI.connectingMessage.text = "Connecting...";
        _launcherUI.connectingMessage.gameObject.SetActive(true);
        if (PhotonNetwork.IsConnected) {
            Debug.Log("Connect(): isConnected");
            PhotonNetwork.JoinRoom("test");
        } else {
            Debug.Log("Connect(), first connecting");
            PhotonNetwork.GameVersion = "1";
            PhotonNetwork.NickName = Settings.getInstance().nickname;
            PhotonNetwork.ConnectUsingSettings(); // connect to photon
        }
    }

    public override void OnConnectedToMaster() {
        Debug.Log("OnConnectedToMaster(), before JoinRandomRoom");
        //  PhotonNetwork.JoinRandomRoom();

        PhotonNetwork.CreateRoom("test", new RoomOptions { MaxPlayers = 5 });
      //  PhotonNetwork.JoinRoom("test");
    }
    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log("onDisconnected() with reason: " + cause);
    }

    public override void OnCreatedRoom() {
        print("OnCreatedRoom");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        print("OnCreateRoomFailed with message: " + message);
        PhotonNetwork.JoinRoom("test");
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("OnJoinRandomFailed(). mith message: " + message);
        Debug.Log("Therefore we create new Room");
        // we failed to join a random room, maybe none exists or they are all full. Therefore we create a new room.
        PhotonNetwork.CreateRoom("test", new RoomOptions { MaxPlayers = 5 });
    }

    public override void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom(): " + PhotonNetwork.NickName);
        _connectedToRoom = true;

        /*
        if (PhotonNetwork.IsMasterClient) {
            print("OnJoinderRoom(), I am master and I will load level for all");
            PhotonNetwork.LoadLevel("Room for 2");
        } */

        if (PhotonNetwork.IsMasterClient) {
            Debug.Log("load the Room for 3");
            PhotonNetwork.LoadLevel("Room for 5");
            //  StartCoroutine(Wait());
        }
    }

    private IEnumerator Wait() {
        while (PhotonNetwork.CurrentRoom.PlayerCount != 2) {
            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("load the Room for 3");
        PhotonNetwork.LoadLevel("Room for 3");
    }
}
