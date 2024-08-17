using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour
{
    [System.Serializable]
    public class TeraindDetails
    {
        public Vector2 offset;
        public float scale;
        public float height;
    }

    void Start()
    {
        Generate();
    }
    public bool generate = false;
    public Terrain ground;
    public TeraindDetails[] detailsLayers;
    public Transform watersParent;
    public Transform waterPrefab;

    private void RemoveAlreadyGeneratedElements()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            while (watersParent.childCount > 0)
                DestroyImmediate(watersParent.GetChild(0).gameObject);
        else
            foreach (Transform chil in watersParent) Destroy(chil.gameObject);
    }
    private void Generate()
    {
        RemoveAlreadyGeneratedElements();
        float[,] heights = new float[ground.terrainData.heightmapResolution, ground.terrainData.heightmapResolution];
        Vector2Int size = new Vector2Int(heights.GetLength(0), heights.GetLength(1));
        foreach(var layer in detailsLayers)
            layer.offset = new Vector2(Random.value * 100, Random.value * 100);
        Debug.Log("map size: " + size);
        for (int i = 0; i < size.x; i++)
            for (int j = 0; j < size.y; j++)
            {
                float x = (float)i / size.x;
                float y = (float)j / size.y;
                float val = 0;
                foreach (var layer in detailsLayers)
                {
                    float xx = x * layer.scale;
                    float yy = y * layer.scale;
                    if(i == 10 && j == 10)
                    {
                        Debug.Log(xx + " " + yy);
                    }
                    xx += layer.offset.x;
                    yy += layer.offset.y;
                    val += Mathf.PerlinNoise(xx, yy) * layer.height;
                }
                //Debug.Log(val);
                heights[i,j] = val;
            }

        ground.terrainData.SetHeights(0, 0, heights);

        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
            {
                var newW = Instantiate(waterPrefab, watersParent);
                newW.localPosition = new Vector3(i * 100, 0, j * 100);
            }
    }

    void Update()
    {
        if(generate)
        {
            Generate();
            generate = false;
        }
    }
}
