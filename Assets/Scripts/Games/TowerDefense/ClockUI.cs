using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    /// <summary>
    /// Controls the clock UI.
    /// </summary>
    public class ClockUI : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField]
        private Image arrowImage;
        [SerializeField]
        private Image fill;
        [SerializeField]
        private Gradient gradient;
        private Vector3 rot;
        private Transform arrow;
        
        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        public void Start()
        {
            arrow = arrowImage.GetComponent<Transform>();
        }

        /// <summary>
        /// Sets the position and rotation of the clock arrow based on the time remaining.
        /// </summary>
        /// <param name="maxseconds">The maximum duration in seconds.</param>
        /// <param name="seconds">The remaining time in seconds.</param>

        public void SetArrow(float maxseconds, float seconds)
        {
            float percent = seconds / maxseconds;
            fill.color = gradient.Evaluate(1 - percent);
            float rotation = 360 * percent;
            rot.z = rotation;
            arrow.eulerAngles = rot;
        }
    }
}