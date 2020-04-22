using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NicknameManager {
    private Dictionary<GameObject, RectTransform> _playersDict = new Dictionary<GameObject, RectTransform>();
    private RectTransform _canvasRect;
    private GameObject _nicknamePrefab;

    private NicknameManager(GameObject canvasGO, GameObject nicknamePrefab) {
        _canvasRect = canvasGO.GetComponent<RectTransform>();
        _nicknamePrefab = nicknamePrefab;
    }

    public static NicknameManager GetInstance(GameObject canvasGO, GameObject nicknamePrefab) {
        return new NicknameManager(canvasGO, nicknamePrefab);
    }

    public void Update() {
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
        if (_playersDict.ContainsKey(player)) {
            return;
        }

        RectTransform __nicknameRT =
                GameObject.Instantiate(_nicknamePrefab, _canvasRect.transform).GetComponent<RectTransform>();
        _playersDict.Add(player, __nicknameRT);
        SetNickname(player, nickname);
    }

    public void DeletePlayer(GameObject player) {
        if (_playersDict.ContainsKey(player)) {
            Object.Destroy(_playersDict[player].gameObject); // destroy text
            _playersDict.Remove(player);
            Object.Destroy(player);
        }
    }
}
