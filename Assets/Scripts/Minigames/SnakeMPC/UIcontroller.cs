using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using GameData;
using GameManagement;

namespace Snake
{

    /// <summary>
    /// Controls Snake UI (question only)
    /// </summary>
    public class UIcontroller : MonoBehaviour
    {
        TextElement text; // question text

        private void OnEnable()
        {
            // fetch text element
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            text = root.Q<TextElement>("question");
        }

        /// <summary>
        /// Displays question at a certain index in the minigame object
        /// </summary>
        public void SetQuestion(int index)
        {
            if (GameManager.Instance != null)
            {
                text.text = ((MultipleChoice)GameManager.Instance.minigame).questions[index];
            }
            else
            {
                text.text = MockData.instance.minigame.questions[index];
            }
        }
    }
}

