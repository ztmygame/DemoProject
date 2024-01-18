using DG.Tweening;
using UnityEngine;

public class PlayerTriggeredFade : MonoBehaviour
{
    private SpriteRenderer[] m_sprite_renderers;

    private void Awake()
    {
        m_sprite_renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void FadeOut()
    {
        Color target_color = new Color(1, 1, 1, GameplaySettings.m_target_alpha);
        foreach(SpriteRenderer renderer in m_sprite_renderers)
        {
            renderer.DOColor(target_color, GameplaySettings.m_fade_duration);
        }
    }

    public void FadeIn()
    {
        Color target_color = new Color(1, 1, 1, 1);
        foreach (SpriteRenderer renderer in m_sprite_renderers)
        {
            renderer.DOColor(target_color, GameplaySettings.m_fade_duration);
        }
    }
}
