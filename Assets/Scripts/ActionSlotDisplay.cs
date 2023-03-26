using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ActionSlotDisplay : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void AddActionGO(GameObject gameObjectAction, GameObject target, bool isVisible = true, bool isPass = false, bool isSwap = false) {
        GameObject gameObjectAction2 = null;

        ExecuteEvents.Execute(gameObjectAction, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);

        // Si c'est une carte ou une capacité
        if (gameObjectAction.GetComponent<CardDisplay>() != null || gameObjectAction.GetComponent<AbilityDisplay>() != null) {
            // Si c'est une carte (autre qu'un sbire sur le terrain qui attaque)
            if (gameObjectAction.GetComponent<CardDisplay>() != null && (gameObjectAction.GetComponent<CardDisplay>().card.type != CardType.Sbire || gameObjectAction.GetComponent<CardDisplay>().status != CardStatus.SlotVisible)) {
                gameObjectAction2 = gameObjectAction;
                gameObjectAction2.GetComponent<LayoutElement>().ignoreLayout = false;
                gameObjectAction2.GetComponent<CardDisplay>().status = CardStatus.ActionSlot;
            }
            // Si c'est un sbire sur le terrain
            else if (gameObjectAction.GetComponent<CardDisplay>() != null && gameObjectAction.GetComponent<CardDisplay>().card.type == CardType.Sbire && gameObjectAction.GetComponent<CardDisplay>().status == CardStatus.SlotVisible) {
                // On clone le sbire
                gameObjectAction2 = Instantiate(gameObjectAction);
                gameObjectAction2.GetComponent<LayoutElement>().ignoreLayout = false;
                gameObjectAction2.GetComponent<CardDisplay>().status = CardStatus.ActionSlot;
            }
            // Si c'est une capacité
            else if (gameObjectAction.GetComponent<AbilityDisplay>() != null) {
                // On clone la capacité et on deplace la carte dans un emplacement d'action
                gameObjectAction2 = Instantiate(gameObjectAction);
                gameObjectAction2.GetComponent<AbilityDisplay>().abilityStatus = AbilityStatus.Action;
                gameObjectAction2.GetComponent<AbilityDisplay>().ShowHideTooltip(true);
                gameObjectAction2.GetComponent<AbilityDisplay>().refreshDisplayAbility();
            }

            gameObjectAction2.transform.SetParent(transform);
            gameObjectAction2.transform.localPosition = Vector3.zero;
            Vector3 newVec3 = Constante.FlatScale(Constante.SCALE_CARD_ACTION);
            gameObjectAction2.transform.localScale = newVec3;

            // On active la la fleche de ciblage
            GameObject arrowEmitter = FindAnyObjectByType<GameManager>().ArrowEmitter;
            arrowEmitter.GetComponent<BezierArrow>().isFixed = true;
            arrowEmitter.transform.position = new Vector3(gameObjectAction2.transform.position.x, gameObjectAction2.transform.position.y, -3);
            arrowEmitter.GetComponent<BezierArrow>().targetPosition = target.transform.position;
            arrowEmitter.SetActive(true);
            arrowEmitter.GetComponent<BezierArrow>().activeTargetArrow();
        }
        // Si c'est une action de swap
        else if (isSwap) {
            Debug.Log("Affiche card swap");
        }
        // Si c'est une action de passer
        else if (isPass) {
            GameObject GO_CardPass = FindAnyObjectByType<GameManager>().GO_CardPass;
            gameObjectAction2 = Instantiate(GO_CardPass);
            gameObjectAction2.GetComponent<LayoutElement>().ignoreLayout = false;
            gameObjectAction2.GetComponent<CardDisplay>().status = CardStatus.ActionSlot;
            gameObjectAction2.transform.SetParent(transform);
            gameObjectAction2.transform.localPosition = Vector3.zero;
            Vector3 newVec3 = Constante.FlatScale(Constante.SCALE_CARD_ACTION);
            gameObjectAction2.transform.localScale = newVec3;

            Debug.Log("Affiche card pass");
        }
    }
}
