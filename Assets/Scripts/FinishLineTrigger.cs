using UnityEngine;

namespace F1DirtDrift
{
    /// <summary>
    /// Detects when cars cross the finish line to register lap completions.
    /// Auto-corrects position if misaligned with finish waypoint.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class FinishLineTrigger : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Unique identifier for this finish line")]
        [SerializeField] private string lapId = "Main";
        
        [Tooltip("If true, any collider triggers completion. If false, only specific collider.")]
        [SerializeField] private bool completeOnAnyCollider = false;
        
        [Tooltip("Tolerance for position validation (units)")]
        [SerializeField] private float positionTolerance = 5f;

        private BoxCollider2D triggerCollider;
        private TrackManager trackManager;
        private Vector3 finishWaypointPosition;
        private bool isInitialized = false;

        #region Initialization

        private void Start()
        {
            InitializeFinishLine();
        }

        private void InitializeFinishLine()
        {
            // Get or add BoxCollider2D
            triggerCollider = GetComponent<BoxCollider2D>();
            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true;
            }

            // Find TrackManager
            trackManager = FindFirstObjectByType<TrackManager>();
            if (trackManager == null)
            {
                Debug.LogWarning("[FinishLineTrigger] TrackManager not found!");
                return;
            }

            // Wait for TrackManager to initialize waypoints
            if (trackManager.IsInitialized())
            {
                ValidatePosition();
            }
            else
            {
                Invoke(nameof(ValidatePosition), 0.5f); // Retry after delay
            }

            isInitialized = true;
        }

        /// <summary>
        /// Validates and auto-corrects finish line position relative to finish waypoint
        /// </summary>
        private void ValidatePosition()
        {
            if (trackManager == null || !trackManager.IsInitialized()) return;

            var waypoints = trackManager.GetWaypoints();
            if (waypoints.Count == 0)
            {
                Debug.LogWarning("[FinishLineTrigger] No waypoints found for validation!");
                return;
            }

            // Finish line should align with first/last waypoint (lap completion point)
            finishWaypointPosition = waypoints[0];
            float distance = Vector3.Distance(transform.position, finishWaypointPosition);

            if (distance > positionTolerance)
            {
                Debug.LogWarning($"[FinishLineTrigger] Position misaligned by {distance:F2} units! Auto-correcting...");
                
                // Auto-correct position
                transform.position = finishWaypointPosition;
                
                Debug.Log($"[FinishLineTrigger] Position corrected to {finishWaypointPosition}");
            }
            else
            {
                Debug.Log($"[FinishLineTrigger] Position validated (within {distance:F2} units of waypoint)");
            }
        }

        #endregion

        #region Collision Detection

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if it's a car
            CarController car = other.GetComponent<CarController>();
            if (car == null) return;

            // Only process if car is racing
            if (!car.IsRacing()) return;

            // Complete lap
            car.OnLapCompleted();
            
            Debug.Log($"[FinishLineTrigger] {other.gameObject.name} crossed finish line (Lap {car.GetCurrentLap()})");
        }

        #endregion

        #region Debug Visualization

        private void OnDrawGizmos()
        {
            // Draw finish line trigger area
            Gizmos.color = Color.yellow;
            if (triggerCollider != null)
            {
                Gizmos.DrawWireCube(transform.position, triggerCollider.size);
            }
            else
            {
                Gizmos.DrawWireCube(transform.position, new Vector3(50f, 20f, 1f));
            }

            // Draw connection to finish waypoint if initialized
            if (isInitialized && trackManager != null && trackManager.IsInitialized())
            {
                var waypoints = trackManager.GetWaypoints();
                if (waypoints.Count > 0)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(transform.position, waypoints[0]);
                    Gizmos.DrawSphere(waypoints[0], 10f);
                }
            }
        }

        #endregion
    }
}
