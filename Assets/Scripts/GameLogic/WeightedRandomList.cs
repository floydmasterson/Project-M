using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class WeightedRandomList<T>
{
    [System.Serializable]
    public struct Pair
    {
  
        [HorizontalGroup("Item"), HideLabel]
        public T item;
        [HorizontalGroup("Item"), LabelWidth(45)]
        public float weight;

        public Pair(T item, float weight)
        {
            this.item = item;
            this.weight = weight;
       
        }
    }
    [LabelText("Random Table")]
    public List<Pair> list = new List<Pair>();

    public int Count
    {
        get => list.Count;
    }

    public void Add(T item, float weight)
    {
        list.Add(new Pair(item, weight));
    }

    public T GetRandom()
    {
        float totalWeight = 0;

        foreach (Pair p in list)
        {
            totalWeight += p.weight;
        }

        float value = Random.value * totalWeight;

        float sumWeight = 0;

        foreach (Pair p in list)
        {
            sumWeight += p.weight;

            if (sumWeight >= value)
            {
                return p.item;
            }
        }

        return default(T);
    }
}