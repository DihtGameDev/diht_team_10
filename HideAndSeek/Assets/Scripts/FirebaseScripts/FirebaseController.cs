using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Unity.Editor;
using System;
using System.Threading.Tasks;

public enum AnalyticsType {
    LAUNCH_GAME, START_PLAY, PLAYED_MORE_2_MINS, WORKSHOP
}

public class ReadyState {
    public bool isReady = false;
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

    public event Action<FirebaseGameData> OnGettingFirebaseData;

    public FbAuth auth;
    public bool isReady = false;

    protected void Start() {
        InitFirebase();
        IncrementAnalyticsData(AnalyticsType.LAUNCH_GAME);
    }

    protected void Update() {
    }

    public void SetFirebaseGameDataToPlayerPrefs(string firebaseUserId) {
        GetFirebaseDataAsync(
            firebaseUserId,
            (firebaseGameData) => { Settings.getInstance().SetFirebaseGameData(_firebaseGameData); }
        );
    }

    public void SetDefaultUserDataToFirebase(string firebaseUserId) {
        DatabaseReference userReference = instance._databaseRoot.Child("users").Child(firebaseUserId);
        userReference.Child("coins").SetValueAsync(0);
        userReference.Child("abilities").Child("seeker").Child(Constants.AbilitiesTags.Seeker.DEFAULT).SetValueAsync(1);
        userReference.Child("abilities").Child("hideman").Child(Constants.AbilitiesTags.Hideman.DEFAULT).SetValueAsync(1);
    }

    public void MoveFirebaseGameDataToNewFirebaseUserId(string oldFirebaseUserId, string newFirebaseUserId) {
        GetFirebaseDataAsync(
            oldFirebaseUserId,
            (firebaseData) => {
                DatabaseReference userReference = instance._databaseRoot.Child("users").Child(newFirebaseUserId);
                userReference.Child("coins").SetValueAsync(firebaseData.coins);
                for (int i = 0; i < firebaseData.abilityTags.size; ++i) {
                    string abilityType = firebaseData.abilityTags.buffer[i][0] == 'S' ? "seeker" : "hideman";
                    userReference.Child("abilities").Child(abilityType).Child(firebaseData.abilityTags.buffer[i]).SetValueAsync(1);
                }

                Settings.getInstance().firebaseUserId = newFirebaseUserId;
                Settings.getInstance().save();
            }
        );
    }

    private void InitFirebase() {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                _app = Firebase.FirebaseApp.DefaultInstance;
                _app.SetEditorDatabaseUrl(_databaseURL);
                _databaseRoot = FirebaseDatabase.DefaultInstance.RootReference;
                auth = new FbAuth(FirebaseAuth.DefaultInstance);
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
    public ReadyState CheckAndResetAbilities() {
        ReadyState readyState = new ReadyState();
        GetFirebaseDataAsync(
            Settings.getInstance().firebaseUserId,
            (firebaseGameData) => {
                UnityMainThreadDispatcher.instance.Enqueue(
                    () => {
                        if (!firebaseGameData.abilityTags.Contains(Settings.getInstance().hidemanAbility)) {
                            Settings.getInstance().hidemanAbility = Constants.AbilitiesTags.Hideman.DEFAULT;
                        }

                        if (!firebaseGameData.abilityTags.Contains(Settings.getInstance().seekerAbility)) {
                            Settings.getInstance().seekerAbility = Constants.AbilitiesTags.Seeker.DEFAULT;
                        }

                        Settings.getInstance().save();

                        readyState.isReady = true;
                    }
                );
                
            } 
        );

        return readyState;
    }

    public ReadyState AddCoins(int addedCoins) {
        ReadyState readyState = new ReadyState();

        GetFirebaseDataAsync(
            Settings.getInstance().firebaseUserId,
            (firebaseGameData) => {
                UnityMainThreadDispatcher.instance.Enqueue( // you can't use this from non-main thread, therefore we register this in main thread
                    () => {
                        DatabaseReference userReference = instance._databaseRoot.Child("users").Child(Settings.getInstance().firebaseUserId);
                        userReference.Child("coins").SetValueAsync(firebaseGameData.coins + addedCoins);
                        Debugger.Log("Coins added: " + addedCoins);
                        readyState.isReady = true;
                    }
                );
                
            }
        );

        return readyState;
    }

    private void GetFirebaseDataAsync(string firebaseUserId, System.Action<FirebaseGameData> action) {
        instance._databaseRoot.Child("users").Child(firebaseUserId).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                Debugger.Log("fail while settings firebase game data");
                return;
            }

            Dictionary<string, object> result = task.Result.Value as Dictionary<string, object>; // coins {abilities -> seeker -> ability-num. abilities -> hideman -> ability-num.}

            if (result == null) { // this value never setted before
                Debugger.Log("this userId not exists in firebase, therefore i will create it");
                SetDefaultUserDataToFirebase(firebaseUserId);
            } else {
                UnityMainThreadDispatcher.instance.Enqueue(
                    () => {
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

                        OnGettingFirebaseData?.Invoke(_firebaseGameData);

                        action(_firebaseGameData);
                    }    
                );
            }
        });
    }

    private void IncrementAnalyticsDataInFirebase(string firebaseKey) {
        instance._databaseRoot.Child(firebaseKey).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                Debug.LogError("fail while incrementing analytics data");
                return;
            }
            UnityMainThreadDispatcher.instance.Enqueue(
                () => {
                    int val = Convert.ToInt32(task.Result.Value);
                    instance._databaseRoot.Child(firebaseKey).SetValueAsync(val + 1);
                }    
            );
        });
    }
}
