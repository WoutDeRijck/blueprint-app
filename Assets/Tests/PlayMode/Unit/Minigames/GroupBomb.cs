using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using NUnit.Framework;

namespace GameTests
{

    public class GroupBombTests
    {

        private GameObject gameHandler;


        [UnitySetUp]
        public IEnumerator SetUp()
        {
            SceneManager.LoadScene("GroupBomb");
            yield return null;

        }
        

        [UnityTest]
        public IEnumerator Test_Countdown()
        {

            gameHandler = GameObject.FindGameObjectWithTag("GameHandler");

            int initialExplosions = gameHandler.GetComponent<GroupBomb.GameHandler>().explosions;
  
            GameObject bomb = gameHandler.GetComponent<GroupBomb.GameHandler>().GetLevelGrid().SpawnBomb();
            yield return new WaitForSeconds(bomb.GetComponent<GroupBomb.Bomb>().detonationTime);

            Assert.AreEqual(gameHandler.GetComponent<GroupBomb.GameHandler>().explosions, initialExplosions + 1);
            Assert.IsTrue(bomb.GetComponent<GroupBomb.Bomb>().totCountDown > bomb.GetComponent<GroupBomb.Bomb>().detonationTime);
        }

        [UnityTest]
        public IEnumerator Test_Answer()
        {
            gameHandler = GameObject.FindGameObjectWithTag("GameHandler");

            GameObject bomb1 = gameHandler.GetComponent<GroupBomb.GameHandler>().GetLevelGrid().SpawnBomb();

            GameObject bomb2 = gameHandler.GetComponent<GroupBomb.GameHandler>().GetLevelGrid().SpawnBomb();

            gameHandler.GetComponent<GroupBomb.GameHandler>().CheckAnswer(0, bomb1);
            yield return null;

            gameHandler.GetComponent<GroupBomb.GameHandler>().CheckAnswer(1, bomb2);
            yield return null;

            Assert.IsFalse(gameHandler.GetComponent<GroupBomb.GameHandler>() == null);
            
            Assert.IsTrue(bomb1.GetComponent<GroupBomb.Bomb>().state == GroupBomb.Bomb.State.stored);
            Assert.IsTrue(bomb2 == null);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_Pressed_Down()
        {

            gameHandler = GameObject.FindGameObjectWithTag("GameHandler");

            GameObject bomb = gameHandler.GetComponent<GroupBomb.GameHandler>().GetLevelGrid().SpawnBomb();

            Assert.IsFalse(bomb == null);

            Transform initial = bomb.transform;
            float intitialTime = bomb.GetComponent<GroupBomb.Bomb>().totCountDown;

            bomb.GetComponent<GroupBomb.Bomb>().state = GroupBomb.Bomb.State.clickedOn;
            yield return new WaitForSeconds(1.5f);

            Assert.IsTrue(bomb.transform.position == initial.position);
            Assert.IsTrue(bomb.GetComponent<GroupBomb.Bomb>().totCountDown == intitialTime);

        }

    }

}