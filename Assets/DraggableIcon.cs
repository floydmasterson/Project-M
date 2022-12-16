using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableIcon : MonoBehaviour, IDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        canvas = gameObject.GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        transform.position = eventData.position;
    }
}
 
    
