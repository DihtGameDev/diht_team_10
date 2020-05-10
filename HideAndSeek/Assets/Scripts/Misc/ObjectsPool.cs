using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsPool {
    private Dictionary<string, Queue<GameObject>> _pooledObjects = new Dictionary<string, Queue<GameObject>>();

    public ObjectsPool() {
    }

    public void Add(string objType, GameObject gameObject) {
        if (gameObject == null) {
            return;
        }
        gameObject.SetActive(false);

        if (!_pooledObjects.ContainsKey(objType)) {
            _pooledObjects.Add(objType, new Queue<GameObject>());
        }
        _pooledObjects[objType].Enqueue(gameObject);
    }

    public GameObject Get(string objType) {
        if (!_pooledObjects.ContainsKey(objType) || _pooledObjects[objType].Count == 0) {
            return null;
        } else {
            GameObject objectToReturn = _pooledObjects[objType].Dequeue();
            objectToReturn.SetActive(true);
            return objectToReturn;
        }
    }
}
