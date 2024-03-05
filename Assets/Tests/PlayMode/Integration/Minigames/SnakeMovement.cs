using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using NUnit.Framework;
using Snake;

namespace MinigameTests
{

    public class SnakeMovement
    {
        private InputTestFixture inputTestFixture = new InputTestFixture();
        private Keyboard keyboard;
        private SnakeGameHandler gameHandler;
        private Snakes snakes;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            inputTestFixture.Setup();
            SceneManager.LoadScene("SnakeMPC2");
            yield return null;

            keyboard = InputSystem.AddDevice<Keyboard>();

            gameHandler = GameObject.Find("GameHandler").GetComponent<SnakeGameHandler>();
            snakes = GameObject.Find("Snakes").GetComponent<Snakes>();

            gameHandler.StartGame();
            gameHandler.DestroyFood();
        }

        [UnityTest]
        public IEnumerator Snake_RightArrow_Move()
        {
            inputTestFixture.Press(keyboard.rightArrowKey);
            yield return new WaitForSeconds(0.5f);
            inputTestFixture.Release(keyboard.rightArrowKey);

            Vector2 originalPosition = snakes.GetHeadPosition();

            yield return new WaitForSeconds(1f);

            Assert.That((snakes.GetHeadPosition() - originalPosition).normalized, Is.EqualTo(Vector2.right).Using(Vector3EqualityComparer.Instance));
        }


        [UnityTest]
        public IEnumerator Snake_UpArrow_Move()
        {
            inputTestFixture.Press(keyboard.upArrowKey);
            yield return new WaitForSeconds(0.5f);
            inputTestFixture.Release(keyboard.upArrowKey);

            Vector2 originalPosition = snakes.GetHeadPosition();

            yield return new WaitForSeconds(1f);

            Assert.That((snakes.GetHeadPosition() - originalPosition).normalized, Is.EqualTo(Vector2.up).Using(Vector3EqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator Snake_DownArrow_Move()
        {
            inputTestFixture.Press(keyboard.downArrowKey);
            yield return new WaitForSeconds(0.5f);
            inputTestFixture.Release(keyboard.downArrowKey);

            Vector2 originalPosition = snakes.GetHeadPosition();

            yield return new WaitForSeconds(1f);

            Assert.That((snakes.GetHeadPosition() - originalPosition).normalized, Is.EqualTo(Vector2.down).Using(Vector3EqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator Snake_LeftArrow_Move()
        {
            inputTestFixture.Press(keyboard.leftArrowKey);
            yield return new WaitForSeconds(0.5f);
            inputTestFixture.Release(keyboard.leftArrowKey);

            Vector2 originalPosition = snakes.GetHeadPosition();

            yield return new WaitForSeconds(1f);

            Assert.That((snakes.GetHeadPosition() - originalPosition).normalized, Is.EqualTo(Vector2.left).Using(Vector3EqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator Snake_InvalidKey_NoMovement()
        {
            Vector2 originalPosition = snakes.GetHeadPosition();

            inputTestFixture.Press(keyboard.aKey);
            inputTestFixture.Press(keyboard.zKey);
            inputTestFixture.Press(keyboard.eKey);
            inputTestFixture.Press(keyboard.rKey);
            inputTestFixture.Press(keyboard.tKey);
            inputTestFixture.Press(keyboard.yKey);
            yield return new WaitForSeconds(1f);

            Assert.That(snakes.GetHeadPosition(), Is.EqualTo(originalPosition).Using(Vector3EqualityComparer.Instance));
        }


        [TearDown]
        public void TearDown()
        {
            inputTestFixture.TearDown();
            SceneManager.UnloadSceneAsync("SnakeMPC2");
        }

    }

}

