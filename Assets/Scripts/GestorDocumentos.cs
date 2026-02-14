using UnityEngine;
using System;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

public class GestorDocumentos : MonoBehaviour
{
    // Evento para avisar cuando el texto esté listo
    public event Action<string> OnTextoCargado;

    [Header("Configuración")]
    [Tooltip("Límite de caracteres para enviar a la IA")]
    public int limiteCaracteres = 8000;

    /// <summary>
    /// Abre el explorador de archivos nativo en Android/iOS.
    /// Filtra solo archivos de texto plano.
    /// </summary>
    public void AbrirExplorador()
    {
        // Filtro para archivos de textos y PDFs
        string[] fileTypes = { "text/plain", "application/pdf" };

        // Llamada al Plugin Nativo (NativeFilePicker)
        // Nota: Asegúrate de tener instalado el plugin 'NativeFilePicker' de yasirkula
        NativeFilePicker.PickFile((path) =>
        {
            if (path == null)
            {
                Debug.Log("GestorDocumentos: Operación cancelada por el usuario.");
            }
            else
            {
                Debug.Log("GestorDocumentos: Archivo seleccionado: " + path);
                ProcesarArchivo(path);
            }
        }, fileTypes);

        // Debug.Log("GestorDocumentos: Permiso de archivos: " + permission);
    }

    private void ProcesarArchivo(string path)
    {
        try
        {
            string contenido = "";

            // Detectar si es PDF
            if (path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                using (PdfReader reader = new PdfReader(path))
                {
                    StringBuilder text = new StringBuilder();

                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                        text.Append("\n"); // Separador de página
                    }
                    contenido = text.ToString();
                }
            }
            else
            {
                // Asumimos archivo de texto plano
                contenido = File.ReadAllText(path);
            }
            
            Debug.Log($"GestorDocumentos: Texto leído ({contenido.Length} caracteres).");

            // Truncar si excede el límite
            if (contenido.Length > limiteCaracteres)
            {
                contenido = contenido.Substring(0, limiteCaracteres);
                Debug.LogWarning($"GestorDocumentos: Texto truncado a {limiteCaracteres} caracteres.");
            }

            // Notificar a los suscriptores (ej. SesionAR)
            OnTextoCargado?.Invoke(contenido);
        }
        catch (Exception e)
        {
            Debug.LogError($"GestorDocumentos Error al leer archivo: {e.Message}");
            // Opcional: Notificar error o manejarlo en UI
        }
    }
}
