using UnityEngine;
using UnityEngine.EventSystems;

public class RobotInput : MonoBehaviour, IPointerClickHandler
{
    private RobotAsistente robotAsistente;

    private void Start()
    {
        robotAsistente = GetComponent<RobotAsistente>();
        if (robotAsistente == null)
        {
            Debug.LogError("RobotInput: No se encontró el componente RobotAsistente en este objeto.");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (robotAsistente != null)
        {
            Debug.Log("RobotInput: Click detectado en el robot.");
            robotAsistente.AlternarMenuAyuda();
        }
    }
}
