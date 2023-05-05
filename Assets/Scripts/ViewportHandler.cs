using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
//3rd part AnotherFileBrowser system used
using AnotherFileBrowser.Windows;
using UnityEngine.Networking;
using UnityEngine.U2D;
using Button = UnityEngine.UIElements.Button;
using Image = UnityEngine.UI.Image;

/* The class we use as our main controller for the viewport. Main functionallity is updating and rendering
 * the preview for the user based on the user input. It also handles loading in the footage, and fSpy information
 */
public class ViewportHandler : MonoBehaviour
{
    [Header("MenuController")]
    public MenuController menuController;

    [Header("Regular GUI stuff")]
    public Renderer viewportPlane;
    public Image imagePlane;
    public Material defaultMaterial;
    public Material footageMaterial;
    public GameObject pointyBois;

    [Header("FSpy & Camera Preview Rendering")]
    public Camera viewportCam;
    public Image FSpyImagePlane;
    public RenderTexture FSpyTexture;
    public RoadBuilder roadObject;
    public Transform fSpySetupObject;
    public Transform roadCamAnchor;
    public AnchorMovement anchorMovement;
    public bool logRenderingTimes = false;
    public Transform lightTransform;
    public Material shadowReceiver;

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
        menuController.onLightingSettingUpdateEvent += UpdateLight;
    }

    private void Update()
    {
        //If the preview-simulation is running, a sprite for the preview is render every frame
        if(!spriteIsRendering && !roadObject.gameObject.GetComponent<Road>().paused)
        {
            ForceRenderPreviewSprite();
        }
    }

    /* Function for rendering a binary mask of the foreground markings, this is then used for rendering in the real simulation
     */
    public Texture2D RenderMask()
    {
        //set Black background
        Material currentMaterial = viewportPlane.material;
        viewportPlane.material = blackMaterial;
        //set Shapes and dots as white
        SpriteShape defaultSprite = null;
        for(int i = 0; i < pointController.markings.Count; i++)
        {
            //shapes
            if (defaultSprite == null) defaultSprite = pointController.markings[i].spriteShapeController.spriteShape;
            pointController.markings[i].spriteShapeController.spriteShape = whiteSprite;
            //dots
            for(int j = 0; j < pointController.markings[i].dots.Count; j++)
            {
                pointController.markings[i].dots[j].gameObject.GetComponent<AnnotationDotBehaviour>().renderDot.SetActive(false);
            }
        }
        //render the mask frame and save the texture
        renderCamera.gameObject.SetActive(true);
        renderCamera.Render();
        renderCamera.gameObject.SetActive(false);
        Texture2D texture2D = new Texture2D(1500, 720);
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();
        //Turn everything back to previous state
        viewportPlane.material = currentMaterial;
        for (int i = 0; i < pointController.markings.Count; i++)
        {
            pointController.markings[i].spriteShapeController.spriteShape = defaultSprite;
            //dots
            for (int j = 0; j < pointController.markings[i].dots.Count; j++)
            {
                pointController.markings[i].dots[j].gameObject.GetComponent<AnnotationDotBehaviour>().renderDot.SetActive(true);
            }
        }
        //return mask
        return texture2D;
    }

    // opens file browser for a user to find and select their background image
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

    //Function for loading in the fSpy information and applying it to the UI
    public void LoadFSpy(OpenFSpyFromUnity fSpy)
    {
        MenuElementCollection.TransformElements.ResetValues();
        MakeViewportCameraChildOfRoad(false);
        fSpy.FindFSpySavedFiles();
        MakeViewportCameraChildOfRoad(true);
        anchorMovement.ApplyAndSaveFSpyRotation();
        ForceRenderPreviewSprite();
    }

    //Function that loads the selected background image into the UI and preview-simulation
    IEnumerator LoadFootage(string path)
    {
        //Start a request for getting an image file
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
        {
            yield return uwr.SendWebRequest();
            //if the user closed the browser window
            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(uwr.error);
                viewportPlane.material = defaultMaterial;
            }
            //if an image is selected
            else
            {
                MenuElementCollection.TransformElements.ResetValues();
                var uwrTexture = DownloadHandlerTexture.GetContent(uwr);
                userBackgroundInput = uwrTexture;
                //setting the background material and sprite to be the selected background image
                viewportPlane.material = footageMaterial;
                FSpyImagePlane.sprite = Sprite.Create(uwrTexture, new Rect(0, 0, uwrTexture.width, uwrTexture.height), new Vector2(0.5f, 0.5f));
                FSpyImagePlane.color = new Color(100f, 100f, 100f);
                footageButton.style.display = DisplayStyle.None;
                //Centering cam for reasons
                CenterCam();
                //Set footage as successfully loaded
                isFootageLoaded = true;
                //render a sprite for the preview, now with a background
                ForceRenderPreviewSprite();
            }
        }
    }

    //Function this is used as part of loading fSpy, we do not want the viewport cam to be child of road while loading fSpy
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

    //centers so road is in the middle of the preview
    public void CenterCam()
    {
        viewportCam.transform.position = roadCamAnchor.position - viewportCam.transform.forward * menuController.GetRoadTransform().distance;
    }

    //renders a sprite for the preview, if the preview-simulation is not already running
    public void RenderPreviewSprite()
    {
        if(isFootageLoaded && roadObject.GetComponent<Road>().paused) StartCoroutine(RenderPreviewSpriteRutine());
    }

    //forces a render even if preview-simulation is running
    void ForceRenderPreviewSprite()
    {
        if (isFootageLoaded) StartCoroutine(RenderPreviewSpriteRutine());
    }

    /*The corutine for rendering a sprite for the preview
     * Enables the viewport cam, renders a frame and loads it into the preview, disables the viewport cam if stable rendering is enabled
     */
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

    //returns a Texture2D from a render texture
    Texture2D GetTheTexture2D(RenderTexture rTex)
    {
        RenderTexture goodRenderTexture = new RenderTexture(rTex.width, rTex.height, rTex.depth, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        viewportCam.targetTexture = goodRenderTexture;
        viewportCam.Render();
        RenderTexture.active = goodRenderTexture;
        
        tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        viewportCam.targetTexture = rTex;
        Destroy(goodRenderTexture);
        return tex;
    }

    //Enables and disables the preview
    public bool PlayPausePreview()
    {
        Road road = roadObject.gameObject.GetComponent<Road>();
        road.paused = !road.paused;
        pointyBois.SetActive(road.paused);
        RenderPreviewSprite();
        return road.paused;
    }

    //
    // Updaters
    // Updates values on the road object in the preview-simulation and on the camera anchor
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

    void UpdateLight()
    {
        LightingSetting setting = menuController.GetLightingSettings();
        lightTransform.localEulerAngles = setting.direction;
        var col = Color.HSVToRGB(setting.shadowColor.x / 255f, setting.shadowColor.y / 255f, setting.shadowColor.z / 255f);
        Debug.Log(col);
        shadowReceiver.SetColor("_Shadow_Color", new Color(col.r, col.g, col.b, setting.shadowColor.w / 255));
        Debug.Log(shadowReceiver.GetColor("_Shadow_Color"));
    }

    //gets a raod setting from the menu controller based on a name
    RoadSetting GetRoadSetting(string name)
    {
        List<RoadSetting> roadSettings = menuController.GetRoadSettings();
        for(int i = 0; i < roadSettings.Count; i++)
        {
            if (roadSettings[i].name == name) return roadSettings[i];
        }
        return null;
    }

    //Get the background the user selected
    public Texture2D GetUserBackgroundInput()
    {
        return userBackgroundInput;
    }
}


