using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.IO;
using System.ComponentModel;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;
using AnotherFileBrowser.Windows;
using System.Windows.Forms;



public class openFSpyFromUnity : MonoBehaviour
{
    loadFSpy loader = new loadFSpy();
    // Start is called before the first frame update
    void Start()
    {
        /*
        BrowserProperties bp = new BrowserProperties();
        bp.filterIndex = 0;
        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            openFSpy(path);

        });*/
        openFSpy("C:/Program Files/fSpy/fSpy.exe");
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    public void openFSpy(string path) 
    {

        if (File.Exists(path))
        {

            try
            {
                /*
                string pathToFspy = path;
                string arguments = "C:/Users/Alexander/Desktop/PoorAdmin.png";*/

                UnityEngine.Debug.Log("File exists");
                ProcessStartInfo openFspyStartInfo = new ProcessStartInfo();
                openFspyStartInfo.FileName = path;
                //openFspyStartInfo.WorkingDirectory = "C:/Program Files/fSpy";
                //openFspyStartInfo.FileName = pathToFspy;
                //openFspyStartInfo.Arguments = arguments;
                openFspyStartInfo.RedirectStandardInput = true;
                openFspyStartInfo.CreateNoWindow = true;
                openFspyStartInfo.UseShellExecute = false; 

                //openFspyStartInfo.Arguments = ("C:/Users/Alexander/Desktop/PoorAdmin.png");
                //UnityEngine.Debug.Log(openFspyStartInfo.Arguments = "C:/Users/Alexander/Desktop/PoorAdmin.png");



                Process openFSpyProcess = new Process();
                openFSpyProcess.StartInfo = openFspyStartInfo;
                openFSpyProcess.Start();

                openFSpyProcess.WaitForExit();

                BrowserProperties bp = new BrowserProperties();
                bp.filterIndex = 0;
                new FileBrowser().OpenFileBrowser(bp, path => 
                {
                    loader.LoadfSpyFile(path);

                });
            }
            catch(Exception e) 
            {
                UnityEngine.Debug.Log("Something went wrong:" + "\n" );
                UnityEngine.Debug.Log(e);
            }

            
        }

        else
        {
            UnityEngine.Debug.Log("File not found or it does not exists");
        }
    }



    


}
