using UnityEngine;

public class DialogueInteractor : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Conversation m_conversation;

    public void Interact(PlayerController player)
    {
        player.m_dialogue_ui.StartConversation(m_conversation);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player))
        {
            player.m_interactable = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player))
        {
            if(player.m_interactable is DialogueInteractor dialogue_interactor && dialogue_interactor == this)
            {
                player.m_interactable = null;
            }
        }
    }
}
