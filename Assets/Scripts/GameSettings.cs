using UnityEngine;


public class GameSettings : MonoBehaviour
{
    // Where Settings are saved
    public float MusicVolume { get; private set; } = 0.75f;
    public float SfxVolume { get; private set; } = 0.75f;
    public bool HasSkippedStoryBefore { get; private set; } = false;
    public int ResolutionIndex { get; private set; } = 0; 
    public FullScreenMode CurrentFullScreenMode { get; private set; } = FullScreenMode.FullScreenWindow; // Default: Borderless Fullscreen

    private const string MUSIC_VOLUME_KEY = "Game_MusicVolume";
    private const string SFX_VOLUME_KEY = "Game_SfxVolume";
    private const string STORY_SKIPPED_KEY = "Game_StorySkippedBefore";
    private const string RESOLUTION_INDEX_KEY = "Game_ResolutionIndex";
    private const string FULLSCREEN_MODE_KEY = "Game_FullScreenMode"; // Stored as int

    void Awake()
    {
        LoadSettings();
    }

    // Loads game settings from PlayerPrefs. If a key is not found, it uses the default value.
    // After loading, it applies the music volume and display settings.
    private void LoadSettings()
    {
        MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, MusicVolume);
        SfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, SfxVolume);
        HasSkippedStoryBefore = PlayerPrefs.GetInt(STORY_SKIPPED_KEY, HasSkippedStoryBefore ? 1 : 0) == 1;
        ResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_INDEX_KEY, ResolutionIndex);
        CurrentFullScreenMode = (FullScreenMode)PlayerPrefs.GetInt(FULLSCREEN_MODE_KEY, (int)CurrentFullScreenMode);

        Debug.Log($"GameSettings: Loaded Music Volume: {MusicVolume}, SFX Volume: {SfxVolume}, Story Skipped: {HasSkippedStoryBefore}, Resolution Index: {ResolutionIndex}, Fullscreen Mode: {CurrentFullScreenMode}");

        // Apply loaded settings
        ApplyMusicVolume();
        ApplySfxVolume(); 
        ApplyDisplaySettings();
    }


    // Sets the music volume and saves it to PlayerPrefs.
    // Also notifies the AudioManager to update its volume.
    public void SetMusicVolume(float volume)
    {
        MusicVolume = volume;
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
        Debug.Log($"GameSettings: Music Volume set to {volume}");
        ApplyMusicVolume();
    }

    // Sets the SFX volume and saves it to PlayerPrefs.
    public void SetSfxVolume(float volume)
    {
        SfxVolume = volume;
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();
        Debug.Log($"GameSettings: SFX Volume set to {volume}");
        ApplySfxVolume();
    }

    // Sets the status of whether the story has been skipped and saves it to PlayerPrefs.
   
    public void SetStorySkippedStatus(bool skipped)
    {
        HasSkippedStoryBefore = skipped;
        PlayerPrefs.SetInt(STORY_SKIPPED_KEY, skipped ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"GameSettings: Story Skipped Status set to {skipped}");
    }

    
    // Sets the resolution based on its index in the available resolutions list.

    public void SetResolution(int index)
    {
        ResolutionIndex = index;
        PlayerPrefs.SetInt(RESOLUTION_INDEX_KEY, index);
        PlayerPrefs.Save();
        Debug.Log($"GameSettings: Resolution index set to {index}");
        ApplyDisplaySettings();
    }

    
    // Sets the fullscreen mode.

    public void SetFullScreen(FullScreenMode mode)
    {
        CurrentFullScreenMode = mode;
        PlayerPrefs.SetInt(FULLSCREEN_MODE_KEY, (int)mode);
        PlayerPrefs.Save();
        Debug.Log($"GameSettings: Fullscreen mode set to {mode}");
        ApplyDisplaySettings();
    }



    // Applies the current music volume setting to the AudioManager.

    private void ApplyMusicVolume()
    {
        if (SceneFlowManager.Instance != null && SceneFlowManager.Instance.AudioManagerInstance != null)
        {
            SceneFlowManager.Instance.AudioManagerInstance.SetMusicVolume(MusicVolume);
        }
    }

    // Applies the current SFX volume setting to the AudioManager.
    
    private void ApplySfxVolume()
    {
        if (SceneFlowManager.Instance != null && SceneFlowManager.Instance.AudioManagerInstance != null)
        {
            SceneFlowManager.Instance.AudioManagerInstance.SetSfxVolume(SfxVolume); 
        }
    }


    // Applies the current resolution and fullscreen mode to the screen.

    private void ApplyDisplaySettings()
    {
        Resolution[] resolutions = Screen.resolutions;
        if (ResolutionIndex >= 0 && ResolutionIndex < resolutions.Length)
        {
            Resolution resolutionToSet = resolutions[ResolutionIndex];
            Screen.SetResolution(resolutionToSet.width, resolutionToSet.height, CurrentFullScreenMode);
            Debug.Log($"GameSettings: Applying display settings - Resolution: {resolutionToSet.width}x{resolutionToSet.height}, Fullscreen Mode: {CurrentFullScreenMode}");
        }
        else
        {
            Debug.LogWarning("GameSettings: Invalid resolution index, applying current screen resolution.");

            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, CurrentFullScreenMode);
        }
    }


    // Resets all game settings to their default values and deletes them from PlayerPrefs. (Not Used RN, no Default Setting Button)
    public void ResetSettingsToDefault()
    {
        MusicVolume = 0.75f;
        SfxVolume = 0.75f;
        HasSkippedStoryBefore = false;
        ResolutionIndex = 0; 
        CurrentFullScreenMode = FullScreenMode.FullScreenWindow;

        PlayerPrefs.DeleteKey(MUSIC_VOLUME_KEY);
        PlayerPrefs.DeleteKey(SFX_VOLUME_KEY);
        PlayerPrefs.DeleteKey(STORY_SKIPPED_KEY);
        PlayerPrefs.DeleteKey(RESOLUTION_INDEX_KEY);
        PlayerPrefs.DeleteKey(FULLSCREEN_MODE_KEY);
        PlayerPrefs.Save();

        Debug.Log("GameSettings: All settings reset to default and PlayerPrefs cleared for these settings.");

        // Re-apply defaults
        ApplyMusicVolume();
        ApplySfxVolume();
        ApplyDisplaySettings();
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs saved on application quit.");
    }
}