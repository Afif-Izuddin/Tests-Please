using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class GradeEvent : UnityEngine.Events.UnityEvent<bool> { }
public class NextDayEvent : UnityEngine.Events.UnityEvent { }
public class PauseEvent : UnityEngine.Events.UnityEvent { }
public class ContinueEvent : UnityEngine.Events.UnityEvent { }
public class RestartEvent : UnityEngine.Events.UnityEvent { }
public class OptionsEvent : UnityEngine.Events.UnityEvent { } 
public class BackToMainMenuEvent : UnityEngine.Events.UnityEvent { }
public class QuitGameEvent : UnityEngine.Events.UnityEvent { }
public class OptionsBackEvent : UnityEngine.Events.UnityEvent { }
public class StartDayConfirmedEvent : UnityEngine.Events.UnityEvent { }


public class GradingUIManager : MonoBehaviour
{
    // Game Scene UI
    [Header("Main Game UI")]
    public TextMeshProUGUI essayText;
    public TextMeshProUGUI ruleText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI dayText;

    public Button passButton;
    public Button failButton;

    // Day Recap UI
    [Header("Day Recap UI")]
    public GameObject dayRecapPanel; 
    public TextMeshProUGUI gradedPapersText;
    public TextMeshProUGUI correctGradesDayText;
    public TextMeshProUGUI incorrectGradesDayText;
    public TextMeshProUGUI overallCorrectGradesText;
    public Button nextDayButton;

    // Pause Menu UI References
    [Header("Pause Menu UI")]
    public GameObject pausePanel; 
    public Button pauseButtonOnScreen; 

    // Buttons INSIDE the Pause Panel
    public Button continueButton;
    public Button restartButton;
    public Button optionsButtonPause; 
    public Button backToMainMenuButton;
    public Button quitButtonPause;

    // Options Menu UI References
    [Header("Options Menu UI")]
    public GameObject optionsPanel; 
    public Button backButtonOptions; 

    // General Settings UI References
    [Header("General Settings")]
    public Toggle alwaysSkipStoryToggle;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    // Audio Settings UI References
    [Header("Audio Settings")]
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;

    // New Rule Panel UI References
    [Header("New Rule Panel")]
    public GameObject newRulePanel; 
    public TextMeshProUGUI newRulePanelDayText; 
    public TextMeshProUGUI newRulePanelRuleText; 
    public Button startDayButton;
    private List<Resolution> availableResolutions;
    public GradeEvent OnPaperGraded = new GradeEvent();
    public NextDayEvent OnNextDayClicked = new NextDayEvent(); 
    public PauseEvent OnPauseButtonClicked = new PauseEvent(); 
    public ContinueEvent OnContinueClicked = new ContinueEvent();
    public RestartEvent OnRestartClicked = new RestartEvent();
    public OptionsEvent OnOptionsClicked = new OptionsEvent(); 
    public BackToMainMenuEvent OnBackToMainMenuClicked = new BackToMainMenuEvent();
    public QuitGameEvent OnQuitClicked = new QuitGameEvent();
    public OptionsBackEvent OnOptionsBackClicked = new OptionsBackEvent();
    public StartDayConfirmedEvent OnStartDayConfirmed = new StartDayConfirmedEvent();


