using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Creador Automático de Panel de Instrucciones AR
/// 
/// INSTRUCCIONES:
/// 1. Copia este script en Assets/Editor/
/// 2. Ve a: Tools → Create AR Instructions Panel → Create Complete Panel
/// 3. El panel incluirá:
///    - Diseño moderno con bordes redondeados
///    - Título
///    - Lista de instrucciones paso a paso
///    - Botón de volver
///    - Scroll automático
/// </summary>
public class AutoCreateInstructionsPanel : MonoBehaviour
{
#if UNITY_EDITOR
    
    [MenuItem("Tools/Create AR Instructions Panel/Create Complete Panel")]
    public static void CreateInstructionsPanel()
    {
        Debug.Log("🎨 ═══════════════════════════════════════════════════════");
        Debug.Log("🎨 CREANDO PANEL DE INSTRUCCIONES AUTOMÁTICAMENTE");
        Debug.Log("🎨 ═══════════════════════════════════════════════════════");
        
        // ═══════════════════════════════════════════════════════
        // 1. CREAR PANEL PRINCIPAL (OVERLAY)
        // ═══════════════════════════════════════════════════════
        GameObject instructionsPanel = new GameObject("InstructionsPanel");
        Canvas canvas = instructionsPanel.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Siempre en frente
        
        CanvasScaler scaler = instructionsPanel.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        GraphicRaycaster raycaster = instructionsPanel.AddComponent<GraphicRaycaster>();
        
        RectTransform panelRect = instructionsPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Debug.Log("  ✅ Canvas principal creado");
        
        // ═══════════════════════════════════════════════════════
        // 2. FONDO SEMI-TRANSPARENTE (Background Overlay)
        // ═══════════════════════════════════════════════════════
        GameObject background = new GameObject("Background");
        background.transform.SetParent(instructionsPanel.transform, false);
        
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.7f); // Negro semi-transparente
        
        Button bgButton = background.AddComponent<Button>();
        bgButton.transition = Selectable.Transition.None; // Sin interacción visual
        
        Debug.Log("  ✅ Fondo semi-transparente creado");
        
        // ═══════════════════════════════════════════════════════
        // 3. CONTENEDOR PRINCIPAL (Panel Blanco)
        // ═══════════════════════════════════════════════════════
        GameObject mainContainer = new GameObject("MainContainer");
        mainContainer.transform.SetParent(instructionsPanel.transform, false);
        
        RectTransform containerRect = mainContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(900f, 1400f);
        
        Image containerImage = mainContainer.AddComponent<Image>();
        containerImage.color = Color.white;
        
        // Bordes redondeados para el contenedor
        Texture2D containerTexture = CreateRoundedRectTexture(200, 200, 40);
        Sprite containerSprite = Sprite.Create(
            containerTexture,
            new Rect(0, 0, containerTexture.width, containerTexture.height),
            new Vector2(0.5f, 0.5f),
            100, 0, SpriteMeshType.FullRect,
            new Vector4(40, 40, 40, 40)
        );
        containerImage.sprite = containerSprite;
        containerImage.type = Image.Type.Sliced;
        
        // Layout vertical para el contenedor principal
        VerticalLayoutGroup containerLayout = mainContainer.AddComponent<VerticalLayoutGroup>();
        containerLayout.padding = new RectOffset(40, 40, 40, 40);
        containerLayout.spacing = 30f;
        containerLayout.childAlignment = TextAnchor.UpperCenter;
        containerLayout.childControlWidth = true;
        containerLayout.childControlHeight = false;
        containerLayout.childForceExpandWidth = true;
        containerLayout.childForceExpandHeight = false;
        
        Debug.Log("  ✅ Contenedor principal creado");
        
        // ═══════════════════════════════════════════════════════
        // 4. HEADER (Título + Botón Cerrar)
        // ═══════════════════════════════════════════════════════
        GameObject header = new GameObject("Header");
        header.transform.SetParent(mainContainer.transform, false);
        
        RectTransform headerRect = header.AddComponent<RectTransform>();
        headerRect.sizeDelta = new Vector2(0f, 80f);
        
        HorizontalLayoutGroup headerLayout = header.AddComponent<HorizontalLayoutGroup>();
        headerLayout.childAlignment = TextAnchor.MiddleCenter;
        headerLayout.childControlWidth = true;
        headerLayout.childControlHeight = true;
        headerLayout.childForceExpandWidth = true;
        headerLayout.childForceExpandHeight = false;
        headerLayout.spacing = 20f;
        
        // Botón VOLVER (Izquierda)
        GameObject backButton = CreateButton(
            "BackButton",
            "← Volver",
            new Color(0.95f, 0.95f, 0.95f, 1f),
            new Color(0.2f, 0.2f, 0.2f, 1f),
            22f,
            new Vector2(200f, 80f)
        );
        backButton.transform.SetParent(header.transform, false);
        
