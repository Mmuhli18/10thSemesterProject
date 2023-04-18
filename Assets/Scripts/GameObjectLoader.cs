using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnotherFileBrowser.Windows;
using System.Diagnostics;
using UnityEngine.Networking;
using System.IO;
using Siccity.GLTFUtility;
using Autodesk.Fbx;
public class GameObjectLoader: MonoBehaviour
{
    public void Load(string filePath) 
    {
        GameObject obj = Importer.LoadFromFile(filePath);
        if (obj != null) 
        {
            UnityEngine.Debug.Log("Boys and Ladies we have take off...");
        }
        //UnityWebRequest UVR 
        /*
        GameObject obj = (GameObject)Resources.Load(nameOfObject);
        if (obj != null)
        {
            Instantiate(obj);
        }*/
        

    }

    public void OpenFolder() 
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = Application.dataPath + "/"+"Resources";
        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();
    }

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.V)) 
        {
            //OpenFolder();
            //Load("_Road_");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {

            Camera mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            BrowserProperties bp = new BrowserProperties();
            new FileBrowser().OpenFileBrowser(bp,path=> 
            {
                
                Load(path);
            });
            //Load("ActualRoad");
            mainCamera.enabled = false;
            //Camera cam1 = GameObject.Find("ActualTest2.fspy").GetComponent<Camera>();
            //cam1.enabled = true;
        }
    }
}
