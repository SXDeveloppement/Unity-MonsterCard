using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Random = System.Random;

public class MonsterDisplay : MonoBehaviour, IDropHandler
{
    public Monster monster;
    public TMP_Text nameText;
    public GameObject GO_LifeBar;
    public GameObject GO_ManaBar;
    public TMP_Text powerText;
    public TMP_Text guardText;
    public TMP_Text speedText;
    public Image artworkImage;
    public GameObject GO_Affinity;

    public int healthAvailable;
    public int healthMax;
    public int manaMax;
    public int manaAvailable;
    public int powerEquiped; // Power du monstre + des équipements
    public int guardEquiped; // Guard du monstre + des équipements
    public int speedEquiped; // Speed du monstre + des équipements

    public List<Card> deckList; // Liste des cartes dans le deck
    public List<Card> graveList; // Liste des cartes dans le cimetière
    public List<Equipment> equipmentList; // Liste des équipements du monstre

    public List<Card> cardEnchantments; // Liste des cartes d'enchantement


    public bool ownedByOppo;
    public bool isKO;

    Vector2 lifeBarSizeCached;
    Vector2 manaBarSizeCached;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cardEnchantments = new List<Card>() {
            ScriptableObject.CreateInstance<Card>(),
            ScriptableObject.CreateInstance<Card>(),
            ScriptableObject.CreateInstance<Card>(),
            ScriptableObject.CreateInstance<Card>()
        };

        // Création du deck de 30 cartes avec des cartes aléatoires de la DB
        Card[] DBCards = Resources.LoadAll<Card>("Cards");
        for (int i = 0; i < 30; i++) {
            Random rand = new Random();
            deckList.Add(DBCards[rand.Next(DBCards.Length)]);
        }

        // Choisi 4 équipements aléatoire pour le monstre
        Equipment[] DBEquipment = Resources.LoadAll<Equipment>("Equipments");
        for (int i = 0; i < 4; i++) {
            Random rand = new Random();
            equipmentList.Add(DBEquipment[rand.Next(DBEquipment.Length)]);
        }

        // Ajouter la vie bonus des équipements
        healthMax = monster.healthPoint;
        foreach (Equipment equipment in equipmentList) {
            healthMax += equipment.healthPoint;
        }
        healthAvailable = healthMax;

        manaMax = 1;
        manaAvailable = manaMax;

        artworkImage.sprite = monster.artwork;
        // On actualise les stats du monstre
        refreshStats();

        // Affichage des affinités élémentaires du monstre
        foreach (ElementalAffinity affinity in monster.elementalAffinity) {
            GO_Affinity.transform.Find(affinity.ToString()).gameObject.SetActive(true);
        }

        refreshHealthPoint();
        refreshManaPoint();

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (healthAvailable <= 0) {
            isKO = true;
        }
    }

    void IDropHandler.OnDrop(PointerEventData eventData) {
        GameObject cardPlayed = eventData.pointerDrag;
        GameObject target = gameObject;

        // On vérifie les conditions de ciblage pour pouvoir activer la carte
        bool targetCondition = false;
        TargetType[] cardPlayedTargetType = cardPlayed.GetComponent<CardDisplay>().card.targetType;
        foreach (TargetType targetType in cardPlayedTargetType) {
            if (!ownedByOppo && targetType == TargetType.PlayerMonster
                || ownedByOppo && targetType == TargetType.OpponantMonster) {
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

    // Modifie l'affichage pour le monstre de l'adversaire
    public void ownerOppo() {
        ownedByOppo = true;
        Vector3 flipX = new Vector3(-1f, 1f, 1f);
        // On flip sur X tout le prefab
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        // On reflip sur X tous les textes
        powerText.gameObject.transform.localScale = flipX;
        guardText.gameObject.transform.localScale = flipX;
        speedText.gameObject.transform.localScale = flipX;
        GO_LifeBar.transform.Find("Text").localScale = flipX;
        GO_ManaBar.transform.Find("Text").localScale = flipX;
    }

    // Actualise les stats
    public void refreshStats() {
        int power = monster.powerPoint;
        foreach (Equipment equipment in equipmentList) {
            power += equipment.powerPoint;
        }
        powerEquiped = power;
        powerText.text = power.ToString();

        int guard = monster.guardPoint;
        foreach (Equipment equipment in equipmentList) {
            guard += equipment.guardPoint;
        }
        guardEquiped = guard;
        guardText.text = guard.ToString();

        int speed = monster.speedPoint;
        foreach (Equipment equipment in equipmentList) {
            speed += equipment.speedPoint;
        }
        speedEquiped = speed;
        speedText.text = speed.ToString();
    }

    // Actualise la barre de vie
    public void refreshHealthPoint() {
        GO_LifeBar.transform.Find("Text").GetComponent<TMP_Text>().text = healthAvailable.ToString() + "/" + healthMax.ToString();

        // On modifie la taille de la barre
        GO_LifeBar.transform.Find("Health").transform.localScale = new Vector3((float)healthAvailable / healthMax, 1f, 1f);
    }

    // Actualise la barre de mana
    public void refreshManaPoint() {
        GO_ManaBar.transform.Find("Text").GetComponent<TMP_Text>().text = manaAvailable.ToString() + "/" + manaMax.ToString();

        // On modifie la taille de la barre
        GO_ManaBar.transform.Find("Mana").transform.localScale = new Vector3((float)manaAvailable / manaMax, 1f, 1f);
    }

    // Réinitiliation du mana
    public void resetMana() {
        manaAvailable = manaMax;
        refreshManaPoint();
    }

    // Action lors d'un nouveau tour
    public void newTurn() {
        manaMax++;
        if (manaMax > 10) {
            manaMax = 10;
        }

        resetMana();
    }

    // Prendre des dégâts
    public void takeDamage(int amountDamage, ElementalAffinity affinityAttack) {
        healthAvailable -= gameManager.calculateDamage(gameObject, affinityAttack, amountDamage);
        if (healthAvailable < 0) {
            healthAvailable = 0;
        }
        refreshHealthPoint();
    }
}
