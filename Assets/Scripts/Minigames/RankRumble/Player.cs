using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RankRumble 
{

/// <summary>
/// Controls Player Sprite
/// </summary>
public class Player : MonoBehaviour
{

    /// <summary>
    /// Smiley sprites to display the answers and correctness
    /// </summary>
    public Sprite normalSmiley;
    public Sprite happySmiley;
    public Sprite sadSmiley;
    public Sprite horizontal;

    /// <summary>
    /// Variables to rotate pointer sprite the correct way
    /// </summary>
    private Quaternion normal = Quaternion.Euler(0f, 0f, 0f);
    private Quaternion down = Quaternion.Euler(0f, 0f, 90f);

    // Start is called before the first frame update
    void Start()
    {
        //this.gameObject.transform.GetComponent<SpriteRenderer>().sprite = normalSmiley;
        SetSprite("normal");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Function that changes the sprite that is rendered according to the input given by the player
    /// </summary>
    public void SetSprite(string sprite)
    {
        if (sprite == "normal")
        {
            this.gameObject.transform.rotation = normal;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            this.gameObject.GetComponent<SpriteRenderer>().sprite = normalSmiley;
        } else if (sprite == "happy") {
            this.gameObject.transform.rotation = normal;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            this.gameObject.GetComponent<SpriteRenderer>().sprite = happySmiley;
        } else if (sprite == "sad") {
            this.gameObject.transform.rotation = normal;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            this.gameObject.GetComponent<SpriteRenderer>().sprite = sadSmiley;
        } else if (sprite == "left") {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = horizontal;
            this.gameObject.transform.rotation = normal;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        } else if (sprite == "right") {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = horizontal;
            this.gameObject.transform.rotation = normal;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        } else if (sprite == "top") {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = horizontal;
            this.gameObject.transform.rotation = down;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        } else if (sprite == "bottom") {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = horizontal;
            this.gameObject.transform.rotation = down;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        } else {
            Debug.Log("Invalid Input");
        }
    }

}
}