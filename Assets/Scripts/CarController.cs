using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace F1DirtDrift
{
    /// <summary>
    /// Controls car physics, input handling, and AI navigation.
    /// Supports both keyboard (Unity Editor) and touch (mobile) controls.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class CarController : MonoBehaviour
    {
        [Header("Car Data")]
        [SerializeField] private CarData carData;
        [SerializeField] private bool isPlayer = true;
        [SerializeField] private StartPosition startPosition = StartPosition.First;

        [Header("Input Mode")]
        [SerializeField] private InputMode inputMode = InputMode.KeyboardMouse;

        [Header("Visual Components")]
        [SerializeField] private SpriteRenderer carSprite;
        [SerializeField] private ParticleSystem smokeParticles;

        // Physics components
        private Rigidbody2D rb;
        private float currentSpeed = 0f;
        private float currentSteerAngle = 0f;

        // Input values
        private float accelerationInput = 0f;
        private float brakeInput = 0f;
        private float steerInput = 0f;
        private bool isDrifting = false;

        // AI Navigation
        private List<Vector3> waypoints = new List<Vector3>();
        private List<bool> waypointIsCurve = new List<bool>();
        private int currentWaypointIndex = 0;
        private float waypointTolerance = 15f;

        // Race tracking
        private bool isRacing = false;
        private int currentLap = 0;
        private float lapStartTime = 0f;
        private float totalRaceTime = 0f;
        private int waypointsPassed = 0;

        // Performance stats from CarData
        private float topSpeed;
        private float acceleration;
        private float brakeForce;
        private float steeringSpeed;
        private float handling;
        private float driftThreshold;

        #region Initialization

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            carSprite = GetComponent<SpriteRenderer>();

            // Auto-detect input mode based on platform
            #if UNITY_EDITOR || UNITY_STANDALONE
                inputMode = InputMode.KeyboardMouse;
            #elif UNITY_ANDROID || UNITY_IOS
                inputMode = InputMode.Touch;
            #endif
        }

        private void Start()
        {
            InitializeCar();
        }

        private void InitializeCar()
        {
            // Load stats from CarData
            if (carData != null)
            {
                topSpeed = carData.topSpeed;
                acceleration = carData.acceleration;
                brakeForce = carData.brakeForce;
                steeringSpeed = carData.steeringSpeed;
                handling = carData.handling;
                driftThreshold = carData.driftThreshold;

                // Apply car sprite and color
                if (carSprite != null && carData.carSprite != null)
                {
                    carSprite.sprite = carData.carSprite;
                    carSprite.color = carData.carColor;
                }

                // Instantiate smoke particles
                if (carData.smokeParticlePrefab != null && smokeParticles == null)
                {
                    GameObject particles = Instantiate(carData.smokeParticlePrefab, transform);
                    smokeParticles = particles.GetComponent<ParticleSystem>();
                }
            }
            else
            {
                Debug.LogWarning($"[CarController] {gameObject.name} has no CarData assigned! Using defaults.");
                topSpeed = 150f;
                acceleration = 50f;
                brakeForce = 100f;
                steeringSpeed = 180f;
                handling = 1f;
                driftThreshold = 30f;
            }

            // Configure Rigidbody2D
            rb.gravityScale = 0f;
            rb.linearDamping = 0.5f;
            rb.angularDamping = 2f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            // Stop smoke particles initially
            if (smokeParticles != null)
            {
                var emission = smokeParticles.emission;
                emission.enabled = false;
            }

            Debug.Log($"[CarController] {gameObject.name} initialized (Player: {isPlayer}, Input: {inputMode})");
        }

        #endregion

        #region Input Handling

        private void Update()
        {
            if (!isRacing) return;

            if (isPlayer)
            {
                HandlePlayerInput();
            }
            else
            {
                HandleAIInput();
            }

            // Check drift state
            CheckDriftState();
        }

        /// <summary>
        /// Handles keyboard/mouse input for Unity Editor testing
        /// </summary>
        private void HandlePlayerInput()
        {
            if (inputMode == InputMode.KeyboardMouse)
            {
                // Keyboard input (WASD / Arrow Keys)
                accelerationInput = 0f;
                brakeInput = 0f;

                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                {
                    accelerationInput = 1f;
                }
                
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                {
                    brakeInput = 1f;
                }

                // Steering
                steerInput = 0f;
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    steerInput = -1f;
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    steerInput = 1f;
                }
            }
            // Touch input handled via SteeringWheelController and UI buttons
        }

        /// <summary>
        /// AI navigation logic using waypoints
        /// </summary>
        private void HandleAIInput()
        {
            if (waypoints.Count == 0) return;

            // Get current waypoint
            Vector3 targetWaypoint = waypoints[currentWaypointIndex];
            Vector3 directionToWaypoint = (targetWaypoint - transform.position).normalized;

            // Calculate steering angle
            float angleToWaypoint = Vector2.SignedAngle(transform.up, directionToWaypoint);
            steerInput = Mathf.Clamp(angleToWaypoint / 45f, -1f, 1f);

            // AI speed adjustment based on corner detection
            bool isApproachingCurve = waypointIsCurve.Count > currentWaypointIndex && waypointIsCurve[currentWaypointIndex];
            float targetSpeedMultiplier = isApproachingCurve ? carData.aiCornerSpeedReduction : carData.aiSpeedMultiplier;
            float aiTargetSpeed = topSpeed * targetSpeedMultiplier;

            // Accelerate or brake to match target speed
            if (currentSpeed < aiTargetSpeed)
            {
                accelerationInput = 1f;
                brakeInput = 0f;
            }
            else
            {
                accelerationInput = 0f;
                brakeInput = 0.3f; // Light braking
            }

            // Check if waypoint reached
            float distanceToWaypoint = Vector3.Distance(transform.position, targetWaypoint);
            if (distanceToWaypoint < waypointTolerance)
            {
                waypointsPassed++;
                
                // Move to next waypoint
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;

                // Log every 5th waypoint to reduce spam
                if (waypointsPassed % 5 == 0)
                {
                    Debug.Log($"[CarController AI] {gameObject.name} reached waypoint {currentWaypointIndex}/{waypoints.Count}");
                }
            }
        }

        /// <summary>
        /// Sets input from touch controls (called by UI buttons and steering wheel)
        /// </summary>
        public void SetTouchInput(float accel, float brake, float steer)
        {
            if (inputMode == InputMode.Touch && isPlayer)
            {
                accelerationInput = accel;
                brakeInput = brake;
                steerInput = steer;
            }
        }

        #endregion

        #region Physics

        private void FixedUpdate()
        {
            if (!isRacing) return;

            ApplyAcceleration();
            ApplySteering();
            ApplyDrag();
        }

        private void ApplyAcceleration()
        {
            // Calculate target speed based on input
            float targetSpeed = 0f;

            if (accelerationInput > 0)
            {
                targetSpeed = topSpeed;
            }
            else if (brakeInput > 0)
            {
                targetSpeed = 0f;
                // Apply braking force
                currentSpeed = Mathf.Max(0f, currentSpeed - brakeForce * Time.fixedDeltaTime);
            }

            // Accelerate towards target speed
            if (accelerationInput > 0 && currentSpeed < topSpeed)
            {
                currentSpeed += acceleration * accelerationInput * Time.fixedDeltaTime;
                currentSpeed = Mathf.Min(currentSpeed, topSpeed);
            }

            // Apply forward movement
            Vector2 forwardVelocity = transform.up * currentSpeed;
            rb.linearVelocity = forwardVelocity;
        }

        private void ApplySteering()
        {
            if (Mathf.Abs(steerInput) > 0.01f && currentSpeed > 1f)
            {
                // Calculate steering based on speed (slower = tighter turns)
                float steeringFactor = Mathf.Clamp01(currentSpeed / topSpeed);
                float steerAmount = steerInput * steeringSpeed * handling * steeringFactor * Time.fixedDeltaTime;
                
                currentSteerAngle += steerAmount;
                
                // Apply rotation
                float newRotation = transform.eulerAngles.z - steerAmount;
                rb.MoveRotation(newRotation);
            }
        }

        private void ApplyDrag()
        {
            // Natural deceleration when not accelerating
            if (accelerationInput == 0 && brakeInput == 0)
            {
                currentSpeed *= 0.98f; // Gradual slowdown
                if (currentSpeed < 0.1f)
                {
                    currentSpeed = 0f;
                }
            }
        }

        #endregion

        #region Drift & Effects

        private void CheckDriftState()
        {
            // Check if car is drifting (velocity angle vs forward angle)
            float velocityAngle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            float carAngle = transform.eulerAngles.z;
            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(velocityAngle, carAngle + 90f));

            bool wasDrifting = isDrifting;
            isDrifting = angleDifference > driftThreshold && currentSpeed > topSpeed * 0.3f;

            // Control smoke particles
            if (smokeParticles != null)
            {
                var emission = smokeParticles.emission;
                emission.enabled = isDrifting || brakeInput > 0.5f;
            }

            // Play skid sound when starting to drift
            if (isDrifting && !wasDrifting && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySkidSound();
            }
        }

        #endregion

        #region Race Control

        /// <summary>
        /// Starts the car racing (called by GameManager for staggered starts)
        /// </summary>
        public void StartRacing()
        {
            isRacing = true;
            currentLap = 1;
            lapStartTime = Time.time;
            totalRaceTime = 0f;
            waypointsPassed = 0;
            currentWaypointIndex = 0;

            Debug.Log($"[CarController] {gameObject.name} started racing!");
        }

        /// <summary>
        /// Called when car crosses finish line (lap completed)
        /// </summary>
        public void OnLapCompleted()
        {
            if (!isRacing) return;

            float lapTime = Time.time - lapStartTime;
            totalRaceTime += lapTime;

            // Notify GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnCarLapCompleted(this, currentLap, lapTime, totalRaceTime);
            }

            // Check if race is finished
            if (currentLap >= GameManager.Instance.GetTotalLaps())
            {
                FinishRace();
            }
            else
            {
                // Start next lap
                currentLap++;
                lapStartTime = Time.time;
                Debug.Log($"[CarController] {gameObject.name} lap {currentLap} started");
            }
        }

        private void FinishRace()
        {
            isRacing = false;
            Debug.Log($"[CarController] {gameObject.name} finished race! Total time: {totalRaceTime:F2}s");
        }

        #endregion

        #region Waypoint System

        /// <summary>
        /// Sets waypoints for AI navigation (called by TrackManager)
        /// </summary>
        public void SetWaypoints(List<Vector3> waypointsList, List<bool> curveFlags)
        {
            waypoints = waypointsList;
            waypointIsCurve = curveFlags;
            currentWaypointIndex = 0;

            if (!isPlayer)
            {
                Debug.Log($"[CarController AI] {gameObject.name} received {waypoints.Count} waypoints");
            }
        }

        #endregion

        #region Public Accessors

        public bool IsPlayer() => isPlayer;
        public StartPosition GetStartPosition() => startPosition;
        public int GetCurrentLap() => currentLap;
        public float GetCurrentSpeed() => currentSpeed;
        public bool IsRacing() => isRacing;

        #endregion

        #region Debug Visualization

        private void OnDrawGizmos()
        {
            if (!isPlayer || waypoints.Count == 0) return;

            // Draw next waypoint for debugging
            if (currentWaypointIndex < waypoints.Count)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(waypoints[currentWaypointIndex], 5f);
                Gizmos.DrawLine(transform.position, waypoints[currentWaypointIndex]);
            }
        }

        #endregion
    }
}
