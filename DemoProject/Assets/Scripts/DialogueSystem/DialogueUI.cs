using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_text_label;

    [SerializeField]
    private GameObject m_dialogue_box;

    public bool m_is_showing { get; private set; }

    private TypewriterEffect m_typewriter_effect;
    private ResponseUI m_response_handler;

    private void Start()
    {
        m_text_label.text = string.Empty;
        m_typewriter_effect = GetComponent<TypewriterEffect>();
        m_response_handler = GetComponentInChildren<ResponseUI>();

        CloseDialogueBox();
    }

    public void ShowDialogue(Conversation conversation)
    {
        m_is_showing = true;
        m_dialogue_box.SetActive(true);
        StartCoroutine(StepThroughDialogueDataList(conversation));
    }

    private IEnumerator StepThroughDialogueDataList(Conversation conversation)
    {
        for (int i = 0; i < conversation.m_dialogues.Count; ++i)
        {
            Dialogue dialogue = conversation.m_dialogues[i];

            yield return new WaitForSeconds(GameplaySettings.m_wait_time);
            yield return m_typewriter_effect.Run(dialogue, m_text_label, GameplaySettings.m_type_speed);

            // show response boxes for the last dialogue instead of waiting for key down 
            if (i == conversation.m_dialogues.Count - 1 && conversation.HasInvalidResponse)
            {
                break;
            }

            yield return new WaitUntil(() => Input.GetKeyDown(KeycodeSettings.m_continue_key));
        }

        if (conversation.HasInvalidResponse)
        {
            m_response_handler.ShowResponses(conversation.m_responses);
        }
        else
        {
            CloseDialogueBox();
        }
    }

    public void CloseDialogueBox()
    {
        m_is_showing = false;
        m_dialogue_box.SetActive(false);
        m_text_label.text = string.Empty;
    }
}
