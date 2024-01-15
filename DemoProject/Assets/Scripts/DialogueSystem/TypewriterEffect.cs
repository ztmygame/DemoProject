using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField]
    private uint m_type_speed = 30;

    public void Run(string text, TMP_Text text_label)
    {
        StartCoroutine(TypeText(text, text_label));
    }

    private IEnumerator TypeText(string text, TMP_Text text_label)
    {
        text_label.text = string.Empty;

        yield return new WaitForSeconds(1.5f);

        float time = 0f;
        int index = 0;

        while(index < text.Length)
        {
            time += (Time.deltaTime * m_type_speed);
            index = Mathf.Clamp(Mathf.FloorToInt(time), 0, text.Length);

            text_label.text = text[..index];

            yield return null;
        }

        text_label.text = text;
    }

}
