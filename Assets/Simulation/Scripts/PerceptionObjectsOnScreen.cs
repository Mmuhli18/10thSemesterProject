using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.GroundTruth.LabelManagement;

public class PerceptionObjectsOnScreen : MonoBehaviour
{
    [SerializeField] IdLabelConfig labelConfig;

    [SerializeField] Dictionary<string, int> objectsOnScreen;
    PerceptionCamera percepCam;
    List<uint> labelsBeingRendered;
    // Start is called before the first frame update
    void Start()
    {
        objectsOnScreen = new Dictionary<string, int>();
        labelsBeingRendered = new List<uint>();
        percepCam = GetComponent<PerceptionCamera>();
        percepCam.RenderedObjectInfosCalculated += OnRenderedObjectInfosCalculated;
    }
    private void OnDisable()
    {
        percepCam.RenderedObjectInfosCalculated -= OnRenderedObjectInfosCalculated;
    }

    void UpdateDictionary()
    {
        objectsOnScreen.Clear();
        foreach (IdLabelEntry labelEntry in labelConfig.labelEntries)
        {
            objectsOnScreen.Add(labelEntry.label, 0);
        }
        Labeling[] labels = FindObjectsOfType<Labeling>();
        foreach (Labeling label in labels)
        {
            string name = label.labels[0];
            uint id = label.instanceId;
            bool rendered = labelsBeingRendered.Contains(id);
            if (rendered)
            {
                objectsOnScreen[name] += 1;
            }
        }
    }


    private void OnRenderedObjectInfosCalculated(int frameNumber, NativeArray<RenderedObjectInfo> renderedObjectInfoArray, SceneHierarchyInformation sceneHierarchyInfo)
    {
        labelsBeingRendered.Clear();
        foreach (RenderedObjectInfo renderedObjectInfo in renderedObjectInfoArray)
        {
            //Debug.Log("OBJECT INFO:" + renderedObjectInfo.instanceId);
            labelsBeingRendered.Add(renderedObjectInfo.instanceId);
        }
        UpdateDictionary();
    }

    //API
    public Dictionary<string, int> GetObjectsOnScreen()
    {
        return objectsOnScreen;
    }
}
