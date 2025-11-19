using UnityEngine;
using UnityEditor;
using System.IO;

namespace F1DirtDrift.Editor
{
    /// <summary>
    /// Editor utility to automatically create ScriptableObject instances
    /// for CarData and TrackData assets.
    /// </summary>
    public static class ScriptableObjectGenerator
    {
        [MenuItem("F1DirtDrift/Generate All ScriptableObjects")]
        public static void GenerateAllAssets()
        {
            GenerateCarDataAssets();
            GenerateTrackDataAssets();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[ScriptableObjectGenerator] All assets created successfully!");
        }

        [MenuItem("F1DirtDrift/Generate CarData Assets")]
        public static void GenerateCarDataAssets()
        {
            string folderPath = "Assets/Resources/CarData";
            
            // Create folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "CarData");
            }

            // Player Car
            CarData playerCar = ScriptableObject.CreateInstance<CarData>();
            playerCar.carName = "Player Car";
            playerCar.topSpeed = 150f;
            playerCar.acceleration = 55f;
            playerCar.brakeForce = 110f;
            playerCar.steeringSpeed = 180f;
            playerCar.handling = 1.2f;
            playerCar.driftThreshold = 30f;
            playerCar.carColor = new Color(0.2f, 0.6f, 1f); // Blue
            AssetDatabase.CreateAsset(playerCar, $"{folderPath}/PlayerCar.asset");
            Debug.Log("[Generator] Created PlayerCar.asset");

            // AI Car 1 (Red)
            CarData aiCar1 = ScriptableObject.CreateInstance<CarData>();
            aiCar1.carName = "AI Racer 1";
            aiCar1.topSpeed = 145f;
            aiCar1.acceleration = 50f;
            aiCar1.brakeForce = 100f;
            aiCar1.steeringSpeed = 170f;
            aiCar1.handling = 1f;
            aiCar1.driftThreshold = 32f;
            aiCar1.aiSpeedMultiplier = 0.85f;
            aiCar1.aiCornerSpeedReduction = 0.65f;
            aiCar1.carColor = new Color(1f, 0.2f, 0.2f); // Red
            AssetDatabase.CreateAsset(aiCar1, $"{folderPath}/AI_Car_1.asset");
            Debug.Log("[Generator] Created AI_Car_1.asset");

            // AI Car 2 (Yellow)
            CarData aiCar2 = ScriptableObject.CreateInstance<CarData>();
            aiCar2.carName = "AI Racer 2";
            aiCar2.topSpeed = 140f;
            aiCar2.acceleration = 48f;
            aiCar2.brakeForce = 95f;
            aiCar2.steeringSpeed = 165f;
            aiCar2.handling = 0.9f;
            aiCar2.driftThreshold = 35f;
            aiCar2.aiSpeedMultiplier = 0.75f;
            aiCar2.aiCornerSpeedReduction = 0.6f;
            aiCar2.carColor = new Color(1f, 0.9f, 0.2f); // Yellow
            AssetDatabase.CreateAsset(aiCar2, $"{folderPath}/AI_Car_2.asset");
            Debug.Log("[Generator] Created AI_Car_2.asset");
        }

        [MenuItem("F1DirtDrift/Generate TrackData Assets")]
        public static void GenerateTrackDataAssets()
        {
            string folderPath = "Assets/Resources/TrackData";
            
            // Create folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "TrackData");
            }

            // Track 1
            TrackData track1 = ScriptableObject.CreateInstance<TrackData>();
            track1.trackName = "Desert Circuit";
            track1.sceneName = "Track_1";
            track1.totalLaps = 3;
            track1.difficultyRating = 2;
            track1.targetLapTime = 65f;
            track1.trackLength = 950f;
            track1.cornerCount = 6;
            track1.trackDescription = "A beginner-friendly dirt track with wide corners and long straights. Perfect for learning the ropes!";
            track1.isUnlocked = true;
            AssetDatabase.CreateAsset(track1, $"{folderPath}/Track_1_Data.asset");
            Debug.Log("[Generator] Created Track_1_Data.asset");

            // Track 2
            TrackData track2 = ScriptableObject.CreateInstance<TrackData>();
            track2.trackName = "Mountain Pass";
            track2.sceneName = "Track_2";
            track2.totalLaps = 3;
            track2.difficultyRating = 3;
            track2.targetLapTime = 70f;
            track2.trackLength = 1100f;
            track2.cornerCount = 9;
            track2.trackDescription = "A challenging mountain track with elevation changes and technical hairpin turns.";
            track2.isUnlocked = true;
            AssetDatabase.CreateAsset(track2, $"{folderPath}/Track_2_Data.asset");
            Debug.Log("[Generator] Created Track_2_Data.asset");

            // Track 3
            TrackData track3 = ScriptableObject.CreateInstance<TrackData>();
            track3.trackName = "Canyon Sprint";
            track3.sceneName = "Track_3";
            track3.totalLaps = 3;
            track3.difficultyRating = 4;
            track3.targetLapTime = 75f;
            track3.trackLength = 1250f;
            track3.cornerCount = 11;
            track3.trackDescription = "An expert-level canyon track with tight chicanes, blind corners, and narrow passages. Only for the skilled!";
            track3.isUnlocked = true;
            AssetDatabase.CreateAsset(track3, $"{folderPath}/Track_3_Data.asset");
            Debug.Log("[Generator] Created Track_3_Data.asset");
        }
    }
}
