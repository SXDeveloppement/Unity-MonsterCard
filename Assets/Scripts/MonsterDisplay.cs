using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Random = System.Random;
using System.Linq;

public class MonsterDisplay : MonoBehaviour, IDropHandler
{
    public Monster monster;
    public TMP_Text nameText;
    public GameObject GO_LifeBar;
    public GameObject GO_ManaBar;
    public TMP_Text powerText;
    public TMP_Text guardText;
    public TMP_Text speedText;
    public TMP_Text healthText;
    public TMP_Text manaText;
    public Image artworkImage;
    public GameObject GO_Affinity;

    public GameObject monsterLayoutTeamLinked; // GO du monstre affiché dans la fenêtre de l'équipe
    public int healthAvailable;
    public int healthMax;
    public int manaMax;
    public int manaAvailable;
    public int powerEquiped = 0; // Power du monstre + des équipements
    public int guardEquiped = 0; // Guard du monstre + des équipements
    public int speedEquiped = 0; // Speed du monstre + des équipements

    public List<Card> deckList; // Liste des cartes dans le deck
    public List<Card> graveList; // Liste des cartes dans le cimetière
    public List<Equipment> equipmentList; // Liste des équipements du monstre
    public List<Card> cardEnchantments; // Liste des cartes d'enchantement

    public List<BuffDebuff> buffDebuffList; // Liste des buff / debuff du monstre
    public int buffPower; // Power bonus total accordé par les buffs et débuff (positif ou négatif)
    public int buffGuard; // Guard bonus total accordé par les buffs et débuff
    public int buffSpeed; // Speed bonus total accordé par les buffs et débuff
    public int buffMana; // Mana bonus total accordé par les buffs et débuff
    public int buffDamageRaw; // Dommage brute bonus total accordé par les buffs et débuff
    public int buffDamagePercent; // Dommage en pourcentage bonus total accordé par les buffs et débuff


    public bool ownedByOppo;
    public bool isKO;

    Vector2 lifeBarSizeCached;
    Vector2 manaBarSizeCached;
    GameManager gameManager;

    private bool init = true;

    // Variable temporaire pour l'update de l'UI
    int powerEquipedTemp = 0;
    int guardEquipedTemp = 0;
    int speedEquipedTemp = 0;
    int healthAvailableTemp = 0;
    int manaMaxTemp = 0;
    int manaAvailableTemp = 0;
    int buffPowerTemp = 0;
    int buffGuardTemp = 0;
    int buffSpeedTemp = 0; 
    int buffManaTemp = 0;

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

        // Calcule power, guard et speed avec équipement
        calculeStatsEquiped();

        // Ajouter la vie bonus des équipements
        healthMax = monster.healthPoint;
        foreach (Equipment equipment in equipmentList) {
            healthMax += equipment.healthPoint;
        }
        healthAvailable = healthMax;

        // On initialise la mana
        manaMax = 1;
        manaAvailable = manaMax;

        // On ajout l''illustration du monstre
        artworkImage.sprite = monster.artwork;