        LayoutElement backLayoutElement = backButton.AddComponent<LayoutElement>();
        backLayoutElement.preferredWidth = 200f;
        backLayoutElement.flexibleWidth = 0f;
        
        // Título
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(header.transform, false);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "📖 INSTRUCCIONES";
        titleText.fontSize = 32f;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        // Spacer (para balancear el diseño)
        GameObject spacer = new GameObject("Spacer");
        spacer.transform.SetParent(header.transform, false);
        RectTransform spacerRect = spacer.AddComponent<RectTransform>();
        spacerRect.sizeDelta = new Vector2(200f, 80f);
        
        LayoutElement spacerLayoutElement = spacer.AddComponent<LayoutElement>();
        spacerLayoutElement.preferredWidth = 200f;
        spacerLayoutElement.flexibleWidth = 0f;
        
        Debug.Log("  ✅ Header con botón volver creado");
        
        // ═══════════════════════════════════════════════════════
        // 5. SCROLL VIEW (Contenido)
        // ═══════════════════════════════════════════════════════
        GameObject scrollView = new GameObject("ScrollView");
        scrollView.transform.SetParent(mainContainer.transform, false);
        
        RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
        
        LayoutElement scrollLayoutElement = scrollView.AddComponent<LayoutElement>();
        scrollLayoutElement.flexibleHeight = 1f;
        
        ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 40f;
        
        // Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform, false);
        
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        
        Image viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = new Color(1f, 1f, 1f, 0.01f);
        
        Mask viewportMask = viewport.AddComponent<Mask>();
        viewportMask.showMaskGraphic = false;
        
        scroll.viewport = viewportRect;
        
        // Content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.sizeDelta = new Vector2(0f, 0f);
        
        VerticalLayoutGroup contentLayout = content.AddComponent<VerticalLayoutGroup>();
        contentLayout.spacing = 25f;
        contentLayout.padding = new RectOffset(20, 20, 20, 20);
        contentLayout.childAlignment = TextAnchor.UpperLeft;
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = false;
        contentLayout.childForceExpandWidth = true;
        contentLayout.childForceExpandHeight = false;
        
        ContentSizeFitter contentFitter = content.AddComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        scroll.content = contentRect;
        
        Debug.Log("  ✅ Scroll View configurado");
        
        // ═══════════════════════════════════════════════════════
        // 6. AGREGAR INSTRUCCIONES
        // ═══════════════════════════════════════════════════════
        
        // Subtítulo
        CreateInstructionText(
            content.transform,
            "Cómo usar la aplicación AR:",
            28f,
            FontStyles.Bold,
            new Color(0.3f, 0.3f, 0.3f, 1f)
        );
        
        // Instrucciones
        string[] instructions = new string[]
        {
            "1️⃣ DETECTAR SUPERFICIE\n<size=18>Apunta la cámara hacia el suelo o una superficie plana. Verás puntos blancos que indican que la superficie fue detectada.</size>",
            
            "2️⃣ SELECCIONAR MODELO\n<size=18>Presiona el botón 'Seleccionar Modelo' y elige la enfermedad ocular que deseas visualizar en 3D.</size>",
            
            "3️⃣ COLOCAR MODELO\n<size=18>Toca la pantalla sobre la superficie detectada para colocar el modelo 3D en el espacio AR.</size>",
            
            "4️⃣ MOVER MODELO\n<size=18>Usa <b>un dedo</b> para arrastrar el modelo a diferentes posiciones. Puedes moverlo horizontal y verticalmente.</size>",
            
            "5️⃣ ROTAR Y HACER ZOOM\n<size=18>Usa <b>dos dedos</b> para:\n• <b>Rotar:</b> Gira los dedos en círculo\n• <b>Zoom:</b> Pellizca (pinch) para acercar o alejar</size>",
            
            "6️⃣ CAPTURAR PANTALLA\n<size=18>Presiona el botón de cámara para tomar una foto del modelo en AR y compartirla.</size>",
            
            "7️⃣ OBTENER INFORMACIÓN\n<size=18>Si deseas más información sobre la enfermedad, usa el asistente de IA médica disponible en el menú.</size>"
        };
        
        foreach (string instruction in instructions)
        {
            CreateInstructionCard(content.transform, instruction);
        }
        
        Debug.Log("  ✅ Instrucciones agregadas");
        
        // ═══════════════════════════════════════════════════════
        // 7. NOTA FINAL
        // ═══════════════════════════════════════════════════════
        GameObject noteCard = CreateNoteCard(
            content.transform,
            "💡 CONSEJO",
            "Para una mejor experiencia, usa la aplicación en un lugar bien iluminado y con superficies planas y texturizadas."
        );
        
