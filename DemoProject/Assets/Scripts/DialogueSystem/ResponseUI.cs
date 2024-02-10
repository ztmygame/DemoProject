using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResponseUI : MonoBehaviour
{
    [SerializeField] private RectTransform m_panel_transform;
    [SerializeField] private GameObject m_response_box_template;
    [SerializeField] private DialogueUI m_dialogue_panel;
    private List<GameObject> m_response_boxes;

    private void Start()
    {
        // todo: use prefab
        m_response_box_template.SetActive(false);

        m_response_boxes = new List<GameObject>();
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

            GameObject response_box = Instantiate(m_response_box_template, m_panel_transform);
            response_box.gameObject.SetActive(true);
            response_box.GetComponentInChildren<TMP_Text>().text = response.m_text;
            response_box.GetComponentInChildren<Button>().onClick.AddListener(() => OnResponseSelected(response));

            m_response_boxes.Add(response_box);
        }
    }

    private void OnResponseSelected(Response response)
    {
        m_panel_transform.gameObject.SetActive(false);
        foreach(GameObject box in m_response_boxes)
        {
            Destroy(box);
        }
        m_response_boxes.Clear();

        if (response.m_next_conversation)
        {
            m_dialogue_panel.StartConversation(response.m_next_conversation);
        }
        else
        {
            DialogueUI.CloseDialogueBox(null);
        }
    }
}

