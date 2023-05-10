using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/" + nameof(Dialog))]
public class Dialog : ScriptableObject
{
    [SerializeField] DialogText[] texts;

    public int Length => texts.Length;
    public DialogText this[int index] => texts[index];

    [Serializable]
    public class DialogText
    {
#pragma warning disable IDE0052 // Used to set a title in the inspector
        [SerializeField, HideInInspector] string _inspectorTitle; 
#pragma warning restore IDE0052 // Remove unread private members
        [SerializeField] public string Title;
        [SerializeField] public bool WaitForTrigger;
        [SerializeField] public float DisplayTime;
        [SerializeField, TextArea(2, 10)] public string Content;
        [SerializeField] public float EmptyTime;

        public void UpdateInspectorTitle(int index) => _inspectorTitle = $"{index} - {Title}";
    }

    private void OnValidate()
    {
        for (int i = 0; i < texts.Length; i++)
            texts[i].UpdateInspectorTitle(i);
    }
}
