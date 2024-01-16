using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_text_label;

    [SerializeField]
    private DialogueDataListSO m_dialogue_data;

    [SerializeField]
    private GameObject m_dialogue_box;

    private TypewriterEffect m_typewriter_effect;

    [SerializeField]
    private uint m_type_speed = 30;
    [SerializeField]
    private float m_wait_time = 0.0f;
    [SerializeField]
    private KeyCode m_continue_key = KeyCode.Space;

    private void Start()
    {
        m_typewriter_effect = GetComponent<TypewriterEffect>();
        m_text_label.text = string.Empty;

        ShowDialogue(m_dialogue_data);
    }

    public void ShowDialogue(DialogueDataListSO dialogue_data)
    {
        StartCoroutine(StepThroughDialogueDataList(dialogue_data));
    }

    private IEnumerator StepThroughDialogueDataList(DialogueDataListSO dialogue_data)
    {
        foreach(DialogueDefination dialogue in dialogue_data.m_dialogue_list)
        {
            yield return new WaitForSeconds(m_wait_time);
            yield return m_typewriter_effect.Run(dialogue, m_text_label, m_type_speed);
            yield return new WaitUntil(() => Input.GetKeyDown(m_continue_key));
        }

        CloseDialogueBox();
    }

    private void CloseDialogueBox()
    {
        m_dialogue_box.SetActive(false);
        m_text_label.text = string.Empty;
    }
}
