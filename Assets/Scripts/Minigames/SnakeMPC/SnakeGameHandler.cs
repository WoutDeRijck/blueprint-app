using GameData;
using GameManagement;
using Misc;
using NSubstitute.Routing.Handlers;
using Snake;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
using UnityEngine.UI;


namespace Snake
{


    public class SnakeGameHandler : MonoBehaviour
    {
        /// <summary>
        /// State of the Snake minigame
        /// </summary>
        public enum State
        {
            Alive,
            Paused,
            Finished,
            WaitingForSpacebar,
            Correct,
            Wrong
        }

        private State state;

        [SerializeField]
        private SnakeSpawner spawner;
        [SerializeField]
        private UIcontroller UIcontroller;
        [SerializeField]
        private Snakes snakes;
        [SerializeField]
        private InputActionReference spacebar;
        [SerializeField]
        private GameObject eventSystem;


        private int width = 16;
        private int height = 8;
        private float initialSpeed = 2f;

        private List<Vector2> foodPositions = new List<Vector2>(); //holds 4 gridpositions
        private List<GameObject> foodList = new List<GameObject>(); //holds current 4 food objects
        private int correctIndex; //holds the index (0..3) of the correct food objecct
        private int eatenIndex; // holds the index (0..3) of the last eaten food object


        private int questionIndex; //keeps track of the question index from minigame data
        private MultipleChoice mpc; //keeps track of the current mpc data structure

        private float timeNeededForQuestion = 0;

        private int lives;

        private bool inProgress = false;



        private void Start()
        {
            if (GameObject.Find("EventSystem") == null) 
            {
                GameObject eventSystemGameObject = Instantiate(eventSystem);
                eventSystemGameObject.SetActive(true);
            }

            lives = 3;

            spacebar.action.performed += SpacebarPressed;

            spawner.SetupSnakespawner(snakes);
        }

        private void OnDisable()
        {
            spacebar.action.performed -= SpacebarPressed;
        }

        /// <summary>
        /// Starts the minigame: spawn snake, display first question and display answers
        /// </summary>
        public void StartGame()
        {
            if (inProgress) return;
            inProgress = true;

            // remove canvas
            GameObject.Find("Explanation Canvas").GetComponent<Canvas>().enabled = false;


            // spawn initial snake
            if (GameManager.Instance != null)
            {
                initialSpeed = (float)GameManager.Instance?.minigame.speed * 3f + 1;
            }
            spawner.SpawnSnake(2, initialSpeed, 0, new Vector2(7, 4), Quaternion.Euler(new Vector3(0, 0, -90)));

            questionIndex = GameManager.Instance != null ? GameManager.Instance.AL.GetNextQuestionIndex() : MockData.instance.GetNextQuestionIndex();
            mpc = GameManager.Instance != null ? (MultipleChoice)GameManager.Instance.minigame : MockData.instance.minigame;

            if (questionIndex == -1)
            {
                FinishGame();
                return;
            }

            UIcontroller.SetQuestion(questionIndex);

            // add the initial 4 gridpositions to the positionlist
            for (int i = 0; i < 4; i++)
            {
                foodPositions.Add(GenerateNewFoodPosition());
            }

            SpawnFoods();
        }

        private void FixedUpdate()
        {
            timeNeededForQuestion += Time.fixedDeltaTime;

            switch (state)
            {
                case State.Alive:
                    break;

                case State.Correct:
                    GrowSnake();
                    SpawnNextQuestionAndFood();
                    if (state == State.Finished) break;
                    PauseSnake(true);
                    break;

                case State.Paused:
                    break;

                case State.Wrong:
                    ShrinkSnake();
                    PauseSnake(false);
                    ShowFeedback();
                    break;

                case State.Finished:
                    FinishGame();
                    break;

                case State.WaitingForSpacebar:
                    break;

            }
        }

        /// <summary>
        /// Shrinks the snake back to the original size
        /// </summary>
        private void ShrinkSnake()
        {
            snakes.ShrinkSnake();
        }

