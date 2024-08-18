using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainEditor : MonoBehaviour
{
    public TerrainGenerator terainToEdit;
    public bool EditTerain = false;
    public bool SaveMask = false;
    public bool SaveData = false;
    public bool ShowMask = false;
    public bool ShowData = false;

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
        if (text != null)
        {
            var table = text.ToString().Split('\n');
            for(int i=0;i<table.Length;i++)
            {
                var line = table[i].Split(' ');
                for(int j=0;j<line.Length;j++)
                    mask[i,j] = float.Parse(line[j]);
            }
        }
        return mask;
    }

    public static float[,] LoadHeights(Terrain ground)
    {
        var text = Resources.Load<TextAsset>(SavedHeights);
        float[,] heights = new float[ground.terrainData.heightmapResolution, ground.terrainData.heightmapResolution];
        if (text != null)
        {
            var table = text.ToString().Split('\n');
            for (int i = 0; i < table.Length; i++)
            {
                var line = table[i].Split(' ');
                for (int j = 0; j < line.Length; j++)
                    heights[i, j] = float.Parse(line[j]);
            }
        }
        return heights;
    }
    public static float[,,] LoadTextures(Terrain ground)
    {
        var text = Resources.Load<TextAsset>(SavedTextures);
        float[,,] textures = new float[ground.terrainData.alphamapResolution, ground.terrainData.alphamapResolution, ground.terrainData.terrainLayers.Length];
        if (text != null)
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
        return textures;
    }

    private void LoadData()
    {
        if(mask == null)
            mask = LoadMask(terainToEdit.ground);
        if (heights == null)
            heights = LoadHeights(terainToEdit.ground);
    }

    private void GetMask()
    {
        var allTextuer = terainToEdit.ground.terrainData.GetAlphamaps(0, 0, mask.GetLength(0), mask.GetLength(1));
        for (int i = 0; i < mask.GetLength(0); i++)
            for (int j = 0; j < mask.GetLength(1); j++)
            {
                mask[i, j] = allTextuer[i, j, MaskIndex];
            }
    }

    private void GetData()
    {
        heights = terainToEdit.ground.terrainData.GetHeights(0, 0, heights.GetLength(0), heights.GetLength(1));
        var allTextuer = terainToEdit.ground.terrainData.GetAlphamaps(0, 0, mask.GetLength(0), mask.GetLength(1));
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
    }

    private void SaveDataToFile()
    {

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
        float[,,] textures = new float[terainToEdit.ground.terrainData.alphamapResolution, terainToEdit.ground.terrainData.alphamapResolution, terainToEdit.ground.terrainData.terrainLayers.Length];
        float[,] heights = new float[terainToEdit.ground.terrainData.heightmapResolution, terainToEdit.ground.terrainData.heightmapResolution];

        for (int i = 0; i < mask.GetLength(0); i++)
            for (int j = 0; j < mask.GetLength(1); j++)
            {
                var v = mask[i, j];
                heights[i, j] = this.heights[i, j] * v;
                for (int k = 0; k < this.textures.GetLength(2); k++)
                {
                    if (k == MaskIndex)
                        textures[i, j, k] = v;
                    else textures[i, j, k] = this.textures[i, j, k] * v;
                }
            }

        terainToEdit.ground.terrainData.SetHeights(0, 0, heights);
        terainToEdit.ground.terrainData.SetAlphamaps(0, 0, textures);
    }

    void Update()
    {
        if(EditTerain)
        {
            if(!lastEditTerain)
            {
                SetUpTerrain();
            }
        }
        else
        {
            if (lastEditTerain)
            {
                RemakeTerrain();
            }
        }
        lastEditTerain = EditTerain;

        if(SaveMask)
        {
            GetMask();
            SaveMask = false;
        }
        if(SaveData)
        {
            GetData();
            SaveData = false;
        }

        if(ShowMask)
        {
            ShowMaskOnground();
            ShowMask = false;
        }
        if (ShowData)
        {
            ShowDataOnground();
            ShowData = false;
        }
    }
}
