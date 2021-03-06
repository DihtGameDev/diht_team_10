﻿using System.Collections;
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
        abilityBtn.onClick.AddListener(GameManager.instance.UseAbility);

        GameManager.instance.OnPlayersCountChanged += SetPlayersCounter;
    }

    ~UIGame() {
        GameManager.instance.OnPlayersCountChanged -= SetPlayersCounter;
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
        GameManager.instance.Leave();
    }

    public void PrintInChat(string message) {
        PrintInChatAndClear(message, 0f);
    }

    public void PrintInChatAndClear(string message, float clearDelay=0f) {
        chatText.text = chatText.text + "\n" + message;

        if (clearDelay != 0f) {
            GameManager.instance.StartCoroutine(Misc.WaitAndDo(clearDelay, () => { chatText.text = ""; }));
        }
    }

    public void SetPlayersCounter(int playersCount) {
        playerCounterText.text = "Players: " + playersCount;
    }

    public void StartRespawnTimer() {
        GameManager.instance.StartCoroutine(RespawnTick());
    }

    private IEnumerator RespawnTick() {
        for (int i = Constants.RESPAWN_TIME; i >= 1; --i) {
            respawnText.text = "Respawn: " + i;
            yield return new WaitForSeconds(1f);
        }

        respawnText.text = "";
    }
}
