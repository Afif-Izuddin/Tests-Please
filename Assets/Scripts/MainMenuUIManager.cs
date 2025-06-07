using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; 

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Main Menu UI References")]
    public TextMeshProUGUI titleText;
    public Button startGameButton;
    public Button quitGameButton;
    public Button optionsButton; 

    [Header("Options UI References")] 
    public GameObject optionsPanel; 
    public Button optionsBackButton; 

    public Toggle alwaysSkipStoryToggle;
    public TMP_Dropdown resolutionDropdown; 
    public Toggle fullscreenToggle; 
    public Slider bgmVolumeSlider; 
    public Slider sfxVolumeSlider; 

    private Resolution[] availableResolutions;

    void Awake()
    {
        // Main menu buttons
        if (startGameButton != null) startGameButton.onClick.AddListener(OnStartGameClicked);
        if (quitGameButton != null) quitGameButton.onClick.AddListener(OnQuitGameClicked);
        if (optionsButton != null) optionsButton.onClick.AddListener(ShowOptionsPanel); 

        // Options panel buttons and controls
        if (optionsBackButton != null) optionsBackButton.onClick.AddListener(HideOptionsPanel); 

        if (alwaysSkipStoryToggle != null) alwaysSkipStoryToggle.onValueChanged.AddListener(OnAlwaysSkipStoryToggled); 
        if (resolutionDropdown != null) resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged); 
        if (fullscreenToggle != null) fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled); 
        if (bgmVolumeSlider != null) bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged); 
        if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // Ensure options panel is hidden by default
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    void Start()
    {
        if (titleText != null) titleText.text = "Tests, Please";
        InitializeOptionsUI();
    }


    // Initializes all UI elements on the options panel with current game settings.
    private void InitializeOptionsUI()
    {
        if (SceneFlowManager.Instance == null || SceneFlowManager.Instance.GameSettingsInstance == null)
        {
            Debug.LogError("MainMenuUIManager: SceneFlowManager or GameSettingsInstance is null. Cannot initialize options UI.");
            return;
        }

        GameSettings settings = SceneFlowManager.Instance.GameSettingsInstance;

        // Populate Resolution Dropdown
        if (resolutionDropdown != null)
        {
            availableResolutions = Screen.resolutions;
            List<string> options = new List<string>();
            int currentResolutionIndex = 0; 

            for (int i = 0; i < availableResolutions.Length; i++)
            {
                string option = $"{availableResolutions[i].width} x {availableResolutions[i].height}";
                options.Add(option);

                // Find the index of the currently set resolution, or closest match
                if (availableResolutions[i].width == Screen.currentResolution.width &&
                    availableResolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(options);

            resolutionDropdown.value = settings.ResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        // Set Fullscreen Toggle
        if (fullscreenToggle != null)
        {
            
            fullscreenToggle.isOn = (settings.CurrentFullScreenMode == FullScreenMode.FullScreenWindow || settings.CurrentFullScreenMode == FullScreenMode.ExclusiveFullScreen);
        }

        // Set Volume Sliders
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.minValue = 0.0001f; // Avoid 0
            bgmVolumeSlider.maxValue = 1f;
            bgmVolumeSlider.value = settings.MusicVolume;
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0.0001f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.value = settings.SfxVolume;
        }

        // Set Always Skip Story Toggle
        if (alwaysSkipStoryToggle != null)
        {
            alwaysSkipStoryToggle.isOn = settings.HasSkippedStoryBefore;
        }
    }


    //  Main Menu Button Handlers 
    private void OnStartGameClicked()
    {
        Debug.Log("MainMenuUIManager: Start Game button clicked. Requesting StoryScene load.");
        SceneFlowManager.Instance.LoadScene("StoryScene");
    }

    private void OnQuitGameClicked()
    {
        Debug.Log("MainMenuUIManager: Quit Game button clicked. Exiting application.");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // Options Panel Management 

    public void ShowOptionsPanel()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
            SetMainMenuButtonsInteractable(false);
            InitializeOptionsUI(); 
        }
    }

    public void HideOptionsPanel()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
            SetMainMenuButtonsInteractable(true);
        }
    }

    private void SetMainMenuButtonsInteractable(bool interactable)
    {
        if (startGameButton != null) startGameButton.interactable = interactable;
        if (quitGameButton != null) quitGameButton.interactable = interactable;
        if (optionsButton != null) optionsButton.interactable = interactable;
    }

    // Options Control Event Handlers 

    private void OnAlwaysSkipStoryToggled(bool isOn)
    {
        if (SceneFlowManager.Instance != null && SceneFlowManager.Instance.GameSettingsInstance != null)
        {
            SceneFlowManager.Instance.GameSettingsInstance.SetStorySkippedStatus(isOn);
            Debug.Log($"MainMenuUIManager: Always Skip Story toggled to: {isOn}");
        }
    }

    private void OnResolutionChanged(int index)
    {
        if (SceneFlowManager.Instance != null && SceneFlowManager.Instance.GameSettingsInstance != null)
        {
            SceneFlowManager.Instance.GameSettingsInstance.SetResolution(index);
            Debug.Log($"MainMenuUIManager: Resolution changed to index: {index}");
        }
    }

    private void OnFullscreenToggled(bool isOn)
    {
        if (SceneFlowManager.Instance != null && SceneFlowManager.Instance.GameSettingsInstance != null)
        {
            FullScreenMode mode = isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            SceneFlowManager.Instance.GameSettingsInstance.SetFullScreen(mode);
            Debug.Log($"MainMenuUIManager: Fullscreen toggled to: {mode}");
        }
    }

    private void OnBGMVolumeChanged(float value)
    {
        if (SceneFlowManager.Instance != null && SceneFlowManager.Instance.GameSettingsInstance != null)
        {
            SceneFlowManager.Instance.GameSettingsInstance.SetMusicVolume(value);
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (SceneFlowManager.Instance != null && SceneFlowManager.Instance.GameSettingsInstance != null)
        {
            SceneFlowManager.Instance.GameSettingsInstance.SetSfxVolume(value);
        }
    }
}