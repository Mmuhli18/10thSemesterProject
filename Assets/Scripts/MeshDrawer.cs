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
        for(int i = 0; i < faces.Count; i++)
        {
            vertices.AddRange(faces[i].vertices);
            triangles.AddRange(faces[i].triangles);
            uvs.AddRange(faces[i].uvs);
        }
        return new MeshFace(vertices, triangles, uvs);
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
