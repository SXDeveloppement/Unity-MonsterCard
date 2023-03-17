using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class Draggable2D : MonoBehaviour
{
    public float underZoom;

    Vector3 startScale;
    Vector3 zoomScale;
    Vector2 sizeCard;
    Vector2 cachedMousePosition;
    GameObject placeHolder;
    GameManager gameManager;

    Vector3 position;
    GameObject GO_Hand;
    bool outsideHand = false;

    public bool isDragged = false; // Est glissé pour les cartes de la main
    public bool isDraggedTemp;
    public bool isHalfDragged = false; // Est a moitié glissé pour les cartes du terrain
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();

        startScale = transform.localScale;
        sizeCard = transform.GetComponent<RectTransform>().rect.size;

        isDraggedTemp = isDragged;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDraggedTemp != isDragged) {
            isDraggedTemp = isDragged;
            GetComponent<BoxCollider2D>().enabled = !isDragged;
        }
    }

    private void OnMouseDown() {
        GameObject arrowEmitter = gameManager.ArrowEmitter;
        if (GetComponent<CardDisplay>().status == CardStatus.Hand && !GetComponent<CardDisplay>().ownedByOppo && !GameManager.dragged) {
            GameManager.dragged = true;
            isDragged = true;

            position = transform.position;

            zoomScale = transform.localScale;
            transform.localScale = startScale;
            placeHolder = GetComponent<ZoomCard2D>().placeHolder;
            GO_Hand = this.transform.parent.gameObject;
            this.transform.SetParent(this.transform.parent.parent);
        }
        // Si c'est un sbire sur le terrain et qu'il n'a pas attaqué pendant le tour
        else if (!GameManager.dragged
            && GetComponent<CardDisplay>().status == CardStatus.SlotVisible 
            && GetComponent<CardDisplay>().card.type == CardType.Sbire
            && !GetComponent<SbireDisplay>().sbireHasAttacked 
            && !GetComponent<CardDisplay>().ownedByOppo) {
            isHalfDragged = true;
            GameManager.dragged = true;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            arrowEmitter.SetActive(true);
            arrowEmitter.transform.position = new Vector3(transform.position.x, transform.position.y, -3);
            Cursor.visible = false;
        }
        // Si ce n'est pas une carte de contre attaque face caché sur le terrain
        else if (!GameManager.dragged && GetComponent<CardDisplay>().status == CardStatus.SlotHidden && GetComponent<CardDisplay>().card.type != CardType.CounterAttack) {
            GameManager.dragged = true;
            isHalfDragged = true;
            arrowEmitter.SetActive(true);
            arrowEmitter.transform.position = new Vector3(transform.position.x, transform.position.y, -3);
            Cursor.visible = false;
        }
        // Si c'est une carte "Echo" sur le terrain qui n'a pas été posé ce tour ci
        else if (!GameManager.dragged && GetComponent<CardDisplay>().status == CardStatus.SlotVisible && GetComponent<CardDisplay>().card.type == CardType.Echo
        && !GetComponent<CardDisplay>().putOnBoardThisTurn && !GetComponent<CardDisplay>().ownedByOppo) {
            GameManager.dragged = true;
            isHalfDragged = true;
            arrowEmitter.SetActive(true);
            arrowEmitter.transform.position = new Vector3(transform.position.x, transform.position.y, -3);
            Cursor.visible = false;
        }
    }

    private void OnMouseDrag() {
        if (!isDragged && !isHalfDragged) return;

        // Si c'est une carte de la main
        if (GetComponent<CardDisplay>().status == CardStatus.Hand) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = new Vector3(mousePosition.x, mousePosition.y, position.z - 0.2f);

            // Réorganisation des cartes dans la main
            if (mousePosition.y < (GO_Hand.transform.position.y + GO_Hand.GetComponent<RectTransform>().rect.height / 2)) {
                placeHolder.SetActive(true);
                for (int i = 0; i < GO_Hand.transform.childCount; i++) {
                    GameObject cardInHand = GO_Hand.transform.GetChild(i).gameObject;
                    if (mousePosition.x < cardInHand.transform.position.x) {
                        bool prevCardIsPlaceholder = false;
                        if (i > 0) {
                            GameObject cardInHandPrev = GO_Hand.transform.GetChild(i-1).gameObject;
                            if (cardInHandPrev == placeHolder) {
                                prevCardIsPlaceholder = true;
                            }
                        }
                        if (!prevCardIsPlaceholder) {
                            if (placeHolder.transform.GetSiblingIndex() < i) {
                                placeHolder.gameObject.transform.SetSiblingIndex(i - 1);
                            } else {
                                placeHolder.gameObject.transform.SetSiblingIndex(i);
                            }
                        }

                        break;
                    }

                    // Exception last card
                    if (cardInHand != placeHolder && i == GO_Hand.transform.childCount - 1) {
                        placeHolder.gameObject.transform.SetSiblingIndex(GO_Hand.transform.childCount - 1);
                    }
                }

                // Si le scale de la carte est inférieur a startScale, on le met égale a startScale
                if (transform.localScale.x < startScale.x) {
                    transform.localScale = startScale;
                }
            } 
            // Quand on déplace la carte en dehors de la main
            else {
                placeHolder.SetActive(false);
                transform.localScale = new Vector3(underZoom, underZoom, underZoom);
            }
        }
        // Changement du curseur en fonction des cibles valident pour jouer la carte qui est sur le terrain
        else if (GetComponent<CardDisplay>().status == CardStatus.SlotHidden || GetComponent<CardDisplay>().status == CardStatus.SlotVisible) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            GameObject arrowEmitter = gameManager.ArrowEmitter;

            if (hit.collider != null) {
                bool targetAvailable = false;
                
                // Si c'est une carte sbire face visible
                if (GetComponent<CardDisplay>().status == CardStatus.SlotVisible && GetComponent<CardDisplay>().card.type == CardType.Sbire) {
                    // On regarde si l'adversaire possède un sbire avec "Tank"
                    bool sbireHaveTaunt = false;
                    foreach (CardDisplay cardDisplay in gameManager.GO_CounterAttackAreaOppo.GetComponentsInChildren<CardDisplay>()) {
                        if (cardDisplay.card.type == CardType.Sbire) {
                            sbireHaveTaunt = cardDisplay.GetComponent<SbireDisplay>().haveTank();
                            if (sbireHaveTaunt)
                                break;
                        }
                    }

                    // Si la cible est un sbire controlé par l'adversaire
                    if (hit.collider.GetComponent<CardDisplay>() != null) {
                        CardDisplay cardDisplay = hit.collider.GetComponent<CardDisplay>();

                        // Si la cible est un sbire avec "Tank" ou qu'il n'y a aucun sbire avec "Tank"
                        if (!sbireHaveTaunt || cardDisplay.GetComponent<SbireDisplay>().haveTank()) {
                            targetAvailable = true;
                        }
                    } 
                    // Si la cible est un monstre controlé par l'adversaire et qu'il n'y a pas de sbire avec "Tank"
                    else if (hit.collider.GetComponent<MonsterDisplay>() != null) {
                        MonsterDisplay monsterDisplay = hit.collider.GetComponent<MonsterDisplay>();
                        if (monsterDisplay.ownedByOppo && !sbireHaveTaunt) {
                            targetAvailable = true;
                        }
                    }
                } else {
                    targetAvailable = GetComponent<CardDisplay>().targetIsAllowed(hit.collider.gameObject);
                }

                if (targetAvailable) {
                    arrowEmitter.GetComponent<BezierArrow>().changeColor(true);
                } else {
                    arrowEmitter.GetComponent<BezierArrow>().changeColor(false);
                }
            } else {
                arrowEmitter.GetComponent<BezierArrow>().changeColor(false);
            }
        }

    }

    private void OnMouseUp() {
        if (!isDragged && !isHalfDragged) return;

        bool dropZoneValid = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider != null) {
            GameObject dropZone = hit.collider.gameObject;
            // Contre attaque
            if (dropZone.GetComponent<SlotDisplay>() != null) {
                dropZoneValid = dropZone.GetComponent<SlotDisplay>().onDrop(gameObject);
            }
            // Aura
            else if (dropZone.GetComponent<AuraDisplay>() != null) {
                dropZoneValid = dropZone.GetComponent<AuraDisplay>().onDrop(gameObject);
            }
            // Enchantement
            else if (dropZone.GetComponent<EquipmentDisplay>() != null) {
                dropZoneValid = dropZone.GetComponent<EquipmentDisplay>().onDrop(gameObject);
            }
            // Monster
            else if (dropZone.GetComponent<MonsterDisplay>() != null) {
                dropZoneValid = dropZone.GetComponent<MonsterDisplay>().OnDrop(gameObject);
            }
            // Card
            else if (dropZone.GetComponent<CardDisplay>() != null) {
                dropZoneValid = dropZone.GetComponent<CardDisplay>().OnDrop(gameObject);
            }
        }

        GameManager.dragged = false;

        // Si on drop une carte de la main sur aucune dropZone valide
        if (!dropZoneValid && !isHalfDragged) {
            transform.localScale = startScale;
            this.transform.SetParent(GO_Hand.transform);
            GetComponent<ZoomCard2D>().changeWithPlaceholder();
            GetComponentInParent<HandDisplay>().childHaveChanged = true;
        }
        // Si on drop une carte du terrain sur aucune dropZone valide
        else if (!dropZoneValid && isHalfDragged) {
            ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
        }

        isDragged = false;
        isHalfDragged = false;
        gameManager.ArrowEmitter.SetActive(false);
        Cursor.visible = true;
    }
}
