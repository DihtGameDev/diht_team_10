using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : UIBase<UIGameWidget> {
    public GameObject loadingScreenGO => _widget.loadingScreenGO;
    public Joystick moveJoystick => _widget.moveJoystick;
    public Text chatText => _widget.chatText;
    public Text respawnText => _widget.respawnText;

    public Text playerCounterText => _widget.playerCounterText;

    public Button disconnectBtn => _widget.disconnectBtn;
    public Button abilityBtn => _widget.abilityBtn;

    public UIGame(UIGameWidget gameWidget) : base(gameWidget) {
        disconnectBtn.onClick.AddListener(OnDisconnectClick);
        abilityBtn.onClick.AddListener(GameManager.GAME_MANAGER.UseAbility);
    }

    public void LoadingState() {
        loadingScreenGO.SetActive(true);
        playerCounterText.gameObject.SetActive(false);
        moveJoystick.gameObject.SetActive(false);
    }

    public void PlayingState() {
        loadingScreenGO.SetActive(false);
        moveJoystick.gameObject.SetActive(true);
        playerCounterText.gameObject.SetActive(true);
        abilityBtn.gameObject.SetActive(true);
    }

    public void OnDisconnectClick() {
        GameManager.GAME_MANAGER.Leave();
    }

    public void PrintInChat(string message) {
        chatText.text = chatText.text + "\n" + message;
        GameManager.GAME_MANAGER.StartCoroutine(ClearChat());
    }

    private IEnumerator ClearChat() {
        yield return new WaitForSeconds(7f);
        chatText.text = "";
    }

    public void UpdatePlayerCounter() {
        playerCounterText.text = "Players: " + GameManager.GAME_MANAGER.PlayersInTheScene();
    }

    public void StartRespawnTimer() {
        GameManager.GAME_MANAGER.StartCoroutine(RespawnTick());
    }

    private IEnumerator RespawnTick() {
        for (int i = Constants.RESPAWN_TIME; i >= 1; --i) {
            respawnText.text = "Respawn: " + i;
            yield return new WaitForSeconds(1f);
        }

        respawnText.text = "";
    }
}
