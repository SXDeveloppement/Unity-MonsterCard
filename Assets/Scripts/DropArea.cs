using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropArea : MonoBehaviour, IDropHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IDropHandler.OnDrop(PointerEventData eventData) {
        Debug.Log(eventData.pointerDrag.name + " was dropper on " + gameObject.name);
        eventData.pointerDrag.GetComponent<ZoomCard>().changeWithPlaceholder();
    }
}
