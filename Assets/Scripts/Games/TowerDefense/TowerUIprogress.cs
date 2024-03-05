using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    /// <summary>
    /// Handles the Tower progress bar UI.
    /// </summary>
    public class TowerUIprogress : MonoBehaviour
    {
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private Gradient gradient;
        [SerializeField]
        private Image fill;

        /// <summary>
        /// Initializes the slider with default values.
        /// </summary>
        public void Start()
        {
            slider.maxValue = 2f;
            slider.value = 0;
            fill.color = gradient.Evaluate(0f);
        }

        /// <summary>
        /// Sets the progress value and updates the UI accordingly.
        /// </summary>
        /// <param name="progress">The progress value.</param>
        public void SetHealth(double progress)
        {
            Debug.Log("Slider UI progress : " + progress);
            slider.value = (float)progress;
            Debug.Log("Slider Normalized progress : " + slider.normalizedValue);
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }

        /// <summary>
        /// Updates the fill color of the progress bar.
        /// </summary>
        public void Update()
        {
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
}