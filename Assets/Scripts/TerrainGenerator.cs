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
    public Transform plantsParent;
    public Transform[] plantsPrefabs;

    private float[,] heights;

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

        ground.terrainData.SetHeights(0, 0, heights);

        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
            {
                var newW = Instantiate(waterPrefab, watersParent);
                newW.localPosition = new Vector3(i * 100, 0, j * 100);
            }

        //for (int i = 0; i < size.x; i+=10)
        //    for (int j = 0; j < size.y; j+=10)
        //    {
        //        int inx = Random.Range(0, plantsPrefabs.Length - 1);
        //        var trans = Instantiate(plantsPrefabs[inx], plantsParent);
        //
        //        trans.localPosition = new Vector3(i * 2, heights[j, i] * 600, j * 2);
        //    }
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
