using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ListPropositions", menuName = "ScriptableObjects/ListPropositions Data")]
public class ListPropositions : ScriptableObject
{
    [SerializeField] public List<MyKeyValuePair> listPropositions = new();

    [Serializable]
    public class MyKeyValuePair
    {
        [SerializeField] public string key;
        [SerializeField] public Propositions data;
    }
}
