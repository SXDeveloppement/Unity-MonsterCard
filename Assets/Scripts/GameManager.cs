using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = System.Random;

public class GameManager : MonoBehaviour {
    public static bool dragged; // TRUE si on drag&drop une carte

    public GameObject GO_Card; // Prefab
    public GameObject GO_Hand;
    public GameObject GO_DeckText;
    public GameObject GO_GraveText;
    public GameObject GO_GravePlayerList;
    public GameObject GO_Monster; // Prefab
    public GameObject GO_MonsterArea;
    public static GameObject GO_MonsterInvoked;
    public List<GameObject> monstersGOList; // Liste des GO de tous les monstres
    public GameObject GO_MonsterAreaOppo;
    public static GameObject GO_MonsterInvokedOppo;
    public List<GameObject> monstersGOListOppo; // Liste des GO de tous les monstres de l'adversaire
    public GameObject GO_MonsterTeamLayout;
    public GameObject GO_TeamArea;
    public GameObject GO_CounterAttackArea;
    public GameObject GO_CounterAttackAreaOppo;
    public GameObject GO_AuraArea;
    public GameObject GO_AuraAreaOppo;
    public GameObject GO_Equipment; // Prefab
    public GameObject GO_EquipmentArea;
    public GameObject GO_EquipmentAreaOppo;
    public GameObject GO_BuffDebuff; // Prefab
    public GameObject GO_BuffArea;
    public GameObject GO_DebuffArea;
    public GameObject GO_BuffAreaOppo;
    public GameObject GO_DebuffAreaOppo;

    public Texture2D cursorTargetTexture; // Icon cursor lors d'un ciblage
    public Texture2D cursorNoTargetTexture; // Icon cursor lorsque la cible n'est pas valide
    public GameObject ArrowEmitter; // Fleche de ciblage dynamique

    #region Dictionary Elemental Affinity
    static Dictionary<ElementalAffinity, float> fireDico;
    static Dictionary<ElementalAffinity, float> waterDico;
    static Dictionary<ElementalAffinity, float> electricDico;
    static Dictionary<ElementalAffinity, float> earthDico;
    static Dictionary<ElementalAffinity, float> combatDico;
    static Dictionary<ElementalAffinity, float> mentalDico;
    static Dictionary<ElementalAffinity, float> neutralDico;
    #endregion

    private bool init = true;

    // DEBUG
    public List<Card> sbireList;
    public List<Card> auraList;

    // Start is called before the first frame update
    void Start()
    {
        // DEBUG on efface les prefab monstre et équipement en place
        Destroy(GO_MonsterArea.transform.GetChild(0).gameObject);
        Destroy(GO_MonsterAreaOppo.transform.GetChild(0).gameObject);
        foreach (EquipmentDisplay equipment in GO_EquipmentArea.GetComponentsInChildren<EquipmentDisplay>()) {
            Destroy(equipment.gameObject);
        }
        foreach (EquipmentDisplay equipment in GO_EquipmentAreaOppo.GetComponentsInChildren<EquipmentDisplay>()) {
            Destroy(equipment.gameObject);
        }

        dragged = false;
        initializeArrayAffinity();

        Monster[] DBMonsters = Resources.LoadAll<Monster>("Monsters");
        // Création de l'équipe de 4 monstres avec des monstres aléatoires de la DB
        List<Monster> addedMonster = new List<Monster>();
        int i = 0;
        while (monstersGOList.Count < 4) {
            Random rand = new Random();
            int randomInt = rand.Next(DBMonsters.Length);
            if (!addedMonster.Contains(DBMonsters[randomInt])) {
                // Instantie le monstre
                GameObject newMonster = Instantiate(GO_Monster);
                newMonster.name = "Monster_" + i;
                newMonster.GetComponent<MonsterDisplay>().monster = DBMonsters[randomInt];
                newMonster.transform.SetParent(GO_MonsterArea.transform);
                newMonster.transform.localPosition = Vector3.zero;
                newMonster.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                newMonster.GetComponent<MonsterDisplay>().ownedByOppo = false;

                // On ajoute le nouveau monstre a la liste
                monstersGOList.Add(newMonster);
                addedMonster.Add(DBMonsters[randomInt]);

                // On instantie le monstre dans la fenêtre d'équipe du joueur
                GameObject newMonsterTeamLayout = Instantiate(GO_MonsterTeamLayout, GO_TeamArea.GetComponent<TeamLayoutDisplay>().layoutArea.transform);
                newMonsterTeamLayout.name = "MonsterTeamLayout";
                newMonsterTeamLayout.GetComponent<MonsterLayoutTeamDisplay>().monsterLinked = newMonster;
                newMonster.GetComponent<MonsterDisplay>().monsterLayoutTeamLinked = newMonsterTeamLayout;
            }
            i++;
        }


        // Création de l'équipe de 4 monstre pour l'adversaire
        addedMonster = new List<Monster>();
        int j = 0;
        while (monstersGOListOppo.Count < 4) {
            Random rand2 = new Random();
            int randomInt2 = rand2.Next(DBMonsters.Length);
            if (!addedMonster.Contains(DBMonsters[randomInt2])) {
                // Instantie le monstre
                GameObject newMonsterOppo = Instantiate(GO_Monster);
                newMonsterOppo.name = "MonsterOppo_" + j;
                newMonsterOppo.GetComponent<MonsterDisplay>().monster = DBMonsters[randomInt2];
                newMonsterOppo.transform.SetParent(GO_MonsterAreaOppo.transform);
                newMonsterOppo.transform.localPosition = Vector3.zero;
                newMonsterOppo.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                newMonsterOppo.GetComponent<MonsterDisplay>().ownerOppo();

                // On ajoute le nouveau monstre a la liste
                monstersGOListOppo.Add(newMonsterOppo);

                addedMonster.Add(DBMonsters[randomInt2]);
            }
            j++;
        }

        GO_MonsterInvoked = monstersGOList[0];
        GO_MonsterInvokedOppo = monstersGOListOppo[0];

        GO_TeamArea.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (init) {
            instantiateEquipment(GO_MonsterInvoked);
            instantiateEquipment(GO_MonsterInvokedOppo);
            foreach (Transform child in GO_TeamArea.GetComponent<TeamLayoutDisplay>().layoutArea.transform) {
                child.gameObject.GetComponent<MonsterLayoutTeamDisplay>().refreshMonsterUI();
            }
            GO_TeamArea.SetActive(false);
            refreshDeckText();
            refreshGraveText();
            init = false;
        }
    }

