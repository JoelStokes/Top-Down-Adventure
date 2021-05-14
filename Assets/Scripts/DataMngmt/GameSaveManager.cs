using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager instance;

    //public FileData fileData;
    /*
    https://www.youtube.com/watch?v=SNwPq01yHds
    */

    private void Awake()    //Setup to always have GameSaveManager in scene and have it easily be called by others
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this) {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    public bool IsSaveFile()
    {
        return Directory.Exists(Application.persistentDataPath + "/game_save");    //Returns user's application data location
    }

    public void SaveGame()
    {
        if (!IsSaveFile())  //If save doesn't exist, we create it here
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/game_save");
        }

        if (!Directory.Exists(Application.persistentDataPath + "/game_save/save_file"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/game_save/character_data");
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/game_save/character_data/character_save");

        //var json = JsonUtility.ToJson()
    }
}
