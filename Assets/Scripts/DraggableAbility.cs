using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class DraggableAbility : MonoBehaviour
{
    public bool isDragged = false;


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnMouseDown() {
        isDragged = true;

        GameObject arrowEmitter = FindAnyObjectByType<GameManager>().ArrowEmitter;
        arrowEmitter.SetActive(true);
        arrowEmitter.transform.position = new Vector3(transform.position.x, transform.position.y, -3);
        Cursor.visible = false;
    }

    private void OnMouseDrag() {
        if (!isDragged) return;

    }

    private void OnMouseUp() {
        if (!isDragged) return;

        bool dropZoneValid = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider != null) {
            GameObject dropZone = hit.collider.gameObject;
            Debug.Log(dropZone.name);
            //// Contre attaque
            //if (dropZone.GetComponent<SlotDisplay>() != null) {
            //    dropZoneValid = dropZone.GetComponent<SlotDisplay>().onDrop(gameObject);
            //}
            //// Aura
            //else if (dropZone.GetComponent<AuraDisplay>() != null) {
            //    dropZoneValid = dropZone.GetComponent<AuraDisplay>().onDrop(gameObject);
            //}
            //// Enchantement
            //else if (dropZone.GetComponent<EquipmentDisplay>() != null) {
            //    dropZoneValid = dropZone.GetComponent<EquipmentDisplay>().onDrop(gameObject);
            //}
            //// Monster
            //else if (dropZone.GetComponent<MonsterDisplay>() != null) {
            //    dropZoneValid = dropZone.GetComponent<MonsterDisplay>().onDrop(gameObject);
            //}
            //// Card
            //else if (dropZone.GetComponent<CardDisplay>() != null) {
            //    dropZoneValid = dropZone.GetComponent<CardDisplay>().onDrop(gameObject);
            //}
        }

        GameManager.dragged = false;
                
        isDragged = false;
        FindAnyObjectByType<GameManager>().ArrowEmitter.SetActive(false);
        Cursor.visible = true;
    }
}