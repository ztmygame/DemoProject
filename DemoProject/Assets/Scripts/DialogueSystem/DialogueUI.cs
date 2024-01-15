using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_text_label;

    private void Start()
    {
        GetComponent<TypewriterEffect>().Run("Hello!\nThis is the second line.\n哦哦哦哦哦哦哦哦哦哦哦哦哦哦哦", m_text_label);
    }
}
