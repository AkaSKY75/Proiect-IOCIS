using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechInput : MonoBehaviour
{
    private DictationRecognizer dictationRecognizer;
    public ChatGPTRequest chatGPTRequest;

    void Start()
    {
        // Inițializează DictationRecognizer
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
        dictationRecognizer.Start();

        // Asigură-te că chatGPTRequest este inițializat
        if (chatGPTRequest == null)
        {
            chatGPTRequest = FindObjectOfType<ChatGPTRequest>();
            if (chatGPTRequest == null)
            {
                Debug.LogError("ChatGPTRequest component not found in the scene.");
            }
        }
    }

    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log("Dictated Text: " + text);
        // Verifică dacă chatGPTRequest nu este null înainte de a apela metoda
        if (chatGPTRequest != null)
        {
            chatGPTRequest.SendTextToChatGPT(text);
        }
        else
        {
            Debug.LogError("chatGPTRequest is not initialized.");
        }
    }

    void OnDestroy()
    {
        if (dictationRecognizer != null)
        {
            dictationRecognizer.Stop();
            dictationRecognizer.DictationResult -= DictationRecognizer_DictationResult;
            dictationRecognizer.Dispose();
        }
    }
}
