using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int currentSpawnPointIdData;
    public int healthData;

    public int maxHealthData;
    public int activePlayerIdData;
    public int manaData;
    public string[] obtainedPlayersNameData;
    public int[] doorsClosed;
    public PlayerData(int activePlayerId, int currentSpawnPointId, int health, int maxHealth, int mana, string[] obtainedPlayersName, int[] doorsClosedIds)
    {
        activePlayerIdData = activePlayerId;
        currentSpawnPointIdData = currentSpawnPointId;
        healthData = health;
        maxHealthData = maxHealth;
        manaData = mana;
        obtainedPlayersNameData = obtainedPlayersName;
        doorsClosed = doorsClosedIds;
    }
}
