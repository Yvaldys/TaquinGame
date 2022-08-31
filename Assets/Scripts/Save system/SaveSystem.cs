using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    #region Exposed
    
    #endregion

    #region Main methods
    public static void SaveBestTime(BestTime bestTime) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.scores";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, bestTime);
        stream.Close();
    }

    public static BestTime LoadBestTime() {
        string path = Application.persistentDataPath + "/player.scores";
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            BestTime data = formatter.Deserialize(stream) as BestTime;
            stream.Close();

            return data;
        } else return null;
    }

    public static void DestroySaveData() {
        string path = Application.persistentDataPath + "/player.scores";
        File.Delete(path);
    }
    #endregion

    #region Private & Protected
    
    #endregion
}
