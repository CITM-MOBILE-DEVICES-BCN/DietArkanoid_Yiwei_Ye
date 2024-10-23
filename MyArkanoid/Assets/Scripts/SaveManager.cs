using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SAVE_FILE_NAME = "arkanoid_save.json";
    private const string SETTINGS_PREFIX = "Settings_";

    private GameSaveData currentGameData;
    private SettingsData currentSettings;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAll();
        }
        else
        {
            Destroy(gameObject);
        }
    }



    private void LoadAll()
    {
        LoadGameData();
        LoadSettings();
    }

    #region Game Data (JSON)

    public void SaveGameData()
    {
        currentGameData = new GameSaveData
        {
            highScore = Mathf.Max(GameManager.Instance.Score, currentGameData?.highScore ?? 0),
            currentScore = GameManager.Instance.Score,
            currentLevel = GameManager.Instance.CurrentLevel,
            remainingLives = GameManager.Instance.Lives,
            hasSavedGame = true,
            lastSaveDate = System.DateTime.Now.ToString()
        };

        string json = JsonUtility.ToJson(currentGameData, true);
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

        try
        {
            File.WriteAllText(path, json);
            Debug.Log($"Game saved successfully to {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game data: {e.Message}");
        }
    }

    public void LoadGameData()
    {
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                currentGameData = JsonUtility.FromJson<GameSaveData>(json);
                Debug.Log("Game data loaded successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game data: {e.Message}");
                ResetGameData();
            }
        }
        else
        {
            ResetGameData();
            Debug.Log("No save file found, created new game data");
        }
    }

    private void ResetGameData()
    {
        currentGameData = new GameSaveData();
    }

    public void DeleteGameData()
    {
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        currentGameData = new GameSaveData();
        Debug.Log("Game data deleted");
    }

    public bool HasSavedGame()
    {
        return currentGameData?.hasSavedGame ?? false;
    }

    public GameSaveData GetGameData()
    {
        return currentGameData;
    }

    #endregion

    #region Settings (PlayerPrefs)

    public void SaveSettings(SettingsData settings)
    {
        currentSettings = settings;

        PlayerPrefs.SetFloat(SETTINGS_PREFIX + "MusicVolume", settings.musicVolume);
        PlayerPrefs.SetFloat(SETTINGS_PREFIX + "SFXVolume", settings.sfxVolume);
        PlayerPrefs.SetInt(SETTINGS_PREFIX + "Fullscreen", settings.fullscreen ? 1 : 0);
        PlayerPrefs.SetInt(SETTINGS_PREFIX + "VSync", settings.vSync ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("Settings saved to PlayerPrefs");
    }

    public void LoadSettings()
    {
        currentSettings = new SettingsData
        {
            musicVolume = PlayerPrefs.GetFloat(SETTINGS_PREFIX + "MusicVolume", 1f),
            sfxVolume = PlayerPrefs.GetFloat(SETTINGS_PREFIX + "SFXVolume", 1f),
            fullscreen = PlayerPrefs.GetInt(SETTINGS_PREFIX + "Fullscreen", 1) == 1,
            vSync = PlayerPrefs.GetInt(SETTINGS_PREFIX + "VSync", 1) == 1
        };

        Debug.Log("Settings loaded from PlayerPrefs");
    }

    public void DeleteSettings()
    {
        string[] settingsKeys = new string[]
        {
            SETTINGS_PREFIX + "MusicVolume",
            SETTINGS_PREFIX + "SFXVolume",
            SETTINGS_PREFIX + "Fullscreen",
            SETTINGS_PREFIX + "VSync"
        };

        foreach (string key in settingsKeys)
        {
            PlayerPrefs.DeleteKey(key);
        }

        currentSettings = new SettingsData();
        Debug.Log("Settings deleted from PlayerPrefs");
    }

    public SettingsData GetSettings()
    {
        return currentSettings;
    }

    #endregion

    // Helper method to delete all saved data
    public void DeleteAllData()
    {
        DeleteGameData();
        DeleteSettings();
        Debug.Log("All saved data deleted");
    }
}
