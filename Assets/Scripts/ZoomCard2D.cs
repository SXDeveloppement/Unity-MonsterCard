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
                transform.localPosition = new Vector3(localPosition.x, positionY, localPosition.z - 0.02f);
                Debug.Log("PointerEnter");
            }
        }
    }

    // Quand le curseur quitte la carte
    public void OnPointerExit(PointerEventData eventData) {
        if (GetComponent<CardDisplay>().status == Status.Hand && !gmTemp.dragged) {
            transform.localScale = cachedScale;
            transform.localPosition = localPosition;
            changeWithPlaceholder();
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
