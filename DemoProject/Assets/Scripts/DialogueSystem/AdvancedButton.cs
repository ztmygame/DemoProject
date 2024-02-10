using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdvancedButton : Button
{
    private Image m_select_border;
    private Animator m_animator;

    private RectTransform m_rect_transform;

    protected override void Awake()
    {
        base.Awake();

        m_select_border = transform.Find("SelectedImage").GetComponent<Image>();
        m_select_border.enabled = false;

        m_animator = GetComponent<Animator>();

        m_rect_transform = GetComponent<RectTransform>();

        onClick.AddListener(OnButtonClicked);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        Select();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        m_select_border.enabled = true;

        DialogueUI.SetCurrentSelectable(this);
        DialogueUI.SetSelectedCursorPosition(transform.position + new Vector3(m_rect_transform.rect.width / 2.0f * 1.2f, m_rect_transform.rect.height / 2.0f, 0.0f));
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        m_select_border.enabled = false;
    }

    public void OnButtonClicked()
    {
        m_animator.SetTrigger(GameProperties.m_click_button_animation_hash);
        DialogueUI.GetInstance().PlaySelectCursorAnimation();
    }
}
