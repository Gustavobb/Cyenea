using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem 
{
    public static void SavePlayer(int activePlayerId, int currentSpawnPointId, int health, int maxHealth, int mana, string[] obtainedPlayersName, int[] doorsClosedIds)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.miko";
        FileStream stream = new FileStream(path, FileMode.Create);
        PlayerData data = new PlayerData(activePlayerId, currentSpawnPointId, health, maxHealth, mana, obtainedPlayersName, doorsClosedIds);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.miko";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        } 
        else
        {
            Debug.LogError("Save file not found in " + path);
        }

        return null;
    }

    public static bool FileExists()
    {
        string path = Application.persistentDataPath + "/player.miko";
        return File.Exists(path);
    }

    public static void CreateFile()
    {
        SavePlayer(0, 2, 4, 4, 0, new string[1]{"Paladin"}, new int[0]);
    }
}
