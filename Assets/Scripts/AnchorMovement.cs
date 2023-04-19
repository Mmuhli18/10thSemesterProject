using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorMovement : MonoBehaviour
{
    [SerializeField]
    Camera viewportCam;
    [Header("OGs")]
    [SerializeField]
    Vector3 originalPosition;
    [SerializeField]
    Vector3 originalRotation;
    [Header("FSpy")]
    [SerializeField]
    Vector3 fSpyPosition;
    [SerializeField]
    Vector3 fSpyRotation;

    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation.eulerAngles;
    }

    public void SaveFSpyLocation()
    {
        fSpyPosition = originalPosition - transform.position;
        fSpyRotation = originalRotation - transform.rotation.eulerAngles;
    }

    public void UpdateTransform(RoadTransformSetting transformSetting)
    {
        transform.position = originalPosition + fSpyPosition + transformSetting.position;
        transform.rotation = Quaternion.Euler(originalRotation + fSpyRotation + transformSetting.rotation);
        viewportCam.transform.position = transform.position - viewportCam.transform.forward * transformSetting.distance;
    }

    public void UpdateRoadLength(float Length)
    {

    }
}