        /// <summary>
        /// Finishes the minigame and unloads scene
        /// </summary>
        private void FinishGame()
        {
            Debug.Log("finish called!");

            snakes.DestroyAllSnakes();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AL.Score();

                GameObject.Find("Minigame Camera").GetComponent<AudioListener>().enabled = false; //disable minigame camera
                GameObject.Find("Main Camera").GetComponent<AudioListener>().enabled = true; //enable game camera

                GameManager.Instance.changeStatus(GameManager.Status.inGame);

                SceneLoader.Unload("SnakeMPC2");
            }

            state = State.Paused;
        }

        /// <summary>
        /// Grows the snake with 1 unit
        /// </summary>
        private void GrowSnake()
        {
            snakes.GrowSnake();
        }

        /// <summary>
        /// Handles spacebar input
        /// </summary>
        /// <param name="context"></param>
        private void SpacebarPressed(InputAction.CallbackContext context)
        {
            Debug.Log("spacebar pressed");
            if (state == State.WaitingForSpacebar)
            {
                Debug.Log("spacebar handled");
                GameObject.Find("Feedback Canvas").GetComponent<Canvas>().enabled = false;

                if (lives == 0)
                {
                    Debug.Log("-- finished (out of lives)");
                    DestroyFood();
                    state = State.Finished;
                    return;
                }

                SpawnNextQuestionAndFood();

                if (state == State.Finished)
                {
                    Debug.Log("-- finished (end of ECS)");
                    return;
                }

                state = State.Paused;
            }
        }

        /// <summary>
        /// Shows the feedback screen (wrong apples turn gray)
        /// </summary>
        private void ShowFeedback()
        {
            foreach (GameObject food in foodList)
            {
                if (!food.GetComponent<FoodInformation>().correct)
                {
                    food.GetComponent<SpriteRenderer>().material.SetFloat("_GrayscaleAmount", 1);
                }
            }

            // show feedback canvas
            GameObject.Find("Feedback Canvas").GetComponent<Canvas>().enabled = true;

            state = State.WaitingForSpacebar;
        }

        /// <summary>
        /// Sets the snake's speed to zero
        /// </summary>
        /// <param name="setState"></param>
        private void PauseSnake(bool setState)
        {
            snakes.PauseSnake();

            if (setState)
            {
                state = State.Paused;
            }
        }



        /// <summary>
        /// Generate a new position that is not already occupied or obstructed
        /// </summary>
        /// <returns> new gridposition </returns>
        private Vector2 GenerateNewFoodPosition()
        {
            List<Vector2> occupiedBySnake = snakes.GetOccupiedPositions();

            List<Vector2> snakeHeadNeighbours = GetNeighbouringPositions(snakes.GetHeadPosition());

            List<Vector2> occupiedByFoodNeighbours = new List<Vector2>();
            List<int> occupiedRows = new List<int>();
            List<int> occupiedCols = new List<int>();
            foreach (Vector2 position in foodPositions)
            {
                occupiedRows.Add((int) position.x);
                occupiedCols.Add((int) position.y);
                occupiedByFoodNeighbours.AddRange(GetNeighbouringPositions(position));
            }

            Vector2 foodPosition;
            do
            {
                foodPosition = new Vector2(Random.Range(0, width), Random.Range(0, height));

            } while (occupiedBySnake.Contains(foodPosition) //body
                        || occupiedByFoodNeighbours.Contains(foodPosition)
                        || occupiedRows.Contains((int) foodPosition.x) 
                        || occupiedCols.Contains((int) foodPosition.y)
                        || snakeHeadNeighbours.Contains(foodPosition) // in the neighbourhood of snake head
                        );
            return foodPosition;
        }

        /// <summary>
        /// Returns a list of the neighbouring positions
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private List<Vector2> GetNeighbouringPositions(Vector2 position)
        {
            List<Vector2> positions = new List<Vector2>();

            for (int i = -1; i < 2; i++)
            {
                for(int j = -1; j < 2; j++)
                {
                    Vector2 neighbour = new Vector2(position.x + i, position.y + j);
                    positions.Add(neighbour);
                }
            }

            return positions;
        }

