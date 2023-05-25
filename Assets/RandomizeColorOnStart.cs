using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeColorOnStart : MonoBehaviour
{
    public bool useSpecificColor = false;
    public Color color;
    public string materialName;

    // Start is called before the first frame update
    void Start()
    {
        Color colorToUse = color;
        if (!useSpecificColor)
        {
            float H = Random.Range(0, 1f);
            float S = Random.Range(0, 1f);
            float V = Random.Range(0, 1f);
            colorToUse = Color.HSVToRGB(H, S, V);
        }

        string instancedMaterialName = materialName + " (Instance)";

        foreach(MeshRenderer meshRenderer in transform.GetComponentsInChildren<MeshRenderer>())
        {
            foreach(Material mat in meshRenderer.materials)
            {
                if (mat.name.Equals(instancedMaterialName))
                {
                    mat.color = colorToUse;
                }
            }
        }
    }
}
