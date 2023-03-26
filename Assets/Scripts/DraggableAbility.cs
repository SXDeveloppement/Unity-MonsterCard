using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class DraggableAbility : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isDragged = false;


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    // On peut activé la capacité activable avec un clique si elle n'a pas besoin de cible, qu'elle ne soit pas en cooldown et qu'elle ne soit pas possédée par l'adversaire
    public void OnPointerClick(PointerEventData eventData) {
        if (FindAnyObjectByType<GameManager>().playerAction != null) return;
        if (GetComponent<AbilityDisplay>().abilityStatus == AbilityStatus.TeamLayout) return;

        if (GetComponent<AbilityDisplay>().ability.abilityType == AbilityType.Active
            && GetComponent<AbilityDisplay>().ability.targetType.Length <= 0
            && GetComponent<AbilityDisplay>().cooldown <= 0
            && !GetComponent<OwnedByOppo>().monsterOwnThis.ownedByOppo) {
            GameManager.activeAbilityOnTarget(GetComponent<AbilityDisplay>(), GameManager.GO_MonsterInvoked);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (FindAnyObjectByType<GameManager>().playerAction != null) return;
        if (GetComponent<AbilityDisplay>().abilityStatus == AbilityStatus.TeamLayout) return;

        if (GetComponent<AbilityDisplay>().cooldown <= 0 && !GetComponent<OwnedByOppo>().monsterOwnThis.ownedByOppo && GetComponent<AbilityDisplay>().ability.targetType.Length > 0) {
            GameManager.dragged = true;
            isDragged = true;

            GameObject arrowEmitter = FindAnyObjectByType<GameManager>().ArrowEmitter;
            arrowEmitter.SetActive(true);
            arrowEmitter.transform.position = new Vector3(transform.position.x, transform.position.y, -3);
            Cursor.visible = false;
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (!isDragged) return;
        if (GetComponent<AbilityDisplay>().abilityStatus == AbilityStatus.TeamLayout) return;
        if (FindAnyObjectByType<GameManager>().playerAction == null) {

            GameObject arrowEmitter = FindAnyObjectByType<GameManager>().ArrowEmitter;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            // On change la couleur de la fleche de ciblage
            if (hit.collider != null) {
                arrowEmitter.GetComponent<BezierArrow>().changeColor(GetComponent<AbilityDisplay>().loopTargetAllowed(hit.collider.gameObject));
            } else {
                arrowEmitter.GetComponent<BezierArrow>().changeColor(false);
            }
        } else {
            ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.endDragHandler);
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!isDragged) return;
        if (GetComponent<AbilityDisplay>().abilityStatus == AbilityStatus.TeamLayout) return;

        bool dropZoneValid = false;
        if (FindAnyObjectByType<GameManager>().playerAction == null) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null) {
                GameObject dropZone = hit.collider.gameObject;
                // Contre attaque
                if (dropZone.GetComponent<SlotDisplay>() != null) {
                    //dropZoneValid = dropZone.GetComponent<SlotDisplay>().onDrop(gameObject);
                }
                // Aura
                else if (dropZone.GetComponent<AuraDisplay>() != null) {
                    //dropZoneValid = dropZone.GetComponent<AuraDisplay>().onDrop(gameObject);
                }
                // Enchantement
                else if (dropZone.GetComponent<EquipmentDisplay>() != null) {
                    //dropZoneValid = dropZone.GetComponent<EquipmentDisplay>().onDrop(gameObject);
                }
                // Monster
                else if (dropZone.GetComponent<MonsterDisplay>() != null) {
                    dropZoneValid = dropZone.GetComponent<MonsterDisplay>().OnDropAbility(GetComponent<AbilityDisplay>());
                }
                // Card
                else if (dropZone.GetComponent<CardDisplay>() != null) {
                    dropZoneValid = dropZone.GetComponent<CardDisplay>().OnDropAbility(GetComponent<AbilityDisplay>());
                }
            }
        }

        GameManager.dragged = false;

        isDragged = false;
        if (!FindAnyObjectByType<GameManager>().ArrowEmitter.GetComponent<BezierArrow>().isFixed)
            FindAnyObjectByType<GameManager>().ArrowEmitter.SetActive(false);
        Cursor.visible = true;
    }

    //private void OnMouseDown() {
    //    if (FindAnyObjectByType<GameManager>().playerAction != null) return;

    //    if (GetComponent<AbilityDisplay>().cooldown <= 0 && !GetComponent<OwnedByOppo>().monsterOwnThis.ownedByOppo && GetComponent<AbilityDisplay>().ability.targetType.Length > 0) {
    //        GameManager.dragged = true;
    //        isDragged = true;

    //        GameObject arrowEmitter = FindAnyObjectByType<GameManager>().ArrowEmitter;
    //        arrowEmitter.SetActive(true);
    //        arrowEmitter.transform.position = new Vector3(transform.position.x, transform.position.y, -3);
    //        Cursor.visible = false;
    //    }
    //}

    //private void OnMouseDrag() {
    //    if (!isDragged) return;
    //    if (FindAnyObjectByType<GameManager>().playerAction == null) {

    //        GameObject arrowEmitter = FindAnyObjectByType<GameManager>().ArrowEmitter;
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

    //        // On change la couleur de la fleche de ciblage
    //        if (hit.collider != null) {
    //            arrowEmitter.GetComponent<BezierArrow>().changeColor(GetComponent<AbilityDisplay>().loopTargetAllowed(hit.collider.gameObject));
    //        } else {
    //            arrowEmitter.GetComponent<BezierArrow>().changeColor(false);
    //        }
    //    } else {
    //        ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.endDragHandler);
    //    }
    //}

    //private void OnMouseUp() {
    //    if (!isDragged) return;

    //    if (FindAnyObjectByType<GameManager>().playerAction == null) {
    //        bool dropZoneValid = false;

    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
    //        if (hit.collider != null) {
    //            GameObject dropZone = hit.collider.gameObject;
    //            // Contre attaque
    //            if (dropZone.GetComponent<SlotDisplay>() != null) {
    //                //dropZoneValid = dropZone.GetComponent<SlotDisplay>().onDrop(gameObject);
    //            }
    //            // Aura
    //            else if (dropZone.GetComponent<AuraDisplay>() != null) {
    //                //dropZoneValid = dropZone.GetComponent<AuraDisplay>().onDrop(gameObject);
    //            }
    //            // Enchantement
    //            else if (dropZone.GetComponent<EquipmentDisplay>() != null) {
    //                //dropZoneValid = dropZone.GetComponent<EquipmentDisplay>().onDrop(gameObject);
    //            }
    //            // Monster
    //            else if (dropZone.GetComponent<MonsterDisplay>() != null) {
    //                dropZoneValid = dropZone.GetComponent<MonsterDisplay>().OnDropAbility(GetComponent<AbilityDisplay>());
    //            }
    //            // Card
    //            else if (dropZone.GetComponent<CardDisplay>() != null) {
    //                dropZoneValid = dropZone.GetComponent<CardDisplay>().OnDropAbility(GetComponent<AbilityDisplay>());
    //            }
    //        }
    //    }

    //    GameManager.dragged = false;

    //    isDragged = false;
    //    FindAnyObjectByType<GameManager>().ArrowEmitter.SetActive(false);
    //    Cursor.visible = true;
    //}
}