using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using AnotherFileBrowser.Windows;
using UnityEngine.Networking;
using UnityEngine.U2D;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;
using Image = UnityEngine.UI.Image;

public class ViewportHandler : MonoBehaviour
{
    [Header("MenuController")]
    public MenuController menuController;

    [Header("Regular GUI stuff")]
    public Renderer viewportPlane;
    public Image imagePlane;
    public Material defaultMaterial;
    public Material footageMaterial;

    [Header("FSpy & Camera Preview Rendering")]
    public Camera viewportCam;
    public Image FSpyImagePlane;
    public RenderTexture FSpyTexture;
    public RoadBuilder roadObject;
    public Transform fSpySetupObject;
    public Transform roadCamAnchor;
    public AnchorMovement anchorMovement;
    public bool logRenderingTimes = false;

    [Header("For mask rendering stuff")]
    public Material blackMaterial;
    public SpriteShape whiteSprite;
    public Camera renderCamera;
    public RenderTexture renderTexture;
    public PointController pointController;

    private Button footageButton;
    private Texture2D tex;
    private Texture2D userBackgroundInput;
    private Texture2D fSpyTexture2D;
    private bool spriteIsRendering = false;

    [HideInInspector]
    public bool isFootageLoaded { get; private set; }


    private void Start()
    {
        isFootageLoaded = false;
        tex = new Texture2D(0, 0);
        menuController.onRoadTransformUpdateEvent += UpdateTransform;
        menuController.onRoadSettingUpdateEvent += UpdateRoadSetting;
        menuController.onRoadLengthUpdateEvent += UpdateRoadSetting;
        menuController.onTrafficSettingUpdateEvent += UpdateTrafficSetting;
        menuController.onAnomalyOptionUpdateEvent += UpdateTrafficSetting;
    }

