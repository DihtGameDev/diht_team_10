using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilitiesDict { // inefficient, but we will rarely use search
    [System.Serializable]
    public class Node {
        public string abilityTag;
        public AbstractAbility ability;
    }

    public Node[] dict;

    public AbstractAbility Get(string abilityTag) {
        for (int i = 0; i < dict.Length; ++i) {
            if (dict[i].abilityTag == abilityTag) {
                return dict[i].ability;
            }
        }
        Debugger.Log("AbilitiesDict. " + abilityTag + " doesn't exists");
        return null;
    }
}
