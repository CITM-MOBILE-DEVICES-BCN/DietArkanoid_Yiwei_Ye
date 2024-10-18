using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{
    private const string SettingsPrefix = "GameSettings_";
    private const string JsonSaveFileNameFormat = "gamesave_json_{0}.json";
    private const string BinarySaveFileNameFormat = "gamesave_binary_{0}.dat";
    private const int MaxSaveSlots = 3;

    [System.Serializable]
    public class SettingsData
    {
        public float musicVolume = 1f;
        public float sfxVolume = 1f;
        public bool fullscreen = true;
    }

    [System.Serializable]
    public class GameSaveData
    {
        public PlayerData playerData;
        // Add more game state data as needed
    }

    // Settings methods using PlayerPrefs
    public void SaveSettings(SettingsData settings)
    {
        PlayerPrefs.SetFloat(SettingsPrefix + "MusicVolume", settings.musicVolume);
        PlayerPrefs.SetFloat(SettingsPrefix + "SFXVolume", settings.sfxVolume);
        PlayerPrefs.SetInt(SettingsPrefix + "Fullscreen", settings.fullscreen ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Settings saved by PlayerPrefs");
    }

    public SettingsData LoadSettings()
    {
        Debug.Log("Settings loaded by PlayerPrefs");
        return new SettingsData
        {
            musicVolume = PlayerPrefs.GetFloat(SettingsPrefix + "MusicVolume", 1f),
            sfxVolume = PlayerPrefs.GetFloat(SettingsPrefix + "SFXVolume", 1f),
            fullscreen = PlayerPrefs.GetInt(SettingsPrefix + "Fullscreen", 1) == 1
        };
        
    }

    // JSON serialization methods
    public void SaveGameJSON(GameSaveData saveData, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MaxSaveSlots)
        {
            Debug.LogError("Invalid save slot index");
            return;
        }

        string json = JsonUtility.ToJson(saveData, true);
        string path = Path.Combine(Application.persistentDataPath, string.Format(JsonSaveFileNameFormat, slotIndex));
        File.WriteAllText(path, json);
        Debug.Log("Game saved by JSON");
    }

    public GameSaveData LoadGameJSON(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MaxSaveSlots)
        {
            Debug.LogError("Invalid save slot index");
            return null;
        }

        string path = Path.Combine(Application.persistentDataPath, string.Format(JsonSaveFileNameFormat, slotIndex));
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<GameSaveData>(json);
        }
        Debug.Log("Game loaded by JSON");
        return null;
        
    }

    // Binary serialization methods
    public void SaveGameBinary(GameSaveData saveData, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MaxSaveSlots)
        {
            Debug.LogError("Invalid save slot index");
            return;
        }

        string path = Path.Combine(Application.persistentDataPath, string.Format(BinarySaveFileNameFormat, slotIndex));
        using (FileStream stream = File.Create(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, saveData);
        }
        Debug.Log("Game saved by Binary serialization");
    }

    public GameSaveData LoadGameBinary(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MaxSaveSlots)
        {
            Debug.LogError("Invalid save slot index");
            return null;
        }

        string path = Path.Combine(Application.persistentDataPath, string.Format(BinarySaveFileNameFormat, slotIndex));
        if (File.Exists(path))
        {
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (GameSaveData)formatter.Deserialize(stream);
            }
        }
        Debug.Log("Game loaded by Binary serialization");
        return null;
    }
}