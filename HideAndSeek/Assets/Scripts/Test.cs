using UnityEngine;

public class Test : MonoBehaviour, PrefsStorage  { // не очень понятно для чего это всё
    public T Deserealize<T>(string key) where T : class {
        string jsonString = PlayerPrefs.GetString(key, "");
        if (jsonString == "") {
            return null;
        } else {
            return JsonUtility.FromJson<T>(jsonString);
        }
    }

    public void Serialize<T>(string key, T obj) {
        PlayerPrefs.SetString(key, JsonUtility.ToJson(obj));
    }
}