    // On instantie l'équipement d'un monstre
    public void instantiateEquipment(GameObject monster) {
        int i = 0;
        foreach (Equipment equipment in monster.GetComponent<MonsterDisplay>().equipmentList) {
            GameObject newEquipment = Instantiate(GO_Equipment);
            newEquipment.GetComponent<EquipmentDisplay>().equipment = equipment;
            newEquipment.GetComponent<EquipmentDisplay>().slotId = i;
            if (monster == GO_MonsterInvoked) {
                GameObject slotEquipment = GO_EquipmentArea.transform.GetChild(i).GetChild(0).gameObject;
                newEquipment.transform.SetParent(slotEquipment.transform);
            } else {
                GameObject slotEquipment = GO_EquipmentAreaOppo.transform.GetChild(i).GetChild(0).gameObject;
                newEquipment.transform.SetParent(slotEquipment.transform);
            }
            newEquipment.GetComponent<EquipmentDisplay>().monsterOwnThis = monster.GetComponent<MonsterDisplay>();
            newEquipment.transform.localScale = Vector3.one;
            newEquipment.transform.localPosition = Vector3.zero;

            // On instantie les cartes enchantements
            if (monster.GetComponent<MonsterDisplay>().cardEnchantments[i].name != null) {
                GameObject newCardEnchantment = Instantiate(GO_Card);
                newCardEnchantment.GetComponent<CardDisplay>().card = monster.GetComponent<MonsterDisplay>().cardEnchantments[i];
                newCardEnchantment.GetComponent<CardDisplay>().monsterOwnThis = monster.GetComponent<MonsterDisplay>();
                newEquipment.GetComponent<EquipmentDisplay>().cardOnSlot = newCardEnchantment;
                newCardEnchantment.GetComponent<CardDisplay>().putOnBoard(newEquipment, true);
            }

            i++;
        }
    }
    // Event OnEndTurn
    public static event Action OnEndTurn;
    // Termine le tour en cours
    public void endTurn() {
        // On retire un tour au buff / debuff
        buffDebuffAddTurn(-1);
        buffDebuffAddTurnOppo(-1);
        refreshBuffDebuff();

        // On active les listeners
        OnEndTurn?.Invoke();

        newTurn();
    }

    // Event OnNewTurn
    public static event Action OnNewTurn;
    // Commende un nouveau tour
    public void newTurn() {
        //draw(1);
        GO_MonsterInvoked.GetComponent<MonsterDisplay>().newTurn();

        // On passe les sbires sur le terrain en position repos
        foreach (SbireDisplay sbireDisplay in GO_CounterAttackArea.transform.GetComponentsInChildren<SbireDisplay>()) {
            if (sbireDisplay.sbireHealthAvailable > 0) {
                sbireDisplay.newTurn();
            }
        }
        foreach (SbireDisplay sbireDisplay in GO_CounterAttackAreaOppo.transform.GetComponentsInChildren<SbireDisplay>()) {
            if (sbireDisplay.sbireHealthAvailable > 0) {
                sbireDisplay.newTurn();
            }
        }

        // On passe les cartes "Echo" du terrain en position activable
        foreach (CardDisplay cardDisplay in GO_CounterAttackArea.transform.GetComponentsInChildren<CardDisplay>()) {
            if (cardDisplay.name != null) {
                cardDisplay.putOnBoardThisTurn = false;
            }
        }
        foreach (CardDisplay cardDisplay in GO_CounterAttackAreaOppo.transform.GetComponentsInChildren<CardDisplay>()) {
            if (cardDisplay.name != null) {
                cardDisplay.putOnBoardThisTurn = false;
            }
        }

        // On active les listeners
        OnNewTurn?.Invoke();
    }

    // Event OnDraw
    public static event Action<MonsterDisplay> OnDraw;
    // Ajoute X carte dans la main
    public void draw(int drawAmount) {
        if (GO_MonsterInvoked.GetComponent<MonsterDisplay>().deckList.Count > 0) {
            for (int i = 0; i < drawAmount; i++) {
                GameObject newCard = Instantiate(GO_Card);
                newCard.gameObject.transform.SetParent(GO_Hand.gameObject.transform);
                Random rand = new Random();
                int iRand = rand.Next(GO_MonsterInvoked.GetComponent<MonsterDisplay>().deckList.Count);
                newCard.GetComponent<CardDisplay>().card = GO_MonsterInvoked.GetComponent<MonsterDisplay>().deckList[iRand];
                newCard.name = newCard.GetComponent<CardDisplay>().card.name;
                newCard.GetComponent<CardDisplay>().status = CardStatus.Hand;
                newCard.GetComponent<CardDisplay>().monsterOwnThis = GO_MonsterInvoked.GetComponent<MonsterDisplay>();
                GO_MonsterInvoked.GetComponent<MonsterDisplay>().deckList.RemoveAt(iRand);
            }
            refreshDeckText();
            GO_Hand.GetComponent<HandDisplay>().childHaveChanged = true;

            // On active les listeners
            OnDraw?.Invoke(GO_MonsterInvoked.GetComponent<MonsterDisplay>());
        } else {
            Debug.Log("ERR : no card in deck");
        }
    }

