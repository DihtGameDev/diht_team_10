using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {
    public static class PlayerPrefs {
        public const string SETTINGS = "settings";
    }

    public const int USER_ID_LENGTH = 40;

    public const int RESPAWN_TIME = 10;
    public const int MAX_SKELETONS_IN_SCENE = 1;

    public const float SPAWN_SKELETONS_DELAY = 2f;

    public const int ADDED_COINS_AFTER_WIN = 100;

    public const string PLAYER_TAG = "Player";
    public const string SEEKER_TAG = "Seeker";
    public const string HIDEMAN_TAG = "Hideman";
    public const string SKELETON_TAG = "Skeleton";
    public const string GRAVE_TAG = "Grave";
    public const string OCCUPIED_GRAVE_TAG = "OccGrave";
    public const string ARCH_TAG = "Arch";

    public static Vector3[] GRAVE_SPAWN_POSITIONS = {
        new Vector3(-65, 0, 85),
        new Vector3(-65, 0, -85),
        new Vector3(65, 0, 85),
        new Vector3(65, 0, -85)
    };

    public static class AbilitiesTags {
        public static class Seeker {
            public const string DEFAULT = "S_Flare";
            public const string FLARE = "S_Flare";
            public const string SURGE = "S_Surge";
            public const string INVISIBLE = "S_Invisible";
            public const string SHOW_ALL_HIDEMAN = "S_ShowAllHideman";
        }

        public static class Hideman {
            public const string DEFAULT = "H_Surge";
            public const string SURGE = "H_Surge";
            public const string INVISIBLE = "H_Invisible";
        }
    }
}
