using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : UIBase<UIGameWidget> {
    public GameObject loadingScreenGO => _widget.loadingScreenGO;
    public Joystick moveJoystick => _widget.moveJoystick;
    public Text chatText => _widget.chatText;
    public Text respawnText => _widget.respawnText;

    public UIGame(UIGameWidget gameWidget) : base(gameWidget) {
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
        for (int i = Constants.RESPAWN_TIME; i >= 1; --i) {
            respawnText.text = "Появление через " + i;
            yield return new WaitForSeconds(1f);
        }

        respawnText.text = "";
    }
}
