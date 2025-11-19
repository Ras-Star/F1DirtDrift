using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace F1DirtDrift
{
    /// <summary>
    /// Manages all UI panels, buttons, and scene transitions.
    /// Handles main menu, track selection, and in-game UI state.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Main Menu Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject trackSelectionPanel;
        [SerializeField] private GameObject garagePanel;
        [SerializeField] private GameObject aboutPanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Main Menu Buttons")]
        [SerializeField] private Button selectTrackButton;
        [SerializeField] private Button garageButton;
        [SerializeField] private Button aboutButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;

        [Header("Track Selection")]
        [SerializeField] private Button track1Button;
        [SerializeField] private Button track2Button;
        [SerializeField] private Button track3Button;
        [SerializeField] private Button backFromTrackButton;

        [Header("In-Game UI")]
        [SerializeField] private GameObject inGameUI;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        // Panel stack for navigation
        private Stack<GameObject> panelStack = new Stack<GameObject>();
        private GameObject currentPanel;

        #region Initialization

        private void Start()
        {
            SetupMainMenu();
        }

        private void SetupMainMenu()
        {
            // Assign button listeners
            if (selectTrackButton != null)
                selectTrackButton.onClick.AddListener(() => OpenPanel(trackSelectionPanel));
            
            if (garageButton != null)
                garageButton.onClick.AddListener(() => OpenPanel(garagePanel));
            
            if (aboutButton != null)
                aboutButton.onClick.AddListener(() => OpenPanel(aboutPanel));
            
            if (settingsButton != null)
                settingsButton.onClick.AddListener(() => OpenPanel(settingsPanel));
            
            if (exitButton != null)
                exitButton.onClick.AddListener(ExitGame);

            // Track selection buttons
            if (track1Button != null)
                track1Button.onClick.AddListener(() => LoadTrack("Track_1"));
            
            if (track2Button != null)
                track2Button.onClick.AddListener(() => LoadTrack("Track_2"));
            
            if (track3Button != null)
                track3Button.onClick.AddListener(() => LoadTrack("Track_3"));
            
            if (backFromTrackButton != null)
                backFromTrackButton.onClick.AddListener(CloseCurrentPanel);

            // In-game buttons
            if (pauseButton != null)
                pauseButton.onClick.AddListener(PauseGame);
            
            if (resumeButton != null)
                resumeButton.onClick.AddListener(ResumeGame);
            
            if (restartButton != null)
                restartButton.onClick.AddListener(RestartRace);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(ReturnToMainMenu);

            // Show main menu by default
            if (mainMenuPanel != null)
            {
                SetPanelVisible(mainMenuPanel, true);
                currentPanel = mainMenuPanel;
            }

            // Hide all other panels
            HideAllPanelsExcept(mainMenuPanel);
        }

        #endregion

        #region Panel Management

        /// <summary>
        /// Opens a panel and adds it to the navigation stack
        /// </summary>
        public void OpenPanel(GameObject panel)
        {
            if (panel == null) return;

            // Push current panel to stack
            if (currentPanel != null && currentPanel != panel)
            {
                panelStack.Push(currentPanel);
                SetPanelVisible(currentPanel, false);
            }

            // Show new panel
            SetPanelVisible(panel, true);
            currentPanel = panel;
        }

        /// <summary>
        /// Closes the current panel and returns to previous
        /// </summary>
        public void CloseCurrentPanel()
        {
            if (currentPanel != null)
            {
                SetPanelVisible(currentPanel, false);
            }

            // Pop previous panel from stack
            if (panelStack.Count > 0)
            {
                currentPanel = panelStack.Pop();
                SetPanelVisible(currentPanel, true);
            }
        }

        /// <summary>
        /// Sets a panel's visibility
        /// </summary>
        private void SetPanelVisible(GameObject panel, bool visible)
        {
            if (panel != null)
            {
                panel.SetActive(visible);
                
                // Ensure CanvasGroup is interactable
                var canvasGroup = panel.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.interactable = visible;
                    canvasGroup.blocksRaycasts = visible;
                }
            }
        }

        /// <summary>
        /// Hides all panels except the specified one
        /// </summary>
        private void HideAllPanelsExcept(GameObject exceptPanel)
        {
            GameObject[] allPanels = {
                mainMenuPanel, trackSelectionPanel, garagePanel,
                aboutPanel, settingsPanel, pausePanel
            };

            foreach (var panel in allPanels)
            {
                if (panel != null && panel != exceptPanel)
                {
                    SetPanelVisible(panel, false);
                }
            }
        }

        #endregion

        #region Scene Navigation

        /// <summary>
        /// Loads a track scene
        /// </summary>
        private void LoadTrack(string sceneName)
        {
            Debug.Log($"[UIManager] Loading track: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Returns to main menu
        /// </summary>
        private void ReturnToMainMenu()
        {
            Time.timeScale = 1f; // Ensure time is resumed
            SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// Restarts the current race
        /// </summary>
        private void RestartRace()
        {
            Time.timeScale = 1f;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartRace();
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        /// <summary>
        /// Exits the game application
        /// </summary>
        private void ExitGame()
        {
            Debug.Log("[UIManager] Exiting game...");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        #endregion

        #region Pause Management

        private void PauseGame()
        {
            Time.timeScale = 0f;
            OpenPanel(pausePanel);
            
            if (GameManager.Instance != null)
            {
                // GameManager handles pause state
            }
        }

        private void ResumeGame()
        {
            Time.timeScale = 1f;
            CloseCurrentPanel();
        }

        #endregion

        #region Public Accessors

        public void ShowInGameUI(bool show)
        {
            if (inGameUI != null)
            {
                inGameUI.SetActive(show);
            }
        }

        public void ShowPauseButton(bool show)
        {
            if (pauseButton != null)
            {
                pauseButton.gameObject.SetActive(show);
            }
        }

        #endregion
    }
}
