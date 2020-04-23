using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMessage : MonoBehaviour {
    [SerializeField]
    private Button someButton;

    protected void Start() {
        someButton.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick() {
        print("Button is clicked");
    }

    protected void Update() {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject == someButton.gameObject) {
            print("button clicked");
        } else {
            print("button not clicked");
        }
    }
}
