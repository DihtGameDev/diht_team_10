using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings {
    public string nickname = "";
    public string email = "";
    public string hidemanAbility = Constants.AbilitiesTags.Hideman.DEFAULT;
    public string seekerAbility = Constants.AbilitiesTags.Seeker.DEFAULT;
    public string firebaseUserId = "";
    public FirebaseGameData firebaseGameData = new FirebaseGameData();

    private static Settings _settings;

    private Settings() {}

    public static Settings getInstance() {
        if (_settings == null) {
            _settings = ExtentedPlayerPrefs.GetObject<Settings>(Constants.PlayerPrefs.SETTINGS, new Settings());
        }
        return _settings;
    }

    public static Settings getResettedInstance() {
        _settings = null;
        return getInstance();
    }

    public void save() {
        if (_settings != null) {
            ExtentedPlayerPrefs.SetObject<Settings>(Constants.PlayerPrefs.SETTINGS, _settings);
        }
    }

    public void SetFirebaseGameData(FirebaseGameData firebaseGameData) {
        this.firebaseGameData = firebaseGameData;
        save();
    }
}
