using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/InvisibleAbility", fileName = "InvisibleAbility")]
public class InvisibleAbility : AbstractAbility {
    public float cooldown = 15;
    public float invisibleDurationSec = 5f;

    protected void OnEnable() {
    }

    override public void UseAbility() {
        Debugger.Log("UseAbility");
        GameManager.GAME_MANAGER.photonViewMainPlayer.RPC("BecomeInvisibleForTime", Photon.Pun.RpcTarget.All, invisibleDurationSec);
    }

    override public float Cooldown() {
        return cooldown;
    }
}
