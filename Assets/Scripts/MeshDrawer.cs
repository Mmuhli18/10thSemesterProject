using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MeshFace
{
    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Vector2> uvs;
    public MeshFace(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
    }

    public override string ToString()
    {
        string returnString = "Vertice length of " + vertices.Count + "\n";
        foreach (var vert in vertices)
        {
            returnString += vert.ToString() + "; ";
        }
        return returnString;
    }
}
public static class MeshDrawer
{

    public static MeshFace drawFace(List<Vector3> points, bool reverse = false)
    {
        List<Vector3> vertices = points;
        List<int> triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
        List<Vector2> uvs = new List<Vector2>() { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1f), new Vector2(0, 1f) };
        if (reverse)
        {
            vertices.Reverse();
        }
        return new MeshFace(vertices, triangles, uvs);
    }

    public static MeshFace drawScaledFace(List<Vector3> points, bool reverse = false)
    {
        List<Vector3> vertices = points;
        List<int> triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
        if (reverse)
        {
            vertices.Reverse();
        }
        List<Vector2> uvs = new List<Vector2>() { new Vector2(points[0].x, points[0].y), new Vector2(points[1].x, points[1].y), new Vector2(points[2].x, points[2].y), new Vector2(points[3].x, points[3].y) };
        return new MeshFace(vertices, triangles, uvs);
    }

    public static MeshFace combineFaces(List<MeshFace> faces)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        int vertCount = 0;
        for(int i = 0; i < faces.Count; i++)
        {
            vertices.AddRange(faces[i].vertices);
            for(int j = 0; j < faces[i].triangles.Count; j++)
            {
                triangles.Add(faces[i].triangles[j] + vertCount);
            }
            uvs.AddRange(faces[i].uvs);
            vertCount += faces[i].vertices.Count;
        }
        return new MeshFace(vertices, triangles, uvs);
    }

    public static List<MeshFace> extrudeFace(MeshFace face, float depth, Vector3 direction = default(Vector3))
    {
        if (direction == default(Vector3)) direction = new Vector3(1f, 0f, 0f);
        direction *= depth;
        List<MeshFace> faces = new List<MeshFace>();
        faces.Add(face);
        for (int i = 0; i < face.vertices.Count; i++)
        {
            List<Vector3> points = new List<Vector3>();
            points.Add(face.vertices[i]);

            if (i == face.vertices.Count - 1)
            {
                points.Add(face.vertices[0]);
                points.Add(face.vertices[0] + direction);
            }
            else 
            {
                points.Add(face.vertices[i + 1]);
                points.Add(face.vertices[i + 1] + direction);
            }

            points.Add(face.vertices[i] + direction);
            faces.Add(drawFace(points, true));
            
        }
        List<Vector3> topPoints = new List<Vector3>();
        for(int i = 0; i < face.vertices.Count; i++)
        {
            topPoints.Add(face.vertices[i] + direction);
        }
        faces.Add(drawFace(topPoints, true));

        return faces;
    }

    public static Mesh makeMesh(MeshFace faces)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = faces.vertices.ToArray();
        mesh.triangles = faces.triangles.ToArray();
        mesh.uv = faces.uvs.ToArray();
        return mesh;
    }
}
