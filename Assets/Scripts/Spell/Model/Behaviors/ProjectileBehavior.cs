using System.Collections;
using UnityEngine;

namespace Spell.Model.Behaviors
{
    public class ProjectileBehavior : SpellBehaviorBase
    {
        /// <summary>
        /// Speed of the projectile
        /// </summary>
        private const float Speed = 30f;

        /// <summary>
        /// Duration of the projectile's flight
        /// </summary>
        private const int Duration = 3;

        public override void Activate(Vector3 targetPosition, Transform caster)
        {
            // Calculate the direction and distance to the target
            var direction = (targetPosition - transform.position).normalized;

            // Move the projectile towards the target
            StartCoroutine(MoveProjectile(direction));
        }

        private IEnumerator MoveProjectile(Vector3 direction)
        {
            float elapsedTime = 0f;

            while (elapsedTime < Duration)
            {
                transform.position += direction * (Speed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Destroy the projectile after the duration
            Destroy(gameObject);
        }
    }
}