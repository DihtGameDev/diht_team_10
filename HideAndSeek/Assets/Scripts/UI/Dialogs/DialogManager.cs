using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogType {
    LOADING
}

public class DialogManager : SingletonGameObject<DialogManager> {

    [Header("Set in inspector")]
    public AbstractDialog loadingDialog; // set in inspector

    public void ShowDialog(DialogType dialogType, System.Func<bool> showCondition) {
        switch (dialogType) {
            case DialogType.LOADING: {
                StartCoroutine(Misc.WaitWhile(
                    () => { print("Wait"); return GameObject.FindGameObjectWithTag(Constants.CANVAS_TAG) == null; },
                    () => { loadingDialog.ShowWhile(showCondition, GameObject.FindGameObjectWithTag(Constants.CANVAS_TAG)); },
                    0.2f
                    ));
                //loadingDialog.transform.SetParent(GameObject.FindGameObjectWithTag(Constants.CANVAS_TAG).transform);
                
                break;
            }
        }
    }
}
