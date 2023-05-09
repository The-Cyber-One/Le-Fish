using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    [SerializeField] TextMeshPro textMeshPro;
    [SerializeField] float displaySpeed = 0.5f;
    [SerializeField] int currentDialogIndex = 0;
    [SerializeField] Dialog[] dialogs;

    [Serializable]
    public class Dialog
    {
        [SerializeField, HideInInspector] string _inspectorTitle; //Used to set a title in the inspector
        [SerializeField] public string Title;
        [SerializeField] public float DisplayTime;
        [SerializeField, TextArea(1, 10)] public string Text;
        [SerializeField] public float EmptyTime;

        public void UpdateInspectorTitle(int index) => _inspectorTitle = $"{index} - {Title}";
    }

    private void OnValidate()
    {
        for (int i = 0; i < dialogs.Length; i++)
            dialogs[i].UpdateInspectorTitle(i);
    }

    private IEnumerator Start()
    {
        var waitForChar = new WaitForSeconds(displaySpeed);

        for (; currentDialogIndex < dialogs.Length; currentDialogIndex++)
        {
            Dialog dialog = dialogs[currentDialogIndex];
            textMeshPro.text = string.Empty;
            var matches = Regex.Matches(dialog.Text, @"<[^<]*>"); // Find tags that are contained with <> so that these tags will be typed all at once
            List<string> text = new();
            for (int i = 0; i < dialog.Text.Length; i++)
            {
                Match match = matches.FirstOrDefault(position => position.Index == i);
                if (match is not null)
                {
                    text.Add(match.Value);
                    i += match.Length - 1;
                }
                else
                {
                    text.Add(dialog.Text[i].ToString());
                }
            }

            foreach (string textSection in text)
            {
                textMeshPro.text += textSection;
                yield return waitForChar;
            }

            yield return new WaitForSeconds(dialog.DisplayTime);

            textMeshPro.text = "";
            yield return new WaitForSeconds(dialog.EmptyTime);
        }
    }
}