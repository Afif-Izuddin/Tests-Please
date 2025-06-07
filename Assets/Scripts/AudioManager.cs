// AudioManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource; 
    public AudioSource sfxSource;   

    [Header("Music Tracks")]
    public AudioClip mainMenuMusic;
    public AudioClip storyMusic;
    public AudioClip gameMusic;

    // Add SFX here (Pls help)
    // public AudioClip buttonClickSFX;
    // public AudioClip correctAnswer;
    // public AudioClip wrongAnswer;
    // public AudioClip timeTicking;
    // public AudioClip dayEnd;
    // public AudioClip gameOver;

    private string currentSceneName = "";

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

        //Get Audio Source if empty by default it shouldn't have to go here
        if (musicSource == null) musicSource = GetComponent<AudioSource>();
        if (sfxSource == null) 
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length >= 2)
            {
                musicSource = sources[0];
                sfxSource = sources[1];
            } else if (sources.Length == 1) {
                musicSource = sources[0];
                Debug.LogWarning("AudioManager: Only one AudioSource found. Assigning it as musicSource. SFX will not play separately.");
                sfxSource = musicSource;
            }
        }
        if (musicSource != null) musicSource.loop = true;
        if (sfxSource != null) sfxSource.loop = false; 
    }

    void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    //Play BGM
    public void PlayMusicForScene(string sceneName)
    {
        if (currentSceneName == sceneName) return;
        AudioClip clipToPlay = null;
        switch (sceneName)
        {
            case "MainMenuScene": clipToPlay = mainMenuMusic; break;
            case "StoryScene": clipToPlay = storyMusic; break;
            case "GameScene": clipToPlay = gameMusic; break;
            default: Debug.LogWarning($"AudioManager: No specific music for scene: {sceneName}. Stopping music."); musicSource.Stop(); return;
        }

        if (clipToPlay != null && musicSource != null)
        {
            if (musicSource.clip != clipToPlay || !musicSource.isPlaying)
            {
                musicSource.Stop();
                musicSource.clip = clipToPlay;
                musicSource.Play();
                Debug.Log($"AudioManager: Playing music for {sceneName}.");
            }
        }
        else if (musicSource != null)
        {
            musicSource.Stop();
        }

        currentSceneName = sceneName;
    }

    //Change BGM Volume 
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
            Debug.Log($"AudioManager: Music volume set to {volume}");
        }
    }

    //Change SFX Volume 
    public void SetSfxVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
            Debug.Log($"AudioManager: SFX volume set to {volume}");
        }
    }

    //Play SFX
    public void PlaySFX(AudioClip sfxClip, float volumeScale = 1f) 
    {
        if (sfxSource != null && sfxClip != null)
        {
            sfxSource.PlayOneShot(sfxClip, sfxSource.volume * volumeScale); 
            Debug.Log($"AudioManager: Playing SFX: {sfxClip.name}");
        }
    }
}