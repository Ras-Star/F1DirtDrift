using UnityEngine;
using UnityEngine.EventSystems;

namespace F1DirtDrift
{
    /// <summary>
    /// Touch-based steering wheel controller for mobile input.
    /// Provides visual feedback and sends steering input to CarController.
    /// </summary>
    public class SteeringWheelController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("References")]
        [Tooltip("The player's car controller to send input to")]
        [SerializeField] private CarController playerCarController;

        [Header("Steering Settings")]
        [Tooltip("Maximum rotation angle for the steering wheel")]
        [SerializeField] private float maxSteerAngle = 180f;
        
        [Tooltip("Speed at which wheel snaps back to center")]
        [SerializeField] private float snapBackSpeed = 5f;
        
        [Tooltip("Sensitivity of steering response")]
        [Range(0.1f, 3f)]
        [SerializeField] private float steeringSensitivity = 1f;

        // Current state
        private RectTransform rectTransform;
        private float currentAngle = 0f;
        private bool isTouching = false;
        private Vector2 centerPoint;
        private float steerInput = 0f;

        #region Initialization

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            // Auto-find player car if not assigned
            if (playerCarController == null)
            {
                GameObject playerCar = GameObject.FindGameObjectWithTag("Player");
                if (playerCar != null)
                {
                    playerCarController = playerCar.GetComponent<CarController>();
                }
            }

            if (playerCarController == null)
            {
                Debug.LogWarning("[SteeringWheelController] Player car not found!");
            }

            // Calculate center point
            centerPoint = rectTransform.anchoredPosition;
        }

        #endregion

        #region Touch Input

        public void OnPointerDown(PointerEventData eventData)
        {
            isTouching = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isTouching = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isTouching) return;

            // Calculate drag angle relative to wheel center
            Vector2 direction = eventData.position - (Vector2)rectTransform.position;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

            // Calculate steering delta
            float angleDelta = Mathf.DeltaAngle(currentAngle, angle);
            currentAngle += angleDelta * steeringSensitivity;

            // Clamp angle
            currentAngle = Mathf.Clamp(currentAngle, -maxSteerAngle, maxSteerAngle);

            // Update visual rotation
            rectTransform.localEulerAngles = new Vector3(0, 0, -currentAngle);

            // Calculate normalized steer input (-1 to 1)
            steerInput = currentAngle / maxSteerAngle;

            // Send to car controller
            SendSteeringInput();
        }

        #endregion

        #region Update Loop

        private void Update()
        {
            // Snap back to center when not touching
            if (!isTouching && Mathf.Abs(currentAngle) > 0.1f)
            {
                currentAngle = Mathf.Lerp(currentAngle, 0f, snapBackSpeed * Time.deltaTime);
                rectTransform.localEulerAngles = new Vector3(0, 0, -currentAngle);
                
                steerInput = currentAngle / maxSteerAngle;
                SendSteeringInput();
            }
        }

        #endregion

        #region Input Transmission

        /// <summary>
        /// Sends current steering angle to the player's car
        /// </summary>
        private void SendSteeringInput()
        {
            if (playerCarController != null && playerCarController.IsPlayer())
            {
                // For now, just steering. Acceleration/brake handled by UI buttons
                playerCarController.SetTouchInput(0f, 0f, steerInput);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Manually sets the player car reference
        /// </summary>
        public void SetPlayerCar(CarController car)
        {
            playerCarController = car;
        }

        /// <summary>
        /// Resets wheel to center position
        /// </summary>
        public void ResetWheel()
        {
            currentAngle = 0f;
            steerInput = 0f;
            rectTransform.localEulerAngles = Vector3.zero;
        }

        #endregion
    }
}
