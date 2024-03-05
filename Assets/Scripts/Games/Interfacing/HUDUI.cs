using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

namespace TowerDefense
{
    public class HUDUI : MonoBehaviour
    {
        // Start is called before the first frame update

        [SerializeField]
        private TextMeshProUGUI students_activity;

        [SerializeField]
        private TextMeshProUGUI total_progress;

        [SerializeField]
        private TextMeshProUGUI progress_title;
        [SerializeField]
        private TextMeshProUGUI activity_title;

        private List<string> students = new List<string>();

        void Start()
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                GameObject.Find("Canvas HUD").SetActive(false);
            }
        }


        /// <summary>
        /// Change the student activity on the screen
        /// </summary>
        /// <param name="ECS"></param>
        /// <param name="number"></param>
        /// <param name="minigame"></param>
        /// <param name="name"></param>
        /// <param name="all"></param>
        public void ChangeStatus(int ECS, int number, string minigame, string name, double all)
        {
            //change ecs info on index: studentID
            Debug.Log("Changed activity status in the HUD");

            string original_text = students_activity.text;
            if (ECS == 0)
            {
                students_activity.text = name + " heeft nu " + number + " minigame(s) gespeeld. <br>" + original_text;
            }
            else
            {
                students_activity.text = name + " speelt nu " + minigame + " op niveau: " + all.ToString("0.00") + ". <br>" + original_text;
            }
        }

        /// <summary>
        /// Change the progress on the screen
        /// </summary>
        /// <param name="progress"></param>
        public void ChangeProgress(float progress)
        {
            total_progress.text = (Mathf.Round(100*progress)).ToString() + "%";
        }


        public void unloadHUD()
        {
            students_activity.enabled = false;
            total_progress.enabled = false;
            activity_title.enabled = false;
            progress_title.enabled = false;
        }

        public void loadHUD()
        {
            students_activity.enabled = true;
            total_progress.enabled = true;
            activity_title.enabled = true;
            progress_title.enabled = true;
        }

    }

}