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

    public class SnakeBoundaries
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
        public IEnumerator Snake_DoesNotReachRightWall_Movedstraight()
        {
            inputTestFixture.Press(keyboard.rightArrowKey);
            yield return new WaitForSeconds(0.5f);
            inputTestFixture.Release(keyboard.rightArrowKey);

            Vector2 originalPosition = snakes.GetHeadPosition();

            yield return new WaitForSeconds(1f);

            Assert.That((snakes.GetHeadPosition() - originalPosition).normalized, Is.EqualTo(Vector2.right).Using(Vector3EqualityComparer.Instance));
        }


        [UnityTest]
        public IEnumerator Snake_ReachedRightWall_WrapsAround()
        {
            inputTestFixture.Press(keyboard.rightArrowKey);
            yield return new WaitForSeconds(0.5f);
            inputTestFixture.Release(keyboard.rightArrowKey);

            Vector2 originalPosition = snakes.GetHeadPosition();

            yield return new WaitForSeconds(4.5f);

            Assert.That((snakes.GetHeadPosition() - originalPosition).normalized, Is.EqualTo(Vector2.left).Using(Vector3EqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator Snake_DoesNotReachLeftWall_Movedstraight()
        {
            inputTestFixture.Press(keyboard.leftArrowKey);
            yield return new WaitForSeconds(0.5f);
            inputTestFixture.Release(keyboard.leftArrowKey);

            Vector2 originalPosition = snakes.GetHeadPosition();

            yield return new WaitForSeconds(1f);

            Assert.That((snakes.GetHeadPosition() - originalPosition).normalized, Is.EqualTo(Vector2.left).Using(Vector3EqualityComparer.Instance));
        }


        [UnityTest]
        public IEnumerator Snake_ReachedLeftWall_WrapsAround()
        {
            inputTestFixture.Press(keyboard.leftArrowKey);
            yield return new WaitForSeconds(0.5f);
            inputTestFixture.Release(keyboard.leftArrowKey);

            Vector2 originalPosition = snakes.GetHeadPosition();

            yield return new WaitForSeconds(4.5f);

            Assert.That((snakes.GetHeadPosition() - originalPosition).normalized, Is.EqualTo(Vector2.right).Using(Vector3EqualityComparer.Instance));
        }



        [UnityTest]
        public IEnumerator Snake_DoesNotReachTopWall_Movedstraight()
        {
            inputTestFixture.Press(keyboard.upArrowKey);
            yield return new WaitForSeconds(0.5f);
            inputTestFixture.Release(keyboard.upArrowKey);

            Vector2 originalPosition = snakes.GetHeadPosition();

            yield return new WaitForSeconds(1f);

            Assert.That((snakes.GetHeadPosition() - originalPosition).normalized, Is.EqualTo(Vector2.up).Using(Vector3EqualityComparer.Instance));
        }


        [UnityTest]
        public IEnumerator Snake_ReachedTopWall_WrapsAround()
        {
            inputTestFixture.Press(keyboard.upArrowKey);
            yield return new WaitForSeconds(0.5f);
            inputTestFixture.Release(keyboard.upArrowKey);

            Vector2 originalPosition = snakes.GetHeadPosition();

            yield return new WaitForSeconds(3f);

            Assert.That((snakes.GetHeadPosition() - originalPosition).normalized, Is.EqualTo(Vector2.down).Using(Vector3EqualityComparer.Instance));
        }



        [UnityTest]
        public IEnumerator Snake_DoesNotReachBottomWall_Movedstraight()
        {
            inputTestFixture.Press(keyboard.downArrowKey);
            yield return new WaitForSeconds(0.5f);
            inputTestFixture.Release(keyboard.downArrowKey);

            Vector2 originalPosition = snakes.GetHeadPosition();

            yield return new WaitForSeconds(1f);

            Assert.That((snakes.GetHeadPosition() - originalPosition).normalized, Is.EqualTo(Vector2.down).Using(Vector3EqualityComparer.Instance));
        }


        [UnityTest]
        public IEnumerator Snake_ReachedBottomWall_WrapsAround()
        {
            inputTestFixture.Press(keyboard.downArrowKey);
            yield return new WaitForSeconds(0.5f);
            inputTestFixture.Release(keyboard.downArrowKey);

            Vector2 originalPosition = snakes.GetHeadPosition();

            yield return new WaitForSeconds(3f);

            Assert.That((snakes.GetHeadPosition() - originalPosition).normalized, Is.EqualTo(Vector2.up).Using(Vector3EqualityComparer.Instance));
        }



        [TearDown]
        public void TearDown()
        {
            inputTestFixture.TearDown();
            SceneManager.UnloadSceneAsync("SnakeMPC2");
        }

    }

}

