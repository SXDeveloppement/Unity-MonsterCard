using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ZoomCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    public int marginBottom;
    public float scaleZoom;
    public float scaleZoomBoard;
    public GameObject placeHolder;

    Vector3 cachedScale;
    int siblingIndex;
    Vector3 localPosition;
    Vector2 size;

    // Start is called before the first frame update
    void Start()
    {
        cachedScale = transform.localScale;
        siblingIndex = transform.GetSiblingIndex();
        size = gameObject.GetComponent<RectTransform>().rect.size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Quand le curseur entre sur la carte
    public void OnPointerEnter(PointerEventData eventData) {
        // Si on ne déplace pas de carte
        if (!GameObject.Find("GameManager").GetComponent<GameManager>().dragged) {
            // Si la souris est sur une carte de la main
            if (GetComponent<CardDisplay>().status == Status.Hand) {
                siblingIndex = transform.GetSiblingIndex();
                localPosition = transform.localPosition;

                transform.localScale = new Vector3(scaleZoom, scaleZoom, scaleZoom);
                gameObject.GetComponent<LayoutElement>().ignoreLayout = true;
                if (transform.parent.gameObject.name == "Hand") {
                    transform.SetAsLastSibling();
                    float height = gameObject.GetComponent<RectTransform>().rect.size.y;
                    // On calcule la nouvelle position de la carte zoomé pour quelle s'affiche entièrement a l'écran
                    float positionY = localPosition.y + marginBottom + height * (scaleZoom - 1) / 2 + height - transform.parent.GetComponent<RectTransform>().rect.height;
                    transform.localPosition = new Vector3(localPosition.x, positionY, localPosition.z);
                    createPlaceholder();
                }
            // Si la souris est sur une carte placé sur le terrain
            } else if (GetComponent<CardDisplay>().status != Status.Hand && GetComponent<CardDisplay>().status != Status.Graveyard) {
                transform.localScale = new Vector3(scaleZoomBoard, scaleZoomBoard, scaleZoomBoard);
                if (transform.parent.parent.GetComponent<HorizontalLayoutGroup>() != null) {
                    transform.parent.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
                }
                transform.parent.SetAsLastSibling();

                // Si c'est une carte face caché, on la retourne face visible
                if (GetComponent<CardDisplay>().hiddenCard) {
                    GetComponent<CardDisplay>().showVisibleFace();
                    //StartCoroutine(GetComponent<CardDisplay>().flipFront());
                    //StopCoroutine(GetComponent<CardDisplay>().flipFront());
                }
            }
        }
    }

    // Quand le curseur quitte la carte
    public void OnPointerExit(PointerEventData eventData) {
        if (!GameObject.Find("GameManager").GetComponent<GameManager>().dragged && GetComponent<CardDisplay>().status == Status.Hand) {
            transform.localScale = cachedScale;
            gameObject.GetComponent<LayoutElement>().ignoreLayout = false;
            if (transform.parent.gameObject.name == "Hand") {
                transform.localPosition = localPosition;
                changeWithPlaceholder();
            }
        } else if (!GameObject.Find("GameManager").GetComponent<GameManager>().dragged && GetComponent<CardDisplay>().status != Status.Hand && GetComponent<CardDisplay>().status != Status.Graveyard) {
            transform.localScale = cachedScale;

            // Si c'est une carte face caché, on la replace en position face caché
            if (GetComponent<CardDisplay>().hiddenCard) {
                GetComponent<CardDisplay>().showHiddenFace();
                //StartCoroutine(GetComponent<CardDisplay>().flipBack());
                //StopCoroutine(GetComponent<CardDisplay>().flipBack());
            }
        }
    }

    // Crée le placeholder a la position de la carte
    public void createPlaceholder() {
        placeHolder = new GameObject();
        placeHolder.tag = "PlaceHolder";
        placeHolder.name = "PlaceHolder";
        placeHolder.AddComponent<RectTransform>();
        placeHolder.gameObject.GetComponent<RectTransform>().sizeDelta = size;
        placeHolder.gameObject.transform.SetParent(transform.parent);
        placeHolder.transform.SetSiblingIndex(siblingIndex);
    }

    // Prend la position du placeholder dans le layout
    public void changeWithPlaceholder() {
        GameObject.Find("GameManager").GetComponent<GameManager>().dragged = false;
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (placeHolder != null) {
            transform.SetSiblingIndex(placeHolder.transform.GetSiblingIndex());
            Destroy(placeHolder);
        }
        gameObject.GetComponent<LayoutElement>().ignoreLayout = false;
    }

    // Réinitialise la carte
    public void reinitCard() {
        GameObject.Find("GameManager").GetComponent<GameManager>().dragged = false;
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        destroyPlaceholder();
        gameObject.GetComponent<LayoutElement>().ignoreLayout = false;
    }

    // Détruit le placeholder de la carte
    public void destroyPlaceholder() {
        if (placeHolder != null) {
            Destroy(placeHolder);
        }
    }
}
