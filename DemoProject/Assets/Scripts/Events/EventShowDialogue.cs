using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EventShowDialogue", menuName = "Event/Show Dialogue")]
public class EventShowDialogue : EventNodeBase
{
    public List<Dialogue> m_dialogues;

    public override void Execute()
    {
        base.Execute();
        DialogueUI.OpenDialogueBox(ShowDialogueText, m_dialogues.First());
    }

    public void ShowDialogueText()
    {
        DialogueUI.GetInstance().StartCoroutine(StepThroughDialogueDataList());
    }

    public IEnumerator StepThroughDialogueDataList()
    {
        DialogueUI.m_next_text_force_fadein = false;
        for (int i = 0; i < m_dialogues.Count; ++i)
        {
            DialogueUI.m_can_show_next_text = false;    // todo: move to dialogue ui controller

            Dialogue dialogue = m_dialogues[i];

            yield return DialogueUI.ShowDialogue(dialogue);

            yield return new WaitUntil(() => DialogueUI.CanShowNextDialogue());
        }

        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}