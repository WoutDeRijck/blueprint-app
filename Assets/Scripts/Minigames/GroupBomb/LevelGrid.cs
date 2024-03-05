using System.Collections.Generic;
using UnityEngine;
using Misc;
using GameManagement;


namespace GroupBomb
{

/// <summary>
/// Provides Tilemap functionality for correct locations
/// </summary>
public class LevelGrid : MonoBehaviour
{

    //Store dimensions of grid
    private int width;
    private int height;
    //Store the correct Prefab for a Bomb instantiation
    private GameObject bombPrefab;

    //private List<GameObject> bombs;
    //private UIcontroller controller;

    //Setup StartingPositions for the Bombs
    private Vector3 startPosition = new Vector3(-12,8,0);
    
    //Keep a bool to alternate between spawning patterns
    private bool patternTurn = false;
    //Store whether or not game is endeding
    public bool endGame = false;
    public List<GameObject> bombs = new List<GameObject>();
    //flag to set when game needs to be stopped
    bool gameStoppedForcibly = false;


    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
    }


    /*/// <summary>
    /// Function used to setup correct UI controller
    /// </summary>
    public void Setup (UIcontroller controller)
    {
        this.controller = controller;

    }*/


    /// <summary>
    /// Function used to setup correct BombPrefab (gets called from GameHandler)
    /// </summary>
    public void SetBombPrefab(GameObject prefab)
    {
        this.bombPrefab = prefab;
    }

    /// <summary>
    /// Function to correctly spawn one Bomb with a certain value
    /// </summary>
    public GameObject SpawnBomb()
    {

        if (!endGame)
        {
            GameObject bomb = new GameObject();
            bomb.name = "Bomb";
            bomb.tag = "Bomb";
            bomb = Instantiate(bombPrefab, startPosition, Quaternion.identity) as GameObject;
            //SceneManager.MoveGameObjectToScene(bomb, SceneManager.GetSceneByName("GroupBomb"));
            bombs.Add(bomb.gameObject);
            return bomb;
        } else {
            return null;
        }
    }


    /// <summary>
    /// Function that combines the patternTurn Object and SpawnBombs Function
    /// Will Check which turn
    /// On oneven turns: Spawn One Bomb
    /// On Event turns: Spawn two bombs with different movement 
    /// </summary>
    public void SpawnBeacon()
    {

        if (!endGame && !gameStoppedForcibly)
        {
            GameObject bomb = SpawnBomb();
            bomb.GetComponent<Bomb>().gridMoveDirection = new Vector3(Random.Range(-0.1f, 0.1f), -0.1f);
        }
    }

    /// <summary>
    /// Function where all bombs get removed from the scene when the game ends
    /// </summary>
    public void EndGame()
    {
        endGame = true;
        for (int i = 0; i < bombs.Count;i++)
        {
            Destroy(bombs[i]);
        }

        bombs.Clear();

    }

    public void ForceStopGame()
    {
        gameStoppedForcibly = true;
    }

}
}