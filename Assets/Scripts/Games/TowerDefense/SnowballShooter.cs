using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// Shoots snowballs at enemies
    /// </summary>
    public class SnowballShooter : MonoBehaviour
    {
        [SerializeField]
        private Transform snowBall;
        private Vector3 startPosition;
        private float nextShootTime = 0.0f;
        [SerializeField]
        private float period = 1f;
        private Transform snowballTransform;

        private void Awake()
        {
            float x = this.transform.parent.transform.position.x;
            float y = this.transform.parent.transform.position.y + 0.5f;
            startPosition = new Vector3(x, y);
        }

        /// <summary>
        /// Returns a list of at most n enemy transforms, that are close to the tower
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private List<Transform> GetCloseEnemies(int n)
        {
            if (TowerDefenseManager.Instance.enemies.Count == 0) { return null; }
            List<Transform> bestTargets = Enumerable.Repeat<Transform>(null, n).ToList();
            List<float> distances = Enumerable.Repeat<float>(Mathf.Infinity, n).ToList();

            foreach (GameObject potentialTarget in TowerDefenseManager.Instance.enemies)
            {
                if (potentialTarget.tag == "Endboss")
                {
                    Vector3 directionToTarget = potentialTarget.transform.position - startPosition;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < distances.Max())
                    {
                        int index = distances.IndexOf(distances.Max());
                        distances[index] = dSqrToTarget;
                        bestTargets[index] = potentialTarget.transform;
                    }
                }
                else if (potentialTarget != null && !TowerDefenseManager.Instance.enemiesUnderAttack.Contains(potentialTarget))
                {
                    Vector3 directionToTarget = potentialTarget.transform.position - startPosition;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < distances.Max())
                    {
                        int index = distances.IndexOf(distances.Max());
                        distances[index] = dSqrToTarget;
                        bestTargets[index] = potentialTarget.transform;
                    }
                }
            }

            List<Transform> bestTargetsStripped = new List<Transform>();

            for (int i = 0; i < bestTargets.Count; i++)
            {
                if (bestTargets[i] != null)
                {
                    bestTargetsStripped.Add(bestTargets[i]);
                }
            }

            return bestTargetsStripped;
        }

        /// <summary>
        /// Update fuction that will shoot enemies if they are close.
        /// </summary>
        private void Update()
        {
            if (TimeToAttack())
            {
                List<Transform> enemies = GetCloseEnemies(HowManySnowballsToAttack());
                if (enemies != null)
                {
                    foreach (Transform enemy in enemies)
                    {
                        if (enemy != null)
                        {
                            Shoot(enemy);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether it is time to attack
        /// </summary>
        /// <returns></returns>
        private bool TimeToAttack()
        {

            int level = GameObject.Find("TowerUpgrade").GetComponent<TowerUpgrade>().GetUpgradeLevel();
            period = 6f - (float)level;

            if (Time.time > nextShootTime)
            {
                nextShootTime += period;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the number of snowballs the tower should shoot
        /// </summary>
        /// <returns></returns>
        private int HowManySnowballsToAttack()
        {
            if (GameObject.Find("TowerUpgrade").GetComponent<TowerUpgrade>().GetUpgradeLevel() > 3)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Shoots a snowball at target
        /// </summary>
        /// <param name="target"></param>
        private void Shoot(Transform target)
        {
            TowerDefenseManager.Instance.enemiesUnderAttack.Add(target.gameObject);
            snowballTransform = Instantiate(snowBall, startPosition, Quaternion.identity);
            snowballTransform.GetComponent<Snowball>().Setup(target);
        }
    }
}