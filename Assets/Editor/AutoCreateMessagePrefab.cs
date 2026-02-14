using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Creador de Prefabs de Mensajes V3 - Con Bordes Redondeados y Ancho Adaptable
/// 
/// INSTRUCCIONES:
/// 1. Copia este script en Assets/Editor/
/// 2. Ve a: Tools → Create Message Prefabs V3 → Create BOTH Prefabs
/// 3. Los prefabs tendrán:
///    - Mensajes de usuario: Ancho adaptable (más pequeños)
///    - Mensajes de IA: Ancho completo
///    - Ambos con bordes redondeados
/// </summary>
public class AutoCreateMessagePrefabV3 : MonoBehaviour
{
#if UNITY_EDITOR
    
    [MenuItem("Tools/Create Message Prefabs V3/1. User Message Prefab (Adaptable)")]
    public static void CreateUserMessagePrefab()
    {
        CreateMessagePrefab(
            "UserMessagePrefab",
            new Color(0.27f, 0.51f, 0.9f, 1f), // Azul
            Color.white,
            "Mensaje de ejemplo del usuario",
            true, // Ancho adaptable
            TextAlignmentOptions.TopRight // Alineado a la derecha
        );
    }
    
    [MenuItem("Tools/Create Message Prefabs V3/2. AI Message Prefab (Full Width)")]
    public static void CreateAIMessagePrefab()
    {
        CreateMessagePrefab(
            "AIMessagePrefab",
            new Color(0.94f, 0.94f, 0.94f, 1f), // Gris claro
            new Color(0.1f, 0.1f, 0.1f, 1f), // Negro
            "Respuesta de ejemplo de la IA médica. Este mensaje puede ser largo y ocupará todo el ancho disponible para mostrar información detallada.",
            false, // Ancho completo
            TextAlignmentOptions.TopLeft // Alineado a la izquierda
        );
    }
    
    private static void CreateMessagePrefab(string prefabName, Color bgColor, Color textColor, string sampleText, bool adaptiveWidth, TextAlignmentOptions alignment)
    {
        Debug.Log($"🔨 Creando prefab: {prefabName}");
        
        // ═══════════════════════════════════════════════════════
        // 1. CREAR CONTENEDOR PRINCIPAL (para alineación)
        // ═══════════════════════════════════════════════════════
        GameObject messagePrefab = new GameObject(prefabName);
        RectTransform containerRect = messagePrefab.AddComponent<RectTransform>();
        
        // Anchors: Top-Stretch
        containerRect.anchorMin = new Vector2(0f, 1f);
        containerRect.anchorMax = new Vector2(1f, 1f);
        containerRect.pivot = new Vector2(0.5f, 1f);
        containerRect.offsetMin = new Vector2(10f, -100f); // Left, Bottom
        containerRect.offsetMax = new Vector2(-10f, 0f);   // Right, Top
        
        // Layout para el contenedor
        HorizontalLayoutGroup containerLayout = messagePrefab.AddComponent<HorizontalLayoutGroup>();
        containerLayout.childAlignment = adaptiveWidth ? TextAnchor.UpperRight : TextAnchor.UpperLeft;
        containerLayout.childControlWidth = !adaptiveWidth;
        containerLayout.childControlHeight = true;
        containerLayout.childForceExpandWidth = !adaptiveWidth;
        containerLayout.childForceExpandHeight = false;
        
        ContentSizeFitter containerFitter = messagePrefab.AddComponent<ContentSizeFitter>();
        containerFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        containerFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        Debug.Log("  ✅ Contenedor principal configurado");
        
        // ═══════════════════════════════════════════════════════
        // 2. CREAR BURBUJA DE MENSAJE (hijo)
        // ═══════════════════════════════════════════════════════
        GameObject bubble = new GameObject("Bubble");
        bubble.transform.SetParent(messagePrefab.transform, false);
        
        RectTransform bubbleRect = bubble.AddComponent<RectTransform>();
        bubbleRect.anchorMin = new Vector2(0f, 0f);
        bubbleRect.anchorMax = new Vector2(0f, 0f);
        bubbleRect.pivot = new Vector2(0.5f, 0.5f);
        
        // ═══════════════════════════════════════════════════════
        // 3. AGREGAR IMAGE CON BORDES REDONDEADOS
        // ═══════════════════════════════════════════════════════
        Image backgroundImage = bubble.AddComponent<Image>();
        backgroundImage.color = bgColor;
        backgroundImage.raycastTarget = false;
        
        // Crear sprite con bordes redondeados
        Texture2D roundedTexture = CreateRoundedRectTexture(200, 200, 30);
        Sprite roundedSprite = Sprite.Create(
            roundedTexture,
            new Rect(0, 0, roundedTexture.width, roundedTexture.height),
            new Vector2(0.5f, 0.5f),
            100,
            0,
            SpriteMeshType.FullRect,
            new Vector4(30, 30, 30, 30) // Bordes
        );
        
        backgroundImage.sprite = roundedSprite;
        backgroundImage.type = Image.Type.Sliced;
        
        Debug.Log("  ✅ Image con bordes redondeados agregada");
        
        // ═══════════════════════════════════════════════════════
        // 4. LAYOUT DE LA BURBUJA
        // ═══════════════════════════════════════════════════════
        VerticalLayoutGroup bubbleLayout = bubble.AddComponent<VerticalLayoutGroup>();
        bubbleLayout.padding = new RectOffset(15, 15, 12, 12);
        bubbleLayout.spacing = 0f;
        bubbleLayout.childAlignment = TextAnchor.UpperLeft;
        bubbleLayout.childControlWidth = true;
        bubbleLayout.childControlHeight = true;
        bubbleLayout.childForceExpandWidth = true;
        bubbleLayout.childForceExpandHeight = false;
        
        ContentSizeFitter bubbleFitter = bubble.AddComponent<ContentSizeFitter>();
        bubbleFitter.horizontalFit = adaptiveWidth ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
        bubbleFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        LayoutElement bubbleLayoutElement = bubble.AddComponent<LayoutElement>();
        bubbleLayoutElement.minHeight = 50f;
        
        if (adaptiveWidth)
        {
            bubbleLayoutElement.minWidth = 100f;
            bubbleLayoutElement.preferredWidth = 400f; // Máximo ancho para mensajes de usuario
        }
        
        Debug.Log("  ✅ Layout de burbuja configurado");
        
        // ═══════════════════════════════════════════════════════
        // 5. CREAR TEXTO
        // ═══════════════════════════════════════════════════════
        GameObject textObject = new GameObject("MessageText");
        textObject.transform.SetParent(bubble.transform, false);
        
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 0f);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI tmpText = textObject.AddComponent<TextMeshProUGUI>();
        tmpText.text = sampleText;
        tmpText.fontSize = 16f;
        tmpText.color = textColor;
        tmpText.alignment = adaptiveWidth ? TextAlignmentOptions.TopRight : TextAlignmentOptions.TopLeft;
   tmpText.textWrappingMode = TextWrappingModes.Normal;
        tmpText.overflowMode = TextOverflowModes.Overflow;
        tmpText.raycastTarget = false;
        tmpText.richText = true;
        tmpText.horizontalMapping = TextureMappingOptions.Character;
        tmpText.verticalMapping = TextureMappingOptions.Line;
        
