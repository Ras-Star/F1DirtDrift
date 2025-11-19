using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace F1DirtDrift
{
    /// <summary>
    /// Manages track waypoints for AI navigation and lap detection.
    /// Supports both auto-generation from track pieces and manual waypoint placement.
    /// </summary>
    public class TrackManager : MonoBehaviour
    {
        [Header("Waypoint Configuration")]
        [Tooltip("Parent transform containing track piece GameObjects")]
        [SerializeField] private Transform waypointsParent;
        
        [Tooltip("Angle threshold to detect curves (degrees)")]
        [SerializeField] private float curveDetectionAngle = 30f;
        
        [Tooltip("Total number of laps for this track")]
        [SerializeField] private int lapCount = 3;

        [Header("Manual Waypoints")]
        [Tooltip("Manually placed waypoint transforms")]
        [SerializeField] private Transform[] manualWaypoints;
        
        [Tooltip("Use manual waypoints instead of auto-generation")]
        [SerializeField] private bool useManualWaypoints = false;

        [Header("Auto-Generation Settings")]
        [Tooltip("Automatically generate waypoints from track pieces")]
        [SerializeField] private bool autoGenerateWaypoints = true;
        
        [Tooltip("Number of waypoints per track piece")]
        [Range(1, 5)]
        [SerializeField] private int waypointsPerTrackPiece = 3;

        [Header("Car Registration")]
        [Tooltip("All cars racing on this track")]
        [SerializeField] private CarController[] cars;

        // Generated waypoint data
        private List<Vector3> waypoints = new List<Vector3>();
        private List<bool> waypointIsCurve = new List<bool>();
        private List<CarController> registeredCars = new List<CarController>();
        
        // Finish line reference
        private Vector3 finishLinePosition;
        private bool isInitialized = false;

        #region Initialization

        private void Start()
        {
            InitializeTrack();
        }

        private void InitializeTrack()
        {
            if (useManualWaypoints && manualWaypoints != null && manualWaypoints.Length > 0)
            {
                GenerateWaypointsFromManual();
            }
            else if (autoGenerateWaypoints && waypointsParent != null)
            {
                GenerateWaypointsFromTrackPieces();
            }
            else
            {
                Debug.LogError("[TrackManager] No waypoint generation method configured!");
                return;
            }

            // Register cars with GameManager
            RegisterCarsWithGameManager();
            
            isInitialized = true;
            Debug.Log($"[TrackManager] Initialized with {waypoints.Count} waypoints");
        }

        #endregion

        #region Waypoint Generation

        /// <summary>
        /// Generates waypoints from manually placed transforms
        /// </summary>
        private void GenerateWaypointsFromManual()
        {
            waypoints.Clear();
            waypointIsCurve.Clear();

            foreach (var wp in manualWaypoints)
            {
                if (wp != null)
                {
                    waypoints.Add(wp.position);
                }
            }

            // Detect curves by checking angle changes
            DetectCurves();

            Debug.Log($"[TrackManager] Generated {waypoints.Count} waypoints from manual placement");
        }

        /// <summary>
        /// Auto-generates waypoints from track piece GameObjects
        /// </summary>
        private void GenerateWaypointsFromTrackPieces()
        {
            waypoints.Clear();
            waypointIsCurve.Clear();

            if (waypointsParent == null)
            {
                Debug.LogError("[TrackManager] WaypointsParent is not assigned!");
                return;
            }

            // Get all track pieces (children with SpriteRenderer)
            List<Transform> trackPieces = new List<Transform>();
            foreach (Transform child in waypointsParent)
            {
                if (child.GetComponent<SpriteRenderer>() != null)
                {
                    trackPieces.Add(child);
                }
            }

            if (trackPieces.Count == 0)
            {
                Debug.LogWarning("[TrackManager] No track pieces found with SpriteRenderer!");
                return;
            }

            // Sort track pieces by proximity to create a path
            List<Transform> sortedPieces = SortTrackPiecesByProximity(trackPieces);

            // Generate waypoints along each piece
            foreach (var piece in sortedPieces)
            {
                GenerateWaypointsForPiece(piece);
            }

            // Detect curves
            DetectCurves();

            Debug.Log($"[TrackManager] Auto-generated {waypoints.Count} waypoints from {sortedPieces.Count} track pieces");
        }

        /// <summary>
        /// Sorts track pieces to form a continuous path (nearest neighbor approach)
        /// </summary>
        private List<Transform> SortTrackPiecesByProximity(List<Transform> pieces)
        {
            if (pieces.Count == 0) return pieces;

            List<Transform> sorted = new List<Transform>();
            List<Transform> remaining = new List<Transform>(pieces);

            // Start with first piece
            sorted.Add(remaining[0]);
            remaining.RemoveAt(0);

            // Find nearest piece iteratively
            while (remaining.Count > 0)
            {
                Transform last = sorted[sorted.Count - 1];
                Transform nearest = remaining[0];
                float minDistance = Vector3.Distance(last.position, nearest.position);

                foreach (var piece in remaining)
                {
                    float distance = Vector3.Distance(last.position, piece.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = piece;
                    }
                }

                sorted.Add(nearest);
                remaining.Remove(nearest);
            }

            return sorted;
        }

        /// <summary>
        /// Generates waypoints for a single track piece
        /// </summary>
        private void GenerateWaypointsForPiece(Transform piece)
        {
            Vector3 position = piece.position;
            Vector3 forward = piece.right; // Track pieces typically face right

            // For corners, place waypoints at entry, apex, and exit
            if (IsPieceCurved(piece))
            {
                // Entry waypoint
                waypoints.Add(position - forward * 20f);
                // Apex waypoint
                waypoints.Add(position);
                // Exit waypoint
                waypoints.Add(position + forward * 20f);
            }
            else
            {
                // For straight pieces, evenly distribute waypoints
                float pieceLength = 50f; // Approximate length
                for (int i = 0; i < waypointsPerTrackPiece; i++)
                {
                    float t = (i + 1f) / (waypointsPerTrackPiece + 1f);
                    Vector3 waypointPos = position + forward * (t * pieceLength - pieceLength / 2f);
                    waypoints.Add(waypointPos);
                }
            }
        }

        /// <summary>
        /// Checks if a track piece represents a curve based on its name or rotation
        /// </summary>
        private bool IsPieceCurved(Transform piece)
        {
            string name = piece.name.ToLower();
            return name.Contains("corner") || name.Contains("curve") || name.Contains("turn");
        }

        /// <summary>
        /// Detects curves by analyzing angle changes between consecutive waypoints
        /// </summary>
        private void DetectCurves()
        {
            waypointIsCurve.Clear();

            if (waypoints.Count < 3)
            {
                for (int i = 0; i < waypoints.Count; i++)
                {
                    waypointIsCurve.Add(false);
                }
                return;
            }

            for (int i = 0; i < waypoints.Count; i++)
            {
                Vector3 prev = waypoints[i == 0 ? waypoints.Count - 1 : i - 1];
                Vector3 current = waypoints[i];
                Vector3 next = waypoints[(i + 1) % waypoints.Count];

                Vector3 dirToCurrent = (current - prev).normalized;
                Vector3 dirToNext = (next - current).normalized;

                float angle = Vector3.Angle(dirToCurrent, dirToNext);
                waypointIsCurve.Add(angle > curveDetectionAngle);
            }
        }

        #endregion

        #region Car Management

        /// <summary>
        /// Registers all cars with the GameManager
        /// </summary>
        private void RegisterCarsWithGameManager()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("[TrackManager] GameManager not found!");
                return;
            }

            foreach (var car in cars)
            {
                if (car != null)
                {
                    registeredCars.Add(car);
                    bool isPlayer = car.CompareTag("Player");
                    GameManager.Instance.RegisterCar(car, car.name, isPlayer);
                    
                    // Pass waypoints to car for AI navigation
                    car.SetWaypoints(waypoints, waypointIsCurve);
                }
            }

            Debug.Log($"[TrackManager] Registered {registeredCars.Count} cars");
        }

        #endregion

        #region Public Accessors

        public List<Vector3> GetWaypoints() => waypoints;
        public bool IsWaypointCurve(int index) => index >= 0 && index < waypointIsCurve.Count && waypointIsCurve[index];
        public int GetLapCount() => lapCount;
        public int GetTotalWaypoints() => waypoints.Count;
        public bool IsInitialized() => isInitialized;

        /// <summary>
        /// Gets the closest waypoint index to a given position
        /// </summary>
        public int GetClosestWaypointIndex(Vector3 position)
        {
            if (waypoints.Count == 0) return -1;

            int closest = 0;
            float minDistance = Vector3.Distance(position, waypoints[0]);

            for (int i = 1; i < waypoints.Count; i++)
            {
                float distance = Vector3.Distance(position, waypoints[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = i;
                }
            }

            return closest;
        }

        #endregion

        #region Debug Visualization

        private void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Count == 0) return;

            // Draw waypoints
            for (int i = 0; i < waypoints.Count; i++)
            {
                // Color: Red for curves, Green for straights
                Gizmos.color = (waypointIsCurve.Count > i && waypointIsCurve[i]) ? Color.red : Color.green;
                Gizmos.DrawSphere(waypoints[i], 10f);

                // Draw connections
                Gizmos.color = Color.yellow;
                Vector3 next = waypoints[(i + 1) % waypoints.Count];
                Gizmos.DrawLine(waypoints[i], next);
            }
        }

        #endregion
    }
}
