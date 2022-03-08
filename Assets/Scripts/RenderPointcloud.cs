using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

public class RenderPointcloud : MonoBehaviour
{
    private Material mat;
    private Vector3[] coordinates;
    private Color32[] colors;
    private int[] indices;

    public string path_to_pointcloud = "Assets/Pointclouds/points_normalized.txt";
    public int points_to_render = 10000;
    public float point_size = 1.0f;

    void ParsePointcloud(int max_points)
    {
        // Initialize arrays
        coordinates = new Vector3[max_points];
        colors = new Color32[max_points];
        indices = new int[max_points];

        StreamReader reader = new StreamReader(path_to_pointcloud);

        // Reads coorindates line by line and populates coordinates/color array
        for(int i = 0; i < max_points; i++) 
        {
            string line = reader.ReadLine();
            string[] coords = line.Split(new char[0]);
            

            // Creates a Vector3 point representation and converts values to float
            Vector3 point = new Vector3();
            foreach(string c in coords)
            {
                point[0] = float.Parse(coords[0], CultureInfo.InvariantCulture);
                point[1] = float.Parse(coords[2], CultureInfo.InvariantCulture);
                point[2] = float.Parse(coords[1], CultureInfo.InvariantCulture);
            }

            // Set the point color
            Color32 color = Color32.Lerp(Color.green, Color.red, point[1] / 20);

            coordinates[i] = point;
            colors[i] = color;
            indices[i] = i;
        }
    }


    void Start () {
        ParsePointcloud(points_to_render);

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = coordinates;
        mesh.colors32 = colors;
        mesh.SetIndices(indices, MeshTopology.Points, 0);

        mat = GetComponent<Renderer>().sharedMaterial;
    }


    void Update() {
        mat.SetFloat("_PointSize", point_size);
    }
}