    // Event OnDrawOppo
    public static event Action<MonsterDisplay> OnDrawOppo;
    // Ajoute X carte dans la main
    public void drawOppo(int drawAmount) {
        Debug.Log("Draw oppo");
        if (GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>().deckList.Count > 0) {
            for (int i = 0; i < drawAmount; i++) {
                if (GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>().deckList.Count > 0) {
                    //******************
                    // Carte a ajouté dans la main de l'adversaire
                    //*******************

                    //GameObject newCard = Instantiate(GO_Card);
                    //newCard.gameObject.transform.SetParent(GO_Hand.gameObject.transform);
                    //Random rand = new Random();
                    //int iRand = rand.Next(GO_MonsterInvoked.GetComponent<MonsterDisplay>().deckList.Count);
                    //newCard.GetComponent<CardDisplay>().card = GO_MonsterInvoked.GetComponent<MonsterDisplay>().deckList[iRand];
                    //newCard.name = newCard.GetComponent<CardDisplay>().card.name;
                    //newCard.GetComponent<CardDisplay>().status = CardStatus.Hand;
                    //newCard.GetComponent<CardDisplay>().monsterOwnThis = GO_MonsterInvoked.GetComponent<MonsterDisplay>();
                    //GO_MonsterInvoked.GetComponent<MonsterDisplay>().deckList.RemoveAt(iRand);
                }
            }
            //refreshDeckText();
            //GO_Hand.GetComponent<HandDisplay>().childHaveChanged = true;

            // On active les listeners
            OnDrawOppo?.Invoke(GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>());
        } else {
            Debug.Log("ERR : no card in opponant deck");
        }
    }

    // Actualise le nombre de carte restant dans le deck
    public void refreshDeckText() {
        GO_DeckText.gameObject.GetComponent<TMP_Text>().SetText(GO_MonsterInvoked.GetComponent<MonsterDisplay>().deckList.Count.ToString());
    }

    // Envoi une carte au cimetière
    public void inGrave(GameObject activedCard) {
        CardDisplay cardDisplay = activedCard.GetComponent<CardDisplay>();
        MonsterDisplay monsterDisplay = cardDisplay.monsterOwnThis;
        monsterDisplay.graveList.Add(cardDisplay.card);

        // On classe la liste des carte du cimetière par ordre alpha sur le nom de la carte
        monsterDisplay.graveList = monsterDisplay.graveList.OrderBy(o => o.name).ToList();

        Destroy(activedCard);
        dragged = false;

        if (cardDisplay.monsterOwnThis == GO_MonsterInvoked.GetComponent<MonsterDisplay>()) {
            refreshGraveText();
        }
    }

    // Actualise le nombre de carte dans le cimetière
    public void refreshGraveText() {
        GO_GraveText.gameObject.GetComponent<TMP_Text>().SetText(GO_MonsterInvoked.GetComponent<MonsterDisplay>().graveList.Count.ToString());
    }

    // On actualise les infos des monstres dans la fenêtre d'équipe du joueur
    public void refreshTeamAreaLayout() {
        GameObject layoutTeam = GO_TeamArea.GetComponent<TeamLayoutDisplay>().layoutArea;
        foreach (Transform child in layoutTeam.transform) {
            int indexChild = child.GetSiblingIndex();
            MonsterLayoutTeamDisplay monsterLayoutTeamDisplay = child.gameObject.GetComponent<MonsterLayoutTeamDisplay>();
            MonsterDisplay monsterDisplay = monsterLayoutTeamDisplay.monsterLinked.GetComponent<MonsterDisplay>();
            monsterLayoutTeamDisplay.refreshMonsterUI();
        }

        layoutTeam.GetComponent<GridLayoutGroup>().enabled = false;
        layoutTeam.GetComponent<GridLayoutGroup>().enabled = true;
    }

    // On active une capacité
    public static void activeAbilityOnTarget(AbilityDisplay abilityDisplay, GameObject target) {
        // On active la capacité si son cout en mana est inférieur ou égal au mana disponible
        if (abilityDisplay.GetManaCost() <= GO_MonsterInvoked.GetComponent<MonsterDisplay>().manaAvailable) {
            abilityDisplay.activeAbility(target);
            GO_MonsterInvoked.GetComponent<MonsterDisplay>().manaAvailable -= abilityDisplay.GetManaCost();
            GO_MonsterInvoked.GetComponent<MonsterDisplay>().refreshManaPoint();
        } else {
            // On affiche un message d'erreur
            Debug.Log("ERR : no mana available");
        }
    }

    // On active une carte
    public void activeCardOnTarget(GameObject cardPlayed, GameObject target) {
        // On active la carte si son cout en mana est inférieur ou égal au mana disponible
        // Si ce n'est pas une carte ECHO
        if (cardPlayed.GetComponent<CardDisplay>().card.manaCost <= GO_MonsterInvoked.GetComponent<MonsterDisplay>().manaAvailable
            && cardPlayed.GetComponent<CardDisplay>().card.type != CardType.Echo) {
            cardPlayed.GetComponent<CardDisplay>().activeCard(target);
            GO_MonsterInvoked.GetComponent<MonsterDisplay>().manaAvailable -= cardPlayed.GetComponent<CardDisplay>().card.manaCost;
            GO_MonsterInvoked.GetComponent<MonsterDisplay>().refreshManaPoint();
        } 
        // Si c'est une carte ECHO
        else if (cardPlayed.GetComponent<CardDisplay>().card.type == CardType.Echo && cardPlayed.GetComponent<CardDisplay>().status == CardStatus.SlotVisible) {
            cardPlayed.GetComponent<CardDisplay>().activeCard(target);
        } else {
            // On affiche un message d'erreur
            Debug.Log("ERR : no mana available");
        }
    }

