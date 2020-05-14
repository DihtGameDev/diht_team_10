using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Threading.Tasks;

public class FirebaseController : SingletonGameObject<FirebaseController> {
    private FirebaseApp _app;
    private DatabaseReference _databaseRef;
    private static readonly string _databaseURL = "https://goty-277103.firebaseio.com/";

    protected void Start() {
        InitFirebase();
    }

    protected void Update() {
    }

    public void GetValue() {
        instance._databaseRef.Child("ASD").GetValueAsync().ContinueWith(task => {

            if (task.IsFaulted) {
                Debug.LogError("fail while getting value from db");
                return;
            }

            DataSnapshot snapshot = task.Result;
            Dictionary<string, System.Object> d = snapshot.Value as Dictionary<string, System.Object>;

            Debugger.Log("Получено значения: |" + d["field1"].ToString() + "|" + d["field2"].ToString());
        });
    }

    public void SetValue(int value) {
        instance._databaseRef.Child("game_launches").SetValueAsync("" + value).ContinueWith(task => {
            if (task.IsFaulted) {
                Debug.LogError("fail while setting value to db");
                return;
            }
        });
    }

    private void InitFirebase() {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                _app = Firebase.FirebaseApp.DefaultInstance;
                _app.SetEditorDatabaseUrl(_databaseURL);
                _databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
                UpdateLauchesCount();
            } else {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    private void UpdateLauchesCount() {
        instance._databaseRef.Child("game_launches").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                Debug.LogError("fail while update launches count value from db");
                return;
            }
            int launches = Convert.ToInt32(task.Result.Value);
            instance._databaseRef.Child("game_launches").SetValueAsync(launches + 1);
        });
    }
}
