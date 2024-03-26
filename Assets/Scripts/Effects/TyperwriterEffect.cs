using System.Collections;
using TMPro;  
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField]
    public float TypingSpeed;

    public IEnumerator TypeSentence(string sentence, TextMeshProUGUI textComponent)
    {
        textComponent.text = "";  // Clear existing text
        foreach (char letter in sentence.ToCharArray())
        {
            textComponent.text += letter;  // Add one letter
            yield return new WaitForSeconds(TypingSpeed);  // Wait before adding the next one
        }
    }

    // Call this method to start the effect
    public void DisplayText(string text, TextMeshProUGUI textComponent)
    {
        StartCoroutine(TypeSentence(text, textComponent));
    }
}