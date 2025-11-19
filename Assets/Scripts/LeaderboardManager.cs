using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace F1DirtDrift
{
    /// <summary>
    /// Manages the live mini-leaderboard display during races.
    /// Updates in real-time as cars complete laps.
    /// </summary>
    public class LeaderboardManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject leaderboardPanel;
        [SerializeField] private TextMeshProUGUI[] positionTexts; // 3 text elements for 3 cars
        [SerializeField] private TextMeshProUGUI[] carNameTexts;
        [SerializeField] private TextMeshProUGUI[] lapTexts;
        [SerializeField] private TextMeshProUGUI[] timeTexts;

        [Header("Display Settings")]
        [SerializeField] private bool showDuringRace = true;
        [SerializeField] private Color playerColor = Color.cyan;
        [SerializeField] private Color aiColor = Color.white;

        private List<RaceResult> currentStandings = new List<RaceResult>();

        #region Initialization

        private void Start()
        {
            if (leaderboardPanel != null)
            {
                leaderboardPanel.SetActive(showDuringRace);
            }
        }

        #endregion

        #region Leaderboard Updates

        /// <summary>
        /// Updates the leaderboard with current race results
        /// </summary>
        public void UpdateLeaderboard(List<RaceResult> results)
        {
            if (results == null || results.Count == 0) return;

            currentStandings = results;

            // Sort by total race time (lowest first)
            var sortedResults = currentStandings
                .OrderBy(r => r.totalRaceTime > 0 ? r.totalRaceTime : float.MaxValue)
                .ThenBy(r => r.laps.Count(l => l.isCompleted))
                .ToList();

            // Update UI elements for each position
            for (int i = 0; i < sortedResults.Count && i < 3; i++)
            {
                UpdateLeaderboardEntry(i, sortedResults[i], i + 1);
            }

            // Clear remaining entries if fewer than 3 cars
            for (int i = sortedResults.Count; i < 3; i++)
            {
                ClearLeaderboardEntry(i);
            }
        }

        /// <summary>
        /// Updates a single leaderboard entry
        /// </summary>
        private void UpdateLeaderboardEntry(int index, RaceResult result, int position)
        {
            // Position number
            if (positionTexts != null && index < positionTexts.Length && positionTexts[index] != null)
            {
                positionTexts[index].text = GetPositionString(position);
                positionTexts[index].color = result.isPlayer ? playerColor : aiColor;
            }

            // Car name
            if (carNameTexts != null && index < carNameTexts.Length && carNameTexts[index] != null)
            {
                carNameTexts[index].text = result.carName;
                carNameTexts[index].color = result.isPlayer ? playerColor : aiColor;
            }

            // Current lap
            if (lapTexts != null && index < lapTexts.Length && lapTexts[index] != null)
            {
                int completedLaps = result.laps.Count(l => l.isCompleted);
                lapTexts[index].text = $"Lap {completedLaps}/3";
                lapTexts[index].color = result.isPlayer ? playerColor : aiColor;
            }

            // Best lap time
            if (timeTexts != null && index < timeTexts.Length && timeTexts[index] != null)
            {
                string timeDisplay = result.bestLapTime < float.MaxValue 
                    ? result.GetFormattedBestLap() 
                    : "--:--.--";
                timeTexts[index].text = timeDisplay;
                timeTexts[index].color = result.isPlayer ? playerColor : aiColor;
            }
        }

        /// <summary>
        /// Clears a leaderboard entry slot
        /// </summary>
        private void ClearLeaderboardEntry(int index)
        {
            if (positionTexts != null && index < positionTexts.Length && positionTexts[index] != null)
                positionTexts[index].text = "";
            
            if (carNameTexts != null && index < carNameTexts.Length && carNameTexts[index] != null)
                carNameTexts[index].text = "";
            
            if (lapTexts != null && index < lapTexts.Length && lapTexts[index] != null)
                lapTexts[index].text = "";
            
            if (timeTexts != null && index < timeTexts.Length && timeTexts[index] != null)
                timeTexts[index].text = "";
        }

        /// <summary>
        /// Formats position number with ordinal suffix
        /// </summary>
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

        #endregion

        #region Public Controls

        /// <summary>
        /// Shows or hides the leaderboard panel
        /// </summary>
        public void SetLeaderboardVisible(bool visible)
        {
            if (leaderboardPanel != null)
            {
                leaderboardPanel.SetActive(visible);
            }
        }

        /// <summary>
        /// Clears all leaderboard data
        /// </summary>
        public void ClearLeaderboard()
        {
            for (int i = 0; i < 3; i++)
            {
                ClearLeaderboardEntry(i);
            }
            currentStandings.Clear();
        }

        #endregion
    }
}