    void Awake()
    {
        passButton.onClick.AddListener(() => OnPaperGraded.Invoke(true));
        failButton.onClick.AddListener(() => OnPaperGraded.Invoke(false));

        nextDayButton.onClick.AddListener(() => OnNextDayClicked.Invoke());

        if (pauseButtonOnScreen != null)
        {
            pauseButtonOnScreen.onClick.AddListener(() => OnPauseButtonClicked.Invoke());
        }
        else
        {
            Debug.LogWarning("PauseButtonOnScreen is not assigned in GradingUIManager.");
        }

        if (continueButton != null)
            continueButton.onClick.AddListener(() => OnContinueClicked.Invoke());
        else Debug.LogWarning("ContinueButton not assigned in GradingUIManager.");

        if (restartButton != null)
            restartButton.onClick.AddListener(() => OnRestartClicked.Invoke());
        else Debug.LogWarning("RestartButton not assigned in GradingUIManager.");

        if (optionsButtonPause != null)
            optionsButtonPause.onClick.AddListener(() => OnOptionsClicked.Invoke());
        else Debug.LogWarning("OptionsButtonPause not assigned in GradingUIManager.");

        if (backToMainMenuButton != null)
            backToMainMenuButton.onClick.AddListener(() => OnBackToMainMenuClicked.Invoke());
        else Debug.LogWarning("BackToMainMenuButton not assigned in GradingUIManager.");

        if (quitButtonPause != null)
            quitButtonPause.onClick.AddListener(() => OnQuitClicked.Invoke());
        else Debug.LogWarning("QuitButtonPause not assigned in GradingUIManager.");

        if (backButtonOptions != null)
            backButtonOptions.onClick.AddListener(() => OnOptionsBackClicked.Invoke());
        else Debug.LogWarning("BackButtonOptions not assigned in GradingUIManager.");

        if (SceneFlowManager.Instance != null && SceneFlowManager.Instance.GameSettingsInstance != null)
        {
            GameSettings settings = SceneFlowManager.Instance.GameSettingsInstance;

            if (alwaysSkipStoryToggle != null)
                alwaysSkipStoryToggle.onValueChanged.AddListener(settings.SetStorySkippedStatus);
            else Debug.LogWarning("AlwaysSkipStoryToggle not assigned in GradingUIManager.");

            if (resolutionDropdown != null)
                resolutionDropdown.onValueChanged.AddListener(settings.SetResolution);
            else Debug.LogWarning("ResolutionDropdown not assigned in GradingUIManager.");

            if (fullscreenToggle != null)
                fullscreenToggle.onValueChanged.AddListener(isOn => settings.SetFullScreen(isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));
            else Debug.LogWarning("FullscreenToggle not assigned in GradingUIManager.");

            if (bgmVolumeSlider != null)
                bgmVolumeSlider.onValueChanged.AddListener(settings.SetMusicVolume);
            else Debug.LogWarning("BGMVolumeSlider not assigned in GradingUIManager.");

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(settings.SetSfxVolume);
            else Debug.LogWarning("SFXVolumeSlider not assigned in GradingUIManager.");
        }
        else
        {
            Debug.LogError("GradingUIManager: SceneFlowManager or GameSettingsInstance is null in Awake. Settings listeners might not be set up correctly.");
        }

        if (startDayButton != null)
            startDayButton.onClick.AddListener(() => OnStartDayConfirmed.Invoke());
        else Debug.LogWarning("StartDayButton not assigned in GradingUIManager.");

        if (dayRecapPanel != null)
        {
            dayRecapPanel.SetActive(false);
        }
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        if (newRulePanel != null)
        {
            newRulePanel.SetActive(false);
        }

        PopulateResolutionDropdown();
    }

    // Displays the paper's content to the UI, showing details based on the current day
     public void DisplayPaper(PaperData paper, int currentDay)
    {
        string display = $"Question: {paper.question}\nAnswer: {paper.studentAnswer}";

        // Conditional display based on the current day
        switch (currentDay)
        {
            case 0: // Day 1: Only Question and Answer
                break;
            case 1: // Day 2: Add Student ID
                if (!string.IsNullOrEmpty(paper.studentID))
                    display += $"\nStudent ID: {paper.studentID}";
                else
                    display += $"\nStudent ID: Missing";
                break;
            case 2: // Day 3: No Change
                if (!string.IsNullOrEmpty(paper.studentID))
                    display += $"\nStudent ID: {paper.studentID}";
                else
                    display += $"\nStudent ID: Missing"; 
                break;
            case 3: // Day 4: Student ID, and Date
                if (!string.IsNullOrEmpty(paper.date))
                    display += $"\nDate: {paper.date}";
                else
                    display += $"\nDate: Missing"; 

                if (!string.IsNullOrEmpty(paper.studentID))
                    display += $"\nStudent ID: {paper.studentID}";
                else
                    display += $"\nStudent ID: Missing"; 
                break;
            case 4: // Day 5 : All details
            default:
                display += $"\nUM Logo: {(paper.hasLogo ? "Present" : "Missing")}";
                if (!string.IsNullOrEmpty(paper.studentID))
                    display += $"\nStudent ID: {paper.studentID}";
                else
                    display += $"\nStudent ID: Missing";

                if (!string.IsNullOrEmpty(paper.date))
                    display += $"\nDate: Missing"; 
                else
                    display += $"\nDate: Missing"; 
                break;
        }

        essayText.text = display;
        SetButtonsInteractable(true); 
    }
    
