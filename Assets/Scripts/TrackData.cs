using UnityEngine;

namespace F1DirtDrift
{
    /// <summary>
    /// ScriptableObject that stores track information and metadata.
    /// Used for track selection menu and loading the correct scene.
    /// </summary>
    [CreateAssetMenu(fileName = "NewTrackData", menuName = "F1DirtDrift/Track Data", order = 2)]
    public class TrackData : ScriptableObject
    {
        [Header("Track Identity")]
        [Tooltip("Display name of the track")]
        public string trackName = "Dirt Circuit 1";
        
        [Tooltip("Scene name to load (e.g., 'Track_1')")]
        public string sceneName = "Track_1";
        
        [Tooltip("Preview image for track selection menu")]
        public Sprite trackPreviewSprite;

        [Header("Race Configuration")]
        [Tooltip("Total number of laps for this track")]
        [Range(1, 10)]
        public int totalLaps = 3;
        
        [Tooltip("Recommended difficulty rating (1 = Easy, 5 = Hard)")]
        [Range(1, 5)]
        public int difficultyRating = 3;
        
        [Tooltip("Estimated lap time for leaderboard reference (seconds)")]
        public float targetLapTime = 60f;

        [Header("Track Statistics")]
        [Tooltip("Track length in units (calculated from waypoints)")]
        public float trackLength = 1000f;
        
        [Tooltip("Number of corners on this track")]
        public int cornerCount = 8;
        
        [Tooltip("Track description for UI display")]
        [TextArea(3, 5)]
        public string trackDescription = "A challenging dirt track with tight corners and long straights.";

        [Header("Best Times (Runtime Data)")]
        [Tooltip("Player's best lap time on this track")]
        public float playerBestLapTime = 0f;
        
        [Tooltip("Player's best total race time on this track")]
        public float playerBestRaceTime = 0f;
        
        [Tooltip("Whether this track has been unlocked")]
        public bool isUnlocked = true;
    }
}
