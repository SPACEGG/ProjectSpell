using System;
using System.Collections.Generic;
using Spell.Model.Behaviors;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Spell.Model.Core
{
    public static class FailureSpellFactory
    {
        private static readonly Dictionary<string, Transform> ParticleCache = new();

        public static GameObject CreateSpellGameObject(Vector3 position)
        {
            var gameObject = new GameObject($"[Spell Failure]")
            {
                tag = "Player",
                layer = LayerMask.NameToLayer("TemporaryProjectile"),
                transform =
                {
                    position = position
                },
            };
            gameObject.AddComponent<FailureBehavior>();

            ApplyVFX(gameObject);

            return gameObject;
        }

        private static void ApplyVFX(GameObject gameObject)
        {
            var particlePrefab = GetRandomParticle();
            if (particlePrefab)
            {
                var particleInstance = Object.Instantiate(particlePrefab, gameObject.transform);
                ConfigureParticleSystem(particleInstance);
            }

            var audioClip = Resources.Load<AudioClip>("VFX/Sound/_tang");
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.playOnAwake = true;
            audioSource.spatialBlend = 1.0f;
            audioSource.Play();
        }

        private static Transform GetRandomParticle()
        {
            var randomIndex = Random.Range(0, 3);
            string particleName = randomIndex switch
            {
                0 => "Fire",
                1 => "Hit",
                _ => "Spark1",
            };

            string fullPath = $"VFX/Particles/{particleName}";

            if (ParticleCache.TryGetValue(fullPath, out var cachedPrefab))
            {
                return cachedPrefab;
            }

            var prefab = Resources.Load<Transform>(fullPath);
            ParticleCache[fullPath] = prefab;
            return prefab;
        }

        private static void ConfigureParticleSystem(Transform particleTransform)
        {
            var particleSystem = particleTransform.GetComponent<ParticleSystem>();
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = particleSystem.main;
            main.playOnAwake = true;
            main.loop = false;
            main.duration = 1f;
            main.startLifetime = 1f;
            main.startSpeed = 5f;
            main.startSize = 0.5f;

            particleSystem.Play();
        }
    }
}