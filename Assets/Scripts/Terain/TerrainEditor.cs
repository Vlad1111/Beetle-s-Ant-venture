using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainEditor : MonoBehaviour
{
    public TerrainGenerator terainToEdit;
    public bool EditTerain = false;
    [Space(20)]
    public bool ShowMask = false;
    public bool ShowData = false;
    [Space(20)]
    public bool SaveMask = false;
    public bool SaveData = false;

    private bool lastEditTerain = false;

    private float[,] mask = null;
    private float[,] heights = null;
    private float[,,] textures = null;
    private const string SavedMask = "TerrainMask";
    private const string SavedHeights = "TerrainHeights";
    private const string SavedTextures = "TerrainTextures";
    private const int MaskIndex = 2;

    public static float[,] LoadMask(Terrain ground)
    {
        var text = Resources.Load<TextAsset>(SavedMask);
        float[,] mask = new float[ground.terrainData.alphamapResolution, ground.terrainData.alphamapResolution];
        if (text != null && text.ToString() != "")
        {
            var table = text.ToString().Split('\n');
            for (int i = 0; i < table.Length; i++)
            {
                var line = table[i].Split(' ');
                for (int j = 0; j < line.Length; j++)
                    mask[i, j] = float.Parse(line[j]);
            }
        }
        else Debug.Log("SavedMask not found");
        return mask;
    }

    public static float[,] LoadHeights(Terrain ground)
    {
        var text = Resources.Load<TextAsset>(SavedHeights);
        float[,] heights = new float[ground.terrainData.heightmapResolution, ground.terrainData.heightmapResolution];
        if (text != null && text.ToString() != "")
        {
            var table = text.ToString().Split('\n');
            for (int i = 0; i < table.Length; i++)
            {
                var line = table[i].Split(' ');
                for (int j = 0; j < line.Length; j++)
                {
                    try
                    {
                        heights[i, j] = float.Parse(line[j]);
                    }
                    catch
                    {
                        Debug.LogError("Some problem with string: " + line[j]);
                    }
                }
            }
        }
        else Debug.Log("SavedHeights not found");
        return heights;
    }
    public static float[,,] LoadTextures(Terrain ground)
    {
        var text = Resources.Load<TextAsset>(SavedTextures);
        float[,,] textures = new float[ground.terrainData.alphamapResolution, ground.terrainData.alphamapResolution, ground.terrainData.terrainLayers.Length];
        if (text != null && text.ToString() != "")
        {
            var table = text.ToString().Split('\n');
            for (int i = 0; i < table.Length; i++)
            {
                var line = table[i].Split(' ');
                for (int j = 0; j < line.Length; j++)
                {
                    var colum = line[j].Split(',');
                    for (int k = 0; k < colum.Length; k++)
                        textures[i, j, k] = float.Parse(colum[k]);
                }
            }
        }
        else Debug.Log("SavedTextures not found");
        return textures;
    }

    private void LoadData()
    {
        if(mask == null)
            mask = LoadMask(terainToEdit.ground);
        if (heights == null)
            heights = LoadHeights(terainToEdit.ground);
        if (textures == null)
            textures = LoadTextures(terainToEdit.ground);
    }

    private float GetMask()
    {
        var sum = 0f;
        var allTextuer = terainToEdit.ground.terrainData.GetAlphamaps(0, 0, mask.GetLength(0), mask.GetLength(1));
        for (int i = 0; i < mask.GetLength(0); i++)
            for (int j = 0; j < mask.GetLength(1); j++)
            {
                mask[i, j] = allTextuer[i, j, MaskIndex];
                sum += mask[i, j];
            }
        return sum;
    }

    private void GetData()
    {
        heights = terainToEdit.ground.terrainData.GetHeights(0, 0, terainToEdit.ground.terrainData.heightmapResolution, terainToEdit.ground.terrainData.heightmapResolution);
        var allTextuer = terainToEdit.ground.terrainData.GetAlphamaps(0, 0, terainToEdit.ground.terrainData.alphamapResolution, terainToEdit.ground.terrainData.alphamapResolution);
        textures = allTextuer;
    }

    private void SaveMaskToFile()
    {
        var strBuilder = new StringBuilder();
        for (int i = 0; i < mask.GetLength(0); i++)
        {
            if (i != 0)
                strBuilder.Append('\n');
            for (int j = 0; j < mask.GetLength(1); j++)
            {
                if (j != 0)
                    strBuilder.Append(' ');
                strBuilder.Append(mask[i, j]);
            }
        }
        File.WriteAllText("./Assets/Resources/" + SavedMask + ".txt", strBuilder.ToString());
        Debug.Log("Mask saved");
    }

    private void SaveDataToFile()
    {
        var strBuilder = new StringBuilder();
        for (int i = 0; i < heights.GetLength(0); i++)
        {
            if (i != 0)
                strBuilder.Append('\n');
            for (int j = 0; j < heights.GetLength(1); j++)
            {
                if (j != 0)
                    strBuilder.Append(' ');
                strBuilder.Append(heights[i, j]);
            }
        }
        File.WriteAllText("./Assets/Resources/" + SavedHeights + ".txt", strBuilder.ToString());
        strBuilder = new StringBuilder();
        for (int i = 0; i < textures.GetLength(0); i++)
        {
            if (i != 0)
                strBuilder.Append('\n');
            for (int j = 0; j < textures.GetLength(1); j++)
            {
                if (j != 0)
                    strBuilder.Append(' ');
                textures[i, j, 0] += textures[i, j, MaskIndex];
                textures[i, j, MaskIndex] = 0;
                for (int k = 0; k < textures.GetLength(2); k++)
                {
                    if (k != 0)
                        strBuilder.Append(',');
                    strBuilder.Append(textures[i, j, k]);
                }    
            }
        }
        File.WriteAllText("./Assets/Resources/" + SavedTextures + ".txt", strBuilder.ToString());
        Debug.Log("Data saved");
    }

    private void SetUpTerrain()
    {
        terainToEdit.RemoveAlreadyGeneratedElements();
        terainToEdit.GenerateWater();
        LoadData();
    }

    private void RemakeTerrain()
    {
        terainToEdit.Generate(false);
        terainToEdit.RemoveAlreadyGeneratedElements();
    }

    private void ShowMaskOnground()
    {
        float[,,] textures = new float[terainToEdit.ground.terrainData.alphamapResolution, terainToEdit.ground.terrainData.alphamapResolution, terainToEdit.ground.terrainData.terrainLayers.Length];
        for (int i = 0; i < mask.GetLength(0); i++)
            for (int j = 0; j < mask.GetLength(1); j++)
            {
                textures[i, j, 0] = 1 - mask[i, j];
                textures[i, j, MaskIndex] = mask[i, j];
            }
        terainToEdit.ground.terrainData.SetAlphamaps(0, 0, textures);
    }
    private void ShowDataOnground()
    {
        float[,] heights = new float[terainToEdit.ground.terrainData.heightmapResolution, terainToEdit.ground.terrainData.heightmapResolution];

        for (int i = 0; i < mask.GetLength(0); i++)
            for (int j = 0; j < mask.GetLength(1); j++)
            {
                var v = mask[i, j];
                heights[i, j] = Mathf.Lerp(heights[i, j], this.heights[i, j], v);
            }

        terainToEdit.ground.terrainData.SetHeights(0, 0, heights);
        terainToEdit.ground.terrainData.SetAlphamaps(0, 0, this.textures);
    }

    void Update()
    {
        if(EditTerain)
        {
            if(!lastEditTerain)
            {
                SetUpTerrain();
                Debug.Log("Editing starting");
            }
        }
        else
        {
            if (lastEditTerain)
            {
                RemakeTerrain();
                Debug.Log("Editing stoped");
            }
        }
        lastEditTerain = EditTerain;

        if(SaveMask)
        {
            var sum = GetMask();
            if (sum < 10)
                Debug.LogError("Mask is too small. something is not right");
            else SaveMaskToFile();
            SaveMask = false;
        }
        if(SaveData)
        {
            GetData();
            SaveDataToFile();
            SaveData = false;
        }

        if(ShowMask)
        {
            ShowMaskOnground();
            ShowMask = false;
            Debug.Log("Mask shown");
        }
        if (ShowData)
        {
            ShowDataOnground();
            ShowData = false;
            Debug.Log("data shown");
        }
    }
}
