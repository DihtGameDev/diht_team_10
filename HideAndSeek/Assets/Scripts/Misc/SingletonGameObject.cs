using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonGameObject<Instance> : MonoBehaviour where Instance : SingletonGameObject<Instance> {
    public static Instance instance;

    protected virtual void Awake() {
        if (instance == null) {
            instance = this as Instance;
        } else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}