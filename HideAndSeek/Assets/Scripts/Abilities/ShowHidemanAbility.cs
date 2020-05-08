using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/ShowHidemanAbility", fileName = "ShowHidemanAbility")]
public class ShowHidemanAbility : AbstractAbility {
    public float cooldown = 15;
    public float markersDuration = 3f;
    public string playerTag = Constants.HIDEMAN_TAG;
    public float bordersCoeff = 0.8f;

    public const string MARKER_TYPE = "hidemanmarker"; // for objects pool

    private List<GameObject> _markers;
    private RectTransform _canvasRT;
    private GameObject[] _players;

    [SerializeField]    
    private GameObject _markerPrefab;

    protected void OnEnable() {
        _markers = new List<GameObject>();
    }

    public static float CalculateAngle(Vector3 from, Vector3 to) {
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }

    private IEnumerator UpdateMarkersPos() {
        if (_canvasRT == null) {
            _canvasRT = GameObject.Find("Canvas").GetComponent<RectTransform>();
        }
        float startTime = Time.time;

        while (Time.time - startTime < markersDuration) {
            for (int i = 0; i < _players.Length; ++i) {
                Vector3 playerPos; // because if object will destroy after if, we have exception
                if (_players[i] == null) {  // player left game while we show markers
                    continue;
                } else {
                    playerPos = _players[i].transform.position;
                }
                Vector3 inRectPos = Misc.WorldToRectTransformPosition(playerPos, _canvasRT);
                
                // screen is bound, therefore we limit the boundaries
                inRectPos.x = Mathf.Clamp(inRectPos.x, -_canvasRT.rect.size.x / 2f, _canvasRT.rect.size.x / 2f) * bordersCoeff;
                inRectPos.y = Mathf.Clamp(inRectPos.y, -_canvasRT.rect.size.y / 2f, _canvasRT.rect.size.y / 2f) * bordersCoeff;

                _markers[i].GetComponent<RectTransform>().anchoredPosition = inRectPos;
                Vector3 delta =
                    Camera.main.WorldToScreenPoint(playerPos) -
                    Camera.main.WorldToScreenPoint(GameManager.GAME_MANAGER.mainPlayer.transform.position);

                _markers[i].transform.eulerAngles = Vector3.forward * (CalculateAngle(delta, Vector3.up));
            }

            yield return null;
        }

        foreach (var marker in _markers) {
            GameManager.GAME_MANAGER.objectsPool.Add(MARKER_TYPE, marker);
        }

        _markers.Clear();
    }

    override public void UseAbility() {
        Debugger.Log("UseAbility");
        _players = GameObject.FindGameObjectsWithTag(playerTag);

        for (int i = 0; i < _players.Length; ++i) {
            GameObject marker = GameManager.GAME_MANAGER.objectsPool.Get(MARKER_TYPE);
            if (marker == null) {
                marker = GameObject.Instantiate(_markerPrefab, GameObject.Find("Canvas").transform);
            }
            _markers.Add(marker);
        }

        GameManager.GAME_MANAGER.StartCoroutine(UpdateMarkersPos());
    }

    override public float Cooldown() {
        return cooldown;
    }
}
