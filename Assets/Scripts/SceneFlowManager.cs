using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowManager : MonoBehaviour
{
    public static SceneFlowManager Instance { get; private set; }

    public GameSettings GameSettingsInstance { get; private set; }
    public AudioManager AudioManagerInstance { get; private set; }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            GameSettingsInstance = GetComponent<GameSettings>();
            if (GameSettingsInstance == null)
            {
                Debug.LogError("GameSettings component not found on SceneFlowManager! Please add it.");
            }

            AudioManagerInstance = GetComponent<AudioManager>();
            if (AudioManagerInstance == null)
            {
                Debug.LogError("AudioManager component not found on SceneFlowManager! Please add it.");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(string sceneName)
    {
        Debug.Log($"SceneFlowManager: Loading scene '{sceneName}'...");
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"SceneFlowManager: Scene '{scene.name}' loaded.");

        if (AudioManagerInstance != null)
        {
            AudioManagerInstance.PlayMusicForScene(scene.name);
        }
        else
        {
            Debug.LogWarning("SceneFlowManager: AudioManagerInstance is null on scene load. Music won't play.");
        }

        switch (scene.name)
        {
            case "MainMenuScene":
                // MainMenuUIManager handles its own setup and button events
                break;
            case "StoryScene":
                // StoryUIManager handles its own setup and story progression
                break;
            case "GameScene":
                GameManager gameManager = FindFirstObjectByType<GameManager>();
                if (gameManager != null)
                {
                    PaperGenerator pg = FindFirstObjectByType<PaperGenerator>();
                    if (pg == null) {
                        GameObject obj = new GameObject("PaperGenerator");
                        obj.AddComponent<PaperGenerator>();
                        Debug.Log("SceneFlowManager: Created PaperGenerator in GameScene.");
                    }
                    GradingLogic gl = FindFirstObjectByType<GradingLogic>();
                    if (gl == null) {
                        GameObject obj = new GameObject("GradingLogic");
                        obj.AddComponent<GradingLogic>();
                        Debug.Log("SceneFlowManager: Created GradingLogic in GameScene.");
                    }
                }
                else
                {
                    Debug.LogError("SceneFlowManager: GameManager not found in GameScene! Game logic won't start.");
                }
                break;
        }
    }
}