        Debug.Log("  ✅ Nota final agregada");
        
        // ═══════════════════════════════════════════════════════
        // 8. GUARDAR EN LA ESCENA
        // ═══════════════════════════════════════════════════════
        
        // Guardar texturas
        string folderPath = "Assets/UI/InstructionsPanel";
        if (!AssetDatabase.IsValidFolder("Assets/UI"))
            AssetDatabase.CreateFolder("Assets", "UI");
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets/UI", "InstructionsPanel");
        
        AssetDatabase.CreateAsset(containerTexture, $"{folderPath}/RoundedTexture.asset");
        AssetDatabase.CreateAsset(containerSprite, $"{folderPath}/RoundedSprite.asset");
        AssetDatabase.SaveAssets();
        
        // Desactivar por defecto (MenuController lo activará)
        instructionsPanel.SetActive(false);
        
        Debug.Log("═══════════════════════════════════════════════════════");
        Debug.Log("✅✅✅ PANEL DE INSTRUCCIONES CREADO CON ÉXITO");
        Debug.Log("Ubicación: Jerarquía de la escena");
        Debug.Log("Estado: Inactivo (MenuController lo activará)");
        Debug.Log("═══════════════════════════════════════════════════════");
        
        Selection.activeGameObject = instructionsPanel;
        EditorGUIUtility.PingObject(instructionsPanel);
        
