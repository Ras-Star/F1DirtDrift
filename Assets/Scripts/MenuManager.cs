using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace F1DirtDrift
{
    /// <summary>
    /// Handles main menu interactions, track selection, and start position choice.
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        [Header("Start Position Selection")]
        [SerializeField] private GameObject startPositionPanel;
        [SerializeField] private Button position1Button;
        [SerializeField] private Button position2Button;
        [SerializeField] private Button position3Button;
        [SerializeField] private TextMeshProUGUI instructionText;

        [Header("Track Info Display")]
        [SerializeField] private TextMeshProUGUI trackNameText;
        [SerializeField] private TextMeshProUGUI trackDescriptionText;
        [SerializeField] private Image trackPreviewImage;

        private string selectedTrackScene = "";
        private StartPosition playerStartPosition = StartPosition.First;

        #region Initialization

        private void Start()
        {
            SetupStartPositionButtons();
            
            if (startPositionPanel != null)
            {
                startPositionPanel.SetActive(false);
            }
        }

        private void SetupStartPositionButtons()
        {
            if (position1Button != null)
                position1Button.onClick.AddListener(() => SelectStartPosition(StartPosition.First));
            
            if (position2Button != null)
                position2Button.onClick.AddListener(() => SelectStartPosition(StartPosition.Second));
            
            if (position3Button != null)
                position3Button.onClick.AddListener(() => SelectStartPosition(StartPosition.Third));
        }

        #endregion

        #region Track Selection

        /// <summary>
        /// Called when player selects a track
        /// </summary>
        public void OnTrackSelected(string sceneName)
        {
            selectedTrackScene = sceneName;
            ShowStartPositionSelection();
        }

        /// <summary>
        /// Shows the start position selection panel
        /// </summary>
        private void ShowStartPositionSelection()
        {
            if (startPositionPanel != null)
            {
                startPositionPanel.SetActive(true);
            }

            if (instructionText != null)
            {
                instructionText.text = "Choose your starting position:\n" +
                    "All positions start at different times (pure time trial)";
            }
        }

        #endregion

        #region Start Position

        /// <summary>
        /// Handles start position selection
        /// </summary>
        private void SelectStartPosition(StartPosition position)
        {
            playerStartPosition = position;
            
            // Store in PlayerPrefs for the race scene to read
            PlayerPrefs.SetInt("PlayerStartPosition", (int)position);
            PlayerPrefs.Save();

            Debug.Log($"[MenuManager] Player selected start position: {position}");

            // Load the selected track
            LoadSelectedTrack();
        }

        /// <summary>
        /// Loads the selected track scene
        /// </summary>
        private void LoadSelectedTrack()
        {
            if (!string.IsNullOrEmpty(selectedTrackScene))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(selectedTrackScene);
            }
            else
            {
                Debug.LogError("[MenuManager] No track scene selected!");
            }
        }

        #endregion

        #region Track Info Display

        /// <summary>
        /// Updates track information display
        /// </summary>
        public void DisplayTrackInfo(TrackData trackData)
        {
            if (trackData == null) return;

            if (trackNameText != null)
            {
                trackNameText.text = trackData.trackName;
            }

            if (trackDescriptionText != null)
            {
                trackDescriptionText.text = trackData.trackDescription;
            }

            if (trackPreviewImage != null && trackData.trackPreviewSprite != null)
            {
                trackPreviewImage.sprite = trackData.trackPreviewSprite;
            }
        }

        #endregion

        #region Public Accessors

        /// <summary>
        /// Gets the player's selected start position (call from race scene)
        /// </summary>
        public static StartPosition GetPlayerStartPosition()
        {
            int position = PlayerPrefs.GetInt("PlayerStartPosition", 0);
            return (StartPosition)position;
        }

        #endregion
    }
}
