using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameWidget : MonoBehaviour {
    public Joystick moveJoystick;
    public Text chatText;
    public Text respawnText;
    public Text playerCounterText;

    [Header("Ability settings")]
    public Button abilityBtn;
    public Image abilityLoadingBar;

    [Header("Pause dialog")]
    public Button pauseBtn;
    public GameObject pauseDialogGO;
    public Button closePauseDialogBtn;
    public Button disconnectBtn;
    public Button soundSettingsBtn;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    [Header("Abilities Icons")]
    public Sprite flareIcon;
    public Sprite speedupIcon;
    public Sprite invisibilityIcon;
    public Sprite radarIcon;
}
