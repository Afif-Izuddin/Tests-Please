// GameManager.cs 
using System.Collections.Generic;
using UnityEngine;
using System.Collections; 
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } 

    [Header("Game Settings")]
    public float timePerDay = 10f; //Change this for timer change well I guess it should be in the Unity Editor
    public int totalGameDays = 5;

    private GradingUIManager uiManager;
    private PaperGenerator paperGenerator;
    private GradingLogic gradingLogic;

    // Game State Variables
    private int currentDay = 0; 
    private int correctGradesOverall = 0; 
    private int papersGradedToday = 0; 
    private int correctGradesToday = 0; 
    private float dailyTimer;
    private bool dayActive = false; 
    private PaperData currentPaper; 

    // Pause State Variables (controlled by UI buttons & new rule panel)
    private bool isGameActive = false; 
    private bool isPaused = false; 


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        uiManager = FindFirstObjectByType<GradingUIManager>();
        paperGenerator = FindFirstObjectByType<PaperGenerator>();
        gradingLogic = FindFirstObjectByType<GradingLogic>();

        if (uiManager == null) Debug.LogError("GradingUIManager not found in scene!");
        if (paperGenerator == null) {
            GameObject obj = new GameObject("PaperGenerator");
            paperGenerator = obj.AddComponent<PaperGenerator>();
            Debug.Log("Created PaperGenerator GameObject.");
        }
        if (gradingLogic == null) {
            GameObject obj = new GameObject("GradingLogic");
            gradingLogic = obj.AddComponent<GradingLogic>();
            Debug.Log("Created GradingLogic GameObject.");
        }
    }

    void Start()
    {
        if (uiManager != null)
        {
            //Game Scene UI
            uiManager.OnPaperGraded.AddListener(HandlePaperGraded);
            uiManager.OnNextDayClicked.AddListener(HandleNextDayButtonClicked);
            uiManager.OnPauseButtonClicked.AddListener(TogglePause); 
            uiManager.OnStartDayConfirmed.AddListener(ResumeDayAfterRuleConfirmed);

            //Paused UI
            uiManager.OnContinueClicked.AddListener(TogglePause);
            uiManager.OnRestartClicked.AddListener(RestartGame);
            uiManager.OnBackToMainMenuClicked.AddListener(BackToMainMenu);
            uiManager.OnQuitClicked.AddListener(QuitGame);
            uiManager.OnOptionsClicked.AddListener(HandleOptionsClicked); 
            uiManager.OnOptionsBackClicked.AddListener(HandleOptionsBackClicked); 

            
            
            // Initialize UI elements with loaded settings (from SceneFlowManager)
            if (SceneFlowManager.Instance != null && SceneFlowManager.Instance.GameSettingsInstance != null)
            {
                uiManager.InitializeSettingsUI();
            }
            else
            {
                 Debug.LogError("GameManager: SceneFlowManager or GameSettingsInstance is null in Start. Cannot initialize UI settings.");
            }
        }

        isGameActive = true; 
        StartNewDay(); 
    }

    void Update()
    {
        // Check for Escape key press to toggle pause/options
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (uiManager != null && uiManager.optionsPanel != null && uiManager.optionsPanel.activeSelf)
            {
                HandleOptionsBackClicked(); // Go back from options if it's open
            }
            else if (uiManager != null && uiManager.pausePanel != null && uiManager.pausePanel.activeSelf)
            {
                TogglePause(); // If pause menu is open, unpause it
            }
            else if (uiManager != null && uiManager.newRulePanel != null && uiManager.newRulePanel.activeSelf)
             {
                // Not allow Escape key presses on new rule panel
             }
            else if (!isPaused)
            {
                TogglePause();
            }
        }

        if (isGameActive && !isPaused) 
        {
            if (dayActive) // Only update timer if the actual grading phase is active (Not Paused)
            {
                dailyTimer -= Time.deltaTime;
                uiManager.UpdateTimerDisplay(dailyTimer);

                if (dailyTimer <= 0)
                {
                    EndDay(); 
                }
            }
        }
    }

    private void StartNewDay()
    {
        // Check if all days are completed
        if (currentDay >= totalGameDays)
        {
            EndGame(true); 
            return;
        }

        Debug.Log($"Preparing Day {currentDay + 1}");

        // Reset daily statistics
        papersGradedToday = 0;
        correctGradesToday = 0; 
        dailyTimer = timePerDay; 
        dayActive = false; 

        // Pause the game and show the new rule panel
        isPaused = true;
        Time.timeScale = 0f;
        uiManager.ShowNewRulePanel(currentDay + 1, gradingLogic.GetRuleTextForDay(currentDay));

        // Update UI displays (main game UI elements will be hidden)
        uiManager.HideDayRecap(); 
        uiManager.HidePausePanel(); 
        uiManager.HideOptionsPanel(); 
        uiManager.SetButtonsActive(false); 
        uiManager.SetMainGameUIVisibility(false); 
    }

    // Called when the player clicks "Start Day" on the New Rule Panel.
    // This resumes the game and starts the grading phase.
    public void ResumeDayAfterRuleConfirmed()
    {
        Debug.Log($"Starting grading for Day {currentDay + 1} after rule confirmation.");
        uiManager.HideNewRulePanel();
        isPaused = false; 
        Time.timeScale = 1f;

        ProceedWithDay();
    }


    // Contains the logic to actually start the grading phase of the day.
    private void ProceedWithDay()
    {
        dayActive = true; 

        uiManager.SetButtonsActive(true); 
        uiManager.SetMainGameUIVisibility(true); 

        GenerateAndDisplayNewPaper(); 

        // Update UI displays
        uiManager.UpdateRuleDisplay(gradingLogic.GetRuleTextForDay(currentDay));
        uiManager.UpdateDayDisplay(currentDay + 1);
        uiManager.UpdateScoreDisplay(papersGradedToday, correctGradesOverall);
        uiManager.UpdateTimerDisplay(dailyTimer);
    }

    // Ends the current game day, triggered by the timer running out.
    private void EndDay()
    {
        dayActive = false; 
        uiManager.ShowDayEndMessage(); 

        Debug.Log($"Day {currentDay + 1} Ended. Papers graded today: {papersGradedToday}, Correct: {correctGradesToday}");

        uiManager.ShowDayRecap(currentDay + 1, papersGradedToday, correctGradesToday, correctGradesOverall);
    }

    
    // Generates a new paper and updates the UI to display it.
    private void GenerateAndDisplayNewPaper()
    {
        currentPaper = paperGenerator.GenerateSinglePaper(currentDay);
        uiManager.DisplayPaper(currentPaper, currentDay); 
        uiManager.SetButtonsInteractable(true); 
    }

    // Handles the player's grading action (Pass/Fail button click).
    private void HandlePaperGraded(bool playerSaidPass)
    {
        if (!dayActive || currentPaper == null || isPaused) 
        {
            Debug.LogWarning("Attempted to grade paper when day is not active, no paper, or game is paused.");
            return;
        }

        bool shouldPass = gradingLogic.ShouldPaperPass(currentPaper, currentDay);

        if (playerSaidPass == shouldPass)
        {
            correctGradesOverall++;
            correctGradesToday++; 
        }
        papersGradedToday++; 

        uiManager.UpdateScoreDisplay(papersGradedToday, correctGradesOverall); 

        GenerateAndDisplayNewPaper(); 
    }

    
    // Handles the "Next Day" button click from the UI.
    public void HandleNextDayButtonClicked()
    {
        currentDay++; 
        StartNewDay(); 
    }

    // Ends the entire game, showing final statistics.
    private void EndGame(bool success)
    {
        isGameActive = false;
        dayActive = false; 
        Time.timeScale = 1f; 
        uiManager.EndGameDisplay(correctGradesOverall, totalGameDays * 100);
        Debug.Log($"Game Over! Final Score: {correctGradesOverall}");
    }

    // Core Pause/Resume functionality (will be called by UI button or Escape)
    public void TogglePause()
    {
        if (uiManager != null && uiManager.newRulePanel != null && uiManager.newRulePanel.activeSelf)
        {
            Debug.Log("GameManager: Cannot toggle pause while New Rule Panel is open. Please click 'Start Day'.");
            return;
        }

        isPaused = !isPaused; 
        if (isPaused)
        {
            Time.timeScale = 0f; 
            Debug.Log("GameManager: Game Paused.");
            uiManager.ShowPausePanel(); 
        }
        else
        {
            Time.timeScale = 1f; 
            Debug.Log("GameManager: Game Resumed.");
            uiManager.HidePausePanel(); 
        }
    }

    // Handlers for showing/hiding options panel
    public void HandleOptionsClicked()
    {
        Debug.Log("GameManager: Showing Options Panel.");
        uiManager.ShowOptionsPanel();
        uiManager.HidePausePanel(); 
        uiManager.HideNewRulePanel();
    }

    public void HandleOptionsBackClicked()
    {
        Debug.Log("GameManager: Hiding Options Panel, showing Pause Panel.");
        uiManager.HideOptionsPanel();
        uiManager.ShowPausePanel(); 
    }
    
    public bool IsGamePaused()
    {
        return isPaused;
    }

    // Methods for main menu/restart/quit
    public void RestartGame()
    {
        Debug.Log("GameManager: Restarting game...");
        Time.timeScale = 1f; 
        isPaused = false; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    public void BackToMainMenu()
    {
        Debug.Log("GameManager: Returning to Main Menu...");
        Time.timeScale = 1f; 
        isPaused = false; 
        if (SceneFlowManager.Instance != null)
        {
            SceneFlowManager.Instance.LoadScene("MainMenuScene"); 
        }
        else
        {
            Debug.LogError("SceneFlowManager instance not found. Cannot return to Main Menu.");
            SceneManager.LoadScene("MainMenuScene"); 
        }
    }

    public void QuitGame()
    {
        Debug.Log("GameManager: Quitting game.");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}