using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    /// <summary>
    /// Controls the castle health UI.
    /// </summary>
    public class CastleUIhealth : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private Gradient gradient;
        [SerializeField]
        private Image fill;

        /// <summary>
        /// Sets the maximum health value and initializes the health slider.
        /// </summary>
        /// <param name="health">The maximum health value.</param>

        public void SetMaxHealth(int health)
        {
            slider.maxValue = health;
            slider.value = health;
            fill.color = gradient.Evaluate(1f);
        }

        /// <summary>
        /// Updates the current health value and adjusts the fill color of the health slider.
        /// </summary>
        /// <param name="prevHealth">The previous health value.</param>
        /// <param name="newHealth">The new health value.</param>

        public void SetHealth(int prevHealth, int newHealth)
        {
            slider.value = newHealth;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }

        /// <summary>
        /// Called once per frame, updates the fill color of the health slider.
        /// </summary>
        public void Update()
        {
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
}
