using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : UIBase {
    public GameObject loadingScreenGO;
    public Joystick moveJoystick;
    public Text chatText;
    public Text respawnText;

    public UIGame(GameObject canvasGO) : base(canvasGO) {
    }

    protected override void Init() {
        loadingScreenGO = canvasGO.transform.Find("LoadingScreen").gameObject;
        moveJoystick = canvasGO.GetComponentInChildren<Joystick>();
        chatText = canvasGO.transform.Find("Chat").GetComponent<Text>();
        respawnText = canvasGO.transform.Find("RespawnText").GetComponent<Text>();
    }

    public void PrintInChat(string message) {
        chatText.text = chatText.text + "\n" + message;
        GameManager.GAME_MANAGER.StartCoroutine(ClearChat());
    }

    private IEnumerator ClearChat() {
        yield return new WaitForSeconds(7f);
        chatText.text = "";
    }

    public void StartRespawnTimer() {
        GameManager.GAME_MANAGER.StartCoroutine(RespawnTick());
    }

    private IEnumerator RespawnTick() {
        for (int i = 20; i >= 1; --i) {
            respawnText.text = "" + i;
            yield return new WaitForSeconds(1f);
        }

        respawnText.text = "";
    }
}
