using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using AnotherFileBrowser.Windows;
using UnityEngine.Networking;
using UnityEngine.U2D;

public class ViewportHandler : MonoBehaviour
{
    public Renderer viewportPlane;
    public Material defaultMaterial;
    public Material footageMaterial;
    [SerializeField]
    GameObject spriteShapeRenderer;
    private Button footageButton;
    public int activeMarking = -1;
    public bool isFootageLoaded { get; private set; }
    public List<ForegroundMarking> markings = new List<ForegroundMarking>();

    private void Start()
    {
        isFootageLoaded = false;
    }

    public void AddMarking()
    {
        markings.Add(new ForegroundMarking());
        markings[markings.Count - 1].spriteShapeController = Instantiate(spriteShapeRenderer, transform).GetComponent<SpriteShapeController>();
        activeMarking = markings.Count - 1;
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

public class ForegroundMarking
{
    public List<Transform> points = new List<Transform>();
    public SpriteShapeController spriteShapeController; 
}
