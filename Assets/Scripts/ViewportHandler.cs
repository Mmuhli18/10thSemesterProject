using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using AnotherFileBrowser.Windows;
using UnityEngine.Networking;
using UnityEngine.U2D;

public class ViewportHandler : MonoBehaviour
{
    [Header("Regular GUI stuff")]
    public Renderer viewportPlane;
    public Material defaultMaterial;
    public Material footageMaterial;

    [Header("For mask rendering stuff")]
    public Material blackMaterial;
    public SpriteShape whiteSprite;
    public Camera renderCamera;
    public RenderTexture renderTexture;
    public PointController pointController;

    private Button footageButton;
    
    [HideInInspector]
    public bool isFootageLoaded { get; private set; }


    private void Start()
    {
        isFootageLoaded = false;
    }

    public Texture2D RenderMask()
    {
        //Black background
        Material currentMaterial = viewportPlane.material;
        viewportPlane.material = blackMaterial;
        //Shapes and dots
        SpriteShape defaultSprite = null;
        for(int i = 0; i < pointController.markings.Count; i++)
        {
            //shapes
            if (defaultSprite == null) defaultSprite = pointController.markings[i].spriteShapeController.spriteShape;
            pointController.markings[i].spriteShapeController.spriteShape = whiteSprite;
            //dots
            for(int j = 0; j < pointController.markings[i].dots.Count; j++)
            {
                pointController.markings[i].dots[j].gameObject.GetComponent<DotBehaviour>().renderDot.SetActive(false);
            }
        }
        renderCamera.Render();
        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height);
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();
        viewportPlane.material = currentMaterial;
        for (int i = 0; i < pointController.markings.Count; i++)
        {
            pointController.markings[i].spriteShapeController.spriteShape = defaultSprite;
            //dots
            for (int j = 0; j < pointController.markings[i].dots.Count; j++)
            {
                pointController.markings[i].dots[j].gameObject.GetComponent<DotBehaviour>().renderDot.SetActive(true);
            }
        }
        return texture2D;
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


