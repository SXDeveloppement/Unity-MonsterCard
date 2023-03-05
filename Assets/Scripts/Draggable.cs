using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float underZoom;

    Vector3 startScale;
    Vector3 zoomScale;
    Vector2 sizeCard;
    Vector2 cachedMousePosition;
    GameObject placeHolder;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        startScale = transform.localScale;
        sizeCard = transform.GetComponent<RectTransform>().rect.size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
        if (GetComponent<CardDisplay>().status == Status.Hand && !GetComponent<CardDisplay>().ownedByOppo) {
            gameManager.dragged = true;

            cachedMousePosition = eventData.position;
            zoomScale = transform.localScale;
            transform.localScale = startScale;
            placeHolder = GetComponent<ZoomCard>().placeHolder;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        // Si c'est un sbire sur le terrain et qu'il n'a pas attaqué pendant le tour
        } else if (GetComponent<CardDisplay>().status == Status.SlotVisible && GetComponent<CardDisplay>().card.type == Type.Sbire 
        && !GetComponent<SbireDisplay>().sbireHasAttacked && !GetComponent<CardDisplay>().ownedByOppo) {
            gameManager.dragged = true;
            Cursor.SetCursor(gameManager.cursorTargetTexture, Vector2.zero, CursorMode.Auto);
        // Si c'est une carte "Echo" sur le terrain qui n'a pas été posé ce tour ci
        } else if (GetComponent<CardDisplay>().status == Status.SlotVisible && GetComponent<CardDisplay>().card.type == Type.Echo
        && !GetComponent<CardDisplay>().putOnBoardThisTurn && !GetComponent<CardDisplay>().ownedByOppo) {
            gameManager.dragged = true;
            Cursor.SetCursor(gameManager.cursorTargetTexture, Vector2.zero, CursorMode.Auto);
        }
        
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        // Si c'est une carte de la main
        if (GetComponent<CardDisplay>().status == Status.Hand) {
            this.transform.position = eventData.position;
            int indexPlaceHolder = placeHolder.gameObject.transform.GetSiblingIndex();
            GameObject GO_Hand = placeHolder.gameObject.transform.parent.gameObject;
            int parentPlaceHolderChildCount = GO_Hand.transform.childCount;
            float layoutSpacing = GO_Hand.GetComponent<HorizontalLayoutGroup>().spacing;

            if (eventData.position.y < GO_Hand.GetComponent<RectTransform>().rect.height) {
                placeHolder.SetActive(true);
                for (int i = 0; i < GO_Hand.transform.childCount - 1; i++) {
                    GameObject cardInHand = GO_Hand.transform.GetChild(i).gameObject;
                    if (eventData.position.x < cardInHand.transform.position.x) {
                        bool prevCardIsPlaceholder = false;
                        if (i > 0) {
                            GameObject cardInHandPrev = GO_Hand.transform.GetChild(i - 1).gameObject;
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
                    if (cardInHand != placeHolder && i == GO_Hand.transform.childCount - 2) {
                        placeHolder.gameObject.transform.SetSiblingIndex(GO_Hand.transform.childCount - 1);
                    }
                }

                // Si le scale de la carte est inférieur a startScale, on le met égale a startScale
                if (transform.localScale.x < startScale.x) {
                    transform.localScale = startScale;
                }
            } else {
                placeHolder.SetActive(false);
                transform.localScale = new Vector3(underZoom, underZoom, underZoom);
            }
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        if (GetComponent<CardDisplay>().status == Status.Hand) {
            gameManager.dragged = false;
            eventData.pointerDrag.GetComponent<ZoomCard>().changeWithPlaceholder();
            transform.localScale = startScale;
        } else if (GetComponent<CardDisplay>().status == Status.SlotVisible 
        && (
        GetComponent<CardDisplay>().card.type == Type.Sbire
        || GetComponent<CardDisplay>().card.type == Type.Echo
        )) {
            gameManager.dragged = false;
            Cursor.SetCursor( null, Vector2.zero, CursorMode.Auto);
        }
    }
}
