using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAbility : ScriptableObject {
    abstract public void UseAbility();
    abstract public float Cooldown();
}