        EditorUtility.DisplayDialog(
            "Panel Creado ✅",
            "El Panel de Instrucciones ha sido creado exitosamente!\n\n" +
            "Características:\n" +
            "✅ Diseño moderno con bordes redondeados\n" +
            "✅ Scroll automático\n" +
            "✅ 7 instrucciones paso a paso\n" +
            "✅ Botón 'Volver' incluido\n" +
            "✅ Nota de consejo final\n\n" +
            "Siguiente paso:\n" +
            "Conecta el botón 'Volver' en el Inspector a tu MenuController",
            "Entendido"
        );
    }
    
    // ═══════════════════════════════════════════════════════
    // HELPER: CREAR BOTÓN
    // ═══════════════════════════════════════════════════════
    private static GameObject CreateButton(string name, string text, Color bgColor, Color textColor, float fontSize, Vector2 size)
    {
        GameObject button = new GameObject(name);
        
        RectTransform btnRect = button.AddComponent<RectTransform>();
        btnRect.sizeDelta = size;
        
        Image btnImage = button.AddComponent<Image>();
        btnImage.color = bgColor;
        
        // Bordes redondeados
        Texture2D btnTexture = CreateRoundedRectTexture(200, 200, 25);
        Sprite btnSprite = Sprite.Create(
            btnTexture,
            new Rect(0, 0, btnTexture.width, btnTexture.height),
            new Vector2(0.5f, 0.5f),
            100, 0, SpriteMeshType.FullRect,
            new Vector4(25, 25, 25, 25)
        );
        btnImage.sprite = btnSprite;
        btnImage.type = Image.Type.Sliced;
        
        Button btn = button.AddComponent<Button>();
        ColorBlock colors = btn.colors;
        colors.normalColor = bgColor;
        colors.highlightedColor = new Color(bgColor.r * 0.9f, bgColor.g * 0.9f, bgColor.b * 0.9f, 1f);
        colors.pressedColor = new Color(bgColor.r * 0.8f, bgColor.g * 0.8f, bgColor.b * 0.8f, 1f);
        btn.colors = colors;
        
        // Texto del botón
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = fontSize;
        tmpText.color = textColor;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.fontStyle = FontStyles.Bold;
        
        return button;
    }
    
    // ═══════════════════════════════════════════════════════
    // HELPER: CREAR TEXTO DE INSTRUCCIÓN
    // ═══════════════════════════════════════════════════════
    private static GameObject CreateInstructionText(Transform parent, string text, float fontSize, FontStyles style, Color color)
    {
        GameObject textObj = new GameObject("InstructionText");
        textObj.transform.SetParent(parent, false);
        
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = fontSize;
        tmpText.fontStyle = style;
        tmpText.color = color;
        tmpText.alignment = TextAlignmentOptions.Left;
        tmpText.textWrappingMode = TextWrappingModes.Normal;
        tmpText.richText = true;
        
        LayoutElement layoutElement = textObj.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = -1;
        
        return textObj;
    }
    
    // ═══════════════════════════════════════════════════════
    // HELPER: CREAR TARJETA DE INSTRUCCIÓN
    // ═══════════════════════════════════════════════════════
    private static GameObject CreateInstructionCard(Transform parent, string text)
    {
        GameObject card = new GameObject("InstructionCard");
        card.transform.SetParent(parent, false);
        
        RectTransform cardRect = card.AddComponent<RectTransform>();
        
        Image cardImage = card.AddComponent<Image>();
        cardImage.color = new Color(0.96f, 0.97f, 0.98f, 1f);
        
        // Bordes redondeados
        Texture2D cardTexture = CreateRoundedRectTexture(200, 200, 15);
        Sprite cardSprite = Sprite.Create(
            cardTexture,
            new Rect(0, 0, cardTexture.width, cardTexture.height),
            new Vector2(0.5f, 0.5f),
            100, 0, SpriteMeshType.FullRect,
            new Vector4(15, 15, 15, 15)
        );
        cardImage.sprite = cardSprite;
        cardImage.type = Image.Type.Sliced;
        
        VerticalLayoutGroup cardLayout = card.AddComponent<VerticalLayoutGroup>();
        cardLayout.padding = new RectOffset(20, 20, 15, 15);
        cardLayout.childAlignment = TextAnchor.UpperLeft;
        cardLayout.childControlWidth = true;
        cardLayout.childControlHeight = true;
        cardLayout.childForceExpandWidth = true;
        cardLayout.childForceExpandHeight = false;
        
        ContentSizeFitter cardFitter = card.AddComponent<ContentSizeFitter>();
        cardFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // Texto
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(card.transform, false);
        
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = 20f;
        tmpText.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        tmpText.alignment = TextAlignmentOptions.Left;
        tmpText.textWrappingMode = TextWrappingModes.Normal;
        tmpText.richText = true;
        
        return card;
    }
    
    // ═══════════════════════════════════════════════════════
    // HELPER: CREAR NOTA CARD
    // ═══════════════════════════════════════════════════════
    private static GameObject CreateNoteCard(Transform parent, string title, string text)
    {
        GameObject card = new GameObject("NoteCard");
        card.transform.SetParent(parent, false);
        
        RectTransform cardRect = card.AddComponent<RectTransform>();
        
        Image cardImage = card.AddComponent<Image>();
        cardImage.color = new Color(1f, 0.95f, 0.7f, 1f); // Amarillo suave
        
        // Bordes redondeados
        Texture2D cardTexture = CreateRoundedRectTexture(200, 200, 15);
        Sprite cardSprite = Sprite.Create(
            cardTexture,
            new Rect(0, 0, cardTexture.width, cardTexture.height),
            new Vector2(0.5f, 0.5f),
            100, 0, SpriteMeshType.FullRect,
            new Vector4(15, 15, 15, 15)
        );
        cardImage.sprite = cardSprite;
        cardImage.type = Image.Type.Sliced;
        
        VerticalLayoutGroup cardLayout = card.AddComponent<VerticalLayoutGroup>();
        cardLayout.padding = new RectOffset(20, 20, 15, 15);
        cardLayout.spacing = 10f;
        cardLayout.childAlignment = TextAnchor.UpperLeft;
        cardLayout.childControlWidth = true;
        cardLayout.childControlHeight = true;
        cardLayout.childForceExpandWidth = true;
        cardLayout.childForceExpandHeight = false;
        
        ContentSizeFitter cardFitter = card.AddComponent<ContentSizeFitter>();
        cardFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // Título
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(card.transform, false);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = title;
        titleText.fontSize = 22f;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = new Color(0.6f, 0.4f, 0f, 1f);
        titleText.alignment = TextAlignmentOptions.Left;
        
        // Texto
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(card.transform, false);
        
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = 18f;
        tmpText.color = new Color(0.3f, 0.2f, 0f, 1f);
        tmpText.alignment = TextAlignmentOptions.Left;
        tmpText.textWrappingMode = TextWrappingModes.Normal;
        
        return card;
    }
    
    // ═══════════════════════════════════════════════════════
    // HELPER: CREAR TEXTURA CON BORDES REDONDEADOS
    // ═══════════════════════════════════════════════════════
    private static Texture2D CreateRoundedRectTexture(int width, int height, int radius)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];
        
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool inRoundedRect = false;
                
                if (x < radius && y > height - radius)
                {
                    float dx = radius - x;
                    float dy = (height - radius) - y;
                    inRoundedRect = (dx * dx + dy * dy) <= (radius * radius);
                }
                else if (x > width - radius && y > height - radius)
                {
                    float dx = x - (width - radius);
                    float dy = (height - radius) - y;
                    inRoundedRect = (dx * dx + dy * dy) <= (radius * radius);
                }
                else if (x < radius && y < radius)
                {
                    float dx = radius - x;
                    float dy = radius - y;
                    inRoundedRect = (dx * dx + dy * dy) <= (radius * radius);
                }
                else if (x > width - radius && y < radius)
                {
                    float dx = x - (width - radius);
                    float dy = radius - y;
                    inRoundedRect = (dx * dx + dy * dy) <= (radius * radius);
                }
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
    
#endif
}