    // Clears the main paper display.
    public void ClearPaperDisplay()
    {
        essayText.text = "No more papers for today. Waiting for day to end...";
        SetButtonsInteractable(false);
    }

    // Updates the rule display text
    public void UpdateRuleDisplay(string rule)
    {
        ruleText.text = rule;
    }


    // Updates the score display text
    public void UpdateScoreDisplay(int papersGradedToday, int totalCorrectOverall)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {totalCorrectOverall}";
        }
    }

    // Updates the timer display text
    public void UpdateTimerDisplay(float timeLeft)
    {
        if (timerText != null)
        {
            timerText.text = $"Time Left: {Mathf.CeilToInt(timeLeft)}s";
        }
    }

    // Updates the current day display text
    public void UpdateDayDisplay(int dayNumber)
    {
        if (dayText != null)
        {
            dayText.text = $"Day: {dayNumber}";
        }
        else
        {
            Debug.LogWarning("DayText is not assigned in the Inspector.");
        }
    }


    // Shows a message indicating the end of the day
    public void ShowDayEndMessage()
    {
        timerText.text = "Day Ended!";
        SetButtonsInteractable(false); // Disable grading buttons
    }

    
    // Displays the day recap panel with current statistics.
    public void ShowDayRecap(int dayNumber, int gradedToday, int correctToday, int overallCorrect)
    {
        if (dayRecapPanel != null)
        {
            dayRecapPanel.SetActive(true);
            gradedPapersText.text = $"Papers Graded: {gradedToday}";
            correctGradesDayText.text = $"Correct Today: {correctToday}";
            incorrectGradesDayText.text = $"Incorrect Today: {gradedToday - correctToday}";
            overallCorrectGradesText.text = $"Overall Correct: {overallCorrect}";

            essayText.text = $"Day {dayNumber} Report";
            ruleText.text = "";
            scoreText.text = "";
            timerText.text = "";
            dayText.text = "";

            SetButtonsActive(false); 
        }
    }

    // Hides the day recap panel.
    public void HideDayRecap()
    {
        if (dayRecapPanel != null)
        {
            dayRecapPanel.SetActive(false);
        }
    }


    // Displays the final game over screen. (Please change this to like become a proper end screen or whatever)
    // Not a game over like if you win or lose (good or bad ending) I guess based on score?
    public void EndGameDisplay(int finalCorrectCount, int totalPossiblePapers)
    {
        HideDayRecap(); 
        essayText.text = $"All Days Complete!\nTotal Correct: {finalCorrectCount}";
        ruleText.text = "Thanks for grading!";
        SetButtonsActive(false); 
        scoreText.text = $"Final Score: {finalCorrectCount}";
        timerText.text = "";
        dayText.text = "Game Over!";
    }

    // Sets the active state (visibility) of the grading buttons.
    public void SetButtonsActive(bool active)
    {
        passButton.gameObject.SetActive(active);
        failButton.gameObject.SetActive(active);
    }

    // Sets the interactable state (clickable) of the grading buttons.
    public void SetButtonsInteractable(bool interactable)
    {
        passButton.interactable = interactable;
        failButton.interactable = interactable;
    }

    // Methods for showing/hiding the Pause Panel
    public void ShowPausePanel()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            SetMainGameUIVisibility(false); 
            HideOptionsPanel(); 
            HideNewRulePanel();
        }
    }

    public void HidePausePanel()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            // Only show main game UI if the game is not paused (i.e., unpausing)
            if (GameManager.Instance != null && !GameManager.Instance.IsGamePaused()) 
            {
                 SetMainGameUIVisibility(true);
            }
        }
    }

    // Methods for showing/hiding the Options Panel
    public void ShowOptionsPanel()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
            
            if (pausePanel != null) pausePanel.SetActive(false);
            if (newRulePanel != null) newRulePanel.SetActive(false); 
            
            InitializeSettingsUI(); 
        }
    }

    public void HideOptionsPanel()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
            // Showing the pause panel again after hiding options is handled by GameManager's HandleOptionsBackClicked
        }
    }

    // Methods for showing/hiding the New Rule Panel
    public void ShowNewRulePanel(int dayNumber, string rule)
    {
        if (newRulePanel != null)
        {
            newRulePanel.SetActive(true);
            if (newRulePanelDayText != null) newRulePanelDayText.text = $"Day {dayNumber} Rules";
            if (newRulePanelRuleText != null) newRulePanelRuleText.text = rule;

            // Hide other UI elements while new rule panel is active
            SetMainGameUIVisibility(false);
            HideDayRecap();
            HidePausePanel();
            HideOptionsPanel();
        }
    }

    public void HideNewRulePanel()
    {
        if (newRulePanel != null)
        {
            newRulePanel.SetActive(false);
        }
    }

    // Helper method to control visibility of main game UI elements
    public void SetMainGameUIVisibility(bool active)
    {
        essayText.gameObject.SetActive(active);
        ruleText.gameObject.SetActive(active);
        scoreText.gameObject.SetActive(active);
        timerText.gameObject.SetActive(active);
        dayText.gameObject.SetActive(active);
        SetButtonsActive(active); 

        if (pauseButtonOnScreen != null)
        {
            pauseButtonOnScreen.gameObject.SetActive(active);
        }
    }

    // Initializes all UI elements on the options panel with current game settings
    // pulled from SceneFlowManager.GameSettingsInstance.
    public void InitializeSettingsUI()
    {
        if (SceneFlowManager.Instance == null || SceneFlowManager.Instance.GameSettingsInstance == null)
        {
            Debug.LogError("GradingUIManager: SceneFlowManager or GameSettingsInstance is null. Cannot initialize options UI.");
            return;
        }

        GameSettings settings = SceneFlowManager.Instance.GameSettingsInstance;

        if (alwaysSkipStoryToggle != null)
        {
            alwaysSkipStoryToggle.onValueChanged.RemoveListener(settings.SetStorySkippedStatus);
            alwaysSkipStoryToggle.isOn = settings.HasSkippedStoryBefore;
            alwaysSkipStoryToggle.onValueChanged.AddListener(settings.SetStorySkippedStatus);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveListener(isOn => settings.SetFullScreen(isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));
            fullscreenToggle.isOn = (settings.CurrentFullScreenMode == FullScreenMode.FullScreenWindow || settings.CurrentFullScreenMode == FullScreenMode.ExclusiveFullScreen);
            fullscreenToggle.onValueChanged.AddListener(isOn => settings.SetFullScreen(isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));
        }

        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.onValueChanged.RemoveListener(settings.SetMusicVolume);
            bgmVolumeSlider.minValue = 0.0001f; 
            bgmVolumeSlider.maxValue = 1f;
            bgmVolumeSlider.value = settings.MusicVolume;
            bgmVolumeSlider.onValueChanged.AddListener(settings.SetMusicVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.RemoveListener(settings.SetSfxVolume);
            sfxVolumeSlider.minValue = 0.0001f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.value = settings.SfxVolume;
            sfxVolumeSlider.onValueChanged.AddListener(settings.SetSfxVolume);
        }
        
        
        PopulateResolutionDropdown(); 
    }


    // Populates the resolution dropdown and sets the current selection.
    private void PopulateResolutionDropdown()
    {
        if (resolutionDropdown == null) return;

        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        availableResolutions = Screen.resolutions
                                .Where(res => res.refreshRateRatio.value >= 59)
                                .GroupBy(res => new { res.width, res.height })
                                .Select(g => g.First())
                                .OrderBy(res => res.width * res.height)
                                .ToList();

        int currentResolutionIndex = 0;
        if (SceneFlowManager.Instance != null && SceneFlowManager.Instance.GameSettingsInstance != null)
        {
            currentResolutionIndex = SceneFlowManager.Instance.GameSettingsInstance.ResolutionIndex;
        }

        for (int i = 0; i < availableResolutions.Count; i++)
        {
            Resolution res = availableResolutions[i];
            string option = $"{res.width} x {res.height}";
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);

        if (resolutionDropdown != null)
        {
            if (SceneFlowManager.Instance != null && SceneFlowManager.Instance.GameSettingsInstance != null)
            {
                resolutionDropdown.onValueChanged.RemoveListener(SceneFlowManager.Instance.GameSettingsInstance.SetResolution);
            }
            
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            if (SceneFlowManager.Instance != null && SceneFlowManager.Instance.GameSettingsInstance != null)
            {
                resolutionDropdown.onValueChanged.AddListener(SceneFlowManager.Instance.GameSettingsInstance.SetResolution);
            }
        }
    }
}