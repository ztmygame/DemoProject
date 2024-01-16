using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public Coroutine Run(DialogueDefination dialogue, TMP_Text text_label, float type_speed)
    {
        return StartCoroutine(TypeText(dialogue, text_label, type_speed));
    }

    private IEnumerator TypeText(DialogueDefination dialogue, TMP_Text text_label, float type_speed)
    {
        text_label.text = string.Empty;

        float time = 0f;
        int index = 0;

        string text = dialogue.m_text;

        while(index < text.Length)
        {
            time += (Time.deltaTime * type_speed);
            index = Mathf.Clamp(Mathf.FloorToInt(time), 0, text.Length);

            text_label.text = text[..index];

            yield return null;
        }

        text_label.text = text;
    }
}
