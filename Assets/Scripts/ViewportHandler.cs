using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ViewportHandler : MonoBehaviour
{
    public Renderer planeRenderer;
    public Material defaultMaterial;
    public Material footageMaterial;

    public void AddFootage(Button button)
    {
        planeRenderer.material = footageMaterial;
        button.style.display = DisplayStyle.None;
    }
}
