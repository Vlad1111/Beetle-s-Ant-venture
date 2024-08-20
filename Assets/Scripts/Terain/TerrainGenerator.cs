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

    [System.Serializable]
    public class PlantPrefabs
    {
        public Transform prefab;
        public int probability;
        public Vector3 minScale;
        public Vector3 maxScale;
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
    public PlantPrefabs[] plantsPrefabs;
    public Transform sticksParent;
    public Transform stickPrefab;

    private float[,] heights;
    private float[,] safeTerrain;
    private float[,] unsafeTerrain;
    private float[,,] textures;

    private float[,] premadeMask;
    private float[,] premadeHeights;
    private float[,,] premadeTextures;

    void Start()
    {
        Generate();
    }

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
        DistroyAllChildrentForParent(sticksParent);
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
    private void GenerateUnSafeZones(Vector2Int size)
    {
        for (int i = 0; i < size.x; i++)
            for (int j = 0; j < size.y; j++)
            {
                float x = (float)i / size.x;
                float y = (float)j / size.y;
                float val = 0;
                foreach (var layer in roadLayers)
                {
                    layer.offset = new Vector2(Random.value * 100, Random.value * 100);
                    float xx = x * layer.scale;
                    float yy = y * layer.scale;
                    xx += layer.offset.x;
                    yy += layer.offset.y;
                    var val2 = Mathf.PerlinNoise(xx, yy);
                    val2 = 1 - Mathf.Pow(Mathf.Abs(val2 - 0.5f) * layer.height, layer.power);
                    val = Mathf.Max(val, val2);
                }
                unsafeTerrain[i, j] = val * seeLevel * 1.2f;
            }
    }

    public void Generate(bool generateAll = true)
    {
        Debug.Log("Generate terrain");
        RemoveAlreadyGeneratedElements();
        heights = new float[ground.terrainData.heightmapResolution, ground.terrainData.heightmapResolution];
        safeTerrain = new float[ground.terrainData.heightmapResolution, ground.terrainData.heightmapResolution];
        unsafeTerrain = new float[ground.terrainData.heightmapResolution, ground.terrainData.heightmapResolution];
        textures = new float[ground.terrainData.alphamapResolution, ground.terrainData.alphamapResolution, ground.terrainData.terrainLayers.Length];

        premadeMask = TerrainEditor.LoadMask(ground);
        premadeHeights = TerrainEditor.LoadHeights(ground);
        premadeTextures = TerrainEditor.LoadTextures(ground);

        Vector2Int size = new Vector2Int(heights.GetLength(0), heights.GetLength(1));

        foreach (var layer in detailsLayers)
            layer.offset = new Vector2(Random.value * 100, Random.value * 100);
        foreach (var layer in safeZoneLayers)
            layer.offset = new Vector2(Random.value * 100, Random.value * 100);
        foreach (var layer in roadLayers)
            layer.offset = new Vector2(Random.value * 100, Random.value * 100);

        GenerateSafeZones(size);
        GenerateUnSafeZones(size);
        for (int i = 0; i < size.x; i++)
            for (int j = 0; j < size.y; j++)
            {
                var mask = 1f;
                if (i < premadeMask.GetLength(0) && j < premadeMask.GetLength(0))
                    mask = premadeMask[i, j];
                float x = (float)i / size.x;
                float y = (float)j / size.y;
                float val = 0;
                foreach (var layer in detailsLayers)
                {
                    float xx = x * layer.scale;
                    float yy = y * layer.scale;
                    xx += layer.offset.x;
                    yy += layer.offset.y;
                    val += Mathf.PerlinNoise(xx, yy) * layer.height;
                }
                val = Mathf.Max(val, safeTerrain[i, j]);
                heights[i, j] = Mathf.Lerp(val, premadeHeights[i, j], mask);
                //heights[i, j] = Mathf.Max(val, safeTerrain[i, j]);
            }
        int calcDistance = 10;
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

                float isPeack = 0;
                if (i >= calcDistance && i < textures.GetLength(0) - calcDistance)
                    if (j >= calcDistance && j < textures.GetLength(1) - calcDistance)
                    {
                        var d = heights[i, j] * 4 -
                            heights[i - calcDistance, j] -
                            heights[i + calcDistance, j] -
                            heights[i, j - calcDistance] -
                            heights[i, j + calcDistance];
                        isPeack = 100 * d * heights[i, j];
                        if (isPeack > 1)
                            isPeack = 1;
                    }

                textures[i, j, 0] = (1 - isPeack) * (1 - safe) * val;
                textures[i, j, 1] = (1 - isPeack) * (1 - safe) * (1 - val);
                textures[i, j, 2] = (1 - isPeack) * safe;
                textures[i, j, 3] = isPeack;
            }

        ground.terrainData.SetHeights(0, 0, heights);
        ground.terrainData.SetAlphamaps(0, 0, textures);

        if (generateAll)
        {
            GenerateWater();

            var scale = 500f / ground.terrainData.heightmapResolution;
            if (plantsParent != null)
            {
                int onceEvery = 3;

                int propProbability = 0;
                foreach (var pref in plantsPrefabs)
                    propProbability += pref.probability;
                for (int i = 0; i < size.x - 1; i += 6)
                    for (int j = 0; j < size.y - 1; j += 6)
                    {
                        int ii = i + (int)Random.Range(-onceEvery, onceEvery);
                        int jj = j + (int)Random.Range(-onceEvery, onceEvery);
                        if (ii < 0 || ii >= size.x)
                            continue;
                        if (jj < 0 || jj >= size.y)
                            continue;
                        var posibility = Mathf.Abs(safeTerrain[jj, ii]) + Mathf.Abs(unsafeTerrain[jj, ii]) + premadeMask[j, i] * seeLevel * 2;
                        if (posibility > seeLevel / 2)
                        {
                            continue;
                        }
                        else
                        {
                            int randVal = Random.Range(0, propProbability);
                            int inx = -1;
                            for (int k = 0; k < plantsPrefabs.Length; k++)
                            {
                                randVal -= plantsPrefabs[k].probability;
                                if (randVal < 0)
                                {
                                    inx = k;
                                    break;
                                }
                            }
                            if (inx == -1)
                                inx = plantsPrefabs.Length - 1;
                            var plant = plantsPrefabs[inx];
                            int count = 1;
                            if (inx == 0)
                                count = 3;
                            for (int _ = 0; _ < count; _++)
                            {
                                var trans = Instantiate(plant.prefab, plantsParent);

                                trans.localPosition = new Vector3(ii * scale, heights[jj, ii] * ground.terrainData.heightmapScale.y, jj * scale);
                                trans.localPosition += new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f);
                                trans.localEulerAngles = new Vector3(Random.Range(-5, 5), Random.Range(0, 360), Random.Range(-5, 5));
                                trans.localScale = new Vector3(
                                                        trans.localScale.x * Random.Range(plant.minScale.x, plant.maxScale.x),
                                                        trans.localScale.y * Random.Range(plant.minScale.y, plant.maxScale.y),
                                                        trans.localScale.z * Random.Range(plant.minScale.z, plant.maxScale.z));
                            }
                        }
                    }
            }

            for (int _ = 0; _ < 4; _++)
            {
                int i = Random.Range(0, size.x - 1);
                int j = Random.Range(0, size.y - 1);
                int count = 50;
                do
                {
                    var posibility = Mathf.Abs(safeTerrain[j, i]) + Mathf.Abs(unsafeTerrain[j, i]) + premadeMask[j, i] * seeLevel * 2;
                    if (posibility > seeLevel / 2)
                    {
                        count--;
                        if (safeTerrain[j, i] > premadeMask[j, i])
                            break;
                    }
                    i = Random.Range(20, size.x - 21);
                    j = Random.Range(20, size.y - 21);
                    if (count == 0)
                        break;
                } while (true);


                var trans = Instantiate(stickPrefab, sticksParent);
                trans.localPosition = new Vector3(i * scale, heights[j, i] * ground.terrainData.heightmapScale.y + 10, j * scale);
                trans.gameObject.name = "Stick";
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
