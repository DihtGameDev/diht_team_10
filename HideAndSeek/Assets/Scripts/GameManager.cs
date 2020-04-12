using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks {
    public GameObject sunObj;
    private GameObject _myPlayer;
    private UIGame uiGame;

    public void Start() {
        uiGame = new UIGame(GameObject.Find("Canvas"));

        uiGame.moveJoystick.gameObject.SetActive(false);

        if (!PhotonNetwork.IsConnected) {
            Debug.Log("Something went wrong, while loading this scene");
            SceneManager.LoadScene("Launcher");
        }

        if (PhotonNetwork.IsMasterClient) {
            Debug.Log("I am master client");

            sunObj.SetActive(false);

            _myPlayer = PhotonNetwork.Instantiate("Seeker", new Vector3(1f, 0f, 1f), Quaternion.identity, 0);
        } else {
            Debug.Log("I am not master client");

            _myPlayer = PhotonNetwork.Instantiate("Hideman",
                new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f)),
                Quaternion.identity, 0);
        }

        _myPlayer.name = "MyPlayer";

        Invoke("StartGame", 5f);
    }

    private void StartGame() {
        uiGame.loadingScreenGO.SetActive(false);
        uiGame.moveJoystick.gameObject.SetActive(true);

        PlayerController pc = _myPlayer.AddComponent<PlayerController>();

        if (PhotonNetwork.IsMasterClient) { // i am seeker
            pc.moveSpeed = pc.moveSpeed * 2f;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log("Player entered: " + newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        Debug.Log("OnPlayerLeftRoom(): " + otherPlayer.NickName);
        if (PhotonNetwork.IsMasterClient) {
            Debug.Log("now I am master!");
        }
    }
}
