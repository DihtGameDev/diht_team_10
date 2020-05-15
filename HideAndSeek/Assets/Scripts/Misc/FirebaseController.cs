using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Threading.Tasks;

public enum AnalyticsType {
    LAUNCH_GAME, START_PLAY, PLAYED_MORE_2_MINS, WORKSHOP
}

[System.Serializable]
public class FirebaseGameData {
    public int coins = 0;
    public StringsList abilityTags;

    public FirebaseGameData() {
        string[] defaultAbilityTags = { Constants.AbilitiesTags.Seeker.FLARE, Constants.AbilitiesTags.Hideman.SURGE };
        abilityTags = new StringsList(defaultAbilityTags);
    }
}

public class FirebaseController : SingletonGameObject<FirebaseController> {
    private FirebaseApp _app;
    private DatabaseReference _databaseRoot;
    private static readonly string _databaseURL = "https://goty-277103.firebaseio.com/";
    private FirebaseGameData _firebaseGameData = new FirebaseGameData(); // optimization

    public bool isReady = false;

    protected void Start() {
        InitFirebase();
        IncrementAnalyticsData(AnalyticsType.LAUNCH_GAME);
    }

    protected void Update() {
    }

    public void SetFirebaseGameDataToPlayerPrefs(string firebaseUserId) {
        OnGettingFirebaseData(
            firebaseUserId,
            (firebaseGameData) => { Settings.getInstance().SetFirebaseGameData(_firebaseGameData);
        });
        instance._databaseRoot.Child("users").Child(firebaseUserId).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                Debug.LogError("fail while settings firebase game data");
                return;
            }

            Dictionary<string, object> result = task.Result.Value as Dictionary<string, object>; // coins {abilities -> seeker -> ability-num. abilities -> hideman -> ability-num.}

            if (result == null) { // this value never setted before
                Debugger.Log("this userId not exists in firebasa, therefore i will create it");
                SetDefaultUserDataToFirebase(firebaseUserId);
            } else {
                _firebaseGameData.coins = Convert.ToInt32(result["coins"]);

                result = result["abilities"] as Dictionary<string, object>;

                var seekerAbilities = result["seeker"] as Dictionary<string, object>;
                var hidemanAbilities = result["hideman"] as Dictionary<string, object>;

                string[] abilityTags = new string[seekerAbilities.Count + hidemanAbilities.Count];

                // adding seeker abilities
                foreach (var abilityAndLevel in seekerAbilities) {
                    int abilityValue = Convert.ToInt32(abilityAndLevel.Value);
                    if (abilityValue > 0) {
                        _firebaseGameData.abilityTags.Append(abilityAndLevel.Key.ToString());
                    }
                }

                // adding hideman ability
                foreach (var abilityAndLevel in hidemanAbilities) {
                    int abilityValue = Convert.ToInt32(abilityAndLevel.Value);
                    if (abilityValue > 0) {
                        _firebaseGameData.abilityTags.Append(abilityAndLevel.Key.ToString());
                    }
                }

                Settings.getInstance().SetFirebaseGameData(_firebaseGameData);
            }
        });
    }

    public void SetDefaultUserDataToFirebase(string firebaseUserId) {
        DatabaseReference userReference = instance._databaseRoot.Child("users").Child(firebaseUserId);
        userReference.Child("coins").SetValueAsync(0);
        userReference.Child("abilities").Child("seeker").Child(Constants.AbilitiesTags.Seeker.FLARE).SetValueAsync(1);
        userReference.Child("abilities").Child("hideman").Child(Constants.AbilitiesTags.Hideman.SURGE).SetValueAsync(1);
    }

    private void InitFirebase() {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                _app = Firebase.FirebaseApp.DefaultInstance;
                _app.SetEditorDatabaseUrl(_databaseURL);
                _databaseRoot = FirebaseDatabase.DefaultInstance.RootReference;
                isReady = true;
            } else {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    public void IncrementAnalyticsData(AnalyticsType type) {
        StartCoroutine(Misc.WaitWhile(
                () => { return instance._databaseRoot == null; },  // wait condition
                () => {
                    switch (type) {
                        case AnalyticsType.LAUNCH_GAME: {
                            IncrementAnalyticsDataInFirebase("game_launches");
                            break;
                        }
                        case AnalyticsType.START_PLAY: {
                            IncrementAnalyticsDataInFirebase("game_start_play");
                            break;
                        }
                        case AnalyticsType.WORKSHOP: {
                            IncrementAnalyticsDataInFirebase("workshop_launches");
                            break;
                        }
                        case AnalyticsType.PLAYED_MORE_2_MINS: {
                            IncrementAnalyticsDataInFirebase("played_more_2_mins");
                            break;
                        }
                    }
                }
        ));
    }

    // check if we have this abilities, and if we havn't, we set default abilities
    public void CheckAndResetAbility() {

    }

    private void OnGettingFirebaseData(string firebaseUserId, System.Action<FirebaseGameData> action) {
        instance._databaseRoot.Child("users").Child(firebaseUserId).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                Debug.LogError("fail while settings firebase game data");
                return;
            }

            Dictionary<string, object> result = task.Result.Value as Dictionary<string, object>; // coins {abilities -> seeker -> ability-num. abilities -> hideman -> ability-num.}

            if (result == null) { // this value never setted before
                Debugger.Log("this userId not exists in firebasa, therefore i will create it");
                SetDefaultUserDataToFirebase(firebaseUserId);
            } else {
                _firebaseGameData.coins = Convert.ToInt32(result["coins"]);

                result = result["abilities"] as Dictionary<string, object>;

                var seekerAbilities = result["seeker"] as Dictionary<string, object>;
                var hidemanAbilities = result["hideman"] as Dictionary<string, object>;

                string[] abilityTags = new string[seekerAbilities.Count + hidemanAbilities.Count];

                // adding seeker abilities
                foreach (var abilityAndLevel in seekerAbilities) {
                    int abilityValue = Convert.ToInt32(abilityAndLevel.Value);
                    if (abilityValue > 0) {
                        _firebaseGameData.abilityTags.Append(abilityAndLevel.Key.ToString());
                    }
                }

                // adding hideman ability
                foreach (var abilityAndLevel in hidemanAbilities) {
                    int abilityValue = Convert.ToInt32(abilityAndLevel.Value);
                    if (abilityValue > 0) {
                        _firebaseGameData.abilityTags.Append(abilityAndLevel.Key.ToString());
                    }
                }

                action(_firebaseGameData);
            }
        });
    }

    private void IncrementAnalyticsDataInFirebase(string firebaseKey) {
        instance._databaseRoot.Child(firebaseKey).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                Debug.LogError("fail while incrementing analytics data");
                return;
            }
            int val = Convert.ToInt32(task.Result.Value);
            instance._databaseRoot.Child(firebaseKey).SetValueAsync(val + 1);
        });
    }
}
