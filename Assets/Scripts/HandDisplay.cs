using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandDisplay : MonoBehaviour
{
    public float zStep;
    public bool childHaveChanged = true;
    public bool clickValidMulligan = false; // Si le mulligan a été validé

    public GameObject GO_Hand;
    public GameObject GO_MulliganFeatures;
    public GameObject GO_ButtonValidMulligan;
    public GameObject GO_ButtonShowBoard;

    private bool mulliganDone = false; // Si le mulligan est terminé
    private bool mulliganIsVisible = true; // Si la fenêtre de mulligan est visible
    
    private Vector3 handPositionCached; // Sauvegarde de la position de la main
    private Vector3 handScaleCached; // Sauvegarde du scale de la main
    private TextAnchor childAlignementCached; // alignement du layout
    private float childSpacingCached; // spacing du layout

    private string butonShowBoardTextInMulligan = "Show board";
    private string butonshowBoardTextInBoard = "Back to mulligan";
    private Vector3 handMulliganPosition = new Vector3(0,0,0); // Position de la main en mode Mulligan
    private Vector3 handMulliganScale = new Vector3(2, 2, 1); // Scale de la main en mode Mulligan
    private TextAnchor childAlignementMulligan = TextAnchor.UpperCenter;
    private float childSpacingMulligan = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (childHaveChanged) {
            childHaveChanged = false;
            GO_Hand.GetComponent<HorizontalLayoutGroup>().enabled = false;
            GO_Hand.GetComponent<HorizontalLayoutGroup>().enabled = true;
            for (int i = 0; i < GO_Hand.transform.childCount; i++) {
                // On réduit la position Z de l'enfant de zStep * l'index
                GO_Hand.transform.GetChild(i).transform.localPosition 
                    = new Vector3(
                        GO_Hand.transform.GetChild(i).transform.localPosition.x,
                        GO_Hand.transform.GetChild(i).transform.localPosition.y, 
                        -zStep * (i + 1)
                        );
            }
        }

        // On replace la main dans sa position normal
        if (!GameManager.firstTurn && !mulliganDone) {
            mulliganDone = true;
            InitHand();
        }
    }

    public IEnumerator MulliganCoroutine() {
        FirstHandMulligan();
        yield return new WaitForSeconds(GameManager.TIME_MULLIGAN);
        if (!clickValidMulligan) {
            clickValidMulligan = true;
            yield return StartCoroutine(ValidMulligan());
        }
    }

    // On affiche la main en mode "Normal"
    public void InitHand() {
        // On modifie la main (position, scale)
        GO_Hand.transform.localPosition = handPositionCached;
        GO_Hand.transform.localScale = handScaleCached;

        // On desactive le fond du parent (HandWrap)
        GetComponent<SpriteRenderer>().enabled = false;
        // On desactive le collider
        GetComponent<BoxCollider2D>().enabled = false;

        // On modifie le horizontal layout
        GO_Hand.GetComponent<HorizontalLayoutGroup>().childAlignment = childAlignementCached;
        GO_Hand.GetComponent<HorizontalLayoutGroup>().spacing = childSpacingCached;

        // On change le status des cartes
        foreach (Transform card in GO_Hand.transform) {
            card.GetComponent<CardDisplay>().status = CardStatus.Hand;
            // On descative les aura des cartes
            card.GetComponent<SbireDisplay>().attackAura.SetActive(false);
        }

        // On cache les boutons
        GO_MulliganFeatures.SetActive(false);
        GO_ButtonShowBoard.SetActive(false);
    }

    // On affiche la main en mode "Mulligan"
    public void FirstHandMulligan() {
        // On sauvegarde les valeurs par défauts
        handPositionCached = GO_Hand.transform.localPosition;
        handScaleCached = GO_Hand.transform.localScale;
        childAlignementCached = GO_Hand.GetComponent<HorizontalLayoutGroup>().childAlignment;
        childSpacingCached = GO_Hand.GetComponent<HorizontalLayoutGroup>().spacing;

        // On modifie la main (position, scale)
        GO_Hand.transform.localPosition = handMulliganPosition;
        GO_Hand.transform.localScale = handMulliganScale;

        // On active le fond du parent (HandWrap)
        GetComponent<SpriteRenderer>().enabled = true;
        // On active le collider
        GetComponent<BoxCollider2D>().enabled = true;

        // On modifie le horizontal layout
        GO_Hand.GetComponent<HorizontalLayoutGroup>().childAlignment = childAlignementMulligan;
        GO_Hand.GetComponent<HorizontalLayoutGroup>().spacing = childSpacingMulligan;

        // On change le status des cartes
        foreach (Transform card in GO_Hand.transform) {
            card.GetComponent<CardDisplay>().MulliganMode();
        }

        // On affiche les boutons
        GO_MulliganFeatures.SetActive(true);
        GO_ButtonShowBoard.SetActive(true);
    }

    // Button d'affichage du board pendant le mulligan
    public void MulliganShowBoard() {
        // On cache la fenêtre de mulligan
        if (mulliganIsVisible) {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            GO_Hand.SetActive(false);
            GO_MulliganFeatures.SetActive(false);
            GO_ButtonShowBoard.GetComponentInChildren<TMP_Text>().text = butonshowBoardTextInBoard;
            mulliganIsVisible = false;
        } 
        // On affiche la fenêtre de mulligan
        else {
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = true;
            GO_Hand.SetActive(true);
            GO_MulliganFeatures.SetActive(true);
            GO_ButtonShowBoard.GetComponentInChildren<TMP_Text>().text = butonShowBoardTextInMulligan;
            mulliganIsVisible = true;
        }
    }

    /// <summary>
    /// Lors d'un click sur le bouton de validation du mulligan
    /// </summary>
    public void ButtonValidMulliganClick() {
        if (clickValidMulligan) return;

        clickValidMulligan = true;
        StartCoroutine(ValidMulligan());
    }

    /// <summary>
    /// Valide le mulligan
    /// </summary>
    public IEnumerator ValidMulligan() {
        // On replace les cartes sélectionné dans le deck
        List<GameObject> listCardMulligan = new List<GameObject>();
        foreach (Transform cardTransform in GO_Hand.transform) {
            if (cardTransform.GetComponent<CardDisplay>().selectedForMulligan) {
                listCardMulligan.Add(cardTransform.gameObject);
            }
        }

        int amountCardForMulligan = listCardMulligan.Count;
        foreach (GameObject card in listCardMulligan) {
            card.GetComponent<CardDisplay>().ReturnInDeck();
        }

        yield return new WaitForSeconds(0.5f);

        // On repioche le même nombre de carte
        FindAnyObjectByType<GameManager>().draw(amountCardForMulligan);
        foreach (Transform cardTransform in GO_Hand.transform) {
            cardTransform.GetComponent<CardDisplay>().MulliganMode();
        }

        yield return new WaitForSeconds(2f);

        InitHand();
        //StopCoroutine(GameManager.mulliganCoroutine);
    }
}
