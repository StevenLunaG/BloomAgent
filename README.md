# 🧠 Bloom Agent: Asistente AR de Aprendizaje Cognitivo

![Unity](https://img.shields.io/badge/Unity-2022.3%20LTS-black?style=flat&logo=unity)
![Platform](https://img.shields.io/badge/Platform-Android%20(ARCore)-green?style=flat&logo=android)
![AI](https://img.shields.io/badge/AI-Groq%20%7C%20Llama3-orange?style=flat&logo=openai)
![License](https://img.shields.io/badge/License-MIT-blue?style=flat)

> **Proyecto de Interacción Humano-Computadora (HCI)**
> Sistema de Realidad Aumentada móvil para el refuerzo del aprendizaje basado en la Taxonomía de Bloom (Niveles 1-4) mediante un asistente inteligente interactivo.

---

## 📖 Descripción del Proyecto

**Bloom Agent** es una aplicación de Realidad Aumentada (AR) diseñada para democratizar el aprendizaje inmersivo utilizando dispositivos móviles estándar. El sistema despliega un **Robot Asistente** y **Tarjetas Holográficas** sobre el escritorio físico del estudiante, transformando cualquier entorno en un espacio de estudio activo.

El núcleo del proyecto integra **Inteligencia Artificial Generativa** (vía Groq API) para leer documentos PDF proporcionados por el usuario y generar desafíos adaptativos en tiempo real, clasificados según los niveles cognitivos de Bloom: *Recordar, Comprender, Aplicar y Analizar*.

---

## 🚀 Características Principales

### 🤖 Dimensión IA (Cerebro Cognitivo)
* **Generación Procedural:** Uso de **Groq API (LPU)** con modelos **Llama-3-8b** o **Mixtral** para crear preguntas únicas en milisegundos.
* **Adaptabilidad:** El sistema evalúa las respuestas y ajusta la dificultad o el nivel de Bloom automáticamente.
* **Salida Estructurada:** Garantía de formato JSON para la integración perfecta con Unity.

### 📱 Dimensión AR (Entorno)
* **Detección de Planos:** Escaneo de superficies horizontales (mesas/escritorios) usando **AR Foundation**.
* **Anclaje Espacial:** Persistencia de objetos virtuales (Robot y UI) en coordenadas del mundo real para evitar el "deslizamiento".
* **Interfaz Dieléctrica:** Burbujas de texto y menús flotantes integrados en el espacio 3D (World Space Canvas).

### 👆 Dimensión HCI (Interacción)
* **Raycasting Táctil:** Interacción mediante "Taps" en pantalla traducidos a coordenadas 3D, minimizando la carga cognitiva.
* **Feedback Multimodal:** Respuesta visual (partículas/colores), animaciones del robot (celebración/pensar) y respuesta háptica (vibración).
* **Usabilidad Móvil:** Diseñado para sesiones de micro-aprendizaje (3-5 min) para evitar fatiga física ("Gorilla Arm").

---

## 🛠️ Stack Tecnológico

* **Motor:** Unity 2022.3 LTS (Universal Render Pipeline - URP).
* **AR Framework:** AR Foundation 5.x + Vuforia Engine.
* **Lenguaje:** C# (Scripting lógico y conexión API).
* **Inteligencia Artificial:** Groq API (RESTful architecture).
* **Formato de Datos:** JSON (Newtonsoft.Json).

---

## 🏗️ Arquitectura del Sistema

El sistema opera bajo una arquitectura de **Cliente Pesado (Thick Client)** con inteligencia externalizada:

1.  **Capa de Percepción (Unity):** Gestiona la cámara, detección de planos y renderizado.
2.  **Capa de Control (C#):** Administra el estado de la sesión y las interacciones táctiles.
3.  **Capa Cognitiva (Groq Cloud):** Recibe el contexto (texto del PDF) + Nivel Bloom y retorna el objeto JSON.

---

## 📋 Requisitos de Instalación

### Hardware (Dispositivo de Despliegue)
* **Dispositivo:** Smartphone Android.
* **OS:** Android 10.0 (API Nivel 29) o superior.
* **Soporte:** Compatibilidad con cualquier dispositivo como camara (Vuforia Engine).
* **Sensores:** Cámara, Giroscopio y Acelerómetro.

### Entorno de Desarrollo (Para editar)
* Unity Hub & Unity 2022.3.x
* Módulo de soporte de compilación para Android.
* API Key válida de [Groq Cloud](https://console.groq.com/).

---

## ⚙️ Configuración y Uso

1.  **Clonar el repositorio:**
    ```bash
    git clone [https://github.com/StevenLunaG/BloomAgent.git](https://github.com/StevenLunaG/BloomAgent.git)
    ```
2.  **Configurar API Key:**
    * Navega a `Assets/Scripts/Managers/GroqClient.cs`.
    * Inserta tu API Key en la variable `private string apiKey = "TU_API_KEY";`.
3.  **Build:**
    * En Unity, ve a `File > Build Settings`.
    * Cambia la plataforma a **Android**.
    * Selecciona tu dispositivo y da clic en **Build and Run**.

### Guía de Usuario
1.  **Escaneo:** Mueve el móvil suavemente de lado a lado para detectar tu escritorio (verás puntos guía).
2.  **Anclaje:** Toca la pantalla cuando aparezca el indicador visual para "llamar" al Robot.
3.  **Carga:** Selecciona un documento PDF de prueba desde el menú flotante.
4.  **Estudio:** Lee la tarjeta holográfica y toca la opción correcta. ¡Observa la reacción del robot!

---

## 📊 Resultados de Evaluación (HCI)

El prototipo fue validado con una muestra de **$n=11$ usuarios** utilizando la escala **SUS (System Usability Scale)**.

| Métrica | Resultado | Calificación |
| :--- | :---: | :--- |
| **Puntaje SUS Global** | **81.1 / 100** | **Excelencia (Grado A-)** |
| Intención de Uso | 4.5 / 5 | Muy Alta |
| Facilidad de Aprendizaje | 4.6 / 5 | Muy Alta |
| Coherencia IA | 91% | Aceptación positiva |

> *Conclusión:* El sistema es altamente usable y la IA genera contenido pedagógicamente pertinente. La principal área de mejora detectada es la optimización del escaneo de superficies inicial.

---

## 👥 Créditos

* **Autor:** Steven Ernesto Luna Gaona
* **Institución:** Universidad Nacional de Loja - Carrera de Computación.
* **Asignatura:** Human-Computer Interaction.
* **Docente:** Ing. Pablo F. Ordoñez O.
