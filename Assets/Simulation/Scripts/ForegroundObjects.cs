using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ForegroundObjects : MonoBehaviour
{
    int imageNumber = 1;

    public GameObject planeOutput;
    public RenderTexture cameraOutput;
    public RenderTexture backgroundOutput;
    public Texture2D mask;
    public Texture2D background;

    bool[,] maskArray;

    Texture2D planeOutputTex;
    Texture2D cameraOutputTex;
    Texture2D backgroundOutputTex;

    public bool saveImage = false;
    public bool saveVideo = false;

    private void Start()
    {

        // Read the mask once, and save the data in the maskArray, which is a bool[,] since we only store if the pixel is part of the mask or not. 
        maskArray = new bool[mask.width, mask.height];
        for(int x = 0; x < mask.width; x++)
        {
            for(int y = 0; y < mask.height; y++)
            {
                if(mask.GetPixel(x, y).r > 0.5f) // Checking if it's equal to Color.White was inconsistent, so we just check if any color channel is above an arbitrary value that isn't 0.
                {
                    maskArray[x, y] = true;
                }
                else
                {
                    maskArray[x, y] = false;
                }
            }
        }
    }

    void Update()
    {
        StartCoroutine(UpdateEndOfFrame());
    }

    IEnumerator UpdateEndOfFrame()
    {
        yield return new WaitForEndOfFrame();

        Destroy(cameraOutputTex);
        Destroy(backgroundOutputTex);
        Destroy(planeOutputTex);
        planeOutputTex = ConvertRenderTextureToTexture2D(cameraOutput, false);
        cameraOutputTex = ConvertRenderTextureToTexture2D(cameraOutput, true);
        backgroundOutputTex = ConvertRenderTextureToTexture2D(backgroundOutput, true);

        for(int x = 0; x < cameraOutputTex.width; x++)
        {
            for(int y = 0; y < cameraOutputTex.height; y++)
            {
                if(maskArray[x, y])
                {
                    Color backgroundPixel = backgroundOutputTex.GetPixel(x, y);
                    cameraOutputTex.SetPixel(x, y, backgroundPixel);
                    planeOutputTex.SetPixel(x, y, backgroundPixel);
                }
            }
        }
        cameraOutputTex.Apply();
        planeOutputTex.Apply();

        if (planeOutput.TryGetComponent(out Renderer renderer))
        {
            renderer.sharedMaterial.mainTexture = planeOutputTex;
        }

        if (saveImage || saveVideo)
        {
            saveImage = false;
            SaveImage(cameraOutputTex);
        }
    }

    public Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture, bool linear)
    {
        // Create a new Texture2D with the same dimensions as the RenderTexture
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false, linear);

        RenderTexture.active = renderTexture;

        // Read the pixels from the RenderTexture into the Texture2D
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);

        // Apply the changes to the Texture2D
        texture.Apply();

        // Return the Texture2D
        return texture;
    }

    void SaveImage(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        var dirPath = Application.dataPath + "/../SaveImages/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        string imageName = "Image" + imageNumber;
        File.WriteAllBytes(dirPath + imageName + ".png", bytes);
        imageNumber++;
    }
}
