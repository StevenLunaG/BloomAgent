using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;

public class GroqClient : MonoBehaviour
{
    [Header("Groq API Settings")]
    [Tooltip("API Key from Groq console")]
    public string apiKey = "gsk_..."; // Replace with actual key or load securely
    public string model = "llama-3.1-8b-instant"; // Updated model
    private const string ApiUrl = "https://api.groq.com/openai/v1/chat/completions";

    [Serializable]
    private class RequestBody
    {
        public string model;
        public List<Message> messages; 
        public float temperature = 0.7f; 
    }

    [Serializable]
    private class Message
    {
        public string role;
        public string content;
    }

    [Serializable]
    private class ResponseRoot
    {
        public Choice[] choices;
    }

    [Serializable]
    private class Choice
    {
        public Message message;
    }

    /// <summary>
    /// Sends a prompt to Groq API (existing simple method).
    /// </summary>
    public IEnumerator EnviarMensajeAGroq(string promptUsuario, Action<string> callbackRespuesta)
    {
        List<Message> msgs = new List<Message>
        {
            new Message { role = "user", content = promptUsuario }
        };
        yield return StartCoroutine(EnviarRequest(msgs, callbackRespuesta));
    }

    /// <summary>
    /// Generates a Bloom Taxonomy question based on context.
    /// Responds with raw JSON.
    /// </summary>
    public IEnumerator GenerarPreguntaBloom(string contexto, string nivelBloom, string tipoDesafio, string preguntasPrevias, Action<string> callback)
    {
        string instructionTipo = "";
        switch (tipoDesafio)
        {
            case "COMPLETAR":
                instructionTipo = "Toma una frase del texto, quita una palabra clave y ponla como la respuesta correcta. Las otras opciones son palabras incorrectas.";
                break;
            case "CASO":
                instructionTipo = "Inventa una situación hipotética breve donde se aplique el concepto.";
                break;
            case "INTRUSO":
                instructionTipo = "Genera 4 conceptos, donde 3 están relacionados y 1 es incorrecto. La pregunta debe ser: ¿Cuál no pertenece?.";
                break;
            case "QUIZ":
            default:
                instructionTipo = "Genera una pregunta directa de opción múltiple.";
                break;
        }

        string historyPrompt = "";
        if (!string.IsNullOrEmpty(preguntasPrevias))
        {
            historyPrompt = $"IMPORTANTE: Genera una pregunta COMPLETAMENTE NUEVA. Está PROHIBIDO repetir o parafrasear estas preguntas anteriores: {preguntasPrevias}\n";
        }

        string systemPrompt = $"Actúa como un experto docente en la Taxonomía de Bloom. " +
            $"Tu objetivo es generar una pregunta de opción múltiple basada en el siguiente contexto. " +
            $"El nivel de complejidad debe ser '{nivelBloom}'.\n" +
            $"TIPO DE DESAFÍO: {tipoDesafio}. Instrucción: {instructionTipo}\n\n" +
            $"{historyPrompt}\n" +
            $"IMPORTANTE: Responde ÚNICAMENTE con un objeto JSON crudo (sin markdown, sin bloques de código ```json) " +
            $"con esta estructura exacta:\n" +
            $"{{\n" +
            $"  \"pregunta\": \"...\",\n" +
            $"  \"opciones\": [\"opcion1\", \"opcion2\", \"opcion3\", \"opcion4\"],\n" +
            $"  \"indice_correcta\": 0,\n" +
            $"  \"retroalimentacion\": \"explicación breve\",\n" +
            $"  \"tipo_desafio\": \"{tipoDesafio}\",\n" +
            $"  \"pista\": \"texto de ayuda breve que no revele la respuesta directa\"\n" +
            $"}}";

        List<Message> msgs = new List<Message>
        {
            new Message { role = "system", content = systemPrompt },
            new Message { role = "user", content = $"Contexto: {contexto}" }
        };

        yield return StartCoroutine(EnviarRequest(msgs, callback));
    }

    /// <summary>
    /// Core request logic.
    /// </summary>
    private IEnumerator EnviarRequest(List<Message> messages, Action<string> callback)
    {
        // 1. Prepare JSON Body
        // Re-creating local struct with array for JsonUtility compatibility
        RequestBodyArray bodyArray = new RequestBodyArray
        {
            model = this.model,
            messages = messages.ToArray(),
            temperature = 0.5f // Lower temp for more deterministic JSON
        };
        
        string jsonBody = JsonUtility.ToJson(bodyArray);
        byte[] rawBody = Encoding.UTF8.GetBytes(jsonBody);

        // 2. Setup WebRequest
        using (UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(rawBody);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            // 3. Send
            yield return request.SendWebRequest();

            // 4. Handle Response
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"GroqClient Error: {request.error}\nResponse: {request.downloadHandler.text}");
                callback?.Invoke(null); // Return null on error
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                try 
                {
                    ResponseRoot response = JsonUtility.FromJson<ResponseRoot>(jsonResponse);
                    if (response.choices != null && response.choices.Length > 0)
                    {
                        string content = response.choices[0].message.content;
                        callback?.Invoke(content);
                    }
                    else
                    {
                        callback?.Invoke(null);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"GroqClient Parse Error: {e.Message}");
                    callback?.Invoke(null);
                }
            }
        }
    }

    [Serializable]
    private class RequestBodyArray
    {
        public string model;
        public Message[] messages;
        public float temperature;
    }
}
