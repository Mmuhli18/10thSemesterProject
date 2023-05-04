using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBuilder : MonoBehaviour
{
    public GameObject roadModel;
    public GameObject sidewalkLeftModel;
    public GameObject sidewalkRightModel;
    public GameObject bikelaneLeftModel;
    public GameObject bikelaneRightModel;

    public GameObject roadHitbox;
    public GameObject sidewalkLeftHitbox;
    public GameObject sidewalkRightHitbox;
    public GameObject bikelaneLeftHitbox;
    public GameObject bikelaneRightHitbox;

    public float roadLength = 50f;
    public float roadScale = 10f;
    public float sidewalkLeftScale = 4f;
    public float sidewalkRightScale = 4f;
    public float bikelaneLeftScale = 1f;
    public float bikelaneRightScale = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // Attempt to load values.
        TryLoadMenuSettings();

        RebuildRoad();
    }

    void TryLoadMenuSettings()
    {
        MenuSettingsForSimulation settings = FindObjectOfType<MenuSettingsForSimulation>();
        if(settings == null || settings.HasExported() == false) { return; }
        roadLength = settings.roadLength;
        roadScale = settings.roadWidth;
        sidewalkLeftScale = settings.sidewalkWidthLeft;
        sidewalkRightScale = settings.sidewalkWidthRight;
        bikelaneLeftScale = settings.bikelaneWidthLeft;
        bikelaneRightScale = settings.bikelaneWidthRight;
    }


    public void RebuildRoad()
    {
        var gameObjectLocalScale = gameObject.transform.localScale;
        gameObjectLocalScale.z = roadLength / 50f; // it has a 1:50 ratio because I'm bad at decision making and too lazy to remake the road to fix it.
        gameObject.transform.localScale = gameObjectLocalScale;

        // Modify scales
        var roadLocalScale = roadModel.transform.localScale;
        roadLocalScale.x = roadScale;
        roadModel.transform.localScale = roadLocalScale;
        roadHitbox.transform.localScale = roadLocalScale;

        var bikelaneLeftLocalScale = bikelaneLeftModel.transform.localScale;
        var bikelaneRightLocalScale = bikelaneRightModel.transform.localScale;
        bikelaneLeftLocalScale.x = bikelaneLeftScale;
        bikelaneRightLocalScale.x = bikelaneRightScale;
        bikelaneLeftModel.transform.localScale = bikelaneLeftLocalScale;
        bikelaneLeftHitbox.transform.localScale = bikelaneLeftLocalScale;
        bikelaneRightModel.transform.localScale = bikelaneRightLocalScale;
        bikelaneRightHitbox.transform.localScale = bikelaneRightLocalScale;

        var bikelaneRightPosition = bikelaneRightModel.transform.localPosition;
        bikelaneRightPosition.x = (roadScale / 2) + (bikelaneRightScale / 2);
        bikelaneRightModel.transform.localPosition = bikelaneRightPosition;
        bikelaneRightHitbox.transform.localPosition = bikelaneRightPosition;
        var bikelaneLeftPosition = bikelaneLeftModel.transform.localPosition;
        bikelaneLeftPosition.x = -((roadScale / 2) + (bikelaneLeftScale / 2));
        bikelaneLeftModel.transform.localPosition = bikelaneLeftPosition;
        bikelaneLeftHitbox.transform.localPosition = bikelaneLeftPosition;

        var sidewalkLeftLocalScale = sidewalkLeftModel.transform.localScale;
        var sidewalkRightLocalScale = sidewalkRightModel.transform.localScale;
        sidewalkLeftLocalScale.x = sidewalkLeftScale;
        sidewalkRightLocalScale.x = sidewalkRightScale;
        sidewalkLeftModel.transform.localScale = sidewalkLeftLocalScale;
        sidewalkLeftHitbox.transform.localScale = sidewalkLeftLocalScale;
        sidewalkRightModel.transform.localScale = sidewalkRightLocalScale;
        sidewalkRightHitbox.transform.localScale = sidewalkRightLocalScale;

        var sidewalkRightPosition = sidewalkRightModel.transform.localPosition;
        sidewalkRightPosition.x = (roadScale / 2) + (sidewalkRightScale / 2) + (bikelaneRightScale);
        sidewalkRightModel.transform.localPosition = sidewalkRightPosition;
        sidewalkRightHitbox.transform.localPosition = sidewalkRightPosition;

        var sidewalkLeftPosition = sidewalkLeftModel.transform.localPosition;
        sidewalkLeftPosition.x = -((roadScale / 2) + (sidewalkLeftScale / 2) + (bikelaneLeftScale));
        sidewalkLeftModel.transform.localPosition = sidewalkLeftPosition;
        sidewalkLeftHitbox.transform.localPosition = sidewalkLeftPosition;
    }

    private void OnValidate()
    {
        roadScale = Mathf.Max(0, roadScale);
        sidewalkLeftScale = Mathf.Max(0, sidewalkLeftScale);
        sidewalkRightScale = Mathf.Max(0, sidewalkRightScale);
        bikelaneLeftScale = Mathf.Max(0, bikelaneLeftScale);
        bikelaneRightScale = Mathf.Max(0, bikelaneRightScale);
        RebuildRoad();
    }
}
