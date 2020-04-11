using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks {
    protected void Awake() {
        // master will synchronize rooms
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Connect() {
        if (PhotonNetwork.IsConnected) {
            print("Connect(): isConnected");
            PhotonNetwork.JoinRandomRoom();
        } else {
            print("Connect(), first connecting");
            PhotonNetwork.GameVersion = "1";
            PhotonNetwork.NickName = Settings.getInstance().nickname;
            PhotonNetwork.ConnectUsingSettings(); // first connecting
        }
    }

    public override void OnConnectedToMaster() {
        print("OnConnectedToMaster(), before JoinRandomRoom");
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnDisconnected(DisconnectCause cause) {
        print("onDisconnected() with reason: " + cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        print("OnJoinRandomFailed(). Therefore we create new Room");
        // we failed to join a random room, maybe none exists or they are all full. Therefore we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 3 });
    }

    public override void OnJoinedRoom() {
        print("OnJoinedRoom(): " + PhotonNetwork.NickName);

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

        print("load the Room for 3");
        PhotonNetwork.LoadLevel("Room for 3");
    }
}
