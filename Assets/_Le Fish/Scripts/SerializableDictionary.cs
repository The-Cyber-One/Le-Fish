using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        dictionary.Clear();

        for (int i = 0; i < keys.Count; i++)
        {
            dictionary.Add(keys[i], values[i]);
        }
    }

    public TValue this[TKey key]
    {
        get { return dictionary[key]; }
        set { dictionary[key] = value; }
    }

    public void Add(TKey key, TValue value)
    {
        dictionary.Add(key, value);
    }

    public bool Remove(TKey key)
    {
        return dictionary.Remove(key);
    }

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return dictionary.TryGetValue(key, out value);
    }

    public ICollection<TKey> Keys
    {
        get { return dictionary.Keys; }
    }

    public ICollection<TValue> Values
    {
        get { return dictionary.Values; }
    }

    public int Count
    {
        get { return dictionary.Count; }
    }
}

