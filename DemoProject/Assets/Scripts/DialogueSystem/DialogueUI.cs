using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private DialogueBox m_dialogue_box;

    public bool m_is_showing { get; private set; }

    private ResponseUI m_response_handler;

    private Conversation m_current_conversation;

    private bool m_can_show_next_text;
    private bool m_next_text_force_fadein;

    private void Start()
    {
        m_response_handler = GetComponentInChildren<ResponseUI>();
    }

    public void StartConversation(Conversation conversation)
    {
        m_current_conversation = conversation;

        m_dialogue_box.SetFirstDialogue(m_current_conversation.m_dialogues.First());   // for showing first speaker's name and avatar
        OpenDialogueBox(ShowDialogueText);
    }

    public void ShowDialogueText()
    {
        StartCoroutine(StepThroughDialogueDataList(m_current_conversation));
    }

    private IEnumerator StepThroughDialogueDataList(Conversation conversation)
    {
        m_next_text_force_fadein = false;

        for (int i = 0; i < conversation.m_dialogues.Count; ++i)
        {
            m_can_show_next_text = false;

            Dialogue dialogue = conversation.m_dialogues[i];

            yield return m_dialogue_box.GetComponent<DialogueBox>().ShowText(dialogue, false, m_next_text_force_fadein);

            // show response boxes for the last dialogue instead of waiting for key down 
            if (i == conversation.m_dialogues.Count - 1 && conversation.HasInvalidResponse)
            {
                break;
            }

            yield return new WaitUntil(() => m_can_show_next_text == true);
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

    public void OpenDialogueBox(Action open_box_callback)
    {
        m_is_showing = true;
        m_dialogue_box.SetNextTextStatusAction(NextTextStatus);
        m_dialogue_box.Open(open_box_callback);
    }

    public void CloseDialogueBox()
    {
        m_is_showing = false;
        m_dialogue_box.Close(null);
    }

    public void NextTextStatus(bool next_text_force_fadein)
    {
        m_can_show_next_text = true;
        m_next_text_force_fadein = next_text_force_fadein;
    }
}
