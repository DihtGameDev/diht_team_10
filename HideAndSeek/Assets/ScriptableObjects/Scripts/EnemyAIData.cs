using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAIData/Default", fileName = "DefaultEnemyAI")]
public class EnemyAIData : ScriptableObject {
    [Range(0, 360)]
    public float viewAngle = 120f;
    public float viewRadius = 20f;

    public float lerpAngleCoeff = 3f;
    public float addingAngleWhenCame = 120f;
    
    public float minPathfinderRadius = 10f;
    public float maxPathfinderRadius = 80f;

    public LayerMask targetMask;
    public LayerMask obstacleMask;
}
