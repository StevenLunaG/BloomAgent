using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using System.Collections;
using System; 

public class RobotAsistente : MonoBehaviour
{
    // Evento para notificar al sistema que se PIDIÓ la pista (para penalización)
    public event Action OnPistaSolicitada;

    [Header("References")]
    [Tooltip("Prefab for the speech bubble (AIMessagePrefab)")]
    public GameObject chatBubblePrefab;

    [Tooltip("Transform point above the robot's head for bubbles")]
    public Transform bubbleSpawnPoint;
    
    [Header("Menu Prefabs")]
    public GameObject botonPistaPrefab;
    public GameObject botonSalirPrefab;
    public GameObject botonEmpezarPrefab;

    [Header("Settings")]
    public float bubbleLifetime = 5.0f;

    private Camera mainCamera;
    private string pistaActual = "";
    private GameObject menuBubble; // Referencia al menú abierto actual
    
    // Study Mode State
    private bool esModoEstudio = false;
    private bool esperandoConfirmacion = false;
    public string[] frasesIntro = { "¡Vamos a iniciar la sesión!", "¿Tienes tu archivo listo?", "Será un gusto ayudarte a estudiar." };

    private void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(LookAtCameraRoutine());
    }

    /// <summary>
    /// Coroutine to make the robot (or specific parts) face the camera.
    /// Implements a smooth Y-axis billboard effect.
    /// </summary>
    private IEnumerator LookAtCameraRoutine()
    {
        while (true)
        {
            if (mainCamera != null)
            {
                // Calculate direction to camera, but keep only X and Z for Y-axis rotation
                Vector3 directionToCamera = mainCamera.transform.position - transform.position;
                directionToCamera.y = 0; // Flatten vector

                if (directionToCamera.sqrMagnitude > 0.001f) // Avoid zero vector warning
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
                    // Smoothly rotate towards camera
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                }
            }
            yield return null; // Wait for next frame
        }
    }

    /// <summary>
    /// Displays a speech bubble with the given text.
    /// </summary>
    /// <param name="texto">Text to display.</param>
    /// <param name="duration">Duration (< 0 for default).</param>
    public GameObject Decir(string texto, float duration = -1f)
    {
        Debug.Log("🤖 INTENTANDO HABLAR: " + texto);
    
        // Si hay un menú abierto, lo cerramos para hablar
        if (menuBubble != null) Destroy(menuBubble);

        // Limpiar cualquier burbuja anterior
        foreach (Transform child in bubbleSpawnPoint)
        {
            Destroy(child.gameObject);
        }

        if (chatBubblePrefab == null) 
        {
            Debug.LogError("Falta asignar el Chat Bubble Prefab");
            return null;
        }

        // 1. Instanciamos DIRECTAMENTE dentro del bubbleSpawnPoint
        GameObject bubble = Instantiate(chatBubblePrefab, bubbleSpawnPoint);

        // 2. Reseteamos la posición
        bubble.transform.localPosition = Vector3.zero;
        bubble.transform.localRotation = Quaternion.identity;
        
        // 3. Forzamos la escala a 1
        bubble.transform.localScale = Vector3.one; 

        // 4. Asignamos el texto
        TMPro.TMP_Text textComponent = bubble.GetComponentInChildren<TMPro.TMP_Text>();
        if (textComponent != null)
        {
            textComponent.text = texto;
        }

        // 5. Destruir si duration no es 0
        if (duration != 0)
        {
            float t = (duration < 0) ? bubbleLifetime : duration;
            Destroy(bubble, t);
        }

        return bubble;
    }

    public void GuardarPista(string pista)
    {
        pistaActual = pista;
    }

    public void ActivarModoEstudio()
    {
        esModoEstudio = true;
    }
    
    /// <summary>
    /// Llamado desde SesionAR al reiniciar o terminar.
    /// Resetea flags internos del robot.
    /// </summary>
    public void ResetearEstado()
    {
        esModoEstudio = false;
        esperandoConfirmacion = false;
        pistaActual = "";
    }

    /// <summary>
    /// Alterna el menú de ayuda (Pista / Salir) o dice una frase aleatoria si no está en modo estudio.
    /// </summary>
    public void AlternarMenuAyuda()
    {
        // 1. Bloqueo Prematuro: Si estamos esperando confirmación de inicio, NO hacer nada.
        // Esto evita que al tocar el robot se destruya el botón de "Empezar".
        if (esperandoConfirmacion) return;

        if (!esModoEstudio)
        {
            if (frasesIntro.Length > 0)
            {
                int index = UnityEngine.Random.Range(0, frasesIntro.Length);
                Decir(frasesIntro[index]);
            }
            return;
        }

        if (menuBubble != null)
        {
            // Si ya existe, lo cerramos (toggle off)
            Destroy(menuBubble);
            return;
        }

        // Limpiar cualquier burbuja anterior (incluso si no es menuBubble)
        foreach (Transform child in bubbleSpawnPoint)
        {
            Destroy(child.gameObject);
        }

        // Abrir menú
        // ... (rest of menu logic remains unchanged) ...
        Debug.Log("RobotAsistente: Abriendo menú de ayuda...");
        menuBubble = Instantiate(chatBubblePrefab, bubbleSpawnPoint);
        menuBubble.transform.localPosition = Vector3.zero;
        menuBubble.transform.localRotation = Quaternion.identity;
        menuBubble.transform.localScale = Vector3.one;

        // Buscar si el prefab trae un fondo de texto por defecto y ocultarlo
        Transform fondoTexto = menuBubble.transform.Find("Bubble");
        if (fondoTexto != null)
        {
            fondoTexto.gameObject.SetActive(false);
        }

        // Limpiar texto de la burbuja para que no estorbe a los botones
        var textComp = menuBubble.GetComponentInChildren<TMP_Text>();
        if (textComp != null) textComp.text = ""; 

        // Instanciar botón de Pista
        if (botonPistaPrefab != null)
        {
            GameObject objPista = Instantiate(botonPistaPrefab, menuBubble.transform);
            Button btnPista = objPista.GetComponent<Button>();
            if (btnPista != null) 
            {
                btnPista.onClick.AddListener(() => {
                    this.DecirPista(); 
                    // Nota: DecirPista llama a Decir(), que cierra el menú automáticamente.
                });
            }
        }

        // Instanciar botón de Salir
        if (botonSalirPrefab != null)
        {
            GameObject objSalir = Instantiate(botonSalirPrefab, menuBubble.transform);
            Button btnSalir = objSalir.GetComponent<Button>();
            
            // Buscar SesionAR para terminar sesión
            SesionAR sesion = FindFirstObjectByType<SesionAR>();
            if (btnSalir != null && sesion != null) 
            {
                btnSalir.onClick.AddListener(() => {
                     sesion.TerminarSesion();
                });
            }
        }
        
        // NO destruimos automáticamente el menú, espera input del usuario
        
        // Forzar actualización del layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(menuBubble.GetComponent<RectTransform>());
    }

    public void MostrarConfirmacionInicio(System.Action alConfirmar)
    {
        esperandoConfirmacion = true;

        // 1. Limpieza
        if (menuBubble != null) Destroy(menuBubble);
        foreach (Transform child in bubbleSpawnPoint) Destroy(child.gameObject);

        // 2. Instanciar Burbuja Base
        if (chatBubblePrefab != null)
        {
            menuBubble = Instantiate(chatBubblePrefab, bubbleSpawnPoint);
            menuBubble.transform.localPosition = Vector3.zero;
            menuBubble.transform.localRotation = Quaternion.identity;
            menuBubble.transform.localScale = Vector3.one;
        }
        else
        {
            Debug.LogError("RobotAsistente: Falta chatBubblePrefab");
            return;
        }

        // 3. Configurar Texto (IMPORTANTE: No ocultamos el fondo 'Bubble')
        var textComp = menuBubble.GetComponentInChildren<TMP_Text>();
        if (textComp != null)
        {
            textComp.text = "¿Listo para empezar la sesión de estudio?";
        }

        // 4. Instanciar Botón dentro de la misma burbuja (Vertical Layout)
        if (botonEmpezarPrefab != null)
        {
            GameObject btnObj = Instantiate(botonEmpezarPrefab);
            // El padre es menuBubble para que el VerticalLayoutGroup los apile
            btnObj.transform.SetParent(menuBubble.transform, false);
            btnObj.transform.localPosition = Vector3.zero;
            btnObj.transform.localRotation = Quaternion.identity;
            btnObj.transform.localScale = Vector3.one;

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => {
                    esperandoConfirmacion = false; // Desbloquear interacción
                    alConfirmar?.Invoke();
                    if (menuBubble != null) Destroy(menuBubble);
                });
            }
        }
        
        // 5. Forzar actualización visual inmediata
        LayoutRebuilder.ForceRebuildLayoutImmediate(menuBubble.GetComponent<RectTransform>());
    }

    public void DecirPista()
    {
        if (string.IsNullOrEmpty(pistaActual))
        {
            Decir("Hmm, no tengo ninguna pista para esto.");
        }
        else
        {
            Decir($"Pista: {pistaActual}");
            // Notificar que se usó la pista
            OnPistaSolicitada?.Invoke();
        }
    }
}
