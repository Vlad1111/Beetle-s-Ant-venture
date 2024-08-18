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
        public float power = 1;
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
    public TeraindDetails[] safeZoneLayers;
    public TeraindDetails[] roadLayers;
    public Transform watersParent;
    public Transform waterPrefab;
    public Transform plantsParent;
    public Transform[] plantsPrefabs;

    private float[,] heights;
    private float[,] safeTerrain;
    private float[,] unsafeTerrain;
    private float[,,] textures;

    private void DistroyAllChildrentForParent(Transform parent)
    {
        if (parent == null)
            return;
        if (Application.platform == RuntimePlatform.WindowsEditor)
            while (parent.childCount > 0)
                DestroyImmediate(parent.GetChild(0).gameObject);
        else
            foreach (Transform chil in parent) Destroy(chil.gameObject);
    }
    public void RemoveAlreadyGeneratedElements()
    {
        DistroyAllChildrentForParent(watersParent);
        DistroyAllChildrentForParent(plantsParent);
    }

    public void GenerateWater()
    {
        if (watersParent)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                {
                    var newW = Instantiate(waterPrefab, watersParent);
                    newW.localPosition = new Vector3(i * 100, 0, j * 100);
                }
        }
    }

    private void GenerateSafeZones(Vector2Int size)
    {
        for (int i = 0; i < size.x; i++)
            for (int j = 0; j < size.y; j++)
            {
                float x = (float)i / size.x;
                float y = (float)j / size.y;
                float val = 0;
                foreach (var layer in safeZoneLayers)
                {
                    float xx = x * layer.scale;
                    float yy = y * layer.scale;
                    xx += layer.offset.x;
                    yy += layer.offset.y;
                    val += Mathf.Pow(Mathf.PerlinNoise(xx, yy) * layer.height, layer.power);
                }
                foreach (var layer in roadLayers)
                {
                    float xx = x * layer.scale;
                    float yy = y * layer.scale;
                    xx += layer.offset.x;
                    yy += layer.offset.y;
                    var val2 = Mathf.PerlinNoise(xx, yy);
                    val2 = 1 - Mathf.Pow(Mathf.Abs(val2 - 0.5f) * layer.height, layer.power);
                    val = Mathf.Max(val, val2);
                }
                safeTerrain[i, j] = val * seeLevel * 1.2f;
            }
    }

    public void Generate(bool generateAll = true)
    {
        RemoveAlreadyGeneratedElements();
        heights = new float[ground.terrainData.heightmapResolution, ground.terrainData.heightmapResolution];
        safeTerrain = new float[ground.terrainData.heightmapResolution, ground.terrainData.heightmapResolution];
        unsafeTerrain = new float[ground.terrainData.heightmapResolution, ground.terrainData.heightmapResolution];
        textures = new float[ground.terrainData.alphamapResolution, ground.terrainData.alphamapResolution, ground.terrainData.terrainLayers.Length];
        Vector2Int size = new Vector2Int(heights.GetLength(0), heights.GetLength(1));
        foreach (var layer in detailsLayers)
            layer.offset = new Vector2(Random.value * 100, Random.value * 100);
        foreach (var layer in safeZoneLayers)
            layer.offset = new Vector2(Random.value * 100, Random.value * 100);
        foreach (var layer in roadLayers)
            layer.offset = new Vector2(Random.value * 100, Random.value * 100);
        GenerateSafeZones(size);
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
                    if (i == 10 && j == 10)
                    {
                        Debug.Log(xx + " " + yy);
                    }
                    xx += layer.offset.x;
                    yy += layer.offset.y;
                    val += Mathf.PerlinNoise(xx, yy) * layer.height;
                }
                heights[i, j] = Mathf.Max(val, safeTerrain[i, j]);
            }
        for (int i = 0; i < size.x && i < textures.GetLength(0); i++)
            for (int j = 0; j < size.y && j < textures.GetLength(1); j++)
            {
                var safe = Mathf.Min(1, safeTerrain[i, j] * 0);
                var val = heights[i, j] - seeLevel;
                val *= 100;
                if (val > 1)
                    val = 1;
                else if (val < 0)
                    val = 0;
                textures[i, j, 0] = (1 - safe) * val;
                textures[i, j, 1] = (1 - safe) * (1 - val);
                textures[i, j, 2] = safe;
            }

        ground.terrainData.SetHeights(0, 0, heights);
        ground.terrainData.SetAlphamaps(0, 0, textures);

        if (generateAll)
        {
            GenerateWater();

            if (plantsParent != null)
            {
                var scale = 500f / ground.terrainData.heightmapResolution;
                int onceEvery = 3;
                for (int i = 0; i < size.x; i += 6)
                    for (int j = 0; j < size.y; j += 6)
                    {
                        int ii = i + (int)Random.Range(-onceEvery, onceEvery);
                        int jj = j + (int)Random.Range(-onceEvery, onceEvery);
                        if (ii < 0 || ii >= size.x)
                            continue;
                        if (jj < 0 || jj >= size.y)
                            continue;
                        var posibility = Mathf.Abs(safeTerrain[jj, ii]) + Mathf.Abs(unsafeTerrain[jj, ii]);
                        if (posibility > seeLevel / 2)
                            continue;
                        int inx = Random.Range(0, plantsPrefabs.Length - 1);
                        var trans = Instantiate(plantsPrefabs[inx], plantsParent);

                        trans.localPosition = new Vector3(ii * scale, heights[jj, ii] * ground.terrainData.heightmapScale.y, jj * scale);
                        trans.localPosition += new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f);
                        trans.localEulerAngles = new Vector3(Random.Range(-5, 5), Random.Range(0, 360), Random.Range(-5, 5));
                        trans.localScale = new Vector3(
                                                trans.localScale.x * Random.Range(0.8f, 1.5f),
                                                trans.localScale.y * Random.Range(0.6f, 2),
                                                trans.localScale.z * Random.Range(0.8f, 1.5f));
                    }
            }
        }
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
