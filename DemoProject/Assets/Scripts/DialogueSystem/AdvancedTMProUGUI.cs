using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class AdvancedTextPreprocessor : ITextPreprocessor
{
    public Dictionary<int, float> m_pause_map { get; private set; }

    public AdvancedTextPreprocessor()
    {
        m_pause_map = new Dictionary<int, float>();
    }

    string ITextPreprocessor.PreprocessText(string text)
    {
        m_pause_map.Clear();
        if(text == "")
        {
            return text;
        }

        // find all rich text labels from the text string
        // pause labels: record index and remove them from text
        // other labels: accumulate pause label index offset
        string processing_text = text;
        int pause_label_index_offset = 0;

        string rich_text_pattern = "<.*?>";

        MatchCollection rich_text_matches = Regex.Matches(processing_text, rich_text_pattern);

        foreach(Match match in rich_text_matches)
        {
            if (!match.Success)
            {
                continue;
            }

            string value = match.Value[1..^1];
            if (float.TryParse(value, out float pause_time))
            {
                int index = match.Index - pause_label_index_offset - 1;
                if (index > 0)
                {
                    m_pause_map[index] = pause_time;
                }

                processing_text = processing_text.Remove(match.Index, match.Length);
            }
            else
            {
                pause_label_index_offset += match.Length;

                // add a placeholder for sprite label
                if(Regex.IsMatch(value, "^sprite=.+"))
                {
                    pause_label_index_offset -= 1;
                }
            }
        }

        return processing_text;
    }
}

public class AdvancedTMProUGUI : TextMeshProUGUI
{
    public AdvancedTMProUGUI()
    {
        textPreprocessor = new AdvancedTextPreprocessor();
    }

    public Coroutine ShowText(Dialogue dialogue)
    {
        SetText(dialogue.m_text);
        return StartCoroutine(Typing());
    }

    IEnumerator Typing()
    {
        ForceMeshUpdate();

        for(int i = 0; i < m_characterCount; ++i)
        {
            ModifyCharacterAlphaAtIndex(i, 0);
        }

        for (int i = 0; i < m_characterCount; ++i)
        {
            // display single character
            if (textInfo.characterInfo[i].isVisible)
            {
                yield return CharacterFadeIn(i);
            }

            // pause
            if ((textPreprocessor as AdvancedTextPreprocessor).m_pause_map.TryGetValue(i, out float pause_time))
            {
                yield return new WaitForSecondsRealtime(pause_time);
            }
            else
            {
                yield return new WaitForSecondsRealtime(1.0f / GameplaySettings.m_type_speed);
            }
        }

        yield return null;
    }

    IEnumerator CharacterFadeIn(int index)
    {
        float duration = GameplaySettings.m_character_fade_in_duration;
        if (duration <= 0)
        {
            ModifyCharacterAlphaAtIndex(index, 255);
        }
        else
        {
            float time = 0.0f;
            while(time < GameplaySettings.m_character_fade_in_duration)
            {
                time = Mathf.Clamp(time + Time.unscaledDeltaTime, 0.0f, duration);
                ModifyCharacterAlphaAtIndex(index, (byte)(time / duration * 255));
                yield return null;
            }
        }
    }

    private void ModifyCharacterAlphaAtIndex(int index, byte alpha)
    {
        TMP_CharacterInfo character_info = textInfo.characterInfo[index];
        int material_index = character_info.materialReferenceIndex;
        int vertex_index = character_info.vertexIndex;

        for(int i = 0; i < 4; ++i)
        {
            textInfo.meshInfo[material_index].colors32[vertex_index + i].a = alpha;
        }

        UpdateVertexData();
    }

    protected override void Start()
    {
        base.Start();
    }
}
