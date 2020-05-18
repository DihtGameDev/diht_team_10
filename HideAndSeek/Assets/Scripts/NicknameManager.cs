using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NicknameManager {
    private Dictionary<GameObject, RectTransform> _hidemansDict = new Dictionary<GameObject, RectTransform>();
    private Dictionary<GameObject, RectTransform> _seekersDict = new Dictionary<GameObject, RectTransform>();

    private RectTransform _canvasRect;
    private GameObject _nicknamePrefab;

    private PlayerType _visiblePlayerType = PlayerType.SEEKER;

    public const string NICKNAME_PO_TYPE = "nickname"; // pool objects type

    private NicknameManager(GameObject canvasGO, GameObject nicknamePrefab) {
        _canvasRect = canvasGO.GetComponent<RectTransform>();
        _nicknamePrefab = nicknamePrefab;
    }

    public static NicknameManager GetInstance(GameObject canvasGO, GameObject nicknamePrefab) {
        return new NicknameManager(canvasGO, nicknamePrefab);
    }

    public void UpdatePositions() {
        var visiblePlayers = GetDictFromForPlayerType(_visiblePlayerType);

        foreach (var player in visiblePlayers) {
            Vector2 inRectPos = Misc.WorldToRectTransformPosition(player.Key.transform.position, _canvasRect);
            inRectPos.y += 55f; // for above player position
            player.Value.anchoredPosition = inRectPos;
        }
    }

    public void SetVisiblePlayerType(PlayerType type) {
        if (_visiblePlayerType != type) { // disable old and enable new drawing when switching type
            foreach (var pair in GetDictFromForPlayerType(_visiblePlayerType)) {
                pair.Value.gameObject.SetActive(false);
            }

            foreach (var pair in GetDictFromForPlayerType(type)) {
                pair.Value.gameObject.SetActive(true);
            }
        }

        _visiblePlayerType = type;
    }

    public void AddPlayer(GameObject player, PlayerType playerType, string nickname) {
        var suitableDict = GetDictFromForPlayerType(playerType);

        if (suitableDict.ContainsKey(player)) {
            return;
        }

        GameObject nicknameObject = GameManager.instance.objectsPool.Get(NICKNAME_PO_TYPE);

        if (nicknameObject == null) { // if objects pool is empty, we instantiate
            nicknameObject = GameObject.Instantiate(_nicknamePrefab, _canvasRect.transform);
        }

        suitableDict.Add(player, nicknameObject.GetComponent<RectTransform>());

        suitableDict[player].GetComponent<Text>().text = nickname;
    }
        
    public void DeletePlayer(GameObject player, PlayerType playerType) {
        var suitableDict = GetDictFromForPlayerType(playerType);

        if (suitableDict.ContainsKey(player)) {
            if (suitableDict[player] != null && suitableDict[player].gameObject != null) {
                GameManager.instance.objectsPool.Add(NICKNAME_PO_TYPE, suitableDict[player].gameObject);
            }
            suitableDict.Remove(player);
        }
    }

    public void Clear() {
        foreach (var player in _hidemansDict) {
            GameManager.instance.objectsPool.Add(NICKNAME_PO_TYPE, player.Value.gameObject);
        }
        foreach (var player in _seekersDict) {
            GameManager.instance.objectsPool.Add(NICKNAME_PO_TYPE, player.Value.gameObject);
        }

        _hidemansDict.Clear();
        _seekersDict.Clear();
    }

    private Dictionary<GameObject, RectTransform> GetDictFromForPlayerType(PlayerType type) {
        return type == PlayerType.SEEKER ? _seekersDict : _hidemansDict;
    }
}
