using System;
using System.Collections;
using UnityEngine;

[RequireComponent (typeof(CanvasGroup))]
public class FadeEffect : MonoBehaviour
{
    private CanvasGroup m_canvas_group;

    [SerializeField]
    private AnimationCurve m_fading_curve;

    private Coroutine m_fade_coroutine;

    private void Awake()
    {
        m_canvas_group = GetComponent<CanvasGroup>();
        m_canvas_group.alpha = 0.0f;
        m_fading_curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    public void Fade(float a, float duration, Action callback)
    {
        if(m_canvas_group.alpha == a)
        {
            callback?.Invoke();
            return;
        }
        

        if (duration <= 0)
        {
             m_canvas_group.alpha = a;
            callback?.Invoke();
        }
        else
        {
            if(m_fade_coroutine != null)
            {
                StopCoroutine(m_fade_coroutine);
            }

            StartCoroutine(Fading(a, duration, callback));
        }
    }

    private IEnumerator Fading(float a, float duration, Action callback)
    {
        float time = 0;
        float former_a = m_canvas_group.alpha;
        while(time < duration)
        {
            time = Mathf.Clamp(time + Time.unscaledDeltaTime, 0, duration);
            m_canvas_group.alpha = Mathf.Lerp(former_a, a, m_fading_curve.Evaluate(time / duration));
            yield return null;
        }

        callback?.Invoke();
    }
}
