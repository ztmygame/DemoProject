using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public DialogueUI m_dialogue_ui;

    public IInteractable m_interactable;

    private float m_input_x;
    private float m_input_y;

    private Rigidbody2D m_rigidbody;

    public void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        if (m_dialogue_ui.m_is_showing)
        {
            return;
        }

        ProcessInput();

        if (Input.GetKeyDown(KeycodeSettings.m_interact_key))
        {
            m_interactable?.Interact(this);
        }
    }

    public void FixedUpdate()
    {
        if (m_dialogue_ui.m_is_showing)
        {
            return;
        }

        PlayerMovement();
    }

    private void ProcessInput()
    {
        m_input_x = Input.GetAxisRaw("Horizontal");
        m_input_y = Input.GetAxisRaw("Vertical");
    }

    private void PlayerMovement()
    {
        if (m_input_x != 0 || m_input_y != 0)
        {
            float2 movement_input = math.normalize(new float2(m_input_x, m_input_y));
            m_rigidbody.MovePosition(m_rigidbody.position + (new Vector2(movement_input.x, movement_input.y)) * GameplaySettings.m_walk_speed * Time.deltaTime);
        }
    }
}
