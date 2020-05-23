using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingDialog : AbstractDialog {
    public float addRemoveDotTime = 0.3f;
    public Text loadingText;
    public string loadingTextString = "Loading";

    private bool isActive = false;

    protected void Enable() {
        Debug.Log("LOADING DIALOG ENABLE");
        isActive = true;
        StartCoroutine(ChangeDotsNum());
    }

    protected void Disable() {
        isActive = false;
    }

    private IEnumerator ChangeDotsNum() {
        while (isActive) {
            for (int i = 0; i <= 3; ++i) {
                loadingText.text = loadingTextString + '.' * i;
                Debug.Log(loadingTextString + '.' * i);
                yield return new WaitForSeconds(addRemoveDotTime);
            }
        }
    }
}
