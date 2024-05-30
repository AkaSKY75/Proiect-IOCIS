using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ChatGPTRequest : MonoBehaviour
{
    private string apiKey = "";
    //public TextToSpeech textToSpeech;

    public void SendTextToChatGPT(string text)
    {
        StartCoroutine(PostRequest("https://api.openai.com/v1/chat/completions", text));
    }

    IEnumerator PostRequest(string url, string prompt)
    {
        string jsonBody = "{\"model\": \"gpt-3.5-turbo\", \"messages\": [{\"role\": \"user\", \"content\": \"" + prompt + "\"}], \"max_tokens\": 100, \"temperature\": 0.5}";
        Debug.Log("JSON Body: " + jsonBody);

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
            Debug.Log($"Error Response: {request.downloadHandler.text}");
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            // Procesare răspuns și conversie text-to-speech (dacă ai nevoie)
        }
    }



}

[System.Serializable]
public class ChatGPTResponse
{
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public string text;
}
