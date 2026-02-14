using UnityEngine;
using TMPro;
using System.Collections.Generic; // List
using System.Linq; // string.Join

public class SesionAR : MonoBehaviour
{
    [Header("Dependencies")]
    public RobotAsistente robot;
    public GroqClient groq;
    public GestorDocumentos gestorDocumentos;
    public TarjetaDesafioUI tarjetaUI;
    public ResultadosUI resultadosUI;
    public BloomManager bloomManager;
    
    [Header("UI")]
    public GameObject panelIntro; // Panel de bienvenida
    public TMP_Text textoNivelHUD;
    public TMP_Text textoInstruccion;

    [Header("New HUD")]
    public GameObject panelHUD;
    public GameObject panelInstruccion;
    public GameObject panelConfirmacionSalir;

    [Header("State")]
    private bool experienciaIniciada = false;
    private string contextoActual = ""; // Texto del documento cargado
    private bool pistaUsadaEnPreguntaActual = false;
    
    // Historial de preguntas para evitar repeticiones
    private List<string> historialPreguntas = new List<string>();

    private void Start()
    {
        // Forzar orientación horizontal
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        // 1. Configuración Inicial
        if (panelIntro != null) panelIntro.SetActive(true);
        if (tarjetaUI != null) tarjetaUI.gameObject.SetActive(false); // UI Fix
        
        // Ocultar resultados y robot al inicio
        if (resultadosUI != null) resultadosUI.gameObject.SetActive(false);
        if (robot != null) robot.gameObject.SetActive(false);

        // Suscribirse al evento de carga de archivos
        if (gestorDocumentos != null)
        {
            gestorDocumentos.OnTextoCargado += AlRecibirTextoDelArchivo;
        }

        // Suscribirse eventos de UI y Gamification
        if (tarjetaUI != null)
        {
            tarjetaUI.OnRespuestaContestada += ProcesarRespuestaUsuario;
        }

        if (robot != null)
        {
            robot.OnPistaSolicitada += () => 
            { 
                pistaUsadaEnPreguntaActual = true; 
                if (bloomManager != null) bloomManager.RegistrarUsoDePista();
            };
        }
    }

    private void OnDestroy()
    {
        if (gestorDocumentos != null)
        {
            gestorDocumentos.OnTextoCargado -= AlRecibirTextoDelArchivo;
        }
        if (tarjetaUI != null)
        {
            tarjetaUI.OnRespuestaContestada -= ProcesarRespuestaUsuario;
        }
        if (robot != null)
        {
            robot.OnPistaSolicitada -= () => { pistaUsadaEnPreguntaActual = true; };
        }
    }

    /// <summary>
    /// Called by Vuforia Event (PlaneFinder -> OnContentPlaced)
    /// </summary>
    public void IniciarExperiencia()
    {
        if (experienciaIniciada) return;
        
        experienciaIniciada = true;
        Debug.Log("SesionAR: Experience Started");

        if (robot != null)
        {
            robot.gameObject.SetActive(true);
            robot.Decir("¡Conexión establecida! Iniciando prueba de inteligencia...");
        }
    }

    /// <summary>
    /// Método para el botón del Panel Intro
    /// </summary>
    public void IniciarCargaDesdeIntro()
    {
        if (gestorDocumentos != null)
        {
            gestorDocumentos.AbrirExplorador();
        }
    }

    /// <summary>
    /// Callback ejecutado cuando GestorDocumentos termina de leer el archivo.
    /// </summary>
    private void AlRecibirTextoDelArchivo(string contenidoCargado)
    {
        // Fix Race Condition
        experienciaIniciada = true;
        
        contextoActual = contenidoCargado;
        Debug.Log($"SesionAR: Texto recibido. Iniciando ciclo de preguntas...");
        
        // Ocultar Intro
        if (panelIntro != null) panelIntro.SetActive(false);

        // Activar HUD y resetear paneles secundarios
        if (panelHUD != null) panelHUD.SetActive(true);
        if (panelInstruccion != null) panelInstruccion.SetActive(false);
        if (panelConfirmacionSalir != null) panelConfirmacionSalir.SetActive(false);

        if (robot != null) 
        {
            robot.gameObject.SetActive(true); // Mostrar robot
            robot.Decir("Analizando documento... Generando desafío.");
        }
        
        // Iniciar confirmación
        if (robot != null)
        {
            robot.MostrarConfirmacionInicio(ComenzarExperienciaReal);
        }

        // Set default instruction
        OnTrackingPerdido();
    }

