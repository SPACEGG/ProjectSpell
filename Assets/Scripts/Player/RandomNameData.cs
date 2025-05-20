using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "RandomNameData", menuName = "Scriptable Objects/Common/Random Name Data")]
    public class RandomNameData : ScriptableObject
    {
        [Header("Adjectives")]
        public string[] adjectives = {
            "Angry", "Happy", "Brave", "Clever",
            "Swift", "Mighty", "Gentle", "Wild",
            "Noble", "Fierce", "Calm", "Eager",
            "Bold", "Wise", "Proud", "Lively"
        };

        [Header("Animals")]
        public string[] animals = {
            "Lion", "Dolphin", "Eagle", "Wolf",
            "Tiger", "Bear", "Fox", "Hawk",
            "Dragon", "Phoenix", "Panther", "Falcon",
            "Lynx", "Shark", "Leopard", "Raven"
        };

        public string GetRandomName()
        {
            string randomAdjective = adjectives[Random.Range(0, adjectives.Length)];
            string randomAnimal = animals[Random.Range(0, animals.Length)];
            return $"{randomAdjective} {randomAnimal}";
        }
    }
}