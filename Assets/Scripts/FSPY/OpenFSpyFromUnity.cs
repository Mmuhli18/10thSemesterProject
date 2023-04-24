using System.Diagnostics;
using System;
using System.IO;
using UnityEngine;
using AnotherFileBrowser.Windows;


public class OpenFSpyFromUnity : MonoBehaviour
{
    LoadFSpy loader;
    Saver_Loader filePathSaver;
    DataToBeSaved pathToFspyExe;
    DataToBeSaved pathToFSpySavedFiles;
    BrowserProperties bp;
    string FSpy_Exe = "FSpy_Exe";
    string FSpy_Json = "FSpy_Json";
    string type_Json = ".json";

    public void Start()
    {
        FindFSpySavedFiles();
    }
    /*
     * Opens the fileexplore if fSpy has not been open before, else it just opens FSpy.
     * If FSpy has not been open before, then it saves the path to the FSpy.exe. 
     * If the fpsy.exe is not in the folder it opens the file explore.
     */
    public void OpenFSpy() 
    {
        // Instiates the .json loader/saver for the paths 
        filePathSaver = new Saver_Loader();
        // Instatiates the class that stores the paths.
        pathToFspyExe = new DataToBeSaved();
        // Instatiates the broweser properties for the file explore, set to default  
        bp = new BrowserProperties();
        // Checks if the application does not have access, to json file that contains the path to the fspy.exe.
       
        if (!File.Exists(Application.persistentDataPath + "/" + FSpy_Exe + type_Json))
        {
            // if it does not, then it opens the file explore, with the title "Please find: fspy exe file" and then opens the program, and then saves the path to the fpsy.exe 
            bp.title = "Please find: fspy exe file";
            new FileBrowser().OpenFileBrowser(bp, path =>
            {
                if (File.Exists(path))
                {
                    // opens the program 
                    OpenProgram(path);

                    //Saves the path to Spy.exe*
                    saveNewPath(path, FSpy_Exe);

                }
                else 
                {
                    return;
                }
            });
            bp.title = null;
        }
        else
        {
            // If the json file with the path to the fpsy.exe exits, then it it loads the path, and then opens the program at this path. 
            
            //Loads the path to FSpy.exe
            pathToFspyExe = filePathSaver.loadJson(FSpy_Exe);
            //Tries to open FSpy*
            if (Directory.Exists(Path.GetDirectoryName(pathToFspyExe.getPath()))) 
            { 
                OpenProgram(pathToFspyExe.getPath());
            }
            else 
            {
                bp.title = "Please find: FSpy exe file";
                new FileBrowser().OpenFileBrowser(bp, path =>
                {
                    try
                    {
                        //Tries to open FSpy* 
                        OpenProgram(path);
                        //Saves the path to Spy.exe*
                        saveNewPath(path, FSpy_Exe);

                    }
                    catch (Exception e)
                    {
                        // Could be a idea to add a dialog box, saying that we where unable to to start FSpy for some reason.
                        UnityEngine.Debug.Log(e);
                    }
                });

                bp.title = null;
            }
            
        }
    }

    /*
     * Opens any program which path is provided, and is allowed by windows. 
     * It then pauses our program, and waits for the users to finsish using the FSpy (Our program countiniues when the user cloeses FSpy)
     */

    public void OpenProgram(string path) 
    {
        if (File.Exists(path))
        {
            try
            {
                ProcessStartInfo openFspyStartInfo = new ProcessStartInfo();
                openFspyStartInfo.FileName = path;
                openFspyStartInfo.UseShellExecute = true;

                Process openFSpyProcess = new Process();
                openFSpyProcess.StartInfo = openFspyStartInfo;
                openFSpyProcess.Start();
                openFSpyProcess.WaitForExit();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Something went wrong:" + e);

            }
        }
        else
        {
            UnityEngine.Debug.Log("File not found or it does not exists");
            return;
        }
    }

    /*
     * Opens the file explore so that the users can import FSpy filies into our program. 
     * If the does not exits an .json file contianin the path to the saved files, it creates one and saves the path to the diretory. 
     * Otherwise it tries, opens the file explore at the directory where the filies where the first time it was opened.
     * It then loads the information from the FSpy .json files. 
     */

    public void FindFSpySavedFiles() 
    {
        if (bp == null) 
        {
            bp = new BrowserProperties();
        }
        if (filePathSaver == null) 
        {
            filePathSaver = new Saver_Loader();
        }
        loader = new LoadFSpy();
        pathToFSpySavedFiles = new DataToBeSaved();

        if (!File.Exists(Application.persistentDataPath + "/" + FSpy_Json + type_Json))
        {
            try
            {
                bp.title = "Please find: FSpy.json files ";
                new FileBrowser().OpenFileBrowser(bp, path =>
                {
                    loader.LoadfSpyFile(path);
                    saveNewPath(path, FSpy_Json);
 
                });
                bp.title = null; 
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Something went wrong when trying to save the path to the saved files: " + e);
            }
        }
        else
        {

            pathToFSpySavedFiles = filePathSaver.loadJson(FSpy_Json);
            bp.title = "Please find: FSpy.json file";
            if (Directory.Exists(pathToFSpySavedFiles.getPath())) 
            {
                bp.restoreDirectory = false;
                bp.initialDir = pathToFSpySavedFiles.getPath();
                
                new FileBrowser().OpenFileBrowser(bp, path =>
                {
                    loader.LoadfSpyFile(path);
                });
            } 
            else 
            {
                bp.restoreDirectory = true;
                new FileBrowser().OpenFileBrowser(bp, path =>
                {
                    loader.LoadfSpyFile(path);
                });
            }
            bp.title = null;
        }
    }

    private void saveNewPath(string path, string nameOfFile) 
    {
        if (nameOfFile == "FSpy_Exe")
        {
            pathToFspyExe.setPath(path);
            filePathSaver.saveAsJson(pathToFspyExe, nameOfFile); 
        }
        else if (nameOfFile == "FSpy_Json") 
        {
            pathToFSpySavedFiles.setPath(Path.GetDirectoryName(path));
            filePathSaver.saveAsJson(pathToFspyExe, nameOfFile);
        }
        else 
        {
            UnityEngine.Debug.Log("File name unkown");
            return;
        }

    }

}

