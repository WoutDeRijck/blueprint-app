using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using NUnit.Framework;

namespace GameTests
{

    public class PlayerMovement
    {
        private InputTestFixture inputTestFixture = new InputTestFixture();
        private Keyboard keyboard;
        private GameObject player;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            inputTestFixture.Setup();
            SceneManager.LoadScene("Lobby");
            yield return null;
            
            keyboard = InputSystem.AddDevice<Keyboard>();
            player = GameObject.FindGameObjectWithTag("Player");
        }

        [UnityTest]
        public IEnumerator Player_ArrowKeys_Move()
        {
            var originalPosition = player.transform.position;

            inputTestFixture.Press(keyboard.leftArrowKey);
            yield return new WaitForSeconds(1.5f);
            inputTestFixture.Release(keyboard.leftArrowKey);
            Assert.That((player.transform.position - originalPosition).normalized, Is.EqualTo(Vector3.left).Using(new Vector3EqualityComparer(0.1f)));
            player.transform.position = originalPosition;

            yield return null;

            inputTestFixture.Press(keyboard.rightArrowKey);
            yield return new WaitForSeconds(1.5f);
            inputTestFixture.Release(keyboard.rightArrowKey);
            Assert.That((player.transform.position - originalPosition).normalized, Is.EqualTo(Vector3.right).Using(new Vector3EqualityComparer(0.1f)));
            player.transform.position = originalPosition;

            yield return null;

            inputTestFixture.Press(keyboard.upArrowKey);
            yield return new WaitForSeconds(1.5f);
            inputTestFixture.Release(keyboard.upArrowKey);
            Assert.That((player.transform.position - originalPosition).normalized, Is.EqualTo(Vector3.up).Using(new Vector3EqualityComparer(0.1f)));
            player.transform.position = originalPosition;

            yield return null;

            inputTestFixture.Press(keyboard.downArrowKey);
            yield return new WaitForSeconds(1.5f);
            inputTestFixture.Release(keyboard.downArrowKey);
            Assert.That((player.transform.position - originalPosition).normalized, Is.EqualTo(Vector3.down).Using(new Vector3EqualityComparer(0.1f)));
        }

        [UnityTest]
        public IEnumerator Player_literalKeysaZeRtY_NoMovement()
        {
            var originalPosition = player.transform.position;

            inputTestFixture.Press(keyboard.aKey);
            yield return new WaitForSeconds(1f);
            inputTestFixture.Release(keyboard.aKey);
            Assert.That((player.transform.position - originalPosition).normalized, Is.EqualTo(Vector3.zero));
            player.transform.position = originalPosition;

            yield return null;

            inputTestFixture.Press(keyboard.shiftKey);
            inputTestFixture.Press(keyboard.zKey);
            yield return new WaitForSeconds(1f);
            inputTestFixture.Release(keyboard.zKey);
            inputTestFixture.Release(keyboard.shiftKey);
            Assert.That((player.transform.position - originalPosition).normalized, Is.EqualTo(Vector3.zero));
            player.transform.position = originalPosition;

            yield return null;

            inputTestFixture.Press(keyboard.eKey);
            yield return new WaitForSeconds(1f);
            inputTestFixture.Release(keyboard.eKey);
            Assert.That((player.transform.position - originalPosition).normalized, Is.EqualTo(Vector3.zero));
            player.transform.position = originalPosition;

            yield return null;

            inputTestFixture.Press(keyboard.shiftKey);
            inputTestFixture.Press(keyboard.rKey);
            yield return new WaitForSeconds(1f);
            inputTestFixture.Release(keyboard.rKey);
            inputTestFixture.Release(keyboard.shiftKey);
            Assert.That((player.transform.position - originalPosition).normalized, Is.EqualTo(Vector3.zero));
            player.transform.position = originalPosition;

            yield return null;

            inputTestFixture.Press(keyboard.tKey);
            yield return new WaitForSeconds(1f);
            inputTestFixture.Release(keyboard.tKey);
            Assert.That((player.transform.position - originalPosition).normalized, Is.EqualTo(Vector3.zero));
            player.transform.position = originalPosition;

            yield return null;

            inputTestFixture.Press(keyboard.shiftKey);
            inputTestFixture.Press(keyboard.yKey);
            yield return new WaitForSeconds(1f);
            inputTestFixture.Release(keyboard.yKey);
            inputTestFixture.Release(keyboard.shiftKey);
            Assert.That((player.transform.position - originalPosition).normalized, Is.EqualTo(Vector3.zero));
        }

        [TearDown]
        public void TearDown()
        {
            inputTestFixture.TearDown();
        }

    }

}