    /// <summary>
    /// Renvoi le type de la cible
    /// </summary>
    public static TargetType typeTarget(GameObject target) {
        // C'est une carte
        if (target.GetComponent<CardDisplay>() != null) {
            CardDisplay targetCardDisplay = target.GetComponent<CardDisplay>();

            if (targetCardDisplay.card != null) {
                // De sbire
                if (targetCardDisplay.card.type == CardType.Sbire) {
                    // Que le joueur controle
                    if (!targetCardDisplay.monsterOwnThis.ownedByOppo)
                        return TargetType.PlayerCardSbire;
                    // Que l'opposant controle
                    else
                        return TargetType.OpponantCardSbire;
                }
                // D'aura
                else if (targetCardDisplay.card.type == CardType.Aura) {
                    // Que le joueur controle
                    if (!targetCardDisplay.monsterOwnThis.ownedByOppo)
                        return TargetType.PlayerCardAura;
                    // Que l'opposant controle
                    else
                        return TargetType.OpponantCardAura;
                }
                // D'enchantement
                else if (targetCardDisplay.card.type == CardType.Enchantment) {
                    // Que le joueur controle
                    if (!targetCardDisplay.monsterOwnThis.ownedByOppo)
                        return TargetType.PlayerCardEnchantment;
                    // Que l'opposant controle
                    else
                        return TargetType.OpponantCardEnchantment;
                }
            }
        }
        // C'est un monstre
        else if (target.GetComponent<MonsterDisplay>() != null) {
            MonsterDisplay targetMonsterDisplay = target.GetComponent<MonsterDisplay>();

            // Que le joueur controle
            if (!targetMonsterDisplay.ownedByOppo) {
                return TargetType.PlayerMonster;
            }
            // Que l'opposant controle
            else {
                return TargetType.OpponantMonster;
            }
        }
        // C'est un équipement
        else if (target.GetComponent<EquipmentDisplay>() != null) {
            EquipmentDisplay targetEquipmentDisplay = target.GetComponent<EquipmentDisplay>();

            // Que le joueur controle
            if (!targetEquipmentDisplay.monsterOwnThis.ownedByOppo) {
                return TargetType.PlayerEquipment;
            } else {
                return TargetType.OpponantEquipment;
            }
        }
        // C'est un emplacement d'aura
        else if (target.GetComponent<AuraDisplay>() != null) {
            AuraDisplay targetAuraDisplay = target.GetComponent<AuraDisplay>();

            // Que le joueur controle
            if (!targetAuraDisplay.ownedByOppo) {
                return TargetType.PlayerAura;
            }
        }
        // C'est un emplacement de contre attaque
        else if (target.GetComponent<SlotDisplay>() != null) {
            SlotDisplay targetSlotDisplay = target.GetComponent<SlotDisplay>();

            // Que le joueur controle
            if (!targetSlotDisplay.ownedByOppo) {
                return TargetType.SlotVisible;
            }
        }

        return TargetType.Null;
    }

    // On place la carte sur le terrain
    public bool tryToPutOnBoard(GameObject cardPlayed, GameObject target, bool isVisible) {
        // Carte face visible, on doit dépenser du mana
        if (isVisible) {
            // On place la carte si son cout en mana est inférieur ou égal au mana disponible
            if (cardPlayed.GetComponent<CardDisplay>().card.manaCost <= GO_MonsterInvoked.GetComponent<MonsterDisplay>().manaAvailable) {
                cardPlayed.GetComponent<CardDisplay>().putOnBoard(target, true);
                GO_MonsterInvoked.GetComponent<MonsterDisplay>().manaAvailable -= cardPlayed.GetComponent<CardDisplay>().card.manaCost;
                GO_MonsterInvoked.GetComponent<MonsterDisplay>().refreshManaPoint();

                // Si c'est un enchantement on stock le GO de la carte dans MonsterDisplay
                if (target.GetComponent<EquipmentDisplay>() != null) {
                    GO_MonsterInvoked.GetComponent<MonsterDisplay>().cardEnchantments[target.GetComponent<EquipmentDisplay>().slotId] = cardPlayed.GetComponent<CardDisplay>().card;
                    cardPlayed.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
                    cardPlayed.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                    cardPlayed.transform.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                }
                return true;
            } else {
                // On affiche un message d'erreur
                Debug.Log("ERR : no mana available");
            }
        }
        // Carte face caché, pas besoin de dépenser de mana
        else { 
            cardPlayed.GetComponent<CardDisplay>().putOnBoard(target, false);
            return true;
        }

        return false;
    }

