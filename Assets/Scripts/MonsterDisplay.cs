using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Random = System.Random;
using System.Linq;

public class MonsterDisplay : MonoBehaviour
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
    public SpriteRenderer illustration;
    public GameObject GO_Affinity;
    public AbilityDisplay abilityDisplay; // La capacité du monstre

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
        gameManager = GameObject.FindAnyObjectByType<GameManager>();

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

        // Choisi une capacité aléatoire
        Ability[] DBAbility = Resources.LoadAll<Ability>("Abilities");
        Random rand2 = new Random();
        abilityDisplay.ability = DBAbility[rand2.Next(DBAbility.Length)];

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

        // On ajout l'illustration du monstre
        if (artworkImage != null)
            artworkImage.sprite = monster.artwork;
        if (illustration != null)
            illustration.sprite = monster.artwork;

        // Affichage des affinités élémentaires du monstre
        foreach (ElementalAffinity affinity in monster.elementalAffinity) {
            GO_Affinity.transform.Find("Layout").Find(affinity.ToString()).gameObject.SetActive(true);
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (init) {
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
            if (gameObject == GameManager.GO_MonsterInvoked || gameObject == GameManager.GO_MonsterInvokedOppo)
                StartCoroutine(gameManager.refreshAllDamageText());
        }
        if (guardEquiped != guardEquipedTemp || buffGuard != buffGuardTemp) {
            guardEquipedTemp = guardEquiped;
            buffGuardTemp = buffGuard;
            refreshGuard();
            if (!ownedByOppo)
                monsterLayoutTeamLinked.GetComponent<MonsterLayoutTeamDisplay>().refreshMonsterUI();
            if (gameObject == GameManager.GO_MonsterInvoked || gameObject == GameManager.GO_MonsterInvokedOppo)
                StartCoroutine(gameManager.refreshAllDamageText());
        }
        if (speedEquiped != speedEquipedTemp || buffSpeed != buffSpeedTemp) {
            speedEquipedTemp = speedEquiped;
            buffSpeedTemp = buffSpeed;
            refreshSpeed();
            if (!ownedByOppo)
                monsterLayoutTeamLinked.GetComponent<MonsterLayoutTeamDisplay>().refreshMonsterUI();
            if (gameObject == GameManager.GO_MonsterInvoked || gameObject == GameManager.GO_MonsterInvokedOppo)
                StartCoroutine(gameManager.refreshAllDamageText());
        }
    }

    public bool OnDrop(GameObject cardPlayed) {
        bool isPutOnBoard = false;

        if (GameManager.dragged) {
            GameObject target = gameObject;

            if (cardPlayed.GetComponent<CardDisplay>().targetIsAllowed(target)) {
                // Si la carte est un sbire et qu'elle est sur le terrain face visible
                if (cardPlayed.GetComponent<CardDisplay>().card.type == CardType.Sbire) {
                
                    bool sbireHaveTaunt = false;
                    foreach (CardDisplay cardDisplay in gameManager.GO_CounterAttackAreaOppo.GetComponentsInChildren<CardDisplay>()) {
                        if (cardDisplay.card.type == CardType.Sbire) {
                            sbireHaveTaunt = cardDisplay.GetComponent<SbireDisplay>().haveTank();
                            if (sbireHaveTaunt)
                                break;
                        }
                    }

                    if (!sbireHaveTaunt) {
                        GameManager.dragged = false;
                        cardPlayed.GetComponent<SbireDisplay>().sbireHasAttacked = true;
                        takeDamage(cardPlayed.GetComponent<SbireDisplay>().sbirePowerAvailable);
                    } else {
                        Debug.Log("ERR : Bad target, one sbire or more have Taunt");
                    }
                
                } else {
                    gameManager.activeCardOnTarget(cardPlayed, target);
                }
            } else {
                Debug.Log("ERR : bad target [" + target.name + "] / ownByOppo = " + ownedByOppo.ToString());
            }
        }

        return isPutOnBoard;
    }

    /// <summary>
    /// Lorsque que l'on active une capacité qui cible ce monstre
    /// </summary>
    /// <param name="abilityDisplay"></param>
    public bool OnDropAbility(AbilityDisplay abilityDisplay) {
        bool abilityIsDown = false;

        if (GameManager.dragged) {
            GameObject target = gameObject;

            if (abilityDisplay.loopTargetAllowed(target)) {
                gameManager.activeAbilityOnTarget(abilityDisplay, target);
                abilityIsDown = true;
            } else {
                Debug.Log("ERR : bad target [" + target.name + "] / ownByOppo = " + ownedByOppo.ToString());
            }
        }

        return abilityIsDown;
    }

    // Modifie l'affichage pour le monstre de l'adversaire
    public void ownerOppo() {
        ownedByOppo = true;
        Vector3 flipX = new Vector3(-1f, 1f, 1f);
        // On reflip sur X tous les textes
        powerText.gameObject.transform.localScale = flipX;
        guardText.gameObject.transform.localScale = flipX;
        speedText.gameObject.transform.localScale = flipX;
        healthText.gameObject.transform.localScale = flipX;
        manaText.gameObject.transform.localScale = flipX;

        // On text et illustration de la capacité
        abilityDisplay.textCooldown.gameObject.transform.localScale = flipX;
        abilityDisplay.textManaCost.gameObject.transform.localScale = flipX;
        abilityDisplay.illustration.gameObject.transform.localScale = flipX;
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

        // On réduit le cooldown de la capacité de 1
        abilityDisplay.newTurn();
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

        if (gameObject == GameManager.GO_MonsterInvoked) {
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

        newGOBuffDebuff.transform.localScale = new Vector3(0.5f, 0.5f, 1);

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

        StartCoroutine(gameManager.refreshAllDamageText());
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
        GO_LifeBar.transform.Find("HealthBar").transform.localPosition = getHealthBarLocalPosition();
    }
    public string getHealthBarString() {
        return healthAvailable.ToString() + "/" + healthMax.ToString();
    }
    public Vector3 getHealthBarScale() {
        return new Vector3((float)healthAvailable / healthMax, 1f, 1f);
    }
    public Vector3 getHealthBarLocalPosition() {
        GameObject healthBar = GO_LifeBar.transform.Find("HealthBar").gameObject;
        float width = healthBar.GetComponent<RectTransform>().rect.width * healthBar.transform.localScale.x;
        return new Vector3(-width * (1 - (float)healthAvailable / healthMax), healthBar.transform.localPosition.y, healthBar.transform.localPosition.z);
    }


    // Actualise la barre de mana
    public void refreshManaPoint() {
        manaText.text = getManaBarString();

        // On modifie la taille de la barre
        GO_ManaBar.transform.Find("ManaBar").transform.localPosition = getManaBarLocalPosition();
    }
    public string getManaBarString() {
        return manaAvailable.ToString() + "/" + manaMax.ToString();
    }
    public Vector3 getManaBarScale() {
        return new Vector3((float)manaAvailable / manaMax, 1f, 1f);
    }
    public Vector3 getManaBarLocalPosition() {
        GameObject manaBar = GO_ManaBar.transform.Find("ManaBar").gameObject;
        float width = manaBar.GetComponent<RectTransform>().rect.width * manaBar.transform.localScale.x;
        return new Vector3(-width * ( 1 - (float)manaAvailable / manaMax), manaBar.transform.localPosition.y, manaBar.transform.localPosition.z);
    }
}
