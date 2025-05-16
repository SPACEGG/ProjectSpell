/*
using System.Collections.Generic;
using Spell.Model.Enums;
using UnityEngine;

namespace Entity.Prefabs
{
    [CreateAssetMenu(fileName = "PrefabVisualData", menuName = "Scriptable Objects/Entity/PrefabVisualData")]
    public class PrefabVisualData : ScriptableObject
    {
        public List<PrefabVisual> entries;

        public List<GameObject> GetPrefabList(ElementType element, ShapeType shape, SizeType size)
        {
            foreach (var entry in entries)
            {
                if (entry.Element == element &&
                    entry.shapeType == shape &&
                    entry.sizeCategory == size)
                {
                    return entry.prefabList;
                }
            }
            return null;
        }
    }
}
*/