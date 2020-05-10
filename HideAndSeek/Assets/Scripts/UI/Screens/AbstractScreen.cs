using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AbstractScreen : MonoBehaviour {
    abstract public void OnEnable();
    abstract public void OnDisable();
}
