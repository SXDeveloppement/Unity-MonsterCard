using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public Card card;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text manaText;
    public TMP_Text priorityText;
    public TMP_Text typeText;
    public Image artworkImage;

    public Status status;

    private string cardDescriptionCached;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
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
    void Update()
    {
        
    }

    // On active les effets de la carte
    public void activeCard(GameObject target) {
        card.activeEffect(target);
        gameObject.GetComponent<ZoomCard>().destroyPlaceholder();
        gameManager.inGrave(gameObject);
    }

    // Renvoi les dégats de base de l'attaque
    public List<int> getBaseDamage() {
        string pattern = @"\%D(\d+)";
        List<int> intList = new List<int>();

        MatchCollection m = Regex.Matches(cardDescriptionCached, pattern, RegexOptions.IgnoreCase);
        foreach(Match m2 in m) {
            intList.Add(int.Parse(m2.Groups[1].Value));
        }

        return intList;
    }

    // On met a jour la description de la carte avec les dégâts qui seront réellement infligés au monstre adverse
    public void refreshDescriptionDamage() {
        string pattern = @"\%D\d+";
        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
        //int trueDamage = gameManager.calculateDamage(gameManager.GO_MonsterInvokedOppo, card.elementalAffinity, getBaseDamage());

        string output = null;
        foreach(int baseDamage in getBaseDamage()) {
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
}
