using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// holds snowball logic
    /// </summary>
    public class Snowball : MonoBehaviour
    {
        private Vector3 shootDirection;
        [SerializeField]
        private float shootSpeed = 5f;
        private Transform target;

        /// <summary>
        /// Setup function, selects target and calls the updateshootdirection.
        /// </summary>
        /// <param name="target">The target</param>

        public void Setup(Transform target)
        {
            this.target = target;
            UpdateShootDirection();
            Invoke("DestroySnowball", 7f); // destroy snowball after 7 seconds if it didn't hit an enemy
        }

        /// <summary>
        /// Destroys the snowball object
        /// </summary>
        private void DestroySnowball()
        {
            if (gameObject != null)
            {
                TowerDefenseManager.Instance.enemiesUnderAttack.Remove(target.gameObject);
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Updates the shoot direction to the target's position
        /// </summary>
        private void UpdateShootDirection()
        {
            if (target != null)
            {
                
                if(target.tag == "Endboss")
                {
                    Debug.Log("corrected position");
                    Vector3 targetPositionCorrected = new Vector3(target.position.x, target.position.y + 1f, target.position.z);
                    this.shootDirection = (targetPositionCorrected +  - transform.position).normalized;
                }
                else
                {
                    this.shootDirection = (target.position - transform.position).normalized;
                }
                transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(shootDirection) + 180);
            }
        }

        /// <summary>
        /// Update function that corrects the direction of the snowball
        /// </summary>
        private void Update()
        {
            UpdateShootDirection();
            transform.position += shootDirection * Time.deltaTime * shootSpeed;
        }

        /// <summary>
        /// Converts Vector2 to degrees
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private float GetAngleFromVector(Vector2 dir)
        {
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) { n += 360; }
            return n;
        }

        /// <summary>
        /// Collider function, that will destroy enemies and will deal damage to the endboss
        /// </summary>
        /// <param name="collider">Collider gameobject</param>
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider != null)
            {
                if (collider.GetComponent<Target>() != null && collider.transform == target)
                {
                    if (collider.tag == "Endboss") //endboss damage
                    {
                        GameObject.Find("Endboss Health").GetComponent<EndbossHealth>().takeDamage();
                        TowerDefenseManager.Instance.enemiesUnderAttack.Remove(collider.gameObject);
                        Destroy(gameObject);
                    }
                    else
                    {
                        Destroy(gameObject);
                        TowerDefenseManager.Instance.enemiesUnderAttack.Remove(collider.gameObject);
                        TowerDefenseManager.Instance.enemies.Remove(collider.gameObject);
                        Destroy(collider.gameObject);

                    }

                }
            }
        }

    }
}
