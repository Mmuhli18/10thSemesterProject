using UnityEngine;

/* Our scene camera for creating the preview is attached to an anchor object, we them move this object instead of the road and 
 * therefore also the camera. This give the impression of moving road in the preview which is better for usabillity. 
 * Movement of this anchor object is done here */

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

    //On being instanciated the anchor saves its coordinates
    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation.eulerAngles;
    }

    //When FSpy data has been loaded this is run to find and save differences from the original position
    //This function is run after FSpy data is loaded
    public void ApplyAndSaveFSpyRotation()
    {
        fSpyPosition = originalPosition - transform.position;
        
        viewportCam.transform.position = transform.position - viewportCam.transform.forward * 80f; //Centering camera at distance of 80
        transform.Rotate(-90f, 90f, 0f); //This line may break future fSpy files, but it is what saved mine. 
        Quaternion camRotation = viewportCam.transform.rotation; //we save the camera rotation to apply to the anchor
        viewportCam.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        transform.rotation = camRotation; //rotation is applied to anchor
        viewportCam.transform.position = transform.position - transform.forward * 80f; //position camera based on anchor rotation
        fSpyRotation = transform.rotation.eulerAngles - originalRotation; //Saving the relative change applied by fSpy
        fSpyRotation = new Vector3(Mathf.Round(fSpyRotation.x), Mathf.Round(fSpyRotation.y), Mathf.Round(fSpyRotation.z));
        MenuElementCollection.TransformElements.fSpyRotation = fSpyRotation;
        //As the UI elements do not currently have the fSpy roation, we reset them to get it, this then also positions the camera
        MenuElementCollection.TransformElements.ResetValues(); 

    }

    //Takes in the seetings from the road transform menu. It then moves and rotates the anchor and places the camera at defined distance
    //We use the camera's forward vector as the camera may have been rotated to a different perpective when loading FSpy data
    public void UpdateTransform(RoadTransformSetting transformSetting)
    {
        transform.position = originalPosition + fSpyPosition - transformSetting.position;
        transform.rotation = Quaternion.Euler(originalRotation + transformSetting.rotation);
        viewportCam.transform.position = transform.position - viewportCam.transform.forward * transformSetting.distance;
    }
}
