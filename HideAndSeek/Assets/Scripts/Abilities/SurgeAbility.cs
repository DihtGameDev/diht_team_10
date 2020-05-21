using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[CreateAssetMenu(menuName = "Abilities/SurgeAbility", fileName = "SurgeAbility")]
public class SurgeAbility : AbstractAbility {
    public float cooldown = 15;
    public float surgeDuration = 3f;
    public float additionalMoveSpeed = 5f;

    protected void OnEnable() {
    }

    override public void UseAbility() {
        Debugger.Log("UseAbility");
        GameManager.instance.mainPlayer.GetComponent<PhotonView>().RPC("UsedSurgeAbilityParticleSystem", RpcTarget.All, surgeDuration);
        GameManager.instance.playerController.SetAdditionalMoveSpeed(additionalMoveSpeed);
        GameManager.instance.StartCoroutine(EndSurge());
    }

    override public float Cooldown() {
        return cooldown;
    }

    private IEnumerator EndSurge() {
        yield return new WaitForSeconds(surgeDuration);
        GameManager.instance.playerController.SetAdditionalMoveSpeed(0f);
    }
}
