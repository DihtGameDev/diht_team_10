using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {
    public static class PlayerPrefs {
        public const string SETTINGS = "settings";
    }

    public static class SceneName {
        public const string LAUNCHER = "Launcher";
        public const string GAME_SCENE = "Room for 5";
    }

    public const int USER_ID_LENGTH = 40;

    public const int RESPAWN_TIME = 10;
    public const int MAX_SKELETONS_IN_SCENE = 1;
    public const int MAX_PLAYERS_IN_SCENE = 5;
    public const int AI_ENEMIES_PER_PLAYER = 2;

    public const float SPAWN_SKELETONS_DELAY = 4f;

    public const int ADDED_COINS_AFTER_WIN = 100;

    public const string PLAYER_TAG = "Player";
    public const string SEEKER_TAG = "Seeker";
    public const string HIDEMAN_TAG = "Hideman";
    public const string SKELETON_TAG = "Skeleton";
    public const string GRAVE_TAG = "Grave";
    public const string OCCUPIED_GRAVE_TAG = "OccGrave";
    public const string ARCH_TAG = "Arch";
    public const string ENEMY_SEEKER_AI_TAG = "AI";

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

    public static string[] HARDCODED_NICKNAMES_LIST = {
        "3D Waffle",
        "Hightower",
        "Papa Smurf",
        "57 Pixels",
        "Hog Butcher",
        "Pepper Legs",
        "101",
        "Houston",
        "Pinball Wizard",
        "Accidental Genius",
        "Hyper",
        "Pluto",
        "Alpha",
        "Jester",
        "Pogue",
        "Airport Hobo",
        "Jigsaw",
        "Prometheus",
        "Bearded Angler",
        "Joker's Grin",
        "Psycho Thinker",
        "Beetle King",
        "Judge",
        "Pusher",
        "Bitmap",
        "Junkyard Dog",
        "Riff Raff",
        "Blister",
        "K-9",
        "Roadblock",
        "Bowie",
        "Keystone",
        "Rooster",
        "Bowler",
        "Kickstart",
        "Sandbox",
        "Breadmaker",
        "Kill Switch",
        "Scrapper",
        "Broomspun",
        "Kingfisher",
        "Screwtape",
        "Buckshot",
        "Kitchen",
        "Sexual Chocolate",
        "Bugger",
        "Knuckles",
        "Shadow Chaser",
        "Cabbie",
        "Lady Killer",
        "Sherwood Gladiator",
        "Candy Butcher",
        "Liquid Science",
        "Shooter",
        "Capital F",
        "Little Cobra",
        "Sidewalk Enforcer",
        "Captain Peroxide",
        "Little General",
        "Skull Crusher",
        "Osprey",
        "Zero Charisma",
        "Highlander Monk",
        "Overrun",
        "Zesty Dragon",
        "Zod"
    };
}
