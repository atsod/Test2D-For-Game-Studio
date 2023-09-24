using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] private Player _player;

    private void Awake()
    {
        LoadGame();
    }

    private void OnDestroy()
    {
        SaveGame();
    }

    void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
          + "/MySaveData.dat");
        SaveData data = new SaveData();

        data.CurrentHealthPoints = _player.CurrentHealthPoints;
        
        bf.Serialize(file, data);
        file.Close();

        Debug.Log("Game data saved!");
    }

    void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath
          + "/MySaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
              File.Open(Application.persistentDataPath
              + "/MySaveData.dat", FileMode.Open);
            file.Position = 0;
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();

            _player.CurrentHealthPoints = data.CurrentHealthPoints;

            Debug.Log("Game data loaded!");
        }
        else
        {
            SaveData data = new SaveData();

            _player.CurrentHealthPoints = data.CurrentHealthPoints;

            Debug.Log("There is no save data!");
        }
            
    }

    public void ResetData()
    {
        if (File.Exists(Application.persistentDataPath
          + "/MySaveData.dat"))
        {
            File.Delete(Application.persistentDataPath
              + "/MySaveData.dat");

            _player.CurrentHealthPoints = _player.MaxHealthPoints;

            Debug.Log("Data reset complete!");
        }
        else
            Debug.LogError("No save data to delete.");
    }

    [System.Serializable]
    class SaveData
    {
        public int CurrentHealthPoints;

        public SaveData()
        {
            CurrentHealthPoints = 100;
        }
    }

}
