using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    [SerializeField] TextMeshPro textMeshPro;
    [SerializeField] float displaySpeed = 0.5f;
    [SerializeField] Dialog[] dialogs;

    [Serializable]
    public class Dialog
    {
        [SerializeField] string title;
        [SerializeField] public float DisplayTime;
        [SerializeField, TextArea(1,10)] public string Text;
    }

    private IEnumerator Start()
    {
        var waitForChar = new WaitForSeconds(displaySpeed);

        foreach (Dialog dialog in dialogs)
        {
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
        }
    }
}
