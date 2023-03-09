using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ZoomCard2D : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    public float marginBottom;
    public float scaleZoom;
    public float scaleZoomBoard;
    public GameObject placeHolder;

    public Vector3 cachedScale;
    int siblingIndex;
    Vector3 localPosition;
    Vector2 size;
    GameObject equipment;

    GMTemp gmTemp;

    // Start is called before the first frame update
    void Start()
    {
        gmTemp = GameObject.FindAnyObjectByType<GMTemp>();

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
        if (gmTemp.dragged) return;

        // Si on ne déplace pas de carte
        //if (!GameObject.Find("GameManager").GetComponent<GameManager>().dragged) {
        if (!gmTemp.dragged) { 
            // Si la souris est sur une carte de la main
            if (GetComponent<CardDisplay>().status == Status.Hand) {
                siblingIndex = transform.GetSiblingIndex();
                localPosition = transform.localPosition;
                transform.localScale = new Vector3(scaleZoom, scaleZoom, scaleZoom);
                gameObject.GetComponent<LayoutElement>().ignoreLayout = true;
                createPlaceholder();

                float height = gameObject.GetComponent<RectTransform>().rect.size.y;
                // On calcule la nouvelle position de la carte zoomé pour quelle s'affiche entièrement a l'écran
                //float positionY = localPosition.y + marginBottom + height * (scaleZoom - 1) / 2 + height - transform.parent.GetComponent<RectTransform>().rect.height;
                float positionY = localPosition.y + (height * scaleZoom - transform.parent.GetComponent<RectTransform>().rect.height) / 2 + marginBottom;
                transform.localPosition = new Vector3(localPosition.x, positionY, localPosition.z - 2f);
            }
            // Si la souris est sur une carte du terrain
            else if (GetComponent<CardDisplay>().status != Status.Hand && GetComponent<CardDisplay>().status != Status.Graveyard) {
                transform.localScale = new Vector3(scaleZoomBoard, scaleZoomBoard, scaleZoomBoard);
                localPosition = transform.localPosition;
                transform.localPosition = new Vector3(localPosition.x, localPosition.y, localPosition.z - 2f);

                // Si c'est une carte face caché, on la retourne face visible
                if (GetComponent<CardDisplay>().hiddenCard) {
                    GetComponent<CardDisplay>().showVisibleFace();
                    //StartCoroutine(GetComponent<CardDisplay>().flipFront());
                    //StopCoroutine(GetComponent<CardDisplay>().flipFront());
                }

                // Si c'est une carte d'enchantement, on affiche l'equipement
                if (GetComponent<CardDisplay>().status == Status.EnchantmentSlot) {
                    equipment = transform.parent.parent.Find("Equipment").GetChild(0).gameObject;
                    equipment.transform.localScale = new Vector3(scaleZoomBoard, scaleZoomBoard, scaleZoomBoard);
                    float positionX = equipment.GetComponent<RectTransform>().rect.width * equipment.transform.localScale.x;
                    equipment.transform.localPosition = new Vector3(localPosition.x + positionX, localPosition.y, localPosition.z - 2f);
                }
            }
        }
    }

    // Quand le curseur quitte la carte
    public void OnPointerExit(PointerEventData eventData) {
        if (GetComponent<CardDisplay>().status == Status.Hand && !gmTemp.dragged) {
            transform.localScale = cachedScale;
            transform.localPosition = localPosition;
            changeWithPlaceholder();
        } else if (GetComponent<CardDisplay>().status != Status.Hand && GetComponent<CardDisplay>().status != Status.Graveyard) {
            transform.localScale = cachedScale;
            transform.localPosition = localPosition;

            // Si c'est une carte face caché, on la replace en position face caché
            if (GetComponent<CardDisplay>().hiddenCard) {
                GetComponent<CardDisplay>().showHiddenFace();
                //StartCoroutine(GetComponent<CardDisplay>().flipBack());
                //StopCoroutine(GetComponent<CardDisplay>().flipBack());
            }

            // Si c'est une carte d'enchantement, on affiche l'equipement
            if (GetComponent<CardDisplay>().status == Status.EnchantmentSlot) {
                equipment.transform.localScale = cachedScale;
                equipment.transform.localPosition = localPosition;
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
        placeHolder.transform.position = transform.position;
        placeHolder.gameObject.transform.SetParent(transform.parent);
        placeHolder.transform.SetSiblingIndex(siblingIndex);        
    }

    // Prend la position du placeholder dans le layout
    public void changeWithPlaceholder() {
        if (placeHolder != null) {
            transform.SetSiblingIndex(placeHolder.transform.GetSiblingIndex());
            transform.position = placeHolder.transform.position;
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
