using UnityEngine;

public class DialogueEvnetsTest : MonoBehaviour, IInteractable
{
    [SerializeField]
    private EventSequenceExecutor executor;

    public void Interact(PlayerController player)
    {
        executor.Initialize(null);
        executor.Execute();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player))
        {
            player.m_interactable = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player))
        {
            if (player.m_interactable is DialogueInteractor dialogue_interactor && dialogue_interactor == this)
            {
                player.m_interactable = null;
            }
        }
    }
}