    // Initialisation du tableau d'affinité
    static void initializeArrayAffinity() {
        fireDico = new Dictionary<ElementalAffinity, float>() {
            {ElementalAffinity.Fire, 1 },
            {ElementalAffinity.Water, 0.5f },
            {ElementalAffinity.Electric, 1 },
            {ElementalAffinity.Earth, 2 },
            {ElementalAffinity.Combat, 1 },
            {ElementalAffinity.Mental, 2 },
            {ElementalAffinity.Neutral, 1 }
        };

        waterDico = new Dictionary<ElementalAffinity, float>() {
            {ElementalAffinity.Fire, 2 },
            {ElementalAffinity.Water, 1 },
            {ElementalAffinity.Electric, 0.5f },
            {ElementalAffinity.Earth, 1 },
            {ElementalAffinity.Combat, 1 },
            {ElementalAffinity.Mental, 2 },
            {ElementalAffinity.Neutral, 1 }
        };

        electricDico = new Dictionary<ElementalAffinity, float>() {
            {ElementalAffinity.Fire, 1 },
            {ElementalAffinity.Water, 2 },
            {ElementalAffinity.Electric, 1 },
            {ElementalAffinity.Earth, 0.5f },
            {ElementalAffinity.Combat, 1 },
            {ElementalAffinity.Mental, 2 },
            {ElementalAffinity.Neutral, 1 }
        };

        earthDico = new Dictionary<ElementalAffinity, float>() {
            {ElementalAffinity.Fire, 0.5f },
            {ElementalAffinity.Water, 1 },
            {ElementalAffinity.Electric, 2 },
            {ElementalAffinity.Earth, 1 },
            {ElementalAffinity.Combat, 2 },
            {ElementalAffinity.Mental, 0.5f },
            {ElementalAffinity.Neutral, 1 }
        };

        combatDico = new Dictionary<ElementalAffinity, float>() {
            {ElementalAffinity.Fire, 1 },
            {ElementalAffinity.Water, 1 },
            {ElementalAffinity.Electric, 1 },
            {ElementalAffinity.Earth, 0.5f },
            {ElementalAffinity.Combat, 2 },
            {ElementalAffinity.Mental, 0.5f },
            {ElementalAffinity.Neutral, 2 }
        };

        mentalDico = new Dictionary<ElementalAffinity, float>() {
            {ElementalAffinity.Fire, 0.5f },
            {ElementalAffinity.Water, 0.5f },
            {ElementalAffinity.Electric, 0.5f },
            {ElementalAffinity.Earth, 2 },
            {ElementalAffinity.Combat, 2 },
            {ElementalAffinity.Mental, 1 },
            {ElementalAffinity.Neutral, 2 }
        };

        neutralDico = new Dictionary<ElementalAffinity, float>() {
            {ElementalAffinity.Fire, 1 },
            {ElementalAffinity.Water, 1 },
            {ElementalAffinity.Electric, 1 },
            {ElementalAffinity.Earth, 1 },
            {ElementalAffinity.Combat, 0.5f },
            {ElementalAffinity.Mental, 0.5f },
            {ElementalAffinity.Neutral, 1 }
        };
    }

    // Calcule du coef entre deux affinité
    private static float coefAffinity(ElementalAffinity affinityAttack, ElementalAffinity affinityDefenser) {
        switch (affinityAttack) {
            case ElementalAffinity.Fire:
                return fireDico[affinityDefenser];
            case ElementalAffinity.Water:
                return waterDico[affinityDefenser];
            case ElementalAffinity.Electric:
                return electricDico[affinityDefenser];
            case ElementalAffinity.Earth:
                return earthDico[affinityDefenser];
            case ElementalAffinity.Combat:
                return combatDico[affinityDefenser];
            case ElementalAffinity.Mental:
                return mentalDico[affinityDefenser];
            case ElementalAffinity.Neutral:
                return neutralDico[affinityDefenser];
            default:
                Debug.Log("Affinity not found");
                return 0;
        }
    }

    // Calcule le plus grand coef d'affinité entre une attaque et un monstre défenseur
    private static float coefAffinityMax(ElementalAffinity affinityAttack, GameObject target) {
        if (target.GetComponent<CardDisplay>() != null)
            if (target.GetComponent<CardDisplay>().card.type == CardType.Sbire)
                target = target.GetComponent<CardDisplay>().monsterOwnThis.gameObject;

        float coefMax = 0;
        foreach(ElementalAffinity affinityDefenser in target.GetComponent<MonsterDisplay>().monster.elementalAffinity) {
            float coefCal = coefAffinity(affinityAttack, affinityDefenser);
            if (coefCal > coefMax) {
                coefMax = coefCal;
            }
        }

        return coefMax;
    }

    // On calcule les dégats réel
    public static int calculateDamage(MonsterDisplay defenser, ElementalAffinity attackAffinity, int amountDamage, MonsterDisplay attacker) {
        
        int power = attacker.powerEquiped + attacker.buffPower;
        int guard = defenser.guardEquiped + defenser.buffGuard;
        int speedAttacker = attacker.speedEquiped + attacker.buffSpeed;
        int speedDefenser = defenser.speedEquiped + defenser.buffSpeed;
        float diffPowerGuard = 0;
        // On vérifie si le power > guard
        if (power > guard) {
            diffPowerGuard = (float)(power - guard) / 100;
        }

        // On calcule le bonus d'affinité du monstre attaquant (+25% de dégâts si l'attaquant posséde la même affinité que l'attaque)
        float bonusAffinity = 1;
        foreach(ElementalAffinity affinity in attacker.monster.elementalAffinity) {
            if (affinity == attackAffinity) {
                bonusAffinity = 1.25f;
                break;
            }
        }

        // On calcule le coef d'affinité max
        float affinityCoef = coefAffinityMax(attackAffinity, defenser.gameObject);

        // Calcule des dégâts avant les multiplicateurs
        float resultDamage = amountDamage * (1 + Mathf.Log(1 + diffPowerGuard) * 2 / 3);
        // Calcule des dégâts avec les multiplicateurs
        resultDamage = resultDamage * bonusAffinity;
        // Calcule des dégâts avec l'impacte de la "Speed"
        resultDamage = resultDamage + (int)((speedAttacker - speedDefenser) / 100);
        // Calcule des dégâts avec le coef d'affinité
        resultDamage = resultDamage * affinityCoef;
        if (resultDamage < 0) {
            resultDamage = 0;
        }
        return Mathf.RoundToInt(resultDamage);
    }

