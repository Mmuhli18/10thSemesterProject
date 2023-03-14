using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class loadFSpy : MonoBehaviour
{
    Camera _camera = Camera.main;
    int _toggleAxis = 0;
    int _rotateX = 0;

    public class fSpyPoint
    {
        public float x;
        public float y;
    }

    public class fSpyMatrix
    {
        public float[,] rows;
    }

    public class fSpyData
    {
        public fSpyPoint principalPoint;
        public fSpyMatrix viewTransform;
        public fSpyMatrix cameraTransform;
        public float horizontalFieldOfView;
        public float verticalFieldOfView;
        public fSpyPoint[] vanishingPoints;
        public string[] vanishingPointAxes;
        public float relativeFocalLength;
        public int imageWidth;
        public int imageHeight;
    }

    public void LoadfSpyFile(string path)
    {
        string _log = "";

        if (string.IsNullOrEmpty(path)) return;
        _log = path;
        File.OpenRead(_log);

        char[] invailPathChars = Path.GetInvalidFileNameChars();
        foreach (char b in invailPathChars)
        {
            
            int Index = path.IndexOf(b);
            if (Index != -1) 
            {
                UnityEngine.Debug.Log("Invalid charater in the path");
                UnityEngine.Debug.Log(b);
                UnityEngine.Debug.Log(Index);
            }
        }

        char[] invaildFileNameChars = Path.GetInvalidFileNameChars();
        string[] pathSplit = path.Split("/");
        UnityEngine.Debug.Log(pathSplit[pathSplit.Length - 1]);
        
        foreach (char a in invaildFileNameChars) 
        {

            string word = pathSplit[pathSplit.Length - 1];
            int Index = word.IndexOf(a);
            
            if (Index != -1)
            {
                UnityEngine.Debug.Log("Invalid charater in the name");
                UnityEngine.Debug.Log(a);
                UnityEngine.Debug.Log(Index);
            }
        }


        if (File.Exists(_log))
        {
            string text = File.ReadAllText(path);
            fSpyData fspy = JsonConvert.DeserializeObject<fSpyData>(text);

            _log += "vanishingPointAxes: ";
            foreach (string ax in fspy.vanishingPointAxes) _log += "[" + ax + "]";
            _log += "\n";

            float[,] m = fspy.cameraTransform.rows;
            Matrix4x4 m4 = new Matrix4x4();
            for (int i = 0; i < 4; i++) m4.SetRow(i, new Vector4(m[i, 0], m[i, 1], m[i, 2], m[i, 3]));

            Vector3 position = new Vector3(m4[0, 3], m4[1, 3], m4[2, 3]);
            Quaternion rotation = Quaternion.Inverse(m4.rotation);

            if (_toggleAxis > 0)
            {
                switch (_toggleAxis)
                {
                    case 1:
                        (position.y, position.z) = (position.z, position.y);
                        (rotation.y, rotation.z) = (rotation.z, rotation.y);
                        rotation *= Quaternion.AngleAxis(90, Vector3.right);
                        break;
                    case 2:
                        position.x *= -1;
                        rotation.x *= -1;
                        break;
                    case 3:
                        position.y *= -1;
                        rotation.y *= -1;
                        break;
                    case 4:
                        position.z *= -1;
                        rotation.z *= -1;
                        break;
                }
                switch (_rotateX)
                {
                    case 1:
                        rotation *= Quaternion.AngleAxis(90, Vector3.right);
                        break;
                    case 2:
                        rotation *= Quaternion.AngleAxis(180, Vector3.right);
                        break;
                }
            }
            else if (fspy.vanishingPointAxes[0] == "xNegative" && fspy.vanishingPointAxes[1] == "zPositive")
            {
                position.y *= -1;
                rotation.y *= -1;
                rotation *= Quaternion.AngleAxis(180, Vector3.right);
            }
            else if (fspy.vanishingPointAxes[0] == "xNegative" && fspy.vanishingPointAxes[1] == "zNegative")
            {
                position.z *= -1;
                rotation.z *= -1;
            }
            else if (fspy.vanishingPointAxes[0] == "xNegative" && fspy.vanishingPointAxes[1] == "yPositive")
            {
                position.y *= -1;
                rotation.y *= -1;
                rotation *= Quaternion.AngleAxis(180, Vector3.right);
            }
            else if (fspy.vanishingPointAxes[0] == "xNegative" && fspy.vanishingPointAxes[1] == "yNegative")
            {
                position.z *= -1;
                rotation.z *= -1;
            }
            else if (fspy.vanishingPointAxes[0] == "xPositive" && fspy.vanishingPointAxes[1] == "zPositive")
            {
                position.y *= -1;
                rotation.y *= -1;
                rotation *= Quaternion.AngleAxis(180, Vector3.right);
            }
            else if (fspy.vanishingPointAxes[0] == "xPositive" && fspy.vanishingPointAxes[1] == "zNegative")
            {
                position.z *= -1;
                rotation.z *= -1;
            }
            else if (fspy.vanishingPointAxes[0] == "xPositive" && fspy.vanishingPointAxes[1] == "yPositive")
            {
                position.y *= -1;
                rotation.y *= -1;
                rotation *= Quaternion.AngleAxis(180, Vector3.right);
            }
            else if (fspy.vanishingPointAxes[0] == "xPositive" && fspy.vanishingPointAxes[1] == "yNegative")
            {
                position.z *= -1;
                rotation.z *= -1;
            }
            else
            {
                _log += "\nError:\n";
                _log += "Unsupported Vanishing Point Axes.\n";
                return;
            }
            _camera.transform.SetPositionAndRotation(position, rotation);
            _camera.fieldOfView = fspy.verticalFieldOfView * Mathf.Rad2Deg;

            _log += "fieldOfView:" + _camera.fieldOfView + "\n";
            _log += "Quaternion:" + m4.rotation.ToString("F3") + "\n";
            _log += "Position:" + position.ToString("F3") + "\n";
        }
        else 
        {
            UnityEngine.Debug.Log("File does not exits or could not be found" + " " + _log);
        }
    }

    public void setCamera(Camera newCamera) 
    {
        _camera = newCamera;
    }
}
