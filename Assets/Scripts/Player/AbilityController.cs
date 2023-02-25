using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityController : MonoBehaviour
{
    private string active1;
    
    /* Abilities in order of when they'll level */
    private List<string> activeAbilitiesLevel = new List<string>() {
        "CatFood", "Stick", "Dice", "Turd"
    };

    private List<string> activeAbilities = new List<string>() {
        "Ball", "DogBone"
    };
    private List<string> passiveAbilities = new List<string>() {
        "Heal", "Shield"
    };
    private Dictionary<string, float> cooldowns = new Dictionary<string, float>() {
        {"Ball", 0.3f},
        {"CatFood", 0.5f},
        {"DogBone", 10f},
        {"Turd", 0.7f},
        {"Fart", 0.5f},
        {"Stick", 10f}
    };
    private Dictionary<string, string> currentAbilities = new Dictionary<string, string>() {
        {"active1", ""},
        {"active2", ""},
        {"passive", ""}
    };
    private Dictionary<string, bool> locks = new Dictionary<string, bool>() {
        {"active1", false},
        {"active2", false},
        {"passive", false}
    };
    private List<Coroutine> cooldownCoroutines = new List<Coroutine>();

    private bool newLevel = false;
    private int level = -1;
    private GameObject levelUpCanvas;
    private Image levelUpAbility;
    private TMP_Text levelUpAbilityName;
    private GameObject levelUpText;
    private int levelDuration = 30;

    // Start is called before the first frame update
    void Start()
    {
        levelUpCanvas = Instantiate(Resources.Load("UI/LevelUp") as GameObject, new Vector3(0, 0, 0), Quaternion.identity);

        levelUpAbility = levelUpCanvas.transform.Find("AbilityImage").GetComponent<Image>();
        levelUpAbilityName = levelUpCanvas.transform.Find("AbilityName").GetComponent<TMP_Text>();
        levelUpText = levelUpCanvas.transform.Find("Text").gameObject; 

        Button continueButton = levelUpCanvas.transform.Find("ContinueButton").GetComponent<Button>();
        continueButton.onClick.AddListener(() => ContinuePlay());
        levelUpCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        StartCoroutine(LevelUp(levelDuration));
    }

    public void SetActiveAbility(string type, string ability)
    {
        currentAbilities[type] = ability;
    }
    public void SetPassiveAbility(string type, string ability)
    {
        currentAbilities[type] = ability;
    }

    public void Fire(int active, Vector2 direction)
    {
        if(0 == active && !locks["active1"]) {
            // fire active1
            if(activeAbilities.Contains(currentAbilities["active1"])) {
                locks["active1"] = true;
                Spawn(currentAbilities["active1"], direction);
                cooldownCoroutines.Add(StartCoroutine(Cooldown("active1", cooldowns[currentAbilities["active1"]])));
            }
        }
        if(1 == active && !locks["active2"]) {
            // fire active2
            if(activeAbilities.Contains(currentAbilities["active2"])) {
                locks["active2"] = true;
                Spawn(currentAbilities["active2"], direction);
                cooldownCoroutines.Add(StartCoroutine(Cooldown("active2", cooldowns[currentAbilities["active2"]])));
            }
        }
    }

    private void Spawn(string ability, Vector2 direction)
    {
        GameObject abilityPrefab = Resources.Load("Abilities/" + ability) as GameObject;
        GameObject newAbility = Instantiate(abilityPrefab, transform.position, Quaternion.identity) as GameObject;
        newAbility.GetComponent<BaseAbilityController>().SetDirection(direction);
    }

    public List<string> GetActiveAbilityNames()
    {
        /* If there was a level up, only give the new ability */
        if(newLevel) {
            newLevel = false;
            if(level < 2) {
                return new List<string>(){activeAbilities[activeAbilities.Count - 1]};
            }
            else {
                /* 2 dice, so 2 abilities */
                List<string> _abilities = new List<string>(){activeAbilities[activeAbilities.Count - 1]};
                string ability = activeAbilities[Random.Range(0, activeAbilities.Count - 2)];
                while(ability == currentAbilities["active1"] || ability == currentAbilities["active2"]) {
                    ability = activeAbilities[Random.Range(0, activeAbilities.Count - 2)];
                }
                _abilities.Add(ability);
                
                return _abilities;
            }
        }

        return new List<string>(activeAbilities);
    }
    public List<string> GetPassiveAbilityNames()
    {
        return passiveAbilities;
    }

    private IEnumerator Cooldown(string slock, float duration)
    {
        yield return new WaitForSeconds(duration);

        locks[slock] = false;
    }

    public void ResetCooldowns()
    {
        foreach(Coroutine cd in cooldownCoroutines) {
            StopCoroutine(cd);
        }

        locks["active1"] = false;
        locks["active2"] = false;
        locks["passive"] = false;
    }

    public void DespawnPermanents()
    {
        GameObject dogBone = GameObject.FindGameObjectWithTag("DogBone");
        GameObject stick = GameObject.FindGameObjectWithTag("Stick");

        if(dogBone) {
            Destroy(dogBone);
        }

        if(stick) {
            Destroy(stick);
        }
    }

    private void LevelUp()
    {
        AudioController.Instance.LevelUp();

        /* Pause game */
        Time.timeScale = 0;

        level++;
        newLevel = true;

        if("Dice" == activeAbilitiesLevel[level]) {
            newLevel = false;
            GameObject dice = GameObject.FindGameObjectWithTag("DiceContainer");
            DiceController diceController = dice.GetComponent<DiceController>();
            diceController.AddDice();

            /* Show level up screen */
            levelUpAbility.sprite = Resources.Load<Sprite>("Sprites/Dice/Dice_Still");
            levelUpAbilityName.text = "Additional Dice!";
            levelUpCanvas.SetActive(true);
            levelUpText.SetActive(false);
        }
        else {
            /* Show level up screen */
            levelUpAbility.sprite = Resources.Load<Sprite>("Sprites/Abilities/" + activeAbilitiesLevel[level]);
            levelUpAbilityName.text = activeAbilitiesLevel[level];
            levelUpText.SetActive(true);
            levelUpCanvas.SetActive(true);

            GameObject.Find("ContinueButton").GetComponent<Button>().Select();

            /* Level ability */
            activeAbilities.Add(activeAbilitiesLevel[level]);

        }
    }

    private void ContinuePlay()
    {
        /* Hide */
        levelUpCanvas.SetActive(false);
        
        /* Unpause game */
        Time.timeScale = 1;

        /* Set timer for next level up */
        if(level < (activeAbilitiesLevel.Count - 1)) {
            StartCoroutine(LevelUp(levelDuration + (levelDuration * (level + 1))));
        }
    }

    private IEnumerator LevelUp(float duration)
    {
        yield return new WaitForSeconds(duration);

        LevelUp();
    }

    public void ResetAbilities()
    {
        /* Reset active abilities */
        activeAbilities = new List<string>() {
            "Ball", "DogBone"
        };

        /* Reset current abilities */
        currentAbilities = new Dictionary<string, string>() {
            {"active1", ""},
            {"active2", ""},
            {"passive", ""}
        };

        /* Reset cooldowns */
        ResetCooldowns();

        /* Despawn permanents */
        DespawnPermanents();

        /* Reset level */
        level = -1;
        newLevel = false;

        Destroy(levelUpCanvas);
    }
}