    private void Update()
    {
        if(!spriteIsRendering && !roadObject.gameObject.GetComponent<Road>().paused)
        {
            ForceRenderPreviewSprite();
        }
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
        renderCamera.gameObject.SetActive(true);
        renderCamera.Render();
        renderCamera.gameObject.SetActive(false);
        Texture2D texture2D = new Texture2D(1500, 720);
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

    public void LoadFSpy(OpenFSpyFromUnity fSpy)
    {
        MenuElementCollection.TransformElements.ResetValues();
        MakeViewportCameraChildOfRoad(false);
        fSpy.FindFSpySavedFiles();
        MakeViewportCameraChildOfRoad(true);
        CenterCam();
        anchorMovement.SaveFSpyLocation();
        ForceRenderPreviewSprite();
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
                MenuElementCollection.TransformElements.ResetValues();
                var uwrTexture = DownloadHandlerTexture.GetContent(uwr);
                userBackgroundInput = uwrTexture;
                //footageMaterial.mainTexture = uwrTexture;
                viewportPlane.material = footageMaterial;
                FSpyImagePlane.sprite = Sprite.Create(uwrTexture, new Rect(0, 0, uwrTexture.width, uwrTexture.height), new Vector2(0.5f, 0.5f));
                FSpyImagePlane.color = new Color(100f, 100f, 100f);
                footageButton.style.display = DisplayStyle.None;
                CenterCam();
                isFootageLoaded = true;
                ForceRenderPreviewSprite();
            }
        }
    }

    public void MakeViewportCameraChildOfRoad(bool yes)
    {
        if (yes)
        {
            viewportCam.transform.parent = roadCamAnchor;
        }
        else
        {
            viewportCam.transform.parent = fSpySetupObject;
        }
    }

    public void CenterCam()
    {
        viewportCam.transform.position = roadCamAnchor.position - viewportCam.transform.forward * menuController.GetRoadTransform().distance;
        roadCamAnchor.Rotate(-90f, 90f, 0f);
    }

    public void RenderPreviewSprite()
    {
        if(isFootageLoaded && roadObject.GetComponent<Road>().paused) StartCoroutine(RenderPreviewSpriteRutine());
    }

    void ForceRenderPreviewSprite()
    {
        if (isFootageLoaded) StartCoroutine(RenderPreviewSpriteRutine());
    }

    IEnumerator RenderPreviewSpriteRutine()
    {
        spriteIsRendering = true;
        float timer = Time.realtimeSinceStartup;
        viewportCam.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        Destroy(fSpyTexture2D);
        fSpyTexture2D = GetTheTexture2D(FSpyTexture);
        if(!menuController.enableStableRendering) viewportCam.gameObject.SetActive(false);
        imagePlane.sprite = Sprite.Create(fSpyTexture2D, new Rect(0, 0, fSpyTexture2D.width, fSpyTexture2D.height), new Vector2(0.5f, 0.5f));
        imagePlane.color = new Color(100f, 100f, 100f);
        spriteIsRendering = false;
        if(logRenderingTimes) Debug.Log("Finished sprite render in time: " + (Time.realtimeSinceStartup - timer));
    }

    Texture2D GetTheTexture2D(RenderTexture rTex)
    {
        RenderTexture goodRenderTexture = new RenderTexture(rTex.width, rTex.height, rTex.depth, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        viewportCam.targetTexture = goodRenderTexture;
        viewportCam.Render();
        RenderTexture.active = goodRenderTexture;
        
        tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        //RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        viewportCam.targetTexture = rTex;
        Destroy(goodRenderTexture);
        return tex;
    }

    public bool PlayPausePreview()
    {
        Road road = roadObject.gameObject.GetComponent<Road>();
        road.paused = !road.paused;
        //road.transparent = !road.paused;
        return road.paused;
    }

    //
    // Updaters
    void UpdateTransform()
    {
        anchorMovement.UpdateTransform(menuController.GetRoadTransform());
    }

    void UpdateRoadSetting()
    {
        roadObject.roadScale = GetRoadSetting("Road").leftValue;
        roadObject.bikelaneLeftScale = GetRoadSetting("Bike lanes").leftValue;
        roadObject.bikelaneRightScale = GetRoadSetting("Bike lanes").rightValue;
        roadObject.sidewalkLeftScale = GetRoadSetting("Side walks").leftValue;
        roadObject.sidewalkRightScale = GetRoadSetting("Side walks").rightValue;
        roadObject.roadLength = menuController.GetRoadLength();
        roadObject.RebuildRoad();
    }
    
    void UpdateTrafficSetting()
    {
        Road road = roadObject.GetComponent<Road>();
        StringToSettingConverter stsc = new StringToSettingConverter();
        List<AnomalyOption> anomalyOptions = menuController.GetAnomalies();
        road.jaywalkCooldownMinMax = road.jaywalkCooldownMinMaxDefault * (100 / stsc.GetNamedSetting("Jaywalking", anomalyOptions).value);
        road.cyclistOnSidewalkCooldownMinMax = road.cyclistOnSidewalkCooldownMinMaxDefault * (100 / (stsc.GetNamedSetting("Cyclist on sidewalk", anomalyOptions)).value);

        List<TrafficSetting> trafficSettings = menuController.GetTrafficSettings();
        road.SetCarSpeedModifier((stsc.GetNamedSetting("Traffic speed", trafficSettings)).value);
        road.carCooldownMinMax = road.carCooldownMinMaxDefault * (100 / (stsc.GetNamedSetting("Traffic density", trafficSettings)).value);
        road.pedestrianCooldownMinMax = road.pedestrianCooldownMinMaxDefault * (100 / (stsc.GetNamedSetting("Pedestrians", trafficSettings)).value);
        road.cyclistCooldownMinMax = road.cyclistCooldownMinMaxDefault * (100 / (stsc.GetNamedSetting("Bikes", trafficSettings)).value);
        road.carLeftOffset = (stsc.GetNamedSetting("Traffic density", trafficSettings)).offsetLeft;
        road.carRightOffset = (stsc.GetNamedSetting("Traffic density", trafficSettings)).offsetRight;
        road.pedestrianLeftsidewalkOffset = (stsc.GetNamedSetting("Pedestrians", trafficSettings)).offsetLeft;
        road.pedestrianRightsidewalkOffset = (stsc.GetNamedSetting("Pedestrians", trafficSettings)).offsetRight;
        road.cyclistLeftbikelaneOffset = (stsc.GetNamedSetting("Bikes", trafficSettings)).offsetLeft;
        road.cyclistRightbikelaneOffset = (stsc.GetNamedSetting("Bikes", trafficSettings)).offsetRight;

        road.DoBigCooldownReset();
    }

    RoadSetting GetRoadSetting(string name)
    {
        List<RoadSetting> roadSettings = menuController.GetRoadSettings();
        for(int i = 0; i < roadSettings.Count; i++)
        {
            if (roadSettings[i].name == name) return roadSettings[i];
        }
        return null;
    }

    public Texture2D GetUserBackgroundInput()
    {
        return userBackgroundInput;
    }
}


