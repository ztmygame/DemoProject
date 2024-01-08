using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpriteFade : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerTriggeredFade fader = collision.GetComponent<PlayerTriggeredFade>();
        fader?.FadeOut();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerTriggeredFade fader = collision.GetComponent<PlayerTriggeredFade>();
        fader?.FadeIn();
    }
}
