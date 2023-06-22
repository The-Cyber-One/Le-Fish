using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class SpeechBubble : Singleton<SpeechBubble>
{
    public int DialogIndex => _dialogIndex;

    [SerializeField] float typeSpeed = 0.05f;
    [SerializeField] Transform camera;
    [SerializeField] float smoothing = 0.1f;

    [SerializeField] int _dialogIndex;
    TextMeshPro _textMeshPro;
    bool _playNextText = true;
    Vector3 _cameraOffset;

    Coroutine _coroutine;
    WaitForSeconds _waitForChar;
    WaitUntil _waitForPlayNextText;

    private void OnValidate()
    {
        _waitForChar = new WaitForSeconds(typeSpeed);
        _waitForPlayNextText = new WaitUntil(() => _playNextText);
    }

    protected override void Awake()
    {
        base.Awake();
        _textMeshPro = _textMeshPro != null ? _textMeshPro : GetComponent<TextMeshPro>();
        _textMeshPro.text = string.Empty;
    }

    private void Update()
    {
        Vector3 textDirection = Vector3.Lerp(transform.parent.forward, camera.forward, smoothing);
        textDirection.y = 0;
        transform.parent.SetPositionAndRotation(Vector3.Lerp(transform.parent.position, camera.position, smoothing), Quaternion.LookRotation(textDirection));
    }

    public delegate void DialogCallBack();

    public void ShowDialog(Dialog dialog, string speakerIconName = "", DialogCallBack callBack = null)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(C_ShowDialog(dialog, callBack, speakerIconName));
    }

    private IEnumerator C_ShowDialog(Dialog dialog, DialogCallBack callBack, string speakerIconName = "")
    {
        _textMeshPro.text = string.Empty;

        string iconTag = string.IsNullOrEmpty(speakerIconName) ? "" : $"<sprite name=\"{speakerIconName}\">";

        for (_dialogIndex = 0; _dialogIndex < dialog.Length; _dialogIndex++)
        {
            Dialog.DialogText dialogText = dialog[_dialogIndex];
            _playNextText = !dialogText.WaitForTrigger;
            yield return _waitForPlayNextText;

            var matches = Regex.Matches(dialogText.Content, @"<[^<]*>"); // Find tags that are contained with <> so that these tags will be typed all at once
            List<string> textSections = new();
            Queue tagIndecies = new();
            for (int textIndex = 0; textIndex < dialogText.Content.Length; textIndex++)
            {
                Match match = matches.FirstOrDefault(position => position.Index == textIndex);
                if (match is not null)
                {
                    textSections.Add(match.Value);
                    tagIndecies.Enqueue(textSections.Count - 1);
                    textIndex += match.Length - 1;
                }
                else
                {
                    textSections.Add(dialogText.Content[textIndex].ToString());
                }
            }


            for (int i = 0; i < textSections.Count; i++)
            {
                _textMeshPro.text = iconTag + string.Join("", textSections.Take(i + 1)) + "<color=#fff0>" + string.Join("", textSections.TakeLast(textSections.Count - 1 - i));
                yield return _waitForChar;
            }

            yield return new WaitForSeconds(dialogText.DisplayTime);

            _textMeshPro.text = string.Empty;
            yield return new WaitForSeconds(dialogText.EmptyTime);
        }

        callBack?.Invoke();
    }

    [ContextMenu(nameof(PlayNextText))]
    public void PlayNextText() => _playNextText = true;
}