using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    /// <summary>
    /// Controls the health of the end boss healthbar UI.
    /// </summary>
    public class EndbossUIhealth : MonoBehaviour
    {
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private Gradient gradient;
        [SerializeField]
        private Image fill;

        /// <summary>
        /// Sets the maximum health value for the end boss UI.
        /// </summary>
        /// <param name="health">The maximum health value.</param>
        public void SetMaxHealth(int health)
        {
            slider.maxValue = health;
            slider.value = health;
            fill.color = gradient.Evaluate(1f);
        }

        /// <summary>
        /// Updates the current health value on the end boss UI.
        /// </summary>
        /// <param name="prevHealth">The previous health value.</param>
        /// <param name="newHealth">The new health value.</param>
        public void SetHealth(int prevHealth, int newHealth)
        {
            slider.value = newHealth;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }

        /// <summary>
        /// Updates the fill color of the health bar on each frame.
        /// </summary>
        public void Update()
        {
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }

}