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
    GameObject gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager");

        startScale = transform.localScale;
        sizeCard = transform.GetComponent<RectTransform>().rect.size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
        if (GetComponent<CardDisplay>().status == Status.Hand) {
            gameManager.GetComponent<GameManager>().dragged = true;

            cachedMousePosition = eventData.position;
            zoomScale = transform.localScale;
            transform.localScale = startScale;
            placeHolder = GetComponent<ZoomCard>().placeHolder;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
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

                // Si le scale de la carte est inf�rieur a startScale, on le met �gale a startScale
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
            eventData.pointerDrag.GetComponent<ZoomCard>().changeWithPlaceholder();
            transform.localScale = startScale;
        }
    }
}