    private void ComenzarExperienciaReal()
    {
        if (robot != null)
        {
            robot.ActivarModoEstudio();
            robot.Decir("¡Aquí vamos!");
        }
        GenerarDesafioActual();
    }

    /// <summary>
    /// Pide a Groq una pregunta basada en el nivel actual del BloomManager.
    /// </summary>
    private void GenerarDesafioActual()
    {
        if (string.IsNullOrEmpty(contextoActual) || groq == null || bloomManager == null) return;

        pistaUsadaEnPreguntaActual = false; // Reset flag

        string nivelBloom, tipoDesafio;
        bloomManager.ObtenerSiguienteDesafio(out nivelBloom, out tipoDesafio);

        // Actualizar HUD
        if (textoNivelHUD != null) textoNivelHUD.text = "NIVEL: " + nivelBloom.ToUpper();

        if (robot != null) robot.Decir($"Generando reto nivel {nivelBloom}...");

        // Preparar historial
        string historyStr = string.Join(" | ", historialPreguntas);

        StartCoroutine(groq.GenerarPreguntaBloom(contextoActual, nivelBloom, tipoDesafio, historyStr, (jsonRespuesta) => 
        {
            if (!string.IsNullOrEmpty(jsonRespuesta))
            {
                // Deserializar para extraer la pista y pregunta
                PreguntaData data = JsonUtility.FromJson<PreguntaData>(jsonRespuesta);
                
                // Añadir al historial
                if (!string.IsNullOrEmpty(data.pregunta))
                {
                    historialPreguntas.Add(data.pregunta);
                }

                if (robot != null) 
                {
                    robot.Decir("¡Aquí tienes el desafío!");
                    // Pasar la pista al robot
                    robot.GuardarPista(data.pista);
                }
                
                if (tarjetaUI != null)
                {
                    tarjetaUI.gameObject.SetActive(true);
                    tarjetaUI.ConfigurarDesafio(jsonRespuesta);
                    tarjetaUI.gameObject.SetActive(true); // Redundant but safe
                }
            }
            else
            {
                if (robot != null) robot.Decir("Hubo un error generando la pregunta.");
            }
        }));
    }

    /// <summary>
    /// Finaliza la sesión actual y muestra resultados.
    /// </summary>
    public void TerminarSesion()
    {
        if (panelHUD != null) panelHUD.SetActive(false);
        if (bloomManager == null) return;

        // Si hay una pregunta activa (tarjeta visible), contarla como fallo/vista
        if (tarjetaUI != null && tarjetaUI.gameObject.activeSelf)
        {
            Debug.Log("SesionAR: Terminando sesión con pregunta activa. Registrando como fallo.");
            bloomManager.RegistrarResultado(false, pistaUsadaEnPreguntaActual);
        }

        // Ocultar desafío actual
        if (tarjetaUI != null) tarjetaUI.gameObject.SetActive(false);

        // Robot se despide
        if (robot != null) robot.Decir("¡Sesión finalizada! Aquí tienes tus resultados.");

        // Mostrar UI de resultados
        if (resultadosUI != null)
        {
            int total = bloomManager.aciertosTotales + bloomManager.fallosTotales;
            resultadosUI.MostrarResultados(
                bloomManager.nivelActual,
                bloomManager.aciertosTotales,
                total,
                bloomManager.pistasUsadas
            );
        }
    }