        /// <summary>
        /// Updates the game state based on the answer
        /// </summary>
        /// <param name="correct"></param>
        public void AnswerQuestion(bool correct)
        {
            GameManager.Instance?.AL.AnswerQuestion(questionIndex, correct, timeNeededForQuestion);

            if (correct)
            {
                state = State.Correct;
            }
            else
            {
                state = State.Wrong;
                LoseLive();
            }

            if (GameManager.Instance != null)
            {
                snakes.SetSpeed((float)GameManager.Instance?.minigame.speed * 3f + 1); // speed will be between 1-4
            }
            else
            {
                float speed = snakes.transform.GetChild(0).GetChild(0).GetComponent<SnakeMovement>().GetSpeed();
                speed += (correct) ? 1 : -1;
                if (speed < 2) speed = 2;
                if (speed > 4) speed = 4;
                snakes.SetSpeed(speed);
            }
        }

        /// <summary>
        /// Spawns a new question and 1 new food object
        /// </summary>
        private void SpawnNextQuestionAndFood()
        {
            timeNeededForQuestion = 0;

            // remove all current food
            DestroyFood();

            // set new question and spawn new food
            questionIndex = GameManager.Instance != null ? GameManager.Instance.AL.GetNextQuestionIndex() : MockData.instance.GetNextQuestionIndex();

            if (questionIndex == -1)
            {
                state = State.Finished;
            }
            else
            {
                UIcontroller.SetQuestion(questionIndex);
                SpawnNextFoods(eatenIndex);
            }
        }

        /// <summary>
        /// Spawns 3 foodobject at existing positions and generates 1 new food position to spawn the fourth food object
        /// </summary>
        private void SpawnNextFoods(int indexToReplace)
        {
            Vector2 newPosition = GenerateNewFoodPosition(); // generate a new grid position
            foodPositions[indexToReplace] = newPosition; // replace previous eaten position with new

            SpawnFoods(); //spawn all 4 foods
        }

        /// <summary>
        /// Spawns 4 food objects
        /// </summary>
        private void SpawnFoods()
        {
            correctIndex = Random.Range(0, 4); //randomly assign correct index for positionList

            // spawn the correct food
            SpawnFood(mpc.correctAnswers[questionIndex], true, foodPositions[correctIndex]);

            // spawn the wrong foods
            int i = 0;
            foreach (string wrongAnswer in mpc.wrongAnswers[questionIndex])
            {
                if (i == correctIndex)
                {
                    i++;
                }
                SpawnFood(wrongAnswer, false, foodPositions[i]);
                i++;
            }
        }

        /// <summary>
        /// Spawns food object with answer as text
        /// </summary>
        private void SpawnFood(string answer, bool correct, Vector2 position)
        {
            GameObject food = spawner.SpawnFood(position, correct); //spawn food object
            food.GetComponentInChildren<Text>().text = answer; //set answer text
            foodList.Add(food); //add reference to foodList
        }

        /// <summary>
        /// Destroys all food objects in the foodList
        /// </summary>
        public void DestroyFood()
        {
            foreach (GameObject food in foodList)
            {
                Destroy(food);
            }
            foodList.Clear(); // clear the foodlist
        }

        /// <summary>
        /// Sets the game state
        /// </summary>
        /// <param name="state"></param>
        public void SetState(State state)
        {
            this.state = state;
        }

        /// <summary>
        /// Returns the game state
        /// </summary>
        /// <returns></returns>
        public State GetState()
        {
            return this.state;
        }

        /// <summary>
        /// Calculates which food was eaten in the foodList
        /// </summary>
        /// <param name="food"></param>
        public void Eat(GameObject food)
        {
            if (foodList.Contains(food))
            {
                eatenIndex = foodPositions.IndexOf(food.transform.position);
            }
        }

        /// <summary>
        /// Reduces the hearts by 1
        /// </summary>
        public void LoseLive()
        {
            if (lives == 3)
            {
                lives--; ;
                Destroy(GameObject.Find("Heart3"));
            }
            else if (lives == 2)
            {
                lives--;
                Destroy(GameObject.Find("Heart2"));
            }
            else if (lives == 1)
            {
                lives--;
                Destroy(GameObject.Find("Heart1"));
            }
            else
            {
                lives--;
            }

        }
    }
}
