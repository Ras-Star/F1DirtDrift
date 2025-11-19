using UnityEngine;

namespace F1DirtDrift
{
    /// <summary>
    /// ScriptableObject that stores car performance characteristics and visual data.
    /// Used for both player and AI cars to define speed, handling, and appearance.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCarData", menuName = "F1DirtDrift/Car Data", order = 1)]
    public class CarData : ScriptableObject
    {
        [Header("Car Identity")]
        [Tooltip("Display name of the car")]
        public string carName = "F1 Racer";
        
        [Tooltip("Car sprite for rendering")]
        public Sprite carSprite;
        
        [Tooltip("Preview sprite for garage/menu selection")]
        public Sprite previewSprite;

        [Header("Performance Stats")]
        [Tooltip("Maximum speed in units per second")]
        [Range(50f, 200f)]
        public float topSpeed = 150f;
        
        [Tooltip("Acceleration rate in units per second squared")]
        [Range(20f, 100f)]
        public float acceleration = 50f;
        
        [Tooltip("Braking force in units per second squared")]
        [Range(50f, 150f)]
        public float brakeForce = 100f;
        
        [Tooltip("Steering response (degrees per second)")]
        [Range(100f, 300f)]
        public float steeringSpeed = 180f;
        
        [Tooltip("How well the car handles (higher = tighter turns)")]
        [Range(0.5f, 2f)]
        public float handling = 1f;
        
        [Tooltip("Drift threshold angle (velocity vs forward vector)")]
        [Range(15f, 45f)]
        public float driftThreshold = 30f;

        [Header("AI Behavior (if AI controlled)")]
        [Tooltip("AI speed multiplier (0.7 = 70% of max speed)")]
        [Range(0.5f, 1f)]
        public float aiSpeedMultiplier = 0.8f;
        
        [Tooltip("Corner speed reduction for AI (0.6 = 60% speed in curves)")]
        [Range(0.4f, 0.9f)]
        public float aiCornerSpeedReduction = 0.6f;

        [Header("Visual Effects")]
        [Tooltip("Color tint for the car sprite")]
        public Color carColor = Color.white;
        
        [Tooltip("Particle system prefab for smoke/drift effects")]
        public GameObject smokeParticlePrefab;
    }
}