        // Affichage des affinités élémentaires du monstre
        foreach (ElementalAffinity affinity in monster.elementalAffinity) {
            GO_Affinity.transform.Find(affinity.ToString()).gameObject.SetActive(true);
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (init) {
            //StartCoroutine(refreshUI());
            if (gameObject.transform.GetSiblingIndex() != 0) {
                gameObject.SetActive(false);
            }
            init = false;
        }

        if (healthAvailable <= 0) {
            isKO = true;
        }

        // On refresh l'UI si des variables ont changé
        if (healthAvailable != healthAvailableTemp) {
            healthAvailableTemp = healthAvailable;
            refreshHealthPoint();
            if (!ownedByOppo)
                monsterLayoutTeamLinked.GetComponent<MonsterLayoutTeamDisplay>().refreshMonsterUI();            
        }
        if (manaAvailable != manaAvailableTemp || manaMax != manaMaxTemp || buffMana != buffManaTemp) {
            manaMaxTemp = manaMax;
            manaAvailableTemp = manaAvailable;
            refreshManaPoint();
            if (!ownedByOppo)
                monsterLayoutTeamLinked.GetComponent<MonsterLayoutTeamDisplay>().refreshMonsterUI();            
        }
        if (powerEquiped != powerEquipedTemp || buffPower != buffPowerTemp) {
            powerEquipedTemp = powerEquiped;
            buffPowerTemp = buffPower;
            refreshPower();
            if (!ownedByOppo)
                monsterLayoutTeamLinked.GetComponent<MonsterLayoutTeamDisplay>().refreshMonsterUI();
            if (gameObject == gameManager.GO_MonsterInvoked || gameObject == gameManager.GO_MonsterInvokedOppo)
                StartCoroutine(gameManager.refreshAllDamageText());
        }
        if (guardEquiped != guardEquipedTemp || buffGuard != buffGuardTemp) {
            guardEquipedTemp = guardEquiped;
            buffGuardTemp = buffGuard;
            refreshGuard();
            if (!ownedByOppo)
                monsterLayoutTeamLinked.GetComponent<MonsterLayoutTeamDisplay>().refreshMonsterUI();
            if (gameObject == gameManager.GO_MonsterInvoked || gameObject == gameManager.GO_MonsterInvokedOppo)
                StartCoroutine(gameManager.refreshAllDamageText());
        }
        if (speedEquiped != speedEquipedTemp || buffSpeed != buffSpeedTemp) {
            speedEquipedTemp = speedEquiped;
            buffSpeedTemp = buffSpeed;
            refreshSpeed();
            if (!ownedByOppo)
                monsterLayoutTeamLinked.GetComponent<MonsterLayoutTeamDisplay>().refreshMonsterUI();
            if (gameObject == gameManager.GO_MonsterInvoked || gameObject == gameManager.GO_MonsterInvokedOppo)
                StartCoroutine(gameManager.refreshAllDamageText());
        }
    }

