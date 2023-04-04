using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FrameSaver : MonoBehaviour
{
    [SerializeField] PerceptionObjectsOnScreen percObjOnScreen;
    [SerializeField] ForegroundObjects foregroundObjects;
    [SerializeField] bool saveFrames = false;
    [SerializeField] string folderName = "SaveImages";
    [SerializeField] bool DEBUGOUTPUT = true;

    List<string> anomalyLabels = new List<string> { "Jaywalker" };
    string savedImagesLocation;
    bool hasGeneratedImages = false;
    int imageNumber = 1;
    string dirPath;

    void Start()
    {
        dirPath = Application.dataPath + "/../" + folderName + "/";
        EnsureFolderExists(dirPath);

        savedImagesLocation = dirPath + System.DateTime.Now.ToString("yyyy/MM/dd HH.mm.ss") + "/";
        EnsureFolderExists(savedImagesLocation);
    }
    private void OnDestroy()
    {
        if (!hasGeneratedImages)
        {
            Directory.Delete(savedImagesLocation);
        }
    }

    void Update()
    {
        if (saveFrames)
        {
            hasGeneratedImages = true;
            Texture2D lastFrame = foregroundObjects.GetOutputFrame();
            if (lastFrame == null)
            {
                Debug.LogError("LastFrame was null");
                return;
            }
            Dictionary<string, int> objectsOnScreenLastFrame = percObjOnScreen.GetObjectsOnScreen();
            SaveImageAnomalyFrameNotation(lastFrame, objectsOnScreenLastFrame);
        }
    }


    void EnsureFolderExists(string path)
    {
        if (!Directory.Exists(path))
        {
            if (DEBUGOUTPUT) { Debug.Log("Creating directory: " + path); }
            Directory.CreateDirectory(path);
        }
    }
    void SaveImageObjectNotation(Texture2D texture, Dictionary<string, int> objectsOnScreene)
    {
        // TODO: Make this, potentially. We don't actually plan to use it for testing I think.
    }

    void SaveImageAnomalyFrameNotation(Texture2D texture, Dictionary<string, int> objectsOnScreen)
    {
        var anomalyPath = savedImagesLocation + "anomalies/";
        var normalPath = savedImagesLocation + "normal/";
        EnsureFolderExists(anomalyPath);
        EnsureFolderExists(normalPath);

        bool anomalyPresent = false;
        foreach (KeyValuePair<string, int> objectOnScreen in objectsOnScreen)
        {
            if (anomalyLabels.Contains(objectOnScreen.Key) && objectOnScreen.Value > 0)
            {
                anomalyPresent = true;
                break;
            }
        }

        byte[] bytes = texture.EncodeToPNG();
        string imageName = "Image" + imageNumber;
        string path = anomalyPresent ? anomalyPath : normalPath;
        File.WriteAllBytes(path + imageName + ".png", bytes);
        imageNumber++;

        if (DEBUGOUTPUT) { Debug.Log("Saved image at " + path + imageName + ".png"); }
    }

    // API
    public void BeginRecording()
    {
        saveFrames = true;
    }
    public void StopRecording()
    {
        saveFrames = false;
    }
}
