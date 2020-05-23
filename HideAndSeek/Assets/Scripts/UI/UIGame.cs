using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : UIBase<UIGameWidget> {
    public Joystick moveJoystick => _widget.moveJoystick;
    public Text chatText => _widget.chatText;
    public Text respawnText => _widget.respawnText;
    public Text playerCounterText => _widget.playerCounterText;
    

    public Button abilityBtn => _widget.abilityBtn;
    public Image abilityLoadingBar => _widget.abilityLoadingBar;

    /*   Pause dialog   */
    public Button pauseBtn => _widget.pauseBtn;
    public GameObject pauseDialogGO => _widget.pauseDialogGO;
    public Button closePauseDialogBtn => _widget.closePauseDialogBtn;
    public Button disconnectBtn => _widget.disconnectBtn;

    public Button soundSettingsBtn => _widget.soundSettingsBtn;

    public event System.Action onUseAbility;

    public bool soundOn = true;

    private bool stopLoadingBar = false;

    public UIGame(UIGameWidget gameWidget) : base(gameWidget) {
        pauseDialogGO.SetActive(false);

        disconnectBtn.onClick.AddListener(OnDisconnectClick);
        abilityBtn.onClick.AddListener(UseAbility);
        pauseBtn.onClick.AddListener(OnPauseClicked);
        soundSettingsBtn.onClick.AddListener(OnSoundSettingsClick);

        closePauseDialogBtn.onClick.AddListener(() => { pauseDialogGO.SetActive(false); });

        SetSoundIconForState(soundOn);

        GameManager.instance.OnPlayersCountChanged += SetPlayersCounter;
    }

    ~UIGame() {
        GameManager.instance.OnPlayersCountChanged -= SetPlayersCounter;
    }

    public void LoadingState() {
        playerCounterText.gameObject.SetActive(false);
        moveJoystick.gameObject.SetActive(false);
        abilityBtn.gameObject.SetActive(false);
    }

    public void PlayingState() {
        moveJoystick.gameObject.SetActive(true);
        playerCounterText.gameObject.SetActive(true);
        abilityBtn.gameObject.SetActive(true);
    }

    public void OnPauseClicked() {
        pauseDialogGO.transform.SetAsLastSibling();
        pauseDialogGO.SetActive(!pauseDialogGO.activeSelf);
    }

    public void OnDisconnectClick() {
        GameManager.instance.Leave();
    }

    public void OnSoundSettingsClick() {
        soundOn = !soundOn;
        SetSoundIconForState(soundOn);
    }

    private void SetSoundIconForState(bool soundOn) {
        soundSettingsBtn.image.sprite = soundOn ? _widget.soundOnSprite : _widget.soundOffSprite;
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

    public void SetAbilityIcon(AbilityType abilityType) {
        abilityLoadingBar.sprite = GetAbilityIconByType(abilityType);
    }

    public Sprite GetAbilityIconByType(AbilityType abilityType) {
        switch (abilityType) {
            case AbilityType.FLARE: {
                return _widget.flareIcon;
            }
            case AbilityType.INVISIBILITY: {
                return _widget.invisibilityIcon;
            }
            case AbilityType.RADAR: {
                return _widget.radarIcon;
            }
            case AbilityType.SURGE: {
                return _widget.speedupIcon;
            }
        }

        return _widget.flareIcon;
    }

    public void SetPlayersCounter(int playersCount) {
        playerCounterText.text = "Players: " + playersCount;
    }

    public void StartRespawnTimer() {
        GameManager.instance.StartCoroutine(RespawnTick());
    }

    public void UseAbility() {
        abilityBtn.interactable = false;
        onUseAbility?.Invoke();
        GameManager.instance.StartCoroutine(UnlockAbilityAndUpdateLoadingBar(GameManager.instance.ability.Cooldown()));
    }

    public IEnumerator UnlockAbilityAndUpdateLoadingBar(float delay) {
        float startTime = Time.time;
        stopLoadingBar = false;

        while (Time.time - startTime < delay) {
            if (stopLoadingBar) {
                yield break;
            }

            float amount = (Time.time - startTime) / delay;
            if (1f - amount < 0.001f) {
                amount = 1f;
            }

            abilityLoadingBar.fillAmount = amount;

            yield return null;
        }

        abilityBtn.interactable = true;
    }

    public void UnlockAbility() {
        stopLoadingBar = true;
        abilityLoadingBar.fillAmount = 1f;
        abilityBtn.interactable = true;
    }

    private IEnumerator RespawnTick() {
        for (int i = Constants.RESPAWN_TIME; i >= 1; --i) {
            respawnText.text = "Respawn: " + i;
            yield return new WaitForSeconds(1f);
        }

        respawnText.text = "";
    }
}
