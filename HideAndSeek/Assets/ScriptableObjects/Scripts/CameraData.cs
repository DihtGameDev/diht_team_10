using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CameraData/Default", fileName = "DefaultCamera")]
public class CameraData : ScriptableObject {
    public Vector3 deltaChasingObjPos = new Vector3(3.89f, 18.6f, -13f);
    public Vector3 eulerAngles = new Vector3(46.2f, -15f, 0f);
    public float cameraChasingLerp = 0.125f;
}
