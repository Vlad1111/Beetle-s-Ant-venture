using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainEditor : MonoBehaviour
{
    public TerrainGenerator terainToEdit;
    public bool EditTerain = false;
    private bool lastEditTerain = false;

    private float[,] mask = null;
    private const string SavedMask = "TerrainMask";

    public static float[,] LoadMask(Terrain ground)
    {
        var text = Resources.Load<TextAsset>(SavedMask);
        float[,] mask = null;
        mask = new float[ground.terrainData.alphamapResolution, ground.terrainData.alphamapResolution];
        if (text == null)
        {
        }
        else
        {

        }
        return mask;
    }

    private void LoadMask()
    {
    }

    private void SetUpTerrain()
    {
        terainToEdit.RemoveAlreadyGeneratedElements();
        terainToEdit.GenerateWater();
    }

    private void RemakeTerrain()
    {
        terainToEdit.Generate(false);
        terainToEdit.RemoveAlreadyGeneratedElements();
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
    }
}
