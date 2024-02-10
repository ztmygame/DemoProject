using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FadeEffect))]
public class ResponseUI : MonoBehaviour
{
    [SerializeField] private RectTransform m_panel_transform;
    [SerializeField] private DialogueUI m_dialogue_panel;
    private GameObject m_response_button_prefab;
    private List<AdvancedButton> m_buttons;
    private FadeEffect m_fade_effect;

    private void Start()
    {
        m_buttons = new List<AdvancedButton>();
        m_response_button_prefab = Resources.Load<GameObject>("ResponseButton");
        m_fade_effect = GetComponent<FadeEffect>();
    }

    public void ShowResponses(List<Response> responses)
    {
        m_panel_transform.gameObject.SetActive(true);
        foreach (Response response in responses)
        {
            if(response.m_text == string.Empty)
            {
                continue;
            }

            GameObject response_box = Instantiate(m_response_button_prefab, m_panel_transform);
            response_box.gameObject.SetActive(true);
            response_box.GetComponentInChildren<TMP_Text>().text = response.m_text;
            response_box.GetComponentInChildren<Button>().onClick.AddListener(() => OnResponseSelected(response));

            m_buttons.Add(response_box.GetComponent<AdvancedButton>());
        }
    }

    private void OnResponseSelected(Response response)
    {
        m_panel_transform.gameObject.SetActive(false);
        foreach (AdvancedButton button in m_buttons)
        {
            Destroy(button.gameObject);
        }
        m_buttons.Clear();

        if (response.m_next_conversation)
        {
            m_dialogue_panel.StartConversation(response.m_next_conversation);
        }
        else
        {
            DialogueUI.CloseDialogueBox(null);
        }
    }

    public void AddResponseButton(Response response, int index, Action<int> confirm_callback)
    {
        GameObject button_go = Instantiate(m_response_button_prefab);

        AdvancedButton button = button_go.GetComponent<AdvancedButton>();

        button.gameObject.name = "ResponseButton" + index;
        button.Initialize(response.m_text, index, confirm_callback);

        button.transform.SetParent(transform);
        button.transform.localScale = Vector3.one;

        button.onClick.AddListener(DisableAllButtons);
        button.m_confirmed_action += (_) => { Close(); };

        m_buttons.Add(button);
    }

    private void DisableAllButtons()
    {
        foreach (AdvancedButton button in m_buttons)
        {
            button.enabled = false;
        }
    }

    public void Open(int default_select_index)
    {
        gameObject.SetActive(true);

        m_fade_effect.m_render_opacity = 0.0f;
        m_fade_effect.Fade(1.0f, GameplaySettings.m_response_fade_in_duration, () =>
        {
            if(default_select_index < m_buttons.Count)
            {
                m_buttons[default_select_index].Select();
            }
            else
            {
                m_buttons[0].Select();
            }
        });
    }

    public void Close()
    {
        DialogueUI.SetCurrentSelectable(null);
        m_fade_effect.Fade(0.0f, GameplaySettings.m_response_fade_out_duration, () =>
        {
            foreach (AdvancedButton button in m_buttons)
            {
                Destroy(button.gameObject);
            }
            m_buttons.Clear();
        });

        DialogueUI.HideSelectCursor();
    }
}

