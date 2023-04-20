using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSettingsForSimulation : MonoBehaviour
{
    bool hasExported = false;
    MenuController menu;
    ViewportHandler viewport;

    public Texture2D background;
    public Texture2D mask;
    public Vector3 roadPosition;
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;

    public float jaywalkFrequency;
    public float cyclistOnSidewalkFrequency;
    public float carDensity;
    public float carSpeed;
    public float pedestrianFrequency;
    public float bikeFrequency;
    public float caroffsetleft;
    public float caroffsetright;
    public float pedestrianoffsetleft;
    public float pedestrianoffsetright;
    public float bikeoffsetleft;
    public float bikeoffsetright;

    public float lightingAmbient;
    public float lightingIntensity;
    public Vector3 lightingDirection;
    public Vector4 lightingShadow;

    public float roadLength;
    public float roadWidth;
    public float bikelaneWidthLeft;
    public float bikelaneWidthRight;
    public float sidewalkWidthLeft;
    public float sidewalkWidthRight;

    public int videoLengthInFrames;



    // Start is called before the first frame update
    void Start()
    {
        transform.parent = null;
        menu = FindObjectOfType<MenuController>();
        viewport = FindObjectOfType<ViewportHandler>();
        if(menu != null && viewport != null)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        menu.onExportClickedEvent += OnExportClicked;
    }

    void OnExportClicked()
    {
        background = viewport.GetUserBackgroundInput();
        mask = menu.GetMask();
        roadPosition = menu.GetRoadTransform().position;
        cameraPosition = Camera.main.transform.position; // Absolutely awful way of getting it, but this is how the FSpy script sets it (Which is honestly also a problem. Might break at any point, just for fun).
        cameraRotation = Camera.main.transform.localEulerAngles;

        foreach(AnomalyOption anomalySetting in menu.GetAnomalies())
        {
            if (anomalySetting.name.Equals("Jaywalking"))
            {
                jaywalkFrequency = anomalySetting.value;
            }
            else if (anomalySetting.name.Equals("Cyclist on sidewalk"))
            {
                cyclistOnSidewalkFrequency = anomalySetting.value;
            }
        }

        foreach(TrafficSetting trafficSetting in menu.GetTrafficSettings())
        {
            if (trafficSetting.name.Equals("Traffic density"))
            {
                carDensity = trafficSetting.value;
                caroffsetleft = trafficSetting.offsetLeft;
                caroffsetright = trafficSetting.offsetRight;
            }
            else if (trafficSetting.name.Equals("Traffic speed"))
            {
                carSpeed = trafficSetting.value;
            }
            else if (trafficSetting.name.Equals("Pedestrians"))
            {
                pedestrianFrequency = trafficSetting.value;
                pedestrianoffsetleft = trafficSetting.offsetLeft;
                pedestrianoffsetright = trafficSetting.offsetRight;
            }
            else if (trafficSetting.name.Equals("Bikes"))
            {
                bikeFrequency = trafficSetting.value;
                bikeoffsetleft = trafficSetting.offsetLeft;
                bikeoffsetright = trafficSetting.offsetRight;
            }
        }

        LightingSetting lightingSetting = menu.GetLightingSettings();
        lightingAmbient = lightingSetting.ambient;
        lightingIntensity = lightingSetting.intensity;
        lightingDirection = lightingSetting.direction;
        lightingShadow = lightingSetting.shadowColor;


        roadLength = menu.GetRoadLength();
        foreach(RoadSetting roadSetting in menu.GetRoadSettings())
        {
            if (roadSetting.name.Equals("Roads"))
            {
                roadWidth = roadSetting.leftValue;
            }
            else if (roadSetting.name.Equals("Bike lanes"))
            {
                bikelaneWidthLeft = roadSetting.leftValue;
                bikelaneWidthRight = roadSetting.rightValue;
            }
            else if (roadSetting.name.Equals("Side walks"))
            {
                sidewalkWidthLeft = roadSetting.leftValue;
                sidewalkWidthRight = roadSetting.rightValue;
            }
        }

        videoLengthInFrames = menu.GetExportSettings().videoLength;

        hasExported = true;
        SceneManager.LoadScene("Simulation Scene");
    }

    public bool HasExported()
    {
        return hasExported;
    }
}
