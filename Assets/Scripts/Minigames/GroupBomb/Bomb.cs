using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

namespace GroupBomb
{

/// <summary>
/// Implements functionality of Bomb GameObjects
/// </summary>
public class Bomb : MonoBehaviour
{

    /// <summary>
    ///  running -> counting down not yet placed
    ///  stored -> sorted in a correct group
    ///  clickedOn -> Bomb is being held at the moment
    /// </summary>


    public enum State
    {
        running,
        stored,
        clickedOn
    }

    //save the state of the bomb: running, stored or clickedOn
    public State state;
    //Save Position on the grid of the bomb
    public Vector3 gridPosition;
    //Save Direction in which the bomb is moving on the grid
    public Vector3 gridMoveDirection;
    //Keep a counter to know time since last move
    private float gridMoveTimer;
    //Define time which gridMoveTimer needs to reach before next movement
    private float gridMoveTimerMax;
    //Save LevelGrid
    private LevelGrid levelGrid;
    //Save value attached to the bomb, this value is what needs to be grouped correctly
    public string value;
    //Save correct Answer
    public int corrAnswer;

    //Objects for correct clicking behaviour
    public GameObject selectedObject;
    Vector3 offset;
    //Save countDown to change color correctly
    private float countDown = 0f;
    //Keep track of the time the bomb is alive
    public float totCountDown = 0f;
    //Define a time when bomb detonates
    public float detonationTime = 10f;
    //Save countDown to change color correctly
    private int colorCounter = 0;

    //Speed attribute for adaptive learning integration, influences time to detonate
    public float speed = 4f;
    //Set BeginColor
    private Color startColor = new Color(255,255,255,255);
    //Set EndColor (red)
    private Color endColor = new Color(255,0,0,255);
    //Save startTime for Lerp color change
    private float startTime;

    //Get access to text on UI
    private GameObject txt;

    //Event in case bomb countdown reaches detonationtime
    public static event BombExplosion OnBombExplosion;

    //Delegate for BombExplosion event object passed will be the bomb gameObject
    public delegate void BombExplosion();

    //Delegate in order to check bombplacement, place is which answer, go is which bomb
    public delegate void BombPlaced(int place, GameObject go);
    //Event when an answer is given and it needs to be checked
    public static event BombPlaced OnBombPlaced;
    //Event to assign value to bomb
    public delegate void Spawn(GameObject bomb);
    public static event Spawn OnSpawn;
    public int givenIndex = -1;

    

    /// <summary>
    /// Set Position, direction, state and timers
    /// </summary>
    private void Awake()
    {
        //Debug.Log("Bomb Awake");
        gridPosition = new Vector3(-12,8);
        gridMoveDirection = new Vector3(0, -0.1f);
        gridMoveTimerMax = 0.025f;
        gridMoveTimer = 0f;
        state = State.running;
        float startTime = Time.time;       
        //GetComponent<SpriteRenderer>().color = startColor; 
        //StartCoroutine(ChangeColor());
        OnSpawn?.Invoke(this.transform.gameObject);
        txt = this.transform.GetChild(0).GetChild(0).gameObject;
        //Debug.Log(txt.GetComponent<TextMeshProUGUI>().text);
        txt.GetComponent<TextMeshProUGUI>().text = value;
    }

    /// <summary>
    /// Store levelGrid for further use
    /// </summary>
    public void Setup(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
    }



    // Update is called once per frame
    /// <summary>
    /// Every frame, update movement, potential clicks, colorchanges, and potential explosion incase the bomb is running
    /// If bomb is stored (correctly answered), only do movement
    /// if bomb is being clickedOn, stop timers and movement temporarely
    /// </summary>
    void Update()
    {
        if (state == State.running)
        {
            HandleGridMovement();
            ClickAndDrag();
            HandleColor();
            HandleExplosion();
        } else if (state == State.stored)
        {
            HandleGridMovement();
        } else
        {
            ClickAndDrag();
        }
    }

    /// <summary>
    /// Check If detonationtime has been reached
    /// </summary>
    private void HandleExplosion()
    {
        if (state == Bomb.State.running)
        {
            totCountDown += Time.deltaTime;
            if (totCountDown >= detonationTime)
            {
                OnBombExplosion?.Invoke();
            }
        }
    }

