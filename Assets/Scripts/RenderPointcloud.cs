using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

public class RenderPointcloud : MonoBehaviour
{
    private Material mat;
    private Vector3[] coordinates;
    private int[] labels;
    private List<int> unique_labels;
    private Color32[] colors;
    private int[] indices;

    public string path_to_pointcloud;
    public int points_to_render = -1;
    public float point_size = 1.0f;

    void ParsePointcloud()
    {
        if(points_to_render == -1)
        {
            points_to_render = File.ReadAllLines(path_to_pointcloud).Length;
        }

        // Initialize arrays
        coordinates = new Vector3[points_to_render];
        colors = new Color32[points_to_render];
        indices = new int[points_to_render];
        labels = new int[points_to_render];
        unique_labels = new List<int>();

        StreamReader reader = new StreamReader(path_to_pointcloud);

        // Reads coorindates line by line and populates coordinates/color array
        for(int i = 0; i < points_to_render; i++) 
        {
            string line = reader.ReadLine();
            string[] coords = line.Split(new[] {',', ' '});
            

            Vector3 point = new Vector3();
            for(int j = 0; j < 3; j++)
            {
                point[0] = float.Parse(coords[0], CultureInfo.InvariantCulture);
                point[1] = float.Parse(coords[2], CultureInfo.InvariantCulture);
                point[2] = float.Parse(coords[1], CultureInfo.InvariantCulture);
            }

            // Points have been given labels
            if(coords.Length == 4)
            {
                int nr = (int)float.Parse(coords[3], CultureInfo.InvariantCulture);
                labels[i] = nr;
                
                if(!unique_labels.Contains(nr))
                {
                    unique_labels.Add(nr);
                }
            }

            Color32 color = Color32.Lerp(Color.green, Color.red, point[1] / 20);

            // Points have been given RGB values
            if(coords.Length == 6)
            {
                color = new Color32((byte)float.Parse(coords[3], CultureInfo.InvariantCulture),
                (byte)float.Parse(coords[4], CultureInfo.InvariantCulture),
                (byte)float.Parse(coords[5], CultureInfo.InvariantCulture), 1);
            }

            // Set the point color
            coordinates[i] = point;
            colors[i] = color;
            indices[i] = i;
        }
    }


    void TranslatePointcoud()
    {
        float min_x = 9999999f;
        float max_x = -1f;
        float min_y = 9999999f;
        float max_y = -1f;

        // Find min/max values
        for(int i = 0; i < points_to_render; i++) 
        {
            Vector3 point = coordinates[i];
            if(point[0] < min_x)
                min_x = point[0];
            else if(point[0] > max_x)
                max_x = point[0];
            if(point[2] < min_y)
                min_y = point[2];
            else if(point[2] > max_y)
                max_y = point[2];
        }

        // Calculate new values
        for(int i = 0; i < points_to_render; i++) 
        {
            Vector3 point = coordinates[i];
            point[0] = point[0] - max_x;
            point[2] = point[2] - max_y;
            coordinates[i] = point;
        }
    }


    void ChangeLabelColor(int label, Color32 color)
    {
        for(int i = 0; i < points_to_render; i++)
        {
            if(labels[i] == label)
            {
                colors[i] = color;
            }
        }
    }


    void Start()
    {
        ParsePointcloud();
        TranslatePointcoud();
        
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = coordinates;
        mesh.colors32 = colors;
        mesh.SetIndices(indices, MeshTopology.Points, 0);
        
        
        for(int i = 0; i < unique_labels.Count; i++)
        {
            Color32 temp_color = new Color32(
                (byte)Random.Range(0, 255), 
                (byte)Random.Range(0, 255), 
                (byte)Random.Range(0, 255),
                (byte)255
            );
            ChangeLabelColor(unique_labels[i], temp_color);
        }

        // 0 - trees, 1 - buildings
        // ChangeLabelColor(0, new Color32(36, 153, 40, 255));
        // ChangeLabelColor(1, new Color32(111, 118, 112, 255));
        mesh.colors32 = colors;

        mat = GetComponent<Renderer>().sharedMaterial;
    }


    void Update()
    {
        mat.SetFloat("_PointSize", point_size);
    }
}
