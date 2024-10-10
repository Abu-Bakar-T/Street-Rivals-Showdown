using TMPro;
using UnityEngine;
using System;
using System.Collections;
using System.Text;

[Serializable]
public class TypeOutScriptTextMesh : MonoBehaviour
{
    public bool On = true;
    public bool reset = false;
    public string FinalText;

    public float TotalTypeTime = -1f;

    public float TypeRate;
    private float LastTime;

    public string RandomCharacter;
    public float RandomCharacterChangeRate = 0.1f;
    private float RandomCharacterTime;

    public int i;

    private string RandomChar()
    {
        byte value = (byte)UnityEngine.Random.Range(41f, 128f);
        string c = Encoding.ASCII.GetString(new byte[] { value });
        return c;
    }

    public void Skip()
    {
        GetComponent<TextMeshProUGUI>().text = FinalText;
        On = false;
    }

    void Update()
    {
        if (TotalTypeTime != -1f)
        {
            TypeRate = TotalTypeTime / (float)FinalText.Length;
        }

        if (On == true)
        {
            if (Time.time - RandomCharacterTime >= RandomCharacterChangeRate)
            {
                RandomCharacter = RandomChar();
                RandomCharacterTime = Time.time;
            }

            try
            {
                // Replace newline character with the actual newline character
                string displayedText = FinalText.Substring(0, i).Replace("\\n", "\n");
                if (i < FinalText.Length)
                {
                    displayedText += RandomCharacter;
                }
                GetComponent<TextMeshProUGUI>().text = displayedText;
            }
            catch (ArgumentOutOfRangeException)
            {
                On = false;
            }

            if (Time.time - LastTime >= TypeRate)
            {
                i++;
                LastTime = Time.time;
            }

            bool isChar = false;

            while (isChar == false)
            {
                if ((i + 1) < FinalText.Length)
                {
                    if (FinalText.Substring(i, 1) == " ")
                    {
                        i++;
                    }
                    else
                    {
                        isChar = true;
                    }
                }
                else
                {
                    isChar = true;
                }
            }

            if (i == FinalText.Length && GetComponent<TextMeshProUGUI>().text.Length == FinalText.Length)
            {
                On = false;
            }
        }

        if (reset == true)
        {
            GetComponent<TextMeshProUGUI>().text = "";
            i = 0;
            reset = false;
        }
    }
}