        Debug.Log("  ✅ TextMeshProUGUI configurado");
        
        // ═══════════════════════════════════════════════════════
        // 6. GUARDAR PREFAB
        // ═══════════════════════════════════════════════════════
        string folderPath = "Assets/Prefabs/ChatMessages";
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets/Prefabs", "ChatMessages");
        
        string prefabPath = $"{folderPath}/{prefabName}.prefab";
        
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            AssetDatabase.DeleteAsset(prefabPath);
            Debug.Log($"  🗑️  Prefab anterior eliminado");
        }
        
        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(messagePrefab, prefabPath);
        DestroyImmediate(messagePrefab);
        
        // Guardar el sprite en el proyecto
        string spritePath = $"{folderPath}/RoundedSprite_{prefabName}.asset";
        AssetDatabase.CreateAsset(roundedSprite, spritePath);
        AssetDatabase.CreateAsset(roundedTexture, $"{folderPath}/RoundedTexture_{prefabName}.asset");
        AssetDatabase.SaveAssets();
        
        Debug.Log($"✅✅✅ PREFAB CREADO: {prefabPath}");
        
        Selection.activeObject = savedPrefab;
        EditorGUIUtility.PingObject(savedPrefab);
        
        EditorUtility.DisplayDialog(
            "Prefab Creado",
            $"El prefab '{prefabName}' ha sido creado con:\n\n" +
            $"- Bordes redondeados ✅\n" +
            $"- Ancho: {(adaptiveWidth ? "Adaptable" : "Completo")} ✅\n" +
            $"- Ubicación: {prefabPath}",
            "OK"
        );
    }
    
    // ═══════════════════════════════════════════════════════
    // CREAR TEXTURA CON BORDES REDONDEADOS
    // ═══════════════════════════════════════════════════════
    private static Texture2D CreateRoundedRectTexture(int width, int height, int radius)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];
        
        // Llenar con transparente
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;
        
        // Dibujar rectángulo con bordes redondeados
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool inRoundedRect = false;
                
                // Esquina superior izquierda
                if (x < radius && y > height - radius)
                {
                    float dx = radius - x;
                    float dy = (height - radius) - y;
                    inRoundedRect = (dx * dx + dy * dy) <= (radius * radius);
                }
                // Esquina superior derecha
                else if (x > width - radius && y > height - radius)
                {
                    float dx = x - (width - radius);
                    float dy = (height - radius) - y;
                    inRoundedRect = (dx * dx + dy * dy) <= (radius * radius);
                }
                // Esquina inferior izquierda
                else if (x < radius && y < radius)
                {
                    float dx = radius - x;
                    float dy = radius - y;
                    inRoundedRect = (dx * dx + dy * dy) <= (radius * radius);
                }
                // Esquina inferior derecha
                else if (x > width - radius && y < radius)
                {
                    float dx = x - (width - radius);
                    float dy = radius - y;
                    inRoundedRect = (dx * dx + dy * dy) <= (radius * radius);
                }
                // Centro del rectángulo
                else if (x >= radius && x <= width - radius || y >= radius && y <= height - radius)
                {
                    inRoundedRect = true;
                }
                
                if (inRoundedRect)
                {
                    pixels[y * width + x] = Color.white;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        
        return texture;
    }
    
    // ═══════════════════════════════════════════════════════
    // CREAR AMBOS PREFABS
    // ═══════════════════════════════════════════════════════
    [MenuItem("Tools/Create Message Prefabs V3/3. Create BOTH Prefabs")]
    public static void CreateBothPrefabs()
    {
        CreateUserMessagePrefab();
        CreateAIMessagePrefab();
        
        Debug.Log("═══════════════════════════════════════════════════════");
        Debug.Log("✅✅✅ AMBOS PREFABS CREADOS CON ÉXITO");
        Debug.Log("Características:");
        Debug.Log("  - Mensajes de usuario: Ancho adaptable (más pequeños)");
        Debug.Log("  - Mensajes de IA: Ancho completo");
        Debug.Log("  - Ambos con bordes redondeados");
        Debug.Log("═══════════════════════════════════════════════════════");
    }
    
#endif
}