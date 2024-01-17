using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConversationSO", menuName = "GameData/Conversation")]
public class Conversation : ScriptableObject
{
    public List<Dialogue> m_dialogues;  // appear in sequence

    public List<Response> m_responses;  // appear after the last dialogue ends

    public bool HasInvalidResponse => (m_responses.Count > 0 && m_responses.FindAll(response => response.m_text != string.Empty).Count > 0);
}
