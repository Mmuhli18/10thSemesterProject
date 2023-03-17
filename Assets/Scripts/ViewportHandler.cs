using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using AnotherFileBrowser.Windows;
using UnityEngine.Networking;

public class ViewportHandler : MonoBehaviour
{
    public Renderer viewportPlane;
    public Material defaultMaterial;
    public Material footageMaterial;
    
    private Button footageButton;
    
    public bool isFootageLoaded { get; private set; }


    private void Start()
    {
        isFootageLoaded = false;
    }

    public void AddFootage(Button button)
    {
        footageButton = button;
        var bp = new BrowserProperties();
        bp.filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            //Load image from local path with UWR
            StartCoroutine(LoadFootage(path));
        });
    }

    IEnumerator LoadFootage(string path)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(uwr.error);
                viewportPlane.material = defaultMaterial;
            }
            else
            {
                var uwrTexture = DownloadHandlerTexture.GetContent(uwr);
                footageMaterial.mainTexture = uwrTexture;
                viewportPlane.material = footageMaterial;
                footageButton.style.display = DisplayStyle.None;
                isFootageLoaded = true;
            }
        }
    }
}


