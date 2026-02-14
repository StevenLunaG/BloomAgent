using UnityEngine;

public class BloomManager : MonoBehaviour
{
    [Header("Progression State")]
    public int nivelActual = 0; // 0: Recordar, 1: Comprender, 2: Aplicar, 3: Analizar
    public int puntos = 0;
    public int rachaAciertos = 0;

    [Header("Statistics")]
    public int aciertosTotales = 0;
    public int fallosTotales = 0;
    public int pistasUsadas = 0;

    // Configuración de niveles (Bloom)
    private readonly string[] nivelesBloom = { "RECORDAR", "COMPRENDER", "APLICAR", "ANALIZAR" };

    // Configuración de tipos de desafío por nivel
    private readonly string[] tiposDesafio = { "QUIZ", "COMPLETAR", "CASO", "INTRUSO" };

    /// <summary>
    /// Devuelve el nivel Bloom y el tipo de desafío correspondientes al estado actual.
    /// </summary>
    /// <param name="nivelBloom">Nivel de Bloom (ej: RECORDAR)</param>
    /// <param name="tipoDesafio">Tipo de desafío (ej: QUIZ)</param>
    public void ObtenerSiguienteDesafio(out string nivelBloom, out string tipoDesafio)
    {
        // Asegurar que el nivel esté en rango
        nivelActual = Mathf.Clamp(nivelActual, 0, 3);

        nivelBloom = nivelesBloom[nivelActual];
        tipoDesafio = tiposDesafio[nivelActual];

        Debug.Log($"BloomManager: Generando desafío Nivel {nivelActual} ({nivelBloom}) - Tipo: {tipoDesafio}");
    }

    /// <summary>
    /// Registra el uso de una pista inmediatamente.
    /// </summary>
    public void RegistrarUsoDePista()
    {
        pistasUsadas++;
        Debug.Log("BloomManager: Pista usada (Registrada inmediatamente).");
    }

    /// <summary>
    /// Procesa el resultado de un desafío y actualiza el progreso.
    /// </summary>
    /// <param name="acierto">Si el usuario respondió correctamente</param>
    /// <param name="usoPistaParaPuntos">Si se usó pista en esta pregunta (para calcular puntos)</param>
    public void RegistrarResultado(bool acierto, bool usoPistaParaPuntos)
    {
        if (acierto)
        {
            aciertosTotales++;
            rachaAciertos++;

            // Calcular puntos base (ej: 100 por nivel)
            int puntosGanados = 100 * (nivelActual + 1);

            // Penalización por pista (mitad de puntos)
            if (usoPistaParaPuntos)
            {
                puntosGanados /= 2;
                // NOTA: Ya se contó en RegistrarUsoDePista
                Debug.Log("BloomManager: Penalización de puntos por pista.");
            }

            puntos += puntosGanados;
            Debug.Log($"BloomManager: Acierto! (+{puntosGanados} pts). Racha: {rachaAciertos}");

            // Verificar subida de nivel (Racha >= 2)
            if (rachaAciertos >= 2)
            {
                SubirNivel();
            }
        }
        else
        {
            fallosTotales++;
            rachaAciertos = 0; // Resetear racha
            
            // NOTA: Si usó pista, ya se contó en RegistrarUsoDePista
            Debug.Log("BloomManager: Fallo. Racha reseteada.");
            // Mantener nivel actual
        }
    }

    private void SubirNivel()
    {
        if (nivelActual < 3)
        {
            nivelActual++;
            rachaAciertos = 0; // Resetear racha al subir de nivel para nueva dificultad
            Debug.Log($"BloomManager: ¡NIVEL ASCENDIDO! Nuevo nivel: {nivelActual} ({nivelesBloom[nivelActual]})");
            // Aquí podríamos lanzar un evento onLevelUp
        }
        else
        {
            Debug.Log("BloomManager: Nivel máximo alcanzado.");
        }
    }

    public void DebugSetLevel(int level)
    {
        nivelActual = Mathf.Clamp(level, 0, 3);
        rachaAciertos = 0;
    }

    /// <summary>
    /// Resetea todo el progreso para una nueva sesión.
    /// </summary>
    public void ResetearProgreso()
    {
        nivelActual = 0;
        puntos = 0;
        rachaAciertos = 0;
        aciertosTotales = 0;
        fallosTotales = 0;
        pistasUsadas = 0;
    }
}
