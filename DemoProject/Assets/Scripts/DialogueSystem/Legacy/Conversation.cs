/*
 * [legacy]
 * Now all dialogues and responses are defined as nodes, and uniformly executed using sequential executors, which brings better flexibility.
 */

/*
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "[legacy] Conversation", menuName = "GameData/Conversation")]
public class Conversation : ScriptableObject
{
    public List<Dialogue> m_dialogues;  // appear in sequence

    public List<Response> m_responses;  // appear after the last dialogue ends

    public bool HasInvalidResponse => (m_responses.Count > 0 && m_responses.FindAll(response => response.m_text != string.Empty).Count > 0);
}
*/
