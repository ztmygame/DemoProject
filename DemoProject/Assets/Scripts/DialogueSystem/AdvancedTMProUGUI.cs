using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class RubyTextInfo
{
    public RubyTextInfo()
    {
        m_begin_index = -1;
        m_end_index = -1;
        m_content = null;
    }

    public RubyTextInfo(int begin_index, string content)
    {
        m_begin_index = begin_index;
        m_end_index = begin_index;
        m_content = content;
    }

    public int m_begin_index { get; set; }
    public int m_end_index { get; set; }
    public string m_content {  get; set; }
}

public class AdvancedTextPreprocessor : ITextPreprocessor
{
    public Dictionary<int, float> m_pause_map { get; private set; }
    public List<RubyTextInfo> m_ruby_list { get; private set; }

    public AdvancedTextPreprocessor()
    {
        m_pause_map = new Dictionary<int, float>();
        m_ruby_list = new List<RubyTextInfo>();
    }

    string ITextPreprocessor.PreprocessText(string text)
    {
        m_pause_map.Clear();
        m_ruby_list.Clear();

        if(string.IsNullOrEmpty(text))
        {
            return "";
        }

        // find all rich text labels from the text string
        // pause labels: record index and remove them from text
        // ruby labels: create ruby data and remove them from text
        // other labels: accumulate pause label index offset
        string processing_text = text;

        int custom_label_index_offset = 0;
        int other_label_index_offset = 0;

        RubyTextInfo ruby = new RubyTextInfo();

        string rich_text_pattern = "<.*?>";

        MatchCollection rich_text_matches = Regex.Matches(processing_text, rich_text_pattern);

        foreach (Match match in rich_text_matches)
        {
            if (!match.Success)
            {
                continue;
            }

            string value = match.Value[1..^1];
            if (float.TryParse(value, out float pause_time))
            {
                int index = match.Index - other_label_index_offset - 1;
                if (index > 0)
                {
                    m_pause_map[index] = pause_time;
                }

                processing_text = processing_text.Remove(match.Index - custom_label_index_offset, match.Length);
                custom_label_index_offset += match.Length;
            }
            else if (Regex.IsMatch(value, "^r=.+"))
            {
                ruby.m_begin_index = match.Index - custom_label_index_offset - other_label_index_offset;
                ruby.m_content = value.Substring(2);

                processing_text = processing_text.Remove(match.Index - custom_label_index_offset, match.Length);
                custom_label_index_offset += match.Length;
            }
            else if (value == "/r")
            {
                ruby.m_end_index = match.Index - custom_label_index_offset - other_label_index_offset;
                m_ruby_list.Add(ruby);
                ruby = new RubyTextInfo();

                processing_text = processing_text.Remove(match.Index - custom_label_index_offset, match.Length);
                custom_label_index_offset += match.Length;
            }
            else
            {
                other_label_index_offset += match.Length;

                // add a placeholder for sprite label
                if (Regex.IsMatch(value, "^sprite=.+"))
                {
                    other_label_index_offset -= 1;
                }
            }
        }

        return processing_text;
    }

    public bool FindRubyAtBeginIndex(int index, out RubyTextInfo ruby)
    {
        ruby = new RubyTextInfo(0, "");
        foreach (RubyTextInfo info in m_ruby_list)
        {
            if (info.m_begin_index == index)
            {
                ruby = info;
                return true;
            }
        }
        return false;
    }
}

public class AdvancedTMProUGUI : TextMeshProUGUI
{
    private GameObject m_ruby_prefab;

    private List<GameObject> m_ruby_text_objects;

    public AdvancedTMProUGUI()
    {
        textPreprocessor = new AdvancedTextPreprocessor();
    }

    protected override void Awake()
    {
        base.Awake();

        m_ruby_prefab = Resources.Load<GameObject>("RubyText");
        m_ruby_text_objects = new List<GameObject>();
    }

    public void Initialize()
    {
        foreach(GameObject ruby in m_ruby_text_objects)
        {
            Destroy(ruby);
        }
        m_ruby_text_objects.Clear();
    }

    public Coroutine ShowText(Dialogue dialogue)
    {
        Initialize();

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
            yield return CharacterFadeIn(i);

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
        if ((textPreprocessor as AdvancedTextPreprocessor).FindRubyAtBeginIndex(index, out RubyTextInfo ruby))
        {
            CreateRubyText(ruby);
        }

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
        if (!textInfo.characterInfo[index].isVisible)
        {
            return;
        }

        TMP_CharacterInfo character_info = textInfo.characterInfo[index];
        int material_index = character_info.materialReferenceIndex;
        int vertex_index = character_info.vertexIndex;

        for(int i = 0; i < 4; ++i)
        {
            textInfo.meshInfo[material_index].colors32[vertex_index + i].a = alpha;
        }

        UpdateVertexData();
    }


    private void CreateRubyText(RubyTextInfo ruby_text_info)
    {
        GameObject ruby = Instantiate(m_ruby_prefab, transform);
        ruby.GetComponent<TextMeshProUGUI>().SetText(ruby_text_info.m_content);
        ruby.GetComponent<TextMeshProUGUI>().color = textInfo.characterInfo[ruby_text_info.m_begin_index].color;
        ruby.transform.localPosition = (textInfo.characterInfo[ruby_text_info.m_begin_index].topLeft + textInfo.characterInfo[ruby_text_info.m_end_index - 1].topRight) / 2.0f;
        m_ruby_text_objects.Add(ruby);
    }
}
