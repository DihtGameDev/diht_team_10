using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks {
    private UILauncher ui;

    protected void Awake() {
        // master will synchronize rooms
        PhotonNetwork.AutomaticallySyncScene = true;

        ui = new UILauncher(GameObject.Find("Canvas"));
        ui.nicknameField.text = Settings.getInstance().nickname;
        ui.quickplayBtn.onClick.AddListener(Connect);
    }

    public void Connect() {
        if (PhotonNetwork.IsConnected) {
            Debug.Log("Connect(): isConnected");
            PhotonNetwork.JoinRandomRoom();
        } else {
            Debug.Log("Connect(), first connecting");
            PhotonNetwork.GameVersion = "1";
            PhotonNetwork.NickName = Settings.getInstance().nickname;
            PhotonNetwork.ConnectUsingSettings(); // first connecting
        }
    }

    public override void OnConnectedToMaster() {
        Debug.Log("OnConnectedToMaster(), before JoinRandomRoom");
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log("onDisconnected() with reason: " + cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("OnJoinRandomFailed(). Therefore we create new Room");
        // we failed to join a random room, maybe none exists or they are all full. Therefore we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 3 });
    }

    public override void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom(): " + PhotonNetwork.NickName);

        /*
        if (PhotonNetwork.IsMasterClient) {
            print("OnJoinderRoom(), I am master and I will load level for all");
            PhotonNetwork.LoadLevel("Room for 2");
        } */

        if (PhotonNetwork.IsMasterClient) {
            StartCoroutine(Wait());
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
