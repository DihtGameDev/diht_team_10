using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Threading.Tasks;

public enum AnalyticsType {
    LAUNCH_GAME, START_PLAY, WORKSHOP
}

public class FirebaseController : SingletonGameObject<FirebaseController> {
    private FirebaseApp _app;
    private DatabaseReference _databaseRoot;
    private static readonly string _databaseURL = "https://goty-277103.firebaseio.com/";

    protected void Start() {
        InitFirebase();
        IncrementAnalyticsData(AnalyticsType.LAUNCH_GAME);
    }

    protected void Update() {
    }

    private void InitFirebase() {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                _app = Firebase.FirebaseApp.DefaultInstance;
                _app.SetEditorDatabaseUrl(_databaseURL);
                _databaseRoot = FirebaseDatabase.DefaultInstance.RootReference;
            } else {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    private void IncrementAnalyticsData(AnalyticsType type) {
        StartCoroutine(Misc.WaitWhile(
                () => { return instance._databaseRoot == null; },  // wait condition
                () => {
                    switch (type) {
                        case AnalyticsType.LAUNCH_GAME: {
                            IncrementAnalyticsData("game_launches");
                            break;
                        }
                        case AnalyticsType.START_PLAY: {
                            IncrementAnalyticsData("game_start_play");
                            break;
                        }
                        case AnalyticsType.WORKSHOP: {
                            IncrementAnalyticsData("workshop_launches");
                            break;
                        }
                    }
                }
        ));
    }

    private void IncrementAnalyticsData(string firebaseKey) {
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
