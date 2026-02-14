using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic; // For List logic
using System.Linq; // For conversion if needed

[Serializable]
public class PreguntaData
{
    public string pregunta;
    public string[] opciones;
    public int indice_correcta;
    public string retroalimentacion;
    public string tipo_desafio; // Ej: 'QUIZ', 'COMPLETAR', 'CASO'
    public string pista; // Texto de ayuda
}

public class TarjetaDesafioUI : MonoBehaviour
{
    // Evento para notificar respuesta (acierto/fallo)
    public event Action<bool> OnRespuestaContestada;

    [Header("UI References")]
    public TMP_Text preguntaText;
    public Button[] botonesOpciones;
    public TMP_Text feedbackText;

    private PreguntaData datosActuales;
    private bool respondido = false;
    private string respuestaCorrectaTexto = ""; // Para validación por texto

    /// <summary>
    /// Configura y muestra el desafío a partir de un JSON.
    /// </summary>
    /// <param name="json">JSON string con la estructura de PreguntaData.</param>
    public void ConfigurarDesafio(string json)
    {
        try
        {
            respondido = false;
            // Resetear feedback
            if (feedbackText != null) feedbackText.text = "";

            // Deserializar
            datosActuales = JsonUtility.FromJson<PreguntaData>(json);

            if (datosActuales == null)
            {
                Debug.LogError("TarjetaDesafioUI: Error al deserializar el JSON.");
                return;
            }

            // Asignar Pregunta
            if (preguntaText != null) preguntaText.text = datosActuales.pregunta;

            // 1. Obtener respuesta correcta original antes de barajar
            if (datosActuales.opciones != null && datosActuales.indice_correcta >= 0 && datosActuales.indice_correcta < datosActuales.opciones.Length)
            {
                respuestaCorrectaTexto = datosActuales.opciones[datosActuales.indice_correcta];
            }
            else
            {
                Debug.LogError("TarjetaDesafioUI: Indice de respuesta fuera de rango.");
                return;
            }

            // 2. Barajar Opciones (Fisher-Yates)
            List<string> opcionesBarajadas = new List<string>(datosActuales.opciones);
            Shuffle(opcionesBarajadas);

            // 3. Asignar a botones
            if (botonesOpciones != null)
            {
                for (int i = 0; i < botonesOpciones.Length; i++)
                {
                    if (i < opcionesBarajadas.Count)
                    {
                        botonesOpciones[i].gameObject.SetActive(true);
                        botonesOpciones[i].interactable = true; // Habilitar
                        
                        string textoOpcion = opcionesBarajadas[i];

                        // Asignar texto al botón
                        TMP_Text btnText = botonesOpciones[i].GetComponentInChildren<TMP_Text>();
                        if (btnText != null)
                        {
                            btnText.text = textoOpcion;
                        }

                        // Reset visual
                        botonesOpciones[i].image.color = Color.white;

                        // Asignar evento click (comparando TEXTO)
                        Button btnRef = botonesOpciones[i]; 
                        botonesOpciones[i].onClick.RemoveAllListeners();
                        botonesOpciones[i].onClick.AddListener(() => VerificarRespuesta(textoOpcion, btnRef));
                    }
                    else
                    {
                        // Ocultar botones sobrantes
                        botonesOpciones[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"TarjetaDesafioUI: Excepción al configurar desafío: {e.Message}");
        }
    }

    /// <summary>
    /// Verifica si la opción seleccionada es la correcta comparando texto.
    /// </summary>
    private void VerificarRespuesta(string textoSeleccionado, Button botonPresionado)
    {
        if (datosActuales == null || respondido) return;

        respondido = true;
        // Comparación de texto (Client-Side Check)
        bool esCorrecta = (textoSeleccionado == respuestaCorrectaTexto);

        // Feedback Visual en el botón
        if (botonPresionado != null)
        {
            botonPresionado.image.color = esCorrecta ? Color.green : Color.red;
        }

        // Deshabilitar botones para evitar doble click
        if (botonesOpciones != null)
        {
            foreach (var btn in botonesOpciones) if (btn != null) btn.interactable = false;
        }

        if (esCorrecta)
        {
            if (feedbackText != null) 
            {
                feedbackText.text = $"¡Correcto!\n{datosActuales.retroalimentacion}";
                feedbackText.color = Color.green;
            }
        }
        else
        {
            if (feedbackText != null) 
            {
                feedbackText.text = "Incorrecto.";
                feedbackText.color = Color.red;
            }
        }

        // Notificar resultado
        OnRespuestaContestada?.Invoke(esCorrecta);
    }

    // Helper for shuffling logic
    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
