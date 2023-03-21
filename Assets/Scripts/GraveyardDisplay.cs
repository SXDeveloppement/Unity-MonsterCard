using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GraveyardDisplay : MonoBehaviour, IScrollHandler
{

    public int pageViewed;
    public int cardPerPage = 14;
    public int maxPage;
    public bool ownedByOppo;
    public GameObject layoutArea;
    public GameObject scrollbar;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void hide() {
        gameObject.SetActive(false);
    }

    public void show() {
        if (gameObject.activeSelf) {
            hide();
        } else {
            gameObject.SetActive(true);
            pageViewed = 1;
            showCardInGrave(pageViewed);
        }
    }

    public void showCardInGrave(int page) {
        GameManager gameManager = GameObject.FindAnyObjectByType<GameManager>();
        // On détruit toutes les cartes du cimetière
        foreach (Transform child in layoutArea.transform) {
            Destroy(child.gameObject);
        }

        GameObject monster;
        if (ownedByOppo) {
            monster = GameManager.GO_MonsterInvokedOppo;
        } else {
            monster = GameManager.GO_MonsterInvoked;
        }
        int countCardInGrave = monster.GetComponent<MonsterDisplay>().graveList.Count;
        maxPage = countCardInGrave / cardPerPage;
        if (countCardInGrave % cardPerPage > 0)
            maxPage++;

        if (page < 1)
            page = 1;
        else if (page > maxPage)
            page = maxPage;

        int limitMin = ((page - 1) * cardPerPage);
        int limitMax = limitMin + cardPerPage;
        if (limitMax > countCardInGrave)
            limitMax = countCardInGrave;

        if (limitMax > 0)
            for (int i = limitMin; i < limitMax; i++) {
                Card card = monster.GetComponent<MonsterDisplay>().graveList[i];
                GameObject newCardInGrave = Instantiate(gameManager.GO_Card);
                newCardInGrave.GetComponent<CardDisplay>().card = card;
                newCardInGrave.transform.SetParent(layoutArea.transform);
                newCardInGrave.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                newCardInGrave.transform.localPosition = Vector3.zero;
                newCardInGrave.GetComponent<CardDisplay>().status = CardStatus.Graveyard;
                //newCardInGrave.GetComponent<CardDisplay>().monsterOwnThis = monster.GetComponent<MonsterDisplay>();
                newCardInGrave.GetComponent<OwnedByOppo>().monsterOwnThis = monster.GetComponent<MonsterDisplay>();
            }

        scrollbar.GetComponent<ScrollBarDisplay>().initScrollbar(maxPage, page);
    }
    
    public void OnScroll(PointerEventData eventData) {
        if (eventData.scrollDelta.y > 0) {
            pageViewed--;
            if (pageViewed < 1) {
                pageViewed = 1;
                return;
            }          
        } else {
            pageViewed++;
            if (pageViewed > maxPage) {
                pageViewed = maxPage;
                return;
            }
        }

        showCardInGrave(pageViewed);
    }
}
