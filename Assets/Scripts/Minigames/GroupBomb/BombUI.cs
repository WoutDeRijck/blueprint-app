using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GroupBomb
{

/// <summary>
/// Provides UI for Bomb GameObject
/// </summary>
public class BombUI : MonoBehaviour
{

    private GameObject bomb;
    //[SerializeField] public Transform unit;

    [SerializeField] public Vector3 offset;



    // Start is called before the first frame update
    void Start()
    {
        //unit = this.transform.parent;
        bomb = this.transform.parent.parent.gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(bomb.name);
        //transform.position = new Transform((bomb.gridPosition + offset).x, (bomb.gridPosition + offset).y, (bomb.gridPosition + offset).z);
        this.transform.position = bomb.transform.position + offset;
        //transform.position = unit.position + offset;
    }
}
}

