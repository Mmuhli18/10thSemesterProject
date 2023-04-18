using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class LoadFSpy
{
    Camera _camera = Camera.main;

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

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        _log = path;

        File.OpenRead(_log);

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
            
            (position.y, position.z) = (position.z, position.y);
            (rotation.y, rotation.z) = (rotation.z, rotation.y);
            rotation *= Quaternion.AngleAxis(90, Vector3.right);
                        
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