    /// <summary>
    /// Function to switch color to red a few seconds before exploding
    /// </summary>
    private void HandleColor()
    {
        /* Color lerpedColor;
        lerpedColor = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time, 1));
        GetComponent<SpriteRenderer>().color = lerpedColor; */
        if (state == Bomb.State.running)
        {
            countDown += Time.deltaTime;
        if (countDown >= (detonationTime - 3f)/255)
        {
            colorCounter += 1;
            GetComponent<SpriteRenderer>().color = new Color(255, 255 - colorCounter, 255 - colorCounter, 255);
            countDown = 0f;
        }
        }
    }

    //Currently unused but might be a beter option to adjust color of Bombs gradually
    private IEnumerator ChangeColor()
    {

        float tick = 0f;
        while (GetComponent<SpriteRenderer>().color != endColor)
        {
            tick += (Time.deltaTime * speed)/detonationTime;
            GetComponent<SpriteRenderer>().color = Color.Lerp(startColor, endColor, tick);
            yield return null;
        }
    }

    /// <summary>
    /// Function to achieve 2 goals
    /// 1) Implement Mouse Clicks on Bombs appropriately
    /// 2) In case Bombs is released in the borders of an answer, check answer
    /// </summary>
    private void ClickAndDrag()
    {
        //Correct clicking behaviour
        if (GameObject.FindGameObjectsWithTag("GroupBombCamera").GetLength(0) == 0) {
            return;
        }
        Camera cam = GameObject.FindGameObjectsWithTag("GroupBombCamera")[0].GetComponent<Camera>() as Camera;
    
        //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);
            if (targetObject && targetObject.GetComponent<Bomb>().state == Bomb.State.running)
            {
                selectedObject = targetObject.transform.gameObject;
                offset = selectedObject.transform.position - mousePosition;
                selectedObject.GetComponent<Bomb>().state = State.clickedOn;
            }
        }
        if (selectedObject)
        {
            if ((mousePosition + offset).x > -26.25 && (mousePosition + offset).x < 0.75 && (mousePosition + offset).y < 8.25 && (mousePosition + offset).y > -7.75)
            {
                selectedObject.transform.position = mousePosition + offset;
                selectedObject.GetComponent<Bomb>().gridPosition = mousePosition + offset;
            }
        }
        //answer check
        if (Input.GetMouseButtonUp(0) && selectedObject)
        {
            CheckWhichAnswer(selectedObject);
            selectedObject = null;
        }
    }

    // 0 is top left
    // 1 is bottom left
    // 2 is top right
    // 3 is bottom right
    private int checkBox(Vector3 pos) {
        if (pos.x > -26.25 && pos.x < -18.25 && pos.y < 8.25 && pos.y > 0.25) {
            //Top Left Box
            return 0;
        } else if (pos.x > -26.25 && pos.x < -18.25 && pos.y < 0.25 && pos.y > -7.75) {
            //Bottom Left Box
            return 1;
        } else if (pos.x < 0.75 && pos.x > -7.25 && pos.y < 8.25 && pos.y > 0.25) {
            //Top Right Box
            return 2;
        } else if (pos.x < 0.75 && pos.x > -7.25 && pos.y < 0.25 && pos.y > -7.75) {
            //Bottom Right Box
            return 3;
        } else {
            // no box...
            return -1;
        }
    }

    /// <summary>
    /// Functions used to see which Group is Answered for a certain Bomb
    /// </summary>
    private void CheckWhichAnswer(GameObject selectedObject)
    {
        // check which box the momb is put in
        int box = checkBox(selectedObject.transform.position);

        if (box == -1) { // invalid box value was returned
            selectedObject.GetComponent<Bomb>().state = Bomb.State.running;
        } else {
            // bomb is in stored state now
            //selectedObject.GetComponent<Bomb>().state = Bomb.State.stored;
            // Invoke (checkAnswer!)
            OnBombPlaced?.Invoke(box, selectedObject);
        }

        selectedObject = null;
    }

    
/* 
    private void OnMouseDown()
    {
        public GameObject selectedObject;
        Vector3 offset;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);
    }
 */


    /// <summary>
    /// Function so bombs move correctly
    /// </summary>
    private void HandleGridMovement()
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridPosition += gridMoveDirection;
            gridMoveTimer = 0f;
            transform.position = new Vector3(gridPosition.x, gridPosition.y);
        }
    }


    /// <summary>
    /// CollisionCheck for obstacles
    /// </summary>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Obstacle_Vertical")
        {
            gridMoveDirection.x = gridMoveDirection.x * (-1);
        } else if (collision.gameObject.tag == "Obstacle_Horizontal")
        {
            gridMoveDirection.y = gridMoveDirection.y * (-1);
        }
        else if (collision.gameObject.tag == "Item")
        {
            //Nothing for now
        }
    }
    

}
}
