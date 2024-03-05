using NUnit.Framework;
using Snake;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace MinigameTests
{

    public class SnakeGrowing
    {
        private SnakeGameHandler gameHandler;
        private GameObject correctFood;
        private GameObject wrongFood;
        private Snakes snakes;

        [UnitySetUp] 
        public IEnumerator SetUp() 
        {
            SceneManager.LoadScene("SnakeMPC2");
            yield return null;

            gameHandler = GameObject.Find("GameHandler").GetComponent<SnakeGameHandler>();
            gameHandler.StartGame();
            gameHandler.DestroyFood();
            

            string foodPath = "Assets/Prefabs/Minigames/Snake/food.prefab";
            correctFood = AssetDatabase.LoadAssetAtPath<GameObject>(foodPath);
            wrongFood = AssetDatabase.LoadAssetAtPath<GameObject>(foodPath);

            snakes = GameObject.Find("Snakes").GetComponent<Snakes>();
        }

        private GameObject GetSnakeHeadGameObject(Snakes snakes)
        {
            for (int i = 0; i < snakes.transform.childCount; i++)
            {
                Transform snakeHead = snakes.transform.GetChild(i).GetChild(0).GetChild(0);

                // check if head of snake is rendered
                if (snakeHead.GetComponent<SpriteRenderer>().enabled)
                {
                    return snakeHead.gameObject;
                }
            }
            return new GameObject();
        }

        private GameObject GetSnakeTailGameObject(Snakes snakes)
        {
            for (int i = 0; i < snakes.transform.childCount; i++)
            {
                Transform snakeHead = snakes.transform.GetChild(i).GetChild(0).GetChild(0);
                Transform snakeTail = snakes.transform.GetChild(i).GetChild(1);

                // check if head of snake is rendered
                if (snakeHead.GetComponent<SpriteRenderer>().enabled)
                {
                    return snakeTail.gameObject;
                }
            }
            return new GameObject();
        }


        [UnityTest]
        public IEnumerator Snake_EatCorrectApple_Grow()
        {
            SnakeTail snaketail = GetSnakeTailGameObject(snakes).GetComponent<SnakeTail>();

            int originalSize = snaketail.GetSize();

            yield return new WaitForSeconds(0.2f);

            CollisionHandler collisionHandler = GetSnakeHeadGameObject(snakes).GetComponent<CollisionHandler>();
            correctFood.GetComponent<FoodInformation>().correct = true;
            collisionHandler.OnTriggerEnter2D(correctFood.GetComponent<Collider2D>());

            yield return new WaitForSeconds(1f);

            snaketail = GetSnakeTailGameObject(snakes).GetComponent<SnakeTail>();
            int newSize = snaketail.GetSize();

            Assert.AreEqual(newSize, originalSize + 1);
            
        }

        [UnityTest]
        public IEnumerator Snake_EatWrongApple_DontGrow()
        {
            SnakeTail snaketail = GetSnakeTailGameObject(snakes).GetComponent<SnakeTail>();

            int originalSize = snaketail.GetSize();

            yield return new WaitForSeconds(0.2f);

            CollisionHandler collisionHandler = GetSnakeHeadGameObject(snakes).GetComponent<CollisionHandler>();
            wrongFood.GetComponent<FoodInformation>().correct = false;
            collisionHandler.OnTriggerEnter2D(wrongFood.GetComponent<Collider2D>());

            yield return new WaitForSeconds(1f);

            snaketail = GetSnakeTailGameObject(snakes).GetComponent<SnakeTail>();
            int newSize = snaketail.GetSize();

            Assert.LessOrEqual(newSize, originalSize);
        }

        [UnityTest]
        public IEnumerator Snake_Eat3CorrectAndWrongApples_GrowAndShrink()
        {
            SnakeTail snaketail = GetSnakeTailGameObject(snakes).GetComponent<SnakeTail>();

            int originalSize = snaketail.GetSize();

            yield return new WaitForSeconds(0.2f);

            CollisionHandler collisionHandler = GetSnakeHeadGameObject(snakes).GetComponent<CollisionHandler>();
            correctFood.GetComponent<FoodInformation>().correct = true;

            for (int i = 0; i < 3; i++)
            {
                collisionHandler.OnTriggerEnter2D(correctFood.GetComponent<Collider2D>());
                yield return new WaitForSeconds(1f);
                int afterCorrectAppleSize = snaketail.GetSize();
                Assert.AreEqual(afterCorrectAppleSize, originalSize + 1 + i);
            }

            wrongFood.GetComponent<FoodInformation>().correct = false;
            collisionHandler.OnTriggerEnter2D(wrongFood.GetComponent<Collider2D>());
            yield return new WaitForSeconds(1f);

            snaketail = GetSnakeTailGameObject(snakes).GetComponent<SnakeTail>();
            int afterWrongAppleSize = snaketail.GetSize();

            Assert.AreEqual(afterWrongAppleSize, originalSize);

        }

        [TearDown]
        public void TearDown()
        {
            SceneManager.UnloadSceneAsync("SnakeMPC2");
        }
    }

}

