using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class GameSaveData
{
    public int highScore;
    public int currentScore;
    public int currentLevel;
    public int remainingLives;
    public bool hasSavedGame;
    public string lastSaveDate; // Adding this for future features

    public GameSaveData()
    {
        highScore = 0;
        currentScore = 0;
        currentLevel = 1;
        remainingLives = 3;
        hasSavedGame = false;
        lastSaveDate = System.DateTime.Now.ToString();
    }

}
