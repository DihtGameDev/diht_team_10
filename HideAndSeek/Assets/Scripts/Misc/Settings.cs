using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings {
    public string nickname = "";

    private static Settings _settings;

    private Settings() {}

    public static Settings getInstance() {
        if (_settings == null) {
            _settings = ExtentedPlayerPrefs.GetObject<Settings>(Constants.PlayerPrefs.SETTINGS, new Settings());
        }
        return _settings;
    }
}
