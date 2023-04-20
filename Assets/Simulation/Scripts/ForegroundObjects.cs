using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ForegroundObjects : MonoBehaviour
{
    public GameObject planeOutput;
    public RenderTexture cameraOutput;
    public RenderTexture backgroundOutput;
    public Texture2D mask;
    public Texture2D background;

    byte[,] maskArray;

    Texture2D cameraOutputTex;
    Texture2D backgroundOutputTex;

    private void Start()
    {
        TryLoadSettingsFromMenu();

        maskArray = new byte[mask.width, mask.height];
        for (int x = 0; x < mask.width; x++)
        {
            for (int y = 0; y < mask.height; y++)
            {
                if (mask.GetPixel(x, y).r > 0.1f)
                {
                    maskArray[x, y] = 1;
                }
                else
                {
                    maskArray[x, y] = 0;
                }
            }
        }
    }
    void TryLoadSettingsFromMenu()
    {
        MenuSettingsForSimulation settings = FindObjectOfType<MenuSettingsForSimulation>();
        if (settings == null) { return; }
        mask = settings.mask;
    }

        // Update is called once per frame
        void Update()
    {
        StartCoroutine(UpdateEndOfFrame());
    }

    IEnumerator UpdateEndOfFrame()
    {
        yield return new WaitForEndOfFrame();

        Destroy(cameraOutputTex);
        Destroy(backgroundOutputTex);
        cameraOutputTex = ConvertRenderTextureToTexture2D(cameraOutput);
        backgroundOutputTex = ConvertRenderTextureToTexture2D(backgroundOutput);

        Color[] pixels = cameraOutputTex.GetPixels();
        Color[] backgroundPixels = backgroundOutputTex.GetPixels();
        //var pixels = cameraOutputTex.GetRawTextureData<byte3>();
        //var backgroundPixels = backgroundOutputTex.GetRawTextureData<byte3>();
        //Debug.Log(pixels.Length);

        int width = cameraOutputTex.width;
        int height = cameraOutputTex.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                byte maskColor = maskArray[x, y];
                if (maskColor == 1)
                {
                    pixels[y * width + x] = backgroundPixels[y * width + x];
                    //cameraOutputTex.SetPixel(x, y, backgroundOutputTex.GetPixel(x, y));
                }
            }
        }
        cameraOutputTex.SetPixels(pixels);
        //cameraOutputTex.SetPixelData(pixels, 0);
        cameraOutputTex.Apply();

        if (planeOutput.TryGetComponent(out Renderer renderer))
        {
            renderer.sharedMaterial.mainTexture = cameraOutputTex;
        }
    }

    public Texture2D GetOutputFrame()
    {
        return cameraOutputTex;
    }

    public Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture)
    {
        // Create a new Texture2D with the same dimensions as the RenderTexture
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false, true);

        RenderTexture.active = renderTexture;

        // Read the pixels from the RenderTexture into the Texture2D
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);

        // Apply the changes to the Texture2D
        texture.Apply();

        // Return the Texture2D
        return texture;
    }
}
