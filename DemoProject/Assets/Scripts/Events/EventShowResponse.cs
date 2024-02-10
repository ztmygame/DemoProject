using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EventShowResponse", menuName = "Event/Show Response")]
public class EventShowResponse : EventNodeBase
{
    // When a player selects a response, the corresponding executor will be triggered.
    // Once the executor is complete, the entile node will be concluded.
    public List<Response> m_responses;
    public List<EventSequenceExecutor> m_executors;
    public int m_default_select_index;

    public override void Initialize(Action<bool> on_finished)
    {
        base.Initialize(on_finished);
        foreach (EventSequenceExecutor executor in m_executors)
        {
            if(executor != null)
            {
                executor.Initialize(m_on_finished);   // conclusion of any executor signifies the end of the entire node
            }
        }
    }

    public override void Execute()
    {
        base.Execute();

        DialogueUI.CreateResponseButtons(m_responses, OnResponseConfirmed, m_default_select_index);
    }

    private void OnResponseConfirmed(int index)
    {
        if(index < m_executors.Count && m_executors[index] != null)
        {
            m_executors[index].Execute();
        }
        else
        {
            m_on_finished(true);
        }
    }
}
