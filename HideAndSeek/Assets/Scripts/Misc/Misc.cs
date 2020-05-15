using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Misc {
    public static Vector2 WorldToRectTransformPosition(Vector3 targetWorldPos, RectTransform rectTransform) {
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(targetWorldPos);
        Vector2 rectPosition = new Vector2(
            ((viewportPosition.x * rectTransform.sizeDelta.x) - (rectTransform.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * rectTransform.sizeDelta.y) - (rectTransform.sizeDelta.y * 0.5f))
            );
        return rectPosition;
    }

    public static IEnumerator LoopWithDelay(float iterationDelay, System.Action action) {
        while (true) {
            action();
            yield return new WaitForSeconds(iterationDelay);
        }
    }

    public static IEnumerator WaitAndDo(float delay, System.Action action) {
        yield return new WaitForSeconds(delay);
        Debugger.Log("ACTION");
        action();
    }

    public static IEnumerator WaitWhile(System.Func<bool> waitCondition, System.Action action, float checkDelay=0) {
        while (waitCondition()) {
            yield return new WaitForSeconds(checkDelay);
        }

        action();
    }
}
