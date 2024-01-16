using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueDataListSO", menuName = "GameData/DialogueDataList")]
public class DialogueDataListSO : ScriptableObject
{
    public List<DialogueDefination> m_dialogue_list;
}