    /// <summary>
    /// Reinicia la sesión con el mismo documento (Reintentar).
    /// </summary>
    public void BotonReintentar()
    {
        Debug.Log("Reiniciando sesión con el mismo documento...");
        
        // 1. Ocultar Resultados
        if (resultadosUI != null) resultadosUI.gameObject.SetActive(false);
        
        // 2. Resetear Estadísticas
        if (bloomManager != null) bloomManager.ResetearProgreso();
        
        // Limpiar historial para nuevos intentos con mismo documento
        historialPreguntas.Clear();
        
        // 3. Reactivar Robot y UI necesaria
        if (robot != null)
        {
            robot.gameObject.SetActive(true);
            robot.Decir("¡Vamos de nuevo! Repasemos el mismo documento.");
        }
        if (textoNivelHUD != null) textoNivelHUD.gameObject.SetActive(true);

        // 4. Iniciar la primera pregunta inmediatamente
        GenerarDesafioActual();

        // Reactivar HUD
        if (panelHUD != null) panelHUD.SetActive(true);
    }

    /// <summary>
    /// Maneja la respuesta del usuario desde la UI.
    /// </summary>
    private void ProcesarRespuestaUsuario(bool esAcierto)
    {
        if (bloomManager != null)
        {
            bloomManager.RegistrarResultado(esAcierto, pistaUsadaEnPreguntaActual);
        }

        if (robot != null)
        {
            if (esAcierto) robot.Decir("¡Correcto! ¡Muy bien!");
            else robot.Decir("Vaya... esa no era. Intenta aprender del error.");
        }

        // Programar siguiente pregunta automática
        Invoke("GenerarDesafioActual", 4.0f);
    }

    public void OcultarInstruccion()
    {
        if (panelInstruccion != null)
            panelInstruccion.SetActive(false);
    }

    public void BotonSalirHUD()
    {
        if (panelConfirmacionSalir != null)
            panelConfirmacionSalir.SetActive(true);
    }

    public void ConfirmarSalida(bool confirmar)
    {
        if (confirmar)
        {
            BotonNuevaSesion();
        }
        else
        {
            if (panelConfirmacionSalir != null)
                panelConfirmacionSalir.SetActive(false);
        }
    }

    /// <summary>
    /// Reinicia la sesión volviendo al Panel Intro.
    /// </summary>
    public void BotonNuevaSesion()
    {
        // Limpiar historial
        historialPreguntas.Clear();

        // Ocultar HUD y paneles asociados
        if (panelHUD != null) panelHUD.SetActive(false);
        if (panelInstruccion != null) panelInstruccion.SetActive(false);
        if (panelConfirmacionSalir != null) panelConfirmacionSalir.SetActive(false);

        // Ocultar resultados
        if (resultadosUI != null) resultadosUI.gameObject.SetActive(false);
        
        // Resetear BloomManager
        if (bloomManager != null) bloomManager.ResetearProgreso();

        // Limpiar contexto
        contextoActual = "";

        // Ocultar Robot y HUD
        if (robot != null) robot.gameObject.SetActive(false);
        if (tarjetaUI != null) tarjetaUI.gameObject.SetActive(false);
        
        // Mostrar Intro
        if (panelIntro != null) panelIntro.SetActive(true);
        experienciaIniciada = false; // Reset experience flag so it can start again
    }

    public void OnTrackingEncontrado()
    {
        if (textoInstruccion != null) 
        {
            textoInstruccion.text = "¡Robot detectado! Toca al robot para interactuar o abrir el menú.";
        }
        if (panelInstruccion != null) panelInstruccion.SetActive(true);
        Invoke("OcultarInstruccion", 5.0f);
    }

    public void OnTrackingPerdido()
    {
        if (textoInstruccion != null)
        {
            textoInstruccion.text = "Apunta con tu cámara a la imagen de referencia para ver al robot.";
        }
        if (panelInstruccion != null) panelInstruccion.SetActive(true);
    }
}
