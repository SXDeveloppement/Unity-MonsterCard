using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour, IDropHandler {
    public Card card;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text manaText;
    public TMP_Text priorityText;
    public TMP_Text typeText;
    public Image artworkImage;

    public GameObject ownByMonster; // GO du monstre qui possède cette carte
    public Status status;
    public bool hiddenCard;

    public AnimationCurve scaleCurve;
    public float duration = 0.5f;

    private string cardDescriptionCached;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start() {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        nameText.text = card.name;
        descriptionText.text = card.description;
        manaText.text = card.manaCost.ToString();
        priorityText.text = card.priority.ToString();
        typeText.text = card.type.ToString();
        artworkImage.sprite = card.artwork;

        cardDescriptionCached = card.description;

        refreshDescriptionDamage();
    }

    // Update is called once per frame
    void Update() {

    }

    void IDropHandler.OnDrop(PointerEventData eventData) {
        GameObject cardPlayed = eventData.pointerDrag;
        GameObject target = gameObject;

        // On vérifie les conditions de ciblage pour pouvoir activer la carte
        bool targetCondition = false;
        TargetType[] cardPlayedTargetType = cardPlayed.GetComponent<CardDisplay>().card.targetType;
        foreach (TargetType cardTargetType in cardPlayedTargetType) {
            if (card.type == Type.Aura && (cardTargetType == TargetType.OpponantCardAura && ownByMonster.GetComponent<MonsterDisplay>().ownedByOppo || cardTargetType == TargetType.PlayerCardAura && !ownByMonster.GetComponent<MonsterDisplay>().ownedByOppo) || card.type == Type.Enchantment && (cardTargetType == TargetType.OpponantCardEnchantment && ownByMonster.GetComponent<MonsterDisplay>().ownedByOppo || cardTargetType == TargetType.PlayerCardEnchantment && !ownByMonster.GetComponent<MonsterDisplay>().ownedByOppo)) 
            {
                targetCondition = true;
                break;
            }
        }

        // On active la carte si les conditions de ciblages sont respectées
        if (targetCondition) {
            gameManager.activeCardOnTarget(cardPlayed, target);
        } else {
            Debug.Log("ERR : bad target");
        }
    }

    // On active les effets de la carte
    public void activeCard(GameObject target) {
        card.activeEffect(target);
        gameObject.GetComponent<ZoomCard>().destroyPlaceholder();
        gameManager.inGrave(gameObject);
    }

    // On place la carte face visible
    public void putOnBoard(GameObject target, bool isVisible) {
        if (isVisible) {
            showVisibleFace();
            hiddenCard = false;
        } else {
            showHiddenFace();
            hiddenCard = true;
        }

        transform.SetParent(target.transform);
        status = Status.Board;
        GetComponent<ZoomCard>().reinitCard();
    }

    // Renvoi les dégats de base de l'attaque
    public List<int> getBaseDamage() {
        string pattern = @"\%D(\d+)";
        List<int> intList = new List<int>();

        MatchCollection m = Regex.Matches(cardDescriptionCached, pattern, RegexOptions.IgnoreCase);
        foreach (Match m2 in m) {
            intList.Add(int.Parse(m2.Groups[1].Value));
        }

        return intList;
    }

    // On met a jour la description de la carte avec les dégâts qui seront réellement infligés au monstre adverse
    public void refreshDescriptionDamage() {
        string pattern = @"\%D\d+";
        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

        string output = null;
        foreach (int baseDamage in getBaseDamage()) {
            int trueDamage = gameManager.calculateDamage(gameManager.GO_MonsterInvokedOppo, card.elementalAffinity, baseDamage);
            if (output != null) {
                output = regex.Replace(output, trueDamage.ToString(), 1);
            } else {
                output = regex.Replace(cardDescriptionCached, trueDamage.ToString(), 1);
            }
        }
        if (output != null) {
            descriptionText.text = output;
        }

    }

    // On retourne la carte face caché
    public void showHiddenFace() {
        gameObject.transform.Find("Front").gameObject.SetActive(false);
        gameObject.transform.Find("Back").gameObject.SetActive(true);
    }

    // On retourne la carte face visible
    public void showVisibleFace() {
        gameObject.transform.Find("Front").gameObject.SetActive(true);
        gameObject.transform.Find("Back").gameObject.SetActive(false);
    }

    // Animation de retournement de carte face visibile
    public IEnumerator flipFront() {
        float time = 0f;
        bool endLoop = false;
        while (!endLoop) {
            float scale;
            if (time >= 1f) {
                endLoop = true;
                scale = scaleCurve.Evaluate(1f);
            } else {
                scale = scaleCurve.Evaluate(time);
            }
            
            if (time < 0.5f) {
                Transform cardBack = transform.Find("Back");
                cardBack.localScale = new Vector3(scale, cardBack.localScale.y, cardBack.localScale.z);
            } else {
                showVisibleFace();
                Transform cardFront = transform.Find("Front");
                cardFront.localScale = new Vector3(scale, cardFront.localScale.y, cardFront.localScale.z);
            }           

            time = time + Time.deltaTime / duration;
            yield return new WaitForFixedUpdate();
        }
    }

    // Animation de retournement de carte face visibile
    public IEnumerator flipBack() {
        float time = 0f;
        bool endLoop = false;
        while (!endLoop) {
            float scale;
            if (time >= 1f) {
                endLoop = true;
                scale = scaleCurve.Evaluate(1f);
            } else {
                scale = scaleCurve.Evaluate(time);
            }

            if (time < 0.5f) {
                Transform cardFront = transform.Find("Front");
                cardFront.localScale = new Vector3(scale, cardFront.localScale.y, cardFront.localScale.z);
            } else {
                showHiddenFace();
                Transform cardBack = transform.Find("Back");
                cardBack.localScale = new Vector3(scale, cardBack.localScale.y, cardBack.localScale.z);
            }

            time = time + Time.deltaTime / duration;
            yield return new WaitForFixedUpdate();
        }
    }
}
