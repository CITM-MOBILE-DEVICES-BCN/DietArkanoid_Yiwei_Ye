using UnityEngine;

public class Player : MonoBehaviour
{
    public int level = 1;
    public float health = 100f;

    public void SaveData(SaveManager.GameSaveData saveData)
    {
        saveData.playerData = new PlayerData
        {
            level = this.level,
            health = this.health,
            position = new SerializableVector3(this.transform.position)
        };
    }

    public void LoadData(SaveManager.GameSaveData saveData)
    {
        if (saveData.playerData != null)
        {
            this.level = saveData.playerData.level;
            this.health = saveData.playerData.health;
            this.transform.position = saveData.playerData.position.ToVector3();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AdvancePlayerState();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            health -= 10;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            health += 10;
        }
    }

    public void AdvancePlayerState()
    {
        level++;
        
        transform.position += Vector3.forward;
        Debug.Log($"Player advanced: Level {level}, Health {health}, Position {transform.position}");
    }

    public void DamagePlayer()
    {
        health -= 10f;
        Debug.Log($"Player damaged: Health {health}");
    }
    public void HealPlayer()
    {
        health += 10f;
        Debug.Log($"Player healed: Health {health}");

    }
}

    [System.Serializable]
public class PlayerData
{
    public int level;
    public float health;
    public SerializableVector3 position;
}