    void IDropHandler.OnDrop(PointerEventData eventData) {
        if (gameManager.dragged) {
            GameObject cardPlayed = eventData.pointerDrag;
            GameObject target = gameObject;

            // Si la carte est un sbire et qu'elle est sur le terrain face visible
            if (cardPlayed.GetComponent<CardDisplay>().card.type == Type.Sbire 
            && cardPlayed.GetComponent<CardDisplay>().status == Status.SlotVisible
            && cardPlayed.GetComponent<CardDisplay>().monsterOwnThis != gameObject) {
                bool sbireHaveTaunt = false;
                foreach (CardDisplay cardDisplay in gameManager.GO_CounterAttackAreaOppo.GetComponentsInChildren<CardDisplay>()) {
                    if (cardDisplay.card.type == Type.Sbire) {
                        foreach (SbirePassifEffect sbirePassifEffect in cardDisplay.card.sbirePassifEffects) {
                            if (sbirePassifEffect == SbirePassifEffect.Tank) {
                                sbireHaveTaunt = true;
                                break;
                            }
                        }

                        if (sbireHaveTaunt) break;
                    }
                }

                if (!sbireHaveTaunt) {
                    gameManager.dragged = false;
                    cardPlayed.GetComponent<SbireDisplay>().sbireHasAttacked = true;
                    takeDamage(cardPlayed.GetComponent<SbireDisplay>().sbirePowerAvailable);
                } else {
                    Debug.Log("ERR : Bad target, one sbire or more have Taunt");
                }
            } else { 
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
                    Debug.Log("ERR : bad target [" + target.name + "] / ownByOppo = " + ownedByOppo.ToString());
                }
            }
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

    // Réinitiliation du mana
    public void resetMana() {
        manaAvailable = manaMax;
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
    public void takeDamage(int takeAmountDamage) {
        healthAvailable -= takeAmountDamage;
        if (healthAvailable < 0) {
            healthAvailable = 0;
        }
    }

    // Instantie un buff / debuff
    public GameObject instantiateBuffDebuff(BuffDebuffType buffDebuffType, int amount, int turnAmount) {
        GameObject newGOBuffDebuff = Instantiate(gameManager.GO_BuffDebuff);
        BuffDebuff newBuffDebuff = newGOBuffDebuff.GetComponent<BuffDebuff>();
        newBuffDebuff.targetMonster = gameObject;
        newBuffDebuff.buffDebuffType = buffDebuffType;
        newBuffDebuff.amount = amount;
        newBuffDebuff.turn = turnAmount;

        if (gameObject == gameManager.GO_MonsterInvoked) {
            if (amount >= 0) {
                newGOBuffDebuff.transform.SetParent(gameManager.GO_BuffArea.transform);
            } else {
                newGOBuffDebuff.transform.SetParent(gameManager.GO_DebuffArea.transform);
            }
        } else {
            if (amount >= 0) {
                newGOBuffDebuff.transform.SetParent(gameManager.GO_BuffAreaOppo.transform);
            } else {
                newGOBuffDebuff.transform.SetParent(gameManager.GO_DebuffAreaOppo.transform);
            }
        }

        return newGOBuffDebuff;
    }

    // Ajout d'un buff / debuff
    public void addBuffDebuff(BuffDebuffType buffDebuffType, int amount, int turnAmount) {
        GameObject buffDebuffGO = instantiateBuffDebuff(buffDebuffType, amount, turnAmount);
        buffDebuffGO.GetComponent<BuffDebuff>().applyRemove(true);
        buffDebuffList.Add(buffDebuffGO.GetComponent<BuffDebuff>());
        sortBuffDebuffList();
    }

    // Suppression d'un buff / debuff
    public void removeBuffDebuff(BuffDebuff buffDebuff, bool refresh = true) {
        buffDebuffList.Remove(buffDebuff);
        sortBuffDebuffList();

        if (refresh) {
            gameManager.refreshBuffDebuff();
        }

        Destroy(buffDebuff.gameObject);
    }

    // Suppression de tous les buff / debuff
    public void removeAllBuffDebuff() {
        List<BuffDebuff> copyList = new List<BuffDebuff>(buffDebuffList);
        foreach (BuffDebuff buffDebuff in copyList) {
            buffDebuff.applyRemove(false, false);
        }

        //StartCoroutine(gameManager.refreshAllDamageText());
    }

    // Réorganise la liste des buff/debuff
    public void sortBuffDebuffList() {
        buffDebuffList.Sort((x, y) => {
            int ret = string.Compare(x.buffDebuffType.ToString(), y.buffDebuffType.ToString());
            if (ret != 0) {
                return ret;
            } else {
                ret = x.amount.CompareTo(y.amount);

                if (ret != 0) {
                    return ret;
                } else {
                    return x.turn.CompareTo(y.turn);
                }
            }
        });
    }

    // On calcule power, guard et speed avec equipement
    public void calculeStatsEquiped() {
        int power = monster.powerPoint;
        foreach (Equipment equipment in equipmentList) {
            power += equipment.powerPoint;
        }
        powerEquiped = power;

        int guard = monster.guardPoint;
        foreach (Equipment equipment in equipmentList) {
            guard += equipment.guardPoint;
        }
        guardEquiped = guard;

        int speed = monster.speedPoint;
        foreach (Equipment equipment in equipmentList) {
            speed += equipment.speedPoint;
        }
        speedEquiped = speed;
    }

    //*********** ACTUALISATION de l'UI ***************//

    // Actualise la puissance du monstre
    public void refreshPower() {
        powerText.text = getPowerPointString();
    }
    public string getPowerPointString() {
        return (powerEquiped + buffPower).ToString();
    }

    // Actualise la defense du monstre
    public void refreshGuard() {
        guardText.text = getGuardPointString();
    }
    public string getGuardPointString() {
        return (guardEquiped + buffGuard).ToString();
    }

    // Actualise la vitesse du monstre
    public void refreshSpeed() {
        speedText.text = getSpeedPointString();
    }
    public string getSpeedPointString() {
        return (speedEquiped + buffSpeed).ToString();
    }

    // Actualise la barre de vie
    public void refreshHealthPoint() {
        healthText.text = getHealthBarString();

        // On modifie la taille de la barre
        GO_LifeBar.transform.Find("Health").transform.localScale = getHealthBarScale();
    }
    public string getHealthBarString() {
        return healthAvailable.ToString() + "/" + healthMax.ToString();
    }
    public Vector3 getHealthBarScale() {
        return new Vector3((float)healthAvailable / healthMax, 1f, 1f);
    }


    // Actualise la barre de mana
    public void refreshManaPoint() {
        manaText.text = getManaBarString();

        // On modifie la taille de la barre
        GO_ManaBar.transform.Find("Mana").transform.localScale = getManaBarScale();
    }
    public string getManaBarString() {
        return manaAvailable.ToString() + "/" + manaMax.ToString();
    }
    public Vector3 getManaBarScale() {
        return new Vector3((float)manaAvailable / manaMax, 1f, 1f);
    }
}
