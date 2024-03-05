using GameManagement;
using UnityEngine;

namespace TowerDefense
{

/// <summary>
/// Camera follows player
/// </summary>
public class CameraFollow : MonoBehaviour
{
    private float followSpeed = 6f;
    private float cameraDistance = -10f;
    private Transform playerTransform;
    private GameObject spawnLocation;

    /// <summary>
    /// Sets up camera position
    /// </summary>
    private void Start()
    {
        playerTransform = GameManager.Instance.player.transform;
        spawnLocation = GameObject.Find("SpawnLocation");
        transform.position = spawnLocation.GetComponent<Transform>().position;
        if(!playerTransform.gameObject.GetComponent<SpriteRenderer>().enabled)
        {
            //the player is the teacher, zoom camera out
            transform.gameObject.GetComponent<Camera>().orthographicSize = 7.5f;
        }
    }


    /// <summary>
    /// Changes camera position to follow the player position smoothly
    /// </summary>
    void Update()
    {
        Vector3 newPos = new Vector3(playerTransform.position.x, playerTransform.position.y, cameraDistance);
        transform.position = Vector3.Slerp(transform.position, newPos, followSpeed * Time.deltaTime);
    }
}
}

