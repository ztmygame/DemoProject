using UnityEngine;

[System.Serializable]
public class Response
{
    [SerializeField]
    public string m_text;

    [SerializeField]
    public Conversation m_next_dialogue;
}
