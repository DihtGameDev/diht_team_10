using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDialog : MonoBehaviour {
    protected void Awake() {
        print("Awake dialog");
        DontDestroyOnLoad(gameObject);
    }

    protected void Destroy() {
        print("On Destroy dialog");
    }

    public void ShowWhile(System.Func<bool> condition, GameObject canvasObj) {
        print("Show WHILE");
        gameObject.transform.SetParent(canvasObj.transform);
        gameObject.SetActive(true);
        StartCoroutine(Misc.WaitWhile(
            condition,
            () => {
                gameObject.SetActive(false);
                gameObject.transform.SetParent(DialogManager.instance.transform);
            }
        ));
    }
}
