using UnityEngine;

namespace F1DirtDrift
{
    /// <summary>
    /// Controls particle effects for cars (smoke, dirt, sparks).
    /// Activates particles based on drift, brake, and collision events.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleController : MonoBehaviour
    {
        [Header("Particle Settings")]
        [Tooltip("Particle system to control")]
        [SerializeField] private ParticleSystem particles;
        
        [Tooltip("Emission rate when drifting")]
        [Range(10f, 100f)]
        [SerializeField] private float driftEmissionRate = 50f;
        
        [Tooltip("Emission rate when braking")]
        [Range(10f, 100f)]
        [SerializeField] private float brakeEmissionRate = 30f;

        [Header("Activation Thresholds")]
        [Tooltip("Minimum speed to emit particles (units/sec)")]
        [SerializeField] private float minimumSpeed = 20f;
        
        [Tooltip("Minimum drift angle to emit particles (degrees)")]
        [SerializeField] private float minimumDriftAngle = 20f;

        private ParticleSystem.EmissionModule emission;
        private ParticleSystem.MainModule mainModule;
        private bool isEmitting = false;

        #region Initialization

        private void Awake()
        {
            // Get particle system component
            if (particles == null)
            {
                particles = GetComponent<ParticleSystem>();
            }

            if (particles != null)
            {
                emission = particles.emission;
                mainModule = particles.main;
                emission.enabled = false; // Start disabled
            }
        }

        #endregion

        #region Particle Control

        /// <summary>
        /// Starts drift particle emission
        /// </summary>
        public void StartDriftParticles()
        {
            if (particles == null) return;

            if (!isEmitting)
            {
                emission.enabled = true;
                emission.rateOverTime = driftEmissionRate;
                isEmitting = true;
            }
        }

        /// <summary>
        /// Starts brake particle emission
        /// </summary>
        public void StartBrakeParticles()
        {
            if (particles == null) return;

            if (!isEmitting)
            {
                emission.enabled = true;
                emission.rateOverTime = brakeEmissionRate;
                isEmitting = true;
            }
        }

        /// <summary>
        /// Stops particle emission
        /// </summary>
        public void StopParticles()
        {
            if (particles == null) return;

            if (isEmitting)
            {
                emission.enabled = false;
                isEmitting = false;
            }
        }

        /// <summary>
        /// Updates particle emission based on car state
        /// </summary>
        public void UpdateParticles(bool isDrifting, bool isBraking, float currentSpeed)
        {
            if (particles == null) return;

            // Only emit if speed is above threshold
            if (currentSpeed < minimumSpeed)
            {
                StopParticles();
                return;
            }

            // Emit particles based on state
            if (isDrifting)
            {
                StartDriftParticles();
            }
            else if (isBraking)
            {
                StartBrakeParticles();
            }
            else
            {
                StopParticles();
            }
        }

        /// <summary>
        /// Plays a burst of particles (for collisions)
        /// </summary>
        public void EmitBurst(int count = 20)
        {
            if (particles != null)
            {
                particles.Emit(count);
            }
        }

        #endregion

        #region Public Accessors

        public bool IsEmitting() => isEmitting;

        /// <summary>
        /// Sets the color of emitted particles
        /// </summary>
        public void SetParticleColor(Color color)
        {
            if (particles != null)
            {
                var main = particles.main;
                main.startColor = color;
            }
        }

        #endregion
    }
}
