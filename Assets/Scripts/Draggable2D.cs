using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    GMTemp gmTemp;
    public bool isDragged = false;
    public bool isDraggedTemp;

    // Start is called before the first frame update
    void Start()
    {
        //gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gmTemp = GameObject.FindAnyObjectByType<GMTemp>();

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
        Debug.Log("BeginDrag");
        if (GetComponent<CardDisplay>().status == Status.Hand && !GetComponent<CardDisplay>().ownedByOppo && !gmTemp.dragged) {
            //gameManager.dragged = true;
            gmTemp.dragged = true;
            isDragged = true;

            position = transform.position;

            zoomScale = transform.localScale;
            transform.localScale = startScale;
            placeHolder = GetComponent<ZoomCard2D>().placeHolder;
            GO_Hand = this.transform.parent.gameObject;
            this.transform.SetParent(this.transform.parent.parent);
            //gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
            // Si c'est un sbire sur le terrain et qu'il n'a pas attaqué pendant le tour
        }
        //} else if (GetComponent<CardDisplay>().status == Status.SlotVisible && GetComponent<CardDisplay>().card.type == Type.Sbire
        //&& !GetComponent<SbireDisplay>().sbireHasAttacked && !GetComponent<CardDisplay>().ownedByOppo) {
        //    gameManager.dragged = true;
        //    Cursor.SetCursor(gameManager.cursorTargetTexture, Vector2.zero, CursorMode.Auto);
        //    // Si c'est une carte "Echo" sur le terrain qui n'a pas été posé ce tour ci
        //} else if (GetComponent<CardDisplay>().status == Status.SlotVisible && GetComponent<CardDisplay>().card.type == Type.Echo
        //&& !GetComponent<CardDisplay>().putOnBoardThisTurn && !GetComponent<CardDisplay>().ownedByOppo) {
        //    gameManager.dragged = true;
        //    Cursor.SetCursor(gameManager.cursorTargetTexture, Vector2.zero, CursorMode.Auto);
        //}

    }

    private void OnMouseDrag() {
        if (!isDragged) return;

        // Si c'est une carte de la main
        if (GetComponent<CardDisplay>().status == Status.Hand) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = new Vector3(mousePosition.x, mousePosition.y, position.z - 0.2f);

            int indexPlaceHolder = placeHolder.transform.GetSiblingIndex();
            int parentPlaceHolderChildCount = GO_Hand.transform.childCount;
            float layoutSpacing = GO_Hand.GetComponent<HorizontalLayoutGroup>().spacing;

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
            } else {
                placeHolder.SetActive(false);
                transform.localScale = new Vector3(underZoom, underZoom, underZoom);
            }
        }
    }

    private void OnMouseUp() {
        transform.localScale = startScale;
        this.transform.SetParent(GO_Hand.transform);
        GetComponent<ZoomCard2D>().changeWithPlaceholder();
        gmTemp.dragged = false;
        isDragged = false;
        GetComponentInParent<HandDisplay>().childHaveChanged = true;
    }

    //void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
    //    Debug.Log("DragEnd");
    //    if (GetComponent<CardDisplay>().status == Status.Hand) {
    //        gameManager.dragged = false;
    //        eventData.pointerDrag.GetComponent<ZoomCard>().changeWithPlaceholder();
    //        transform.localScale = startScale;
    //    } else if (GetComponent<CardDisplay>().status == Status.SlotVisible
    //    && (
    //    GetComponent<CardDisplay>().card.type == Type.Sbire
    //    || GetComponent<CardDisplay>().card.type == Type.Echo
    //    )) {
    //        gameManager.dragged = false;
    //        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    //    }
    //}
}
