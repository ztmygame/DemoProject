using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private DialogueBox m_dialogue_box;
    [SerializeField] private ResponseUI m_response_handler;

    public static bool m_is_showing { get; private set; }

    private Conversation m_current_conversation;

    private static bool m_auto;

    public static bool m_can_show_next_text;
    public static bool m_next_text_force_fadein;

    private static Selectable m_current_selectable;
    public static void SetCurrentSelectable(Selectable selectable)
    {
        m_current_selectable = selectable;
    }

    private void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == null)
        {
            if(m_current_selectable != null)
            {
                m_current_selectable.Select();
            }
        }
    }


    private void Start()
    {
        HideSelectCursor();
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
            CloseDialogueBox(null);
        }
    }

    public void OpenDialogueBox(Action open_box_callback)
    {
        m_is_showing = true;
        m_dialogue_box.SetFirstDialogue(m_current_conversation.m_dialogues.First());   // for showing first speaker's name and avatar
        m_dialogue_box.SetNextTextStatusAction(NextTextStatus);
        m_dialogue_box.Open(open_box_callback);
    }

    public static void CloseDialogueBox(Action close_callback)
    {
        m_is_showing = false;
        s_instance.m_dialogue_box.Close(close_callback);
    }

    public void NextTextStatus(bool next_text_force_fadein)
    {
        m_can_show_next_text = true;
        m_next_text_force_fadein = next_text_force_fadein;
    }

    /// <summary>
    /// 
    /// </summary>
    private static DialogueUI s_instance;
    private void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        s_instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public static DialogueUI GetInstance() => s_instance;

    public static void OpenDialogueBox(Action open_box_callback, Dialogue first_dialogue)
    {
        m_is_showing = true;
        s_instance.m_dialogue_box.SetFirstDialogue(first_dialogue);   // for showing first speaker's name and avatar
        s_instance.m_dialogue_box.SetNextTextStatusAction(s_instance.NextTextStatus);
        s_instance.m_dialogue_box.Open(open_box_callback);
    }

    public static IEnumerator StepThroughDialogueDataList(List<Dialogue> dialogues, Action<bool> on_finished)
    {
        m_next_text_force_fadein = false;
        for (int i = 0; i < dialogues.Count; ++i)
        {
            m_can_show_next_text = false;

            Dialogue dialogue = dialogues[i];

            yield return s_instance.m_dialogue_box.GetComponent<DialogueBox>().ShowText(dialogue, false, m_next_text_force_fadein);

            // show response boxes for the last dialogue instead of waiting for key down 
            if (i == dialogues.Count - 1)
            {
                break;
            }

            yield return new WaitUntil(() => m_can_show_next_text == true);
        }

        on_finished?.Invoke(true);
    }

    public static IEnumerator ShowDialogue(Dialogue dialogue)
    {
        yield return s_instance.m_dialogue_box.GetComponent<DialogueBox>().ShowText(dialogue, m_auto, m_next_text_force_fadein);
    }

    public static bool CanShowNextDialogue() => m_can_show_next_text;

    #region select cursor

    [SerializeField] private RectTransform m_select_cursor_transform;
    public static void SetSelectedCursorPosition(Vector3 position)
    {
        if(!s_instance.m_select_cursor_transform.gameObject.activeSelf)
        {
            s_instance.m_select_cursor_transform.gameObject.SetActive(true);
        }

        s_instance.m_select_cursor_transform.position = position;
    }

    public void PlaySelectCursorAnimation()
    {
        s_instance.m_select_cursor_transform.GetComponent<Animator>().SetTrigger(GameProperties.m_click_button_animation_hash);
    }

    public static void HideSelectCursor()
    {
        s_instance.m_select_cursor_transform.gameObject.SetActive(false);
    }

    #endregion select cursor

    #region responses
    public static void CreateResponseButtons(List<Response> responses, Action<int> confirm_callback, int default_select_index)
    {
        int index = 0;
        foreach(Response response in responses)
        {
            if (response != null && !string.IsNullOrEmpty(response.m_text))
            {
                s_instance.m_response_handler.AddResponseButton(response, index++, confirm_callback);
            }
        }

        s_instance.m_response_handler.Open(default_select_index);
    }

    #endregion responses
}
