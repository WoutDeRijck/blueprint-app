using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    /// <summary>
    /// Controls the time.
    /// </summary>
    public class Clock : MonoBehaviour
    {
        
        [SerializeField]
        private float duration = 180;
        [SerializeField]
        private Text timerText;
        [SerializeField]
        private GameObject Enemyspawner;
        [SerializeField]
        private EnemySpawner2 EnemySpawner2;
        [SerializeField]
        private GameObject ClockObj;
        private ClockUI ClockUI;
        private float timeValue;
        private TowerDefenseNetwork network;
        private bool clockActive;
        private int count = 0;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Start()
        {
            GameObject networkObject = GameObject.Find("TowerDefenseNetwork");
            network = networkObject.GetComponent<TowerDefenseNetwork>();
            Enemyspawner = GameObject.Find("EnemySpawner");
            EnemySpawner2 = Enemyspawner.GetComponent<EnemySpawner2>();
            ClockUI = ClockObj.gameObject.GetComponent<ClockUI>();
        }

        /// <summary>
        /// Activates the clock and sets its initial values.
        /// </summary>
        public void activateClock()
        {
            clockActive = true;
            timeValue = duration;
            timerText.text = string.Empty;
        }

        /// <summary>
        /// Called once per frame, updates the clock's time and checks if it has reached zero.
        /// </summary>
        void Update()
        {
            if (clockActive)
            {
                if (timeValue > 0)
                {
                    timeValue -= Time.deltaTime;
                    ClockUI.SetArrow(duration, timeValue);
                }
                else
                {
                    timeValue = 0;
                    clockActive = false;
                    EnemySpawner2.startWave();
                    ClockUI.SetArrow(duration, duration);
                    count += 1;
                }
                //DisplayTime(timeValue);
            }
            else
            {
                timerText.text = string.Empty;
            }
        }

        /// <summary>
        /// Formats and displays the time in minutes and seconds.
        /// </summary>
        /// <param name="timeToDisplay">The time value to display.</param>

        void DisplayTime(float timeToDisplay)
        {
            if (timeToDisplay < 0)
            {
                timeToDisplay = 0;
            }

            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        /// <summary>
        /// returns the total progress of the game
        /// </summary>
        /// <returns></returns>
        public float GetProgress()
        {
            float total_time = 4 * duration;
            float passed_time = count * duration + (duration - timeValue);
            return passed_time / total_time;
        }
    }
}