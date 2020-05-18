using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerEntity : MonoBehaviour {
    protected void OnEnable() {
        GameManager.instance.nicknameManager.AddPlayer(gameObject, GetMyPlayerType(), GetNickname());
        Debugger.Log("Created entity with name: " + name);
    }

    protected void OnDisable() {  // photon doesn't invoke destroy, only OnDisable
        GameManager.instance?.nicknameManager?.DeletePlayer(gameObject, GetMyPlayerType());
    }

    private PlayerType GetMyPlayerType() {
        switch (tag) {
            case Constants.ENEMY_SEEKER_AI_TAG: {
                return PlayerType.SEEKER;
            }
            case Constants.SEEKER_TAG: {
                return PlayerType.SEEKER;
            }
            case Constants.HIDEMAN_TAG: {
                return PlayerType.HIDEMAN;
            }
            default: {
                Debugger.Log("GET MY PLAYER TYPE SOME ERROR WITH TAG: " + tag);
                return PlayerType.SEEKER;
            }
        }
    }

    private string GetNickname() {
        if (tag != Constants.ENEMY_SEEKER_AI_TAG) {
            return GetComponent<PhotonView>().Owner.NickName;
        } else {
            return GetRandomHardcodedNickname();
        }
    }

    private string GetRandomHardcodedNickname() {
        return Constants.HARDCODED_NICKNAMES_LIST[Random.Range(0, Constants.HARDCODED_NICKNAMES_LIST.Length)];
    }
}
