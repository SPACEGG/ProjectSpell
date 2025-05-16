using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Utils
{
    public abstract class RuntimeCollection<T> : ScriptableObject
    {
        public List<T> items = new();

        public event Action<T> ItemAdded;

        public event Action<T> ItemRemoved;

        public void Add(T item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                ItemAdded?.Invoke(item);
            }
        }

        public void Remove(T item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
                ItemRemoved?.Invoke(item);
            }
        }
    }
}