    /// <summary>
    /// Renvoi les dégats de base de l'attaque
    /// </summary>
    /// <param name="text">String a parser</param>
    /// <param name="ally">TRUE si on cherche les dégâts de base contre une unité alliée</param>
    /// <returns></returns>
    public static List<int> getBaseDamage(string text, bool ally = false) {
        string pattern = @"\%D(\d+)";
        string patternAlly = @"\%DA(\d+)";
        List<int> intList = new List<int>();

        MatchCollection m;
        if (ally) {
            m = Regex.Matches(text, patternAlly, RegexOptions.IgnoreCase);
        } else {
            m = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);
        }
        foreach (Match m2 in m) {
            intList.Add(int.Parse(m2.Groups[1].Value));
        }

        return intList;
    }

    /// <summary>
    /// Renvoi le string d'entré avec le calcule des dégâts réel
    /// </summary>
    /// <returns></returns>
    public static string fullDamageIntegred(string textEffect, ElementalAffinity elementalAffinity, MonsterDisplay monsterOwner) {
        string pattern = @"\%D\d+";
        string patternAlly = @"\%DA\d+";
        Regex regex;

        string output = null;
        regex = new Regex(pattern, RegexOptions.IgnoreCase);
        foreach (int baseDamage in GameManager.getBaseDamage(textEffect, false)) {
            int trueDamage = 0;
            // Si le Owner n'est pas le monstre adverse
            if (GameManager.GO_MonsterInvokedOppo == monsterOwner.gameObject) {
                trueDamage = GameManager.calculateDamage(GameManager.GO_MonsterInvoked.GetComponent<MonsterDisplay>(), elementalAffinity, baseDamage, GameManager.GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>());
            }
            // Si le Owner est le monstre actif du joueur
            else if (GameManager.GO_MonsterInvoked == monsterOwner.gameObject) {
                trueDamage = GameManager.calculateDamage(GameManager.GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>(), elementalAffinity, baseDamage, GameManager.GO_MonsterInvoked.GetComponent<MonsterDisplay>());
            }
            // Si le Owner est un monstre inactif du joueur
            else if (GameManager.GO_MonsterInvoked != monsterOwner.gameObject && !monsterOwner.ownedByOppo) {
                trueDamage = GameManager.calculateDamage(GameManager.GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>(), elementalAffinity, baseDamage, monsterOwner);
            }
          
            if (output != null) {
                output = regex.Replace(output, trueDamage.ToString(), 1);
            } else {
                output = regex.Replace(textEffect, trueDamage.ToString(), 1);
            }
        }
        regex = new Regex(patternAlly, RegexOptions.IgnoreCase);
        foreach (int baseDamage in GameManager.getBaseDamage(textEffect, true)) {
            int trueDamage;
            trueDamage = GameManager.calculateDamage(monsterOwner, elementalAffinity, baseDamage, monsterOwner);
            
            if (output != null) {
                output = regex.Replace(output, trueDamage.ToString(), 1);
            } else {
                output = regex.Replace(textEffect, trueDamage.ToString(), 1);
            }
        }

        return output;
    }

    // Event avant le swap du monstre allié
    public static event Action<MonsterDisplay> OnSwapBefore;
    // Event après le swap du monstre allié
    public static event Action<MonsterDisplay> OnSwapAfter;
    // Change le monstre actif
    public void swapMonster(int indexMonster) {
        GameObject nextMonster = monstersGOList[indexMonster];

        // On compte le nombre de carte dans la main
        int amountCardInHand = GO_Hand.transform.childCount;
        // On remet les cartes de la main dans le deck du monstre précédant
        foreach (Transform cardInHand in GO_Hand.transform) {
            GO_MonsterInvoked.GetComponent<MonsterDisplay>().deckList.Add(cardInHand.gameObject.GetComponent<CardDisplay>().card);
            Destroy(cardInHand.gameObject);
        }

        // On place les cartes de la zone "contre-attaque" dans le cimetière du monstre
        CardDisplay[] cardDisplayCounters = GO_CounterAttackArea.GetComponentsInChildren<CardDisplay>();
        foreach(CardDisplay cardDisplayCounter in cardDisplayCounters) {
            inGrave(cardDisplayCounter.gameObject);
        }

        // On détruit les équipements du monstre actif
        foreach (EquipmentDisplay equipmentDisplay in GO_EquipmentArea.GetComponentsInChildren<EquipmentDisplay>()) {
            // On sauvegarde les enchantemets du monstre actif
            if (equipmentDisplay.cardOnSlot != null) {
                int slotId = equipmentDisplay.slotId;
                GO_MonsterInvoked.GetComponent<MonsterDisplay>().cardEnchantments[slotId] = equipmentDisplay.cardOnSlot.GetComponent<CardDisplay>().card;
            }
            equipmentDisplay.destroyEnchantment();
            Destroy(equipmentDisplay.gameObject);
        }

        // On reset le mana et on supprime tous les buff et debuff du précédent monstre
        GO_MonsterInvoked.GetComponent<MonsterDisplay>().resetMana();
        GO_MonsterInvoked.GetComponent<MonsterDisplay>().removeAllBuffDebuff();

        // On desactive le GO du précédent monstre
        GO_MonsterInvoked.SetActive(false);

        // On désactive les effets passifs de la capacité
        GO_MonsterInvoked.GetComponent<MonsterDisplay>().abilityDisplay.disablePassiveAbility();

        // On active les listeners avant le swap
        OnSwapBefore?.Invoke(GO_MonsterInvoked.GetComponent<MonsterDisplay>());
        
        //***************************** SWAP ************************************

        // On change de monstre actif
        nextMonster.SetActive(true);
        GO_MonsterInvoked = nextMonster;

        //***************************** SWAP ************************************

        // On active les listeners après le swap
        OnSwapAfter?.Invoke(GO_MonsterInvoked.GetComponent<MonsterDisplay>());

        // On active les effets passifs de la capacité
        GO_MonsterInvoked.GetComponent<MonsterDisplay>().abilityDisplay.activePassiveAbility();

        // On instantie l'équipement du nouveau monstre
        instantiateEquipment(GO_MonsterInvoked);

        // On pioche de nouvelle carte dans le deck du nouveau monstre
        draw(amountCardInHand);
        // On réinitialise son mana
        GO_MonsterInvoked.GetComponent<MonsterDisplay>().resetMana();

        // On actualise les dégâts affichés sur les cartes
        StartCoroutine(refreshAllDamageText());

        // On actualise le deck et le cimetière
        refreshDeckText();
        refreshGraveText();
        refreshTeamAreaLayout();

        // On ferme la fenêtre d'équipe
        GO_TeamArea.SetActive(false);
    }

    // Event avant le swap du monstre adverse
    public static event Action<MonsterDisplay> OnSwapOppoBefore;
    // Event après le swap du monstre adverse
    public static event Action<MonsterDisplay> OnSwapOppoAfter;
    public void swapMonsterOppo(int indexMonster) {
        GameObject nextMonster = monstersGOListOppo[indexMonster];

        // On desactive tous les GO des monstres
        foreach (Transform child in GO_MonsterAreaOppo.transform) {
            child.gameObject.SetActive(false);
        }

        // On détruit les équipements du monstre actif
        foreach (EquipmentDisplay equipmentDisplay in GO_EquipmentAreaOppo.GetComponentsInChildren<EquipmentDisplay>()) {
            // On sauvegarde les enchantemets du monstre actif
            if (equipmentDisplay.cardOnSlot != null) {
                int slotId = equipmentDisplay.slotId;
                GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>().cardEnchantments[slotId] = equipmentDisplay.cardOnSlot.GetComponent<CardDisplay>().card;
            }
            equipmentDisplay.destroyEnchantment();
            Destroy(equipmentDisplay.gameObject);
        }

        // On réinitialise le mana de l'ancien monstre
        GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>().resetMana();

        // On désactive les effets passifs de la capacité
        GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>().abilityDisplay.disablePassiveAbility();

        // On active les listeners avant le swap
        OnSwapOppoBefore?.Invoke(GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>());

        //***************************** SWAP ************************************

        // On change de monstre actif
        nextMonster.SetActive(true);
        GO_MonsterInvokedOppo = nextMonster;

        //***************************** SWAP ************************************

        // On active les listeners après le swap
        OnSwapOppoAfter?.Invoke(GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>());

        // On active les effets passifs de la capacité
        GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>().abilityDisplay.activePassiveAbility();

        // On réinitialise le mana du nouveau monstre
        GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>().resetMana();

        //// On instantie l'équipement du nouveau monstre
        instantiateEquipment(GO_MonsterInvokedOppo);

        // On actualise les dégâts affichés sur les cartes
        StartCoroutine(refreshAllDamageText());

        // On actualise le deck et le cimetière
        // De l'adversaire

        refreshTeamAreaLayout();
    }

    // Actualisation des dégâts affichés sur les cartes et capacités
    public IEnumerator refreshAllDamageText() {
        yield return null;
        //// Cartes du joueur
        // Dans la main du joueur
        foreach (CardDisplay cardDisplay in GO_Hand.transform.GetComponentsInChildren<CardDisplay>()) {
            if (cardDisplay.name != null) {
                cardDisplay.refreshDescriptionDamage();
            }
        }
        // Dans la zone de contre-attaque
        foreach (CardDisplay cardDisplay in GO_CounterAttackArea.transform.GetComponentsInChildren<CardDisplay>()) {
            if (cardDisplay.name != null) {
                cardDisplay.refreshDescriptionDamage();
            }
        }
        // Dans la zone d'équipement
        foreach (CardDisplay cardDisplay in GO_EquipmentArea.transform.GetComponentsInChildren<CardDisplay>()) {
            if (cardDisplay.name != null) {
                cardDisplay.refreshDescriptionDamage();
            }
        }
        // Dans la zone d'aura
        foreach (CardDisplay cardDisplay in GO_AuraArea.transform.GetComponentsInChildren<CardDisplay>()) {
            if (cardDisplay.name != null) {
                cardDisplay.refreshDescriptionDamage();
            }
        }

        //// Cartes de l'adversaire
        // Dans la zone de contre-attaque
        foreach (CardDisplay cardDisplay in GO_CounterAttackAreaOppo.transform.GetComponentsInChildren<CardDisplay>()) {
            if (cardDisplay.name != null) {
                cardDisplay.refreshDescriptionDamage();
            }
        }
        // Dans la zone d'équipement
        foreach (CardDisplay cardDisplay in GO_EquipmentAreaOppo.transform.GetComponentsInChildren<CardDisplay>()) {
            if (cardDisplay.name != null) {
                cardDisplay.refreshDescriptionDamage();
            }
        }
        // Dans la zone d'aura
        foreach (CardDisplay cardDisplay in GO_AuraAreaOppo.transform.GetComponentsInChildren<CardDisplay>()) {
            if (cardDisplay.name != null) {
                cardDisplay.refreshDescriptionDamage();
            }
        }

        // Capacité du joueur
        GO_MonsterInvoked.GetComponent<MonsterDisplay>().abilityDisplay.refreshDisplayAbility();

        // Capacité de l'adversaire
        GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>().abilityDisplay.refreshDisplayAbility();
    }

    // On modifie les tours restant pour les buff / debuff du monstre du joueur
    public void buffDebuffAddTurn(int turnAmount) {
        List<BuffDebuff> copyList = new List<BuffDebuff>(GO_MonsterInvoked.GetComponent<MonsterDisplay>().buffDebuffList);
        foreach (BuffDebuff buffDebuff in copyList) {
            buffDebuff.addTurn(turnAmount);
        }
    }

    // On modifie les tours restant pour les buff / debuff du monstre adverse
    public void buffDebuffAddTurnOppo(int turnAmount) {
        List<BuffDebuff> copyList = new List<BuffDebuff>(GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>().buffDebuffList);
        foreach (BuffDebuff buffDebuff in copyList) {
            buffDebuff.addTurn(turnAmount);
        }
    }

    // On actualise l'affichage des buff / debuff
    public void refreshBuffDebuff() {
        // buff / debuff de notre monstre
        List<BuffDebuff> copyList = new List<BuffDebuff>(GO_MonsterInvoked.GetComponent<MonsterDisplay>().buffDebuffList);
        foreach (BuffDebuff buffDebuff in copyList) {
            buffDebuff.GetComponent<BuffDebuffDisplay>().refresh();
        }

        // buff / debuff du monstre adverse
        List<BuffDebuff> copyList2 = new List<BuffDebuff>(GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>().buffDebuffList);
        foreach (BuffDebuff buffDebuff in copyList2) {
            buffDebuff.GetComponent<BuffDebuffDisplay>().refresh();
        }

        // On actualise les dégâts affichés sur les cartes
        StartCoroutine(refreshAllDamageText());
    }

    // DEBUG Affiche le prochain monstre
    public void debug_showNextMonster() {
        if (GO_MonsterInvoked.transform.GetSiblingIndex() == monstersGOList.Count - 1) {
            swapMonster(0);
        } else {
            swapMonster(GO_MonsterInvoked.transform.GetSiblingIndex() + 1);
        }
    }

    // DEBUG Affiche le prochain monstre de l'adversaire
    public void debug_showNextMonsterOppo() {
        if (GO_MonsterInvokedOppo.transform.GetSiblingIndex() == monstersGOListOppo.Count - 1) {
            swapMonsterOppo(0);
        } else {
            swapMonsterOppo(GO_MonsterInvokedOppo.transform.GetSiblingIndex() + 1);
        }
    }

    // DEBUG Ajoute 10 mana au monstre actif
    public void debug_add10Mana() {
        GO_MonsterInvoked.GetComponent<MonsterDisplay>().manaMax = 10;
        GO_MonsterInvoked.GetComponent<MonsterDisplay>().resetMana();
    }

    // DEBUG Ajoute 4 sbires aléatoires sur le terrain de l'oppo
    public void debug_add4Sbire() {
        //int i = 0;
        //foreach(Transform slot in GO_CounterAttackAreaOppo.transform) {
        //    if (slot.transform.GetChild(1).childCount > 0)
        //        Destroy(slot.transform.GetChild(1).GetChild(0).gameObject);

        //    GameObject newSbire = Instantiate(GO_Card, slot.transform.GetChild(1));
        //    newSbire.GetComponent<CardDisplay>().card = sbireList[i];
        //    newSbire.GetComponent<CardDisplay>().monsterOwnThis = GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>();
        //    newSbire.GetComponent<CardDisplay>().status = CardStatus.SlotVisible;
        //    newSbire.GetComponent<SbireDisplay>().invokeSbire();
        //    newSbire.transform.localPosition = Vector3.zero;
        //    newSbire.transform.localScale = Vector3.one;

        //    i++;
        //}

        int i = 0;
        foreach (Card sbire in sbireList) {
            Transform slot = GO_CounterAttackAreaOppo.transform.GetChild(i);
            if (slot.transform.GetChild(1).childCount > 0)
                Destroy(slot.transform.GetChild(1).GetChild(0).gameObject);

            GameObject newSbire = Instantiate(GO_Card, slot.transform.GetChild(1));
            newSbire.GetComponent<CardDisplay>().card = sbire;
            newSbire.GetComponent<CardDisplay>().monsterOwnThis = GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>();
            newSbire.GetComponent<CardDisplay>().status = CardStatus.SlotVisible;
            newSbire.GetComponent<SbireDisplay>().invokeSbire();
            newSbire.transform.localPosition = Vector3.zero;
            newSbire.transform.localScale = Vector3.one;

            i++;
        }
    }

    // DEBUG Ajoute 4 auras sur le terrain de l'oppo
    public void debug_add4Aura() {
        int i = 0;
        foreach (Card aura in auraList) {
            Transform slot = GO_AuraAreaOppo.transform.GetChild(i);
            if (slot.transform.GetChild(1).childCount > 0)
                Destroy(slot.transform.GetChild(1).GetChild(0).gameObject);

            GameObject newAura = Instantiate(GO_Card, slot.transform.GetChild(1));
            newAura.GetComponent<CardDisplay>().card = aura;
            newAura.GetComponent<CardDisplay>().monsterOwnThis = GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>();
            newAura.GetComponent<CardDisplay>().status = CardStatus.AuraSlot;
            newAura.GetComponent<CardDisplay>().activeCard(GO_MonsterInvokedOppo);
            newAura.transform.localPosition = Vector3.zero;
            newAura.transform.localScale = Vector3.one;

            i++;
        }
    }
}
