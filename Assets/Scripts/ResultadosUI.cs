using UnityEngine;
using TMPro;

public class ResultadosUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text textoNivel;
    public TMP_Text textoPrecision;
    public TMP_Text textoPistas;
    public TMP_Text textoMensaje;

    /// <summary>
    /// Muestra el panel con las estadísticas finales.
    /// </summary>
    /// <param name="nivel">Nivel final alcanzado (0-3)</param>
    /// <param name="aciertos">Total de preguntas acertadas</param>
    /// <param name="totalPreguntas">Total de preguntas realizadas</param>
    /// <param name="pistas">Total de pistas utilizadas</param>
    public void MostrarResultados(int nivel, int aciertos, int totalPreguntas, int pistas)
    {
        gameObject.SetActive(true);

        // 1. Mostrar Nivel
        string[] nombresNiveles = { "RECORDAR", "COMPRENDER", "APLICAR", "ANALIZAR" };
        string nombreNivel = (nivel >= 0 && nivel < nombresNiveles.Length) ? nombresNiveles[nivel] : "DESCONOCIDO";
        
        if (textoNivel != null) 
            textoNivel.text = $"Nivel Alcanzado: {nombreNivel}";

        // 2. Calcular y Mostrar Precisión
        float porcentaje = 0f;
        if (totalPreguntas > 0)
        {
            porcentaje = ((float)aciertos / totalPreguntas) * 100f;
        }

        if (textoPrecision != null)
            textoPrecision.text = $"Precisión: {porcentaje:F1}% ({aciertos}/{totalPreguntas})";

        // 3. Mostrar Pistas
        if (textoPistas != null)
            textoPistas.text = $"Pistas usadas: {pistas}";

        // 4. Mensaje final según desempeño
        if (textoMensaje != null)
        {
            if (porcentaje >= 80) textoMensaje.text = "¡Excelente trabajo! Dominas el tema.";
            else if (porcentaje >= 50) textoMensaje.text = "¡Buen esfuerzo! Sigue practicando.";
            else textoMensaje.text = "¡No te rindas! Repasa el material y vuelve a intentarlo.";
        }
    }
    
    /// <summary>
    /// Método para el botón de 'Cerrar' o 'Reiniciar'.
    /// </summary>
    public void Cerrar()
    {
        gameObject.SetActive(false);
    }
}
