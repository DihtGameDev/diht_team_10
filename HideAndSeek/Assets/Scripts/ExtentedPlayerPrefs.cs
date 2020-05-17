using UnityEngine;

public static class ExtentedPlayerPrefs {
    public static void SetObject<T>(string key, T obj) {
        UnityMainThreadDispatcher.instance.Enqueue(Misc.WaitAndDo( // you can't set player prefs from non-main thread
            0f,
            () => {
                PlayerPrefs.SetString(key, JsonUtility.ToJson(obj));
                PlayerPrefs.Save();
            }
        ));
    }

    public static T GetObject<T>(string key, T defaultObj=null) where T : class {
        string jsonString = PlayerPrefs.GetString(key, "");
        if (jsonString == "") {
            return defaultObj;
        } else {
            return JsonUtility.FromJson<T>(jsonString);
        }
    }
}