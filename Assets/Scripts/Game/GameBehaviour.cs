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

    private float timeBeforeBirds = 10;
    private float timeBirds = 0;

    private float timeBeforeAmbient = 20;

    private void Start()
    {
        GoToSpwnPoint();
        timeBeforeBirds = Random.Range(10, 120);
        timeBeforeAmbient = Random.Range(30, 360);
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

    private void Update()
    {
        if (timeBirds > 0)
        {
            timeBirds -= Time.deltaTime;
            timeBeforeBirds -= timeBirds;
            if (timeBeforeBirds < 0)
            {
                SoundManager.PlaySfxClip("Bird");
                timeBeforeBirds = Random.Range(0.2f, 1);
            }
            if (timeBirds < 0)
            {
                timeBeforeBirds = Random.Range(10, 120);
            }
        }
        else
        {
            timeBeforeBirds -= Time.deltaTime;
            if (timeBeforeBirds < 0)
            {
                timeBirds = Random.Range(10, 30);
            }
        }

        timeBeforeAmbient -= Time.deltaTime;
        if (timeBeforeAmbient < 0)
        {
            SoundManager.PlaySfxClip("Ambient");
            timeBeforeAmbient = Random.Range(30, 360);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        Debug.Log(other.name);
        if(other.tag == "Player")
        {
            Debug.Log("Player ended game");
            PlayerControll.Instance.BlockMovement();
            MenuBehaviour.Instance.ShowEndCutscene();
        }
    }
}
