using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Yaogan : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Vector2 vector, Starts;
    int Leng = 50;
    public bool ismove = false;
    public Action<Vector2> move;
    public void OnBeginDrag(PointerEventData eventData) 
    {
        Starts = transform.position;
        ismove = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        vector = eventData.position - Starts;
        transform.position = Vector2.ClampMagnitude(vector, Leng) + Starts;
        move?.Invoke(vector);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector2.zero;
        ismove = false;
    }
}
