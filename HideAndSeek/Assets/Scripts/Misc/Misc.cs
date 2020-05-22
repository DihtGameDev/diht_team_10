using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType {
    NULL, FLARE, INVISIBILITY, SURGE, RADAR
}

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
        action();
    }

    public static IEnumerator WaitWhile(System.Func<bool> waitCondition, System.Action action, float checkDelay=0) {
        while (waitCondition()) {
            yield return new WaitForSeconds(checkDelay);
        }

        action();
    }

    public static AbilityType GetAbilityTypeByTag(string abilityTag) {
        if (abilityTag == null || abilityTag == "") {
            return AbilityType.NULL;
        }

        string suffix = abilityTag.Substring(2);
        switch (suffix) {
            case "Flare": {
                return AbilityType.FLARE;
            }
            case "Surge": {
                return AbilityType.SURGE;
            }
            case "Invisible": {
                return AbilityType.INVISIBILITY;
            }
            case "ShowAllHideman": {
                return AbilityType.RADAR;
            }
        }

        return AbilityType.FLARE;
    }

    public static string GetAbilityTagByType(AbilityType abilityType, bool isSeeker) {  // sorry for all this =((
        string suffix = isSeeker ? "S_" : "H_";

        switch (abilityType) {
            case AbilityType.FLARE: {
                return suffix + "Flare";
            }
            case AbilityType.SURGE: {
                return suffix + "Surge";
            }
            case AbilityType.INVISIBILITY: {
                return suffix + "Invisible";
            }
            case AbilityType.RADAR: {
                return suffix + "ShowAllHideman";
            }
        }

        return suffix + "Flare";
    }
} 
