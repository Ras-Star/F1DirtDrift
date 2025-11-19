using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace F1DirtDrift
{
    /// <summary>
    /// Singleton GameManager that orchestrates the entire race session.
    /// Handles race state, timing, countdown, and coordinates between all systems.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        [Header("References")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private GameObject resultsPanel;
        
        [Header("Audio")]
        [SerializeField] private AudioClip startClip;
        [SerializeField] private AudioClip finishClip;
        
        [Header("Race Configuration")]
        [SerializeField] private int totalLaps = 3;
        [SerializeField] private float countdownDuration = 3f;

        // Current race state
        public RaceState CurrentState { get; private set; } = RaceState.None;
        
        // References to other managers
        private TrackManager trackManager;
        private UIManager uiManager;
        private LeaderboardManager leaderboardManager;
        private AudioManager audioManager;
        
        // Race timing
        private float raceStartTime;
        private float currentRaceTime;
        private bool isRaceActive;
        
        // Car management
        private List<CarController> registeredCars = new List<CarController>();
        private Dictionary<CarController, RaceResult> carResults = new Dictionary<CarController, RaceResult>();
        
        // Staggered start tracking
        private Queue<CarController> carsWaitingToStart = new Queue<CarController>();
        private HashSet<CarController> carsCurrentlyRacing = new HashSet<CarController>();

        #region Initialization

        private void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Find manager references
            trackManager = FindFirstObjectByType<TrackManager>();
            uiManager = FindFirstObjectByType<UIManager>();
            leaderboardManager = FindFirstObjectByType<LeaderboardManager>();
            audioManager = AudioManager.Instance;

            if (trackManager == null)
            {
                Debug.LogWarning("[GameManager] TrackManager not found! Using fallback mode.");
            }

            // Ensure UI elements are set up
            if (countdownText != null)
            {
                countdownText.gameObject.SetActive(false);
            }
            
            if (resultsPanel != null)
            {
                resultsPanel.SetActive(false);
            }

            Debug.Log("[GameManager] Initialized successfully.");
        }

        #endregion

        #region Race Control

        /// <summary>
        /// Registers a car to participate in the race
        /// </summary>
        public void RegisterCar(CarController car, string carName, bool isPlayer)
        {
            if (!registeredCars.Contains(car))
            {
                registeredCars.Add(car);
                carResults[car] = new RaceResult(carName, isPlayer);
                Debug.Log($"[GameManager] Registered car: {carName} (Player: {isPlayer})");
            }
        }

        /// <summary>
        /// Starts the race with staggered start for all cars
        /// </summary>
        public void StartRace()
        {
            if (CurrentState != RaceState.None)
            {
                Debug.LogWarning("[GameManager] Race already in progress!");
                return;
            }

            // Reset all race data
            isRaceActive = false;
            currentRaceTime = 0f;
            carsWaitingToStart.Clear();
            carsCurrentlyRacing.Clear();

            // Order cars by start position (player choice determines their position)
            // For now, first car starts immediately
            foreach (var car in registeredCars.OrderBy(c => c.GetStartPosition()))
            {
                carsWaitingToStart.Enqueue(car);
            }

            // Start countdown for first car
            StartCoroutine(CountdownRoutine());
        }

        private System.Collections.IEnumerator CountdownRoutine()
        {
            CurrentState = RaceState.Countdown;
            
            if (countdownText != null)
            {
                countdownText.gameObject.SetActive(true);
            }

            // Play countdown audio
            if (audioManager != null && startClip != null)
            {
                audioManager.PlaySFX(startClip);
            }

            // Countdown: 3...2...1...GO!
            for (int i = (int)countdownDuration; i > 0; i--)
            {
                if (countdownText != null)
                {
                    countdownText.text = i.ToString();
                }
                yield return new WaitForSeconds(1f);
            }

            if (countdownText != null)
            {
                countdownText.text = "GO!";
            }
            
            yield return new WaitForSeconds(0.5f);
            
            if (countdownText != null)
            {
                countdownText.gameObject.SetActive(false);
            }

            // Start the first car
            BeginRacing();
        }

        private void BeginRacing()
        {
            CurrentState = RaceState.Racing;
            isRaceActive = true;
            raceStartTime = Time.time;

            // Start first car in queue
            if (carsWaitingToStart.Count > 0)
            {
                var firstCar = carsWaitingToStart.Dequeue();
                firstCar.StartRacing();
                carsCurrentlyRacing.Add(firstCar);
                Debug.Log($"[GameManager] First car started: {firstCar.name}");
            }
        }

        /// <summary>
        /// Called when a car completes a lap
        /// </summary>
        public void OnCarLapCompleted(CarController car, int lapNumber, float lapTime, float totalTime)
        {
            if (!carResults.ContainsKey(car)) return;

            // Record lap data
            carResults[car].AddLap(lapNumber, lapTime, totalTime);
            
            Debug.Log($"[GameManager] {carResults[car].carName} completed lap {lapNumber} - Time: {lapTime:F2}s");

            // Update leaderboard
            if (leaderboardManager != null)
            {
                leaderboardManager.UpdateLeaderboard(carResults.Values.ToList());
            }

            // Check if this car finished lap 1 - time to start next car (staggered start)
            if (lapNumber == 2 && carsWaitingToStart.Count > 0)
            {
                var nextCar = carsWaitingToStart.Dequeue();
                nextCar.StartRacing();
                carsCurrentlyRacing.Add(nextCar);
                Debug.Log($"[GameManager] Next car started (staggered): {nextCar.name}");
            }

            // Check if car finished all laps
            if (lapNumber >= totalLaps)
            {
                OnCarFinished(car);
            }
        }

        /// <summary>
        /// Called when a car completes all laps
        /// </summary>
        private void OnCarFinished(CarController car)
        {
            carsCurrentlyRacing.Remove(car);
            Debug.Log($"[GameManager] {carResults[car].carName} finished the race!");

            // Check if all cars have finished
            if (carsCurrentlyRacing.Count == 0 && carsWaitingToStart.Count == 0)
            {
                EndRace();
            }
        }

        /// <summary>
        /// Ends the race and shows results
        /// </summary>
        private void EndRace()
        {
            isRaceActive = false;
            CurrentState = RaceState.Finished;

            // Calculate final positions based on total race time
            var sortedResults = carResults.Values
                .OrderBy(r => r.totalRaceTime)
                .ToList();

            for (int i = 0; i < sortedResults.Count; i++)
            {
                sortedResults[i].finalPosition = i + 1;
            }

            // Play finish audio
            if (audioManager != null && finishClip != null)
            {
                audioManager.PlaySFX(finishClip);
            }

            // Show results panel
            if (resultsPanel != null)
            {
                resultsPanel.SetActive(true);
                var resultsManager = resultsPanel.GetComponent<ResultsManager>();
                if (resultsManager != null)
                {
                    resultsManager.DisplayResults(sortedResults);
                }
            }

            Debug.Log("[GameManager] Race finished! Results displayed.");
        }

        #endregion

        #region Timer Display

        private void Update()
        {
            if (isRaceActive)
            {
                currentRaceTime = Time.time - raceStartTime;
                UpdateTimerDisplay();
            }
        }

        private void UpdateTimerDisplay()
        {
            if (timerText != null)
            {
                int minutes = (int)(currentRaceTime / 60f);
                float seconds = currentRaceTime % 60f;
                timerText.text = $"{minutes:00}:{seconds:00.00}";
            }
        }

        #endregion

        #region Public Accessors

        public bool IsRaceActive() => isRaceActive;
        public RaceState GetCurrentState() => CurrentState;
        public int GetTotalLaps() => totalLaps;
        public float GetRaceStartTime() => raceStartTime;

        /// <summary>
        /// Restarts the current race scene
        /// </summary>
        public void RestartRace()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Returns to main menu
        /// </summary>
        public void ReturnToMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        #endregion
    }
}
