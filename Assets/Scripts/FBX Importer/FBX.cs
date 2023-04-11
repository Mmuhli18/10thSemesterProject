using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBX : MonoBehaviour
{
    public void begin() 
    {
        FBXImporter importer = new FBXImporter();
        importer.ImportAllFBX();
    }
}
