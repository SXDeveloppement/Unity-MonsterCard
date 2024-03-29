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
    public GameObject equipment;

    GameManager gameManager;
    bool pointerIsEnter = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();

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
        if (GameManager.dragged)
            return;
        if (GetComponent<CardDisplay>().status == CardStatus.Hand && GetComponent<OwnedByOppo>().monsterOwnThis.ownedByOppo)
            return;

        // Si on ne d�place pas de carte
        if (!GameManager.dragged) { 
            // Si la souris est sur une carte de la main
            if (GetComponent<CardDisplay>().status == CardStatus.Hand) {
                siblingIndex = transform.GetSiblingIndex();
                localPosition = transform.localPosition;
                transform.localScale = Constante.ScaleComparedParent(Constante.SCALE_CARD_HAND_ZOOM, gameObject);
                gameObject.GetComponent<LayoutElement>().ignoreLayout = true;
                createPlaceholder();

                float height = gameObject.GetComponent<RectTransform>().rect.size.y;
                // On calcule la nouvelle position de la carte zoom� pour quelle s'affiche enti�rement a l'�cran
                float positionY = localPosition.y + (height * scaleZoom - transform.parent.GetComponent<RectTransform>().rect.height) / 2 + marginBottom;
                transform.localPosition = new Vector3(localPosition.x, positionY, localPosition.z - 2f);
                pointerIsEnter = true;
            }
            // Si la souris est sur une carte du terrain
            else if (GetComponent<CardDisplay>().status == CardStatus.SlotVisible || GetComponent<CardDisplay>().status == CardStatus.SlotHidden || GetComponent<CardDisplay>().status == CardStatus.AuraSlot || GetComponent<CardDisplay>().status == CardStatus.EnchantmentSlot || GetComponent<CardDisplay>().status == CardStatus.ActionSlot) {
                transform.localScale = Constante.ScaleComparedParent(Constante.SCALE_CARD_HAND_ZOOM, gameObject);
                localPosition = transform.localPosition;
                transform.localPosition = new Vector3(localPosition.x, localPosition.y, localPosition.z - 2f);

                // Si c'est une carte face cach�, on la retourne face visible
                if (GetComponent<CardDisplay>().hiddenCard) {
                    GetComponent<CardDisplay>().showVisibleFace();
                    //StartCoroutine(GetComponent<CardDisplay>().flipFront());
                    //StopCoroutine(GetComponent<CardDisplay>().flipFront());
                }

                // Si c'est une carte d'enchantement, on affiche l'equipement
                if (GetComponent<CardDisplay>().status == CardStatus.EnchantmentSlot) {
                    equipment = transform.parent.parent.Find("Equipment").GetChild(0).gameObject;
                    equipment.transform.localScale = Constante.ScaleComparedParent(Constante.SCALE_CARD_HAND_ZOOM, gameObject);
                    float positionX = equipment.GetComponent<RectTransform>().rect.width * equipment.transform.localScale.x;
                    equipment.transform.localPosition = new Vector3(localPosition.x + positionX, localPosition.y, localPosition.z - 2f);
                }
                pointerIsEnter = true;
            }
        }
    }

    // Quand le curseur quitte la carte
    public void OnPointerExit(PointerEventData eventData) {
        if (GameManager.dragged || !pointerIsEnter) return;

        if (GetComponent<CardDisplay>().status == CardStatus.Hand) {
            transform.localScale = Constante.ScaleComparedParent(Constante.SCALE_CARD_HAND, gameObject);
            transform.localPosition = localPosition;
            changeWithPlaceholder();
            pointerIsEnter = false;
        } else if (GetComponent<CardDisplay>().status != CardStatus.Hand && GetComponent<CardDisplay>().status != CardStatus.Graveyard) {
            transform.localScale = Constante.ScaleComparedParent(Constante.SCALE_CARD_BOARD, gameObject);
            transform.localPosition = localPosition;

            // Si c'est une carte face cach�, on la replace en position face cach�
            if (GetComponent<CardDisplay>().hiddenCard) {
                GetComponent<CardDisplay>().showHiddenFace();
                //StartCoroutine(GetComponent<CardDisplay>().flipBack());
                //StopCoroutine(GetComponent<CardDisplay>().flipBack());
            }

            // Si c'est une carte d'enchantement, on affiche l'equipement
            if (GetComponent<CardDisplay>().status == CardStatus.EnchantmentSlot && equipment != null) {
                equipment.transform.localScale = Constante.ScaleComparedParent(Constante.SCALE_CARD_BOARD, gameObject);
                equipment.transform.localPosition = localPosition;
            }
            pointerIsEnter = false;
        }
    }

    // Cr�e le placeholder a la position de la carte
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

    // R�initialise la carte
    public void reinitCard() {
        GameManager.dragged = false;
        GetComponent<BoxCollider2D>().enabled = true;
        destroyPlaceholder();
    }

    // D�truit le placeholder de la carte
    public void destroyPlaceholder() {
        if (placeHolder != null) {
            Destroy(placeHolder);
            placeHolder = null;
        }
    }
}
