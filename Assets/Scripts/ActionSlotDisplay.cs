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

    public void AddActionGO(GameObject gameObjectAction, GameObject target, bool isVisible = true) {
        GameObject gameObjectAction2 = null;

        // Si c'est une carte (autre qu'un sbire sur le terrain qui attaque)
        if (gameObjectAction.GetComponent<CardDisplay>() != null && (gameObjectAction.GetComponent<CardDisplay>().card.type != CardType.Sbire || gameObjectAction.GetComponent<CardDisplay>().status != CardStatus.SlotVisible)) {
            gameObjectAction2 = gameObjectAction;

            // On deplace la carte dans un emplacement d'action
            gameObjectAction.transform.SetParent(transform);
            ExecuteEvents.Execute(gameObjectAction, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
            gameObjectAction.transform.localPosition = Vector3.zero;
            gameObjectAction.transform.localScale = Vector3.one;
            gameObjectAction.GetComponent<LayoutElement>().ignoreLayout = false;
            gameObjectAction.GetComponent<CardDisplay>().status = CardStatus.ActionSlot;
        }
        // Si c'est un sbire sur le terrain
        else if (gameObjectAction.GetComponent<CardDisplay>() != null && gameObjectAction.GetComponent<CardDisplay>().card.type == CardType.Sbire && gameObjectAction.GetComponent<CardDisplay>().status == CardStatus.SlotVisible) {
            // On clone le sbire
            gameObjectAction2 = Instantiate(gameObjectAction);

            // On deplace la carte dans un emplacement d'action
            gameObjectAction2.transform.SetParent(transform);
            ExecuteEvents.Execute(gameObjectAction, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
            gameObjectAction2.transform.localPosition = Vector3.zero;
            gameObjectAction2.transform.localScale = Vector3.one;
            gameObjectAction2.GetComponent<LayoutElement>().ignoreLayout = false;
            gameObjectAction2.GetComponent<CardDisplay>().status = CardStatus.ActionSlot;
        }
        // Si c'est une capacité
        else if (gameObjectAction.GetComponent<AbilityDisplay>() != null) {
            // On clone la capacité
            gameObjectAction2 = Instantiate(gameObjectAction);

            // On deplace la carte dans un emplacement d'action
            gameObjectAction2.transform.SetParent(transform);
            ExecuteEvents.Execute(gameObjectAction, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
            gameObjectAction2.transform.localPosition = Vector3.zero;
            gameObjectAction2.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        }

        // On active la la fleche de ciblage
        GameObject arrowEmitter = FindAnyObjectByType<GameManager>().ArrowEmitter;
        arrowEmitter.GetComponent<BezierArrow>().isFixed = true;
        arrowEmitter.transform.position = new Vector3(gameObjectAction2.transform.position.x, gameObjectAction2.transform.position.y, -3);
        arrowEmitter.GetComponent<BezierArrow>().targetPosition = target.transform.position;
        arrowEmitter.SetActive(true);
        arrowEmitter.GetComponent<BezierArrow>().activeTargetArrow();
    }
}
