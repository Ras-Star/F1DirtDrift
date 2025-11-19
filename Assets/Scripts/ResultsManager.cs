using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace F1DirtDrift
{
    /// <summary>
    /// Displays final race results on the results panel.
    /// Shows positions, times, and provides replay/menu options.
    /// </summary>
    public class ResultsManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI positionText;
        [SerializeField] private TextMeshProUGUI playerTimeText;
        [SerializeField] private TextMeshProUGUI playerBestLapText;
        [SerializeField] private TextMeshProUGUI[] opponentNameTexts; // For AI cars
        [SerializeField] private TextMeshProUGUI[] opponentTimeTexts;

        [Header("Buttons")]
        [SerializeField] private Button replayButton;
        [SerializeField] private Button nextTrackButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Visual Effects")]
        [SerializeField] private GameObject[] positionMedals; // Gold, Silver, Bronze
        [SerializeField] private Color goldColor = new Color(1f, 0.84f, 0f);
        [SerializeField] private Color silverColor = new Color(0.75f, 0.75f, 0.75f);
        [SerializeField] private Color bronzeColor = new Color(0.8f, 0.5f, 0.2f);

        #region Initialization

        private void Start()
        {
            // Assign button listeners
            if (replayButton != null)
                replayButton.onClick.AddListener(OnReplayClicked);
            
            if (nextTrackButton != null)
                nextTrackButton.onClick.AddListener(OnNextTrackClicked);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        #endregion

        #region Display Results

        /// <summary>
        /// Displays race results for all cars
        /// </summary>
        public void DisplayResults(List<RaceResult> results)
        {
            if (results == null || results.Count == 0)
            {
                Debug.LogWarning("[ResultsManager] No results to display!");
                return;
            }

            // Find player result
            RaceResult playerResult = results.FirstOrDefault(r => r.isPlayer);
            if (playerResult == null)
            {
                Debug.LogWarning("[ResultsManager] Player result not found!");
                return;
            }

            // Display player position
            DisplayPlayerPosition(playerResult);

            // Display player times
            DisplayPlayerTimes(playerResult);

            // Display opponent results
            DisplayOpponentResults(results.Where(r => !r.isPlayer).ToList());

            // Show appropriate medal
            ShowPositionMedal(playerResult.finalPosition);

            Debug.Log($"[ResultsManager] Displayed results - Player position: {playerResult.finalPosition}");
        }

        /// <summary>
        /// Displays player's final position
        /// </summary>
        private void DisplayPlayerPosition(RaceResult result)
        {
            if (positionText == null) return;

            string positionString = GetPositionString(result.finalPosition);
            string medal = GetMedalEmoji(result.finalPosition);
            
            positionText.text = $"POSITION: {positionString} {medal}";
            positionText.color = GetPositionColor(result.finalPosition);
        }

        /// <summary>
        /// Displays player's race times
        /// </summary>
        private void DisplayPlayerTimes(RaceResult result)
        {
            // Total race time
            if (playerTimeText != null)
            {
                playerTimeText.text = $"YOUR TIME:     {result.GetFormattedTotalTime()}";
            }

            // Best lap time
            if (playerBestLapText != null)
            {
                // Find which lap was best
                int bestLapNumber = 1;
                for (int i = 0; i < result.laps.Length; i++)
                {
                    if (result.laps[i].lapTime == result.bestLapTime)
                    {
                        bestLapNumber = i + 1;
                        break;
                    }
                }

                playerBestLapText.text = $"BEST LAP:      {result.GetFormattedBestLap()} (Lap {bestLapNumber})";
            }
        }

        /// <summary>
        /// Displays opponent results
        /// </summary>
        private void DisplayOpponentResults(List<RaceResult> opponents)
        {
            for (int i = 0; i < opponents.Count && i < 2; i++) // Max 2 opponents
            {
                if (opponentNameTexts != null && i < opponentNameTexts.Length && opponentNameTexts[i] != null)
                {
                    opponentNameTexts[i].text = $"{opponents[i].finalPosition}. {opponents[i].carName}";
                }

                if (opponentTimeTexts != null && i < opponentTimeTexts.Length && opponentTimeTexts[i] != null)
                {
                    opponentTimeTexts[i].text = opponents[i].GetFormattedTotalTime();
                }
            }

            // Clear unused slots
            for (int i = opponents.Count; i < 2; i++)
            {
                if (opponentNameTexts != null && i < opponentNameTexts.Length && opponentNameTexts[i] != null)
                    opponentNameTexts[i].text = "";
                
                if (opponentTimeTexts != null && i < opponentTimeTexts.Length && opponentTimeTexts[i] != null)
                    opponentTimeTexts[i].text = "";
            }
        }

        /// <summary>
        /// Shows the appropriate medal based on position
        /// </summary>
        private void ShowPositionMedal(int position)
        {
            if (positionMedals == null) return;

            // Hide all medals first
            foreach (var medal in positionMedals)
            {
                if (medal != null) medal.SetActive(false);
            }

            // Show appropriate medal (1st = Gold, 2nd = Silver, 3rd = Bronze)
            if (position >= 1 && position <= positionMedals.Length && positionMedals[position - 1] != null)
            {
                positionMedals[position - 1].SetActive(true);
            }
        }

        #endregion

        #region Helper Methods

        private string GetPositionString(int position)
        {
            switch (position)
            {
                case 1: return "1st";
                case 2: return "2nd";
                case 3: return "3rd";
                default: return $"{position}th";
            }
        }

        private string GetMedalEmoji(int position)
        {
            switch (position)
            {
                case 1: return "🥇";
                case 2: return "🥈";
                case 3: return "🥉";
                default: return "";
            }
        }

        private Color GetPositionColor(int position)
        {
            switch (position)
            {
                case 1: return goldColor;
                case 2: return silverColor;
                case 3: return bronzeColor;
                default: return Color.white;
            }
        }

        #endregion

        #region Button Handlers

        private void OnReplayClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartRace();
            }
        }

        private void OnNextTrackClicked()
        {
            // TODO: Load next track in sequence
            Debug.Log("[ResultsManager] Next track feature not yet implemented");
            OnMainMenuClicked(); // For now, go to main menu
        }

        private void OnMainMenuClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReturnToMenu();
            }
        }

        #endregion
    }
}
