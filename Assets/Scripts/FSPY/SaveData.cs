using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saver_Loader
{
    public void saveAsJson(DataToBeSaved data, string nameOfFile) 
    {
        string dataToBeSaved = JsonUtility.ToJson(data);
        string pathToSavedFile = Application.persistentDataPath +"/"+ nameOfFile + ".json";
        System.IO.File.WriteAllText(pathToSavedFile, dataToBeSaved);
    }

    public DataToBeSaved loadJson(string nameOfFileToLoad) 
    {
        DataToBeSaved dataFromJson = new DataToBeSaved();

        string filePath = Application.persistentDataPath + "/" + nameOfFileToLoad + ".json";
        string data = System.IO.File.ReadAllText(filePath);
        dataFromJson = JsonUtility.FromJson<DataToBeSaved>(data);

        return dataFromJson;
    }

}

[System.Serializable]
public class DataToBeSaved 
{
    public string _path ;
    public void setPath(string newPath)
    {
        _path = newPath;
    }
    public string getPath() 
    {
        return _path;
    }

}
