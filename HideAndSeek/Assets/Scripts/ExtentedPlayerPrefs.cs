using UnityEngine;

public static class ExtentedPlayerPrefs {
    public static void SetObject<T>(string key, T obj) {
        PlayerPrefs.SetString(key, JsonUtility.ToJson(obj));
    }

    public static T GetObject<T>(string key) where T : class {
        string jsonString = PlayerPrefs.GetString(key, "");
        if (jsonString == "") {
            return null;
        } else {
            return JsonUtility.FromJson<T>(jsonString);
        }
    }
}