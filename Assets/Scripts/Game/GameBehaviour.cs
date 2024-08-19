using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public static GameBehaviour Instance;
    private void Awake()
    {
        Instance = this;
    }

    public TerrainGenerator terrainGenerator;
    public PlayerControll player;
    public Transform playerSpwnPoint;

    private bool wasGenerated = false;

    private void Start()
    {
        GoToSpwnPoint();
    }

    public void GoToSpwnPoint()
    {
        player.transform.position = playerSpwnPoint.position;
        player.transform.rotation = playerSpwnPoint.rotation;
    }

    public void PlayerDrwoned()
    {
        player.BreakConcentration();
        GoToSpwnPoint();
    }

   // private void Update()
   // {
   //     if(!wasGenerated)
   //     {
   //         wasGenerated = true;
   //         terrainGenerator.Generate();
   //     }
   // }
}
