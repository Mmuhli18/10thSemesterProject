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
    [SerializeField] int outputLengthInFrames = 100;
    [SerializeField] bool DEBUGOUTPUT = true;
    [SerializeField] int skipFrames = 0;
    [SerializeField] List<Texture> backgrounds;
    [SerializeField] GameObject backgroundPlane;

    List<string> anomalyLabels = new List<string> { "Jaywalker" };
    string savedImagesLocation;
    bool hasGeneratedImages = false;
    int imageNumber = 0;
    string dirPath;
    int skipFramesTimer = 0;

    void Start()
    {
        if (backgroundPlane.TryGetComponent(out Renderer renderer))
        {
            renderer.sharedMaterial.mainTexture = backgrounds[0];
        }
        TryLoadSettingsFromMenu();

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

    void TryLoadSettingsFromMenu()
    {
        MenuSettingsForSimulation settings = FindObjectOfType<MenuSettingsForSimulation>();
        if (settings == null || settings.HasExported() == false) { return; }
        outputLengthInFrames = settings.videoLengthInFrames;
    }

    void Update()
    {
        if (saveFrames)
        {
            if(skipFramesTimer != skipFrames)
            {
                skipFramesTimer++;
                return;
            }
            else
            {
                skipFramesTimer = 0;
            }
            hasGeneratedImages = true;
            Texture2D lastFrame = foregroundObjects.GetOutputFrame();
            if (lastFrame == null)
            {
                Debug.LogError("LastFrame was null");
                return;
            }
            Dictionary<string, int> objectsOnScreenLastFrame = percObjOnScreen.GetObjectsOnScreen();
            SaveImageAnomalyFrameNotation(lastFrame, objectsOnScreenLastFrame);

            if (imageNumber > outputLengthInFrames)
            {
                saveFrames = false;
            }
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

        byte[] bytes = texture.EncodeToJPG(100);
        string imageName = imageNumber.ToString();
        if(imageNumber < 10) { imageName = "00" + imageName; }
        else if (imageNumber < 100) { imageName = "0" + imageName; }
        string path = anomalyPresent ? anomalyPath : normalPath;
        File.WriteAllBytes(path + imageName + ".jpg", bytes);
        imageNumber++;

        if (DEBUGOUTPUT) { Debug.Log("Saved image at " + path + imageName + ".jpg"); }
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
