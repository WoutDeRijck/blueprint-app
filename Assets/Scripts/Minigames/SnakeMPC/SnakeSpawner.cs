using UnityEngine;


namespace Snake
{


    /// <summary>
    /// Spawns objects in snake minigame
    /// </summary>
    public class SnakeSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject snakePrefab;

        [SerializeField]
        private GameObject foodPrefab;

        private Snakes snakes;

        /// <summary>
        /// Sets up the snake spawner
        /// </summary>
        /// <param name="snakes"></param>
        public void SetupSnakespawner(Snakes snakes)
        {
            this.snakes = snakes;
        }


        /// <summary>
        /// Spawns a new snake prefab
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public GameObject SpawnSnake(int size, float speed, float currentSpeed, Vector2 position, Quaternion rotation)
        {
            GameObject snake = Instantiate(snakePrefab, new Vector3(0,0,0), Quaternion.identity, snakes.transform);
            snake.transform.GetChild(0).GetComponent<SnakeMovement>().Setup(speed, currentSpeed, position, rotation);
            snake.transform.GetChild(1).GetComponent<SnakeTail>().Setup(size);
            return snake;
        }

        /// <summary>
        /// Spawns a new food prefab
        /// </summary>
        /// <param name="position"></param>
        /// <param name="correct"></param>
        /// <returns></returns>
        public GameObject SpawnFood(Vector2 position, bool correct)
        {
            GameObject food = Instantiate(foodPrefab, new Vector3(position.x, position.y), Quaternion.identity);
            food.GetComponent<FoodInformation>().correct = correct; //set correctness
            return food;
        }
    }


}
