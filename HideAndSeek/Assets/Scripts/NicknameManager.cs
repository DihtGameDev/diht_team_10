using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NicknameManager {
    private Dictionary<GameObject, RectTransform> _playersDict = new Dictionary<GameObject, RectTransform>();
    private RectTransform _canvasRect;
    private GameObject _nicknamePrefab;

    public const string NICKNAME_TYPE = "nickname";

    private NicknameManager(GameObject canvasGO, GameObject nicknamePrefab) {
        _canvasRect = canvasGO.GetComponent<RectTransform>();
        _nicknamePrefab = nicknamePrefab;
    }

    public static NicknameManager GetInstance(GameObject canvasGO, GameObject nicknamePrefab) {
        return new NicknameManager(canvasGO, nicknamePrefab);
    }

    public void UpdatePositions() {
        foreach (var player in _playersDict) {
            Vector2 __viewportPosition = Camera.main.WorldToViewportPoint(player.Key.transform.position);
            Vector2 __worldObject_ScreenPosition = new Vector2(
                ((__viewportPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 0.5f)),
                ((__viewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f))
                );

            __worldObject_ScreenPosition.y += 55f; // for above player position
            player.Value.anchoredPosition = __worldObject_ScreenPosition;
        }
    }

    public void SetNickname(GameObject player, string nickname) {
        _playersDict[player].GetComponent<Text>().text = nickname;
    }

    public void AddPlayer(GameObject player, string nickname="bot") {
        PhotonView pv = player.GetComponent<PhotonView>();
        if (pv != null) {
            nickname = player.GetComponent<PhotonView>().Owner.NickName;
        }

        if (_playersDict.ContainsKey(player)) {
            return;
        }

        GameObject nicknameObject = GameManager.GAME_MANAGER.objectsPool.Get(NICKNAME_TYPE);

        if (nicknameObject == null) {
            nicknameObject = GameObject.Instantiate(_nicknamePrefab, _canvasRect.transform);
        }

        _playersDict.Add(player, nicknameObject.GetComponent<RectTransform>());
        SetNickname(player, nickname);
    }
        
    public void DeletePlayer(GameObject player) {
        if (_playersDict.ContainsKey(player)) {
            if (_playersDict[player].gameObject != null) {
                GameManager.GAME_MANAGER.objectsPool.Add(NICKNAME_TYPE, _playersDict[player].gameObject);
            }
            _playersDict.Remove(player);
        }
    }

    public void AddAllPlayers(GameObject[] players) {
        Clear();

        foreach (var player in players) {
            AddPlayer(player.gameObject);
        }
    }

    public void Clear() {
        foreach (var player in _playersDict) {
            GameManager.GAME_MANAGER.objectsPool.Add(NICKNAME_TYPE, player.Value.gameObject);
        }
        _playersDict.Clear();
    }
}
