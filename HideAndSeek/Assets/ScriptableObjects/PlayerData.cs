using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerData/Default", fileName = "DefaultPlayer")]
public class PlayerData : ScriptableObject {
    public float moveSpeed = 5f;
    public float lerpAngleCoeff = 0.08f;
}
