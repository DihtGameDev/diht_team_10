using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/SurgeAbility", fileName = "SurgeAbility")]
public class SurgeAbility : AbstractAbility {
    public float cooldown = 15;
    public float surgeDuration = 3f;
    public float additionalMoveSpeed = 5f;

    protected void OnEnable() {
    }

    override public void UseAbility() {
        Debugger.Log("UseAbility");
        GameManager.GAME_MANAGER.playerController.SetAdditionalMoveSpeed(additionalMoveSpeed);
        GameManager.GAME_MANAGER.StartCoroutine(EndSurge());
    }

    override public float Cooldown() {
        return cooldown;
    }

    private IEnumerator EndSurge() {
        yield return new WaitForSeconds(surgeDuration);
        GameManager.GAME_MANAGER.playerController.SetAdditionalMoveSpeed(0f);
    }
}
