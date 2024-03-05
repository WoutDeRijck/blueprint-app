using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using NUnit.Framework;
using System;
using System.Linq;

namespace GameTests
{

    public class InputController
    {

        private GameObject gameHandler;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            SceneManager.LoadScene("RankRumble");
            yield return null;

            gameHandler = GameObject.FindGameObjectWithTag("GameHandler");

        }

        [UnityTest]
        public IEnumerator Test_Input()
        {

            gameHandler.GetComponent<RankRumble.GameHandler>().ProcessAnswer("left");
            gameHandler.GetComponent<RankRumble.GameHandler>().ProcessAnswer("right");
            gameHandler.GetComponent<RankRumble.GameHandler>().ProcessAnswer("top");
            gameHandler.GetComponent<RankRumble.GameHandler>().ProcessAnswer("bottom");
            yield return null;

            Assert.IsTrue(gameHandler.GetComponent<RankRumble.GameHandler>() != null);

            Assert.AreEqual(gameHandler.GetComponent<RankRumble.GameHandler>().answers[0], gameHandler.GetComponent<RankRumble.GameHandler>().leftValue);
            Assert.AreEqual(gameHandler.GetComponent<RankRumble.GameHandler>().answers[1], gameHandler.GetComponent<RankRumble.GameHandler>().rightValue);
            Assert.AreEqual(gameHandler.GetComponent<RankRumble.GameHandler>().answers[2], gameHandler.GetComponent<RankRumble.GameHandler>().topValue);
            Assert.AreEqual(gameHandler.GetComponent<RankRumble.GameHandler>().answers[3], gameHandler.GetComponent<RankRumble.GameHandler>().bottomValue);

        }

        [UnityTest]
        public IEnumerator Test_Answers()
        {

            GameObject data = GameObject.FindGameObjectWithTag("Data");
            int initialStreak = gameHandler.GetComponent<RankRumble.GameHandler>().streak;

            gameHandler.GetComponent<RankRumble.GameHandler>().ProcessAnswer("left");
            gameHandler.GetComponent<RankRumble.GameHandler>().ProcessAnswer("right");
            gameHandler.GetComponent<RankRumble.GameHandler>().ProcessAnswer("top");
            gameHandler.GetComponent<RankRumble.GameHandler>().ProcessAnswer("bottom");
            yield return null;

            Assert.IsTrue(gameHandler.GetComponent<RankRumble.GameHandler>() != null);
            Assert.IsFalse(data.GetComponent<RankRumble.MockData>().minigame.correctRankings[0] == null);
            
            Assert.IsTrue(Enumerable.SequenceEqual(gameHandler.GetComponent<RankRumble.GameHandler>().answers, data.GetComponent<RankRumble.MockData>().minigame.correctRankings[0]));
            yield return new WaitForSeconds(1.5f);
            Assert.AreEqual(gameHandler.GetComponent<RankRumble.GameHandler>().streak, initialStreak + 1);

            gameHandler.GetComponent<RankRumble.GameHandler>().LoadNextQuestion();

            gameHandler.GetComponent<RankRumble.GameHandler>().ProcessAnswer("left");
            gameHandler.GetComponent<RankRumble.GameHandler>().ProcessAnswer("right");
            gameHandler.GetComponent<RankRumble.GameHandler>().ProcessAnswer("bottom");
            gameHandler.GetComponent<RankRumble.GameHandler>().ProcessAnswer("top");
            yield return new WaitForSeconds(1.5f);

            Assert.IsFalse(Enumerable.SequenceEqual(gameHandler.GetComponent<RankRumble.GameHandler>().answers, data.GetComponent<RankRumble.MockData>().minigame.correctRankings[1]));
            Assert.AreEqual(gameHandler.GetComponent<RankRumble.GameHandler>().streak, 0);

        }

        [TearDown]
        public void TearDown()
        {
            
        }

    }
}