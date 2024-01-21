using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class DialogueUI : MonoBehaviour
{
    private TMP_Text m_text_label;
    private GameObject m_dialogue_box;

    [SerializeField]
    private FadeEffect m_fade_effect;

    public bool m_is_showing { get; private set; }

    private ResponseUI m_response_handler;

    private Conversation m_current_conversation;

    private void Start()
    {
        m_text_label = GetComponentInChildren<TMP_Text>();
        m_dialogue_box = m_text_label.gameObject.transform.parent.gameObject;

        m_response_handler = GetComponentInChildren<ResponseUI>();

        CloseDialogueBox();
    }

    public void StartConversation(Conversation conversation)
    {
        m_current_conversation = conversation;
        OpenDialogueBox(ShowDialogueText);
    }

    public void ShowDialogueText()
    {
        StartCoroutine(StepThroughDialogueDataList(m_current_conversation));
    }

    private IEnumerator StepThroughDialogueDataList(Conversation conversation)
    {
        for (int i = 0; i < conversation.m_dialogues.Count; ++i)
        {
            Dialogue dialogue = conversation.m_dialogues[i];

            yield return m_text_label.GetComponent<AdvancedTMProUGUI>().ShowText(dialogue);

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

    public void OpenDialogueBox(Action callback)
    {
        m_is_showing = true;
        m_dialogue_box.SetActive(true);
        m_dialogue_box.GetComponent<FadeEffect>()?.Fade(1.0f, GameplaySettings.m_dialogue_box_fadein_duration, callback);
    }

    public void CloseDialogueBox()
    {
        m_is_showing = false;
        m_text_label.text = string.Empty;
        m_dialogue_box.GetComponent<FadeEffect>()?.Fade(0.0f, GameplaySettings.m_dialogue_box_fadein_duration, SetDialogueBoxInactive);
    }

    public void SetDialogueBoxInactive()
    {
        m_dialogue_box.SetActive(false);
    }
}
