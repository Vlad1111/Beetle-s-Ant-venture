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
    public bool destroy = false;
    public Terrain ground;
    public float seeLevel;
    public TeraindDetails[] detailsLayers;
    public Transform watersParent;
    public Transform waterPrefab;
    public Transform plantsParent;
    public Transform[] plantsPrefabs;

    private float[,] heights;
    private float[,,] textures;

    private void DistroyAllChildrentForParent(Transform parent)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            while (parent.childCount > 0)
                DestroyImmediate(parent.GetChild(0).gameObject);
        else
            foreach (Transform chil in parent) Destroy(chil.gameObject);
    }
    private void RemoveAlreadyGeneratedElements()
    {
        DistroyAllChildrentForParent(watersParent);
        DistroyAllChildrentForParent(plantsParent);
    }
    private void Generate()
    {
        RemoveAlreadyGeneratedElements();
        heights = new float[ground.terrainData.heightmapResolution, ground.terrainData.heightmapResolution];
        textures = new float[ground.terrainData.alphamapResolution, ground.terrainData.alphamapResolution, ground.terrainData.terrainLayers.Length];
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
                heights[i,j] = val;
            }
        for (int i = 0; i < size.x && i < textures.GetLength(0); i++)
            for (int j = 0; j < size.y && j < textures.GetLength(1); j++)
            {
                var val = heights[i,j] - seeLevel;
                val *= 100;
                if (val > 1)
                    val = 1;
                else if (val < 0)
                    val = 0;
                textures[i, j, 0] = val;
                textures[i, j, 1] = 1 - val;
            }

        ground.terrainData.SetHeights(0, 0, heights);
        ground.terrainData.SetAlphamaps(0, 0, textures);

        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
            {
                var newW = Instantiate(waterPrefab, watersParent);
                newW.localPosition = new Vector3(i * 100, 0, j * 100);
            }

        //var scale = 500f / ground.terrainData.heightmapResolution;
        //for (int i = 0; i < size.x; i+=6)
        //    for (int j = 0; j < size.y; j+=6)
        //    {
        //        int inx = Random.Range(0, plantsPrefabs.Length - 1);
        //        var trans = Instantiate(plantsPrefabs[inx], plantsParent);
        //
        //        trans.localPosition = new Vector3(i * scale, heights[j, i] * ground.terrainData.heightmapScale.y, j * scale);
        //    }
    }

    void Update()
    {
        if(generate)
        {
            Generate();
            generate = false;
        }
        if (destroy)
        {
            RemoveAlreadyGeneratedElements();
            destroy = false;
        }
    }
}
