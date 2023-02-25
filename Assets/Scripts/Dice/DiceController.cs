using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceController : MonoBehaviour
{
    private GameObject diceContainer;
    private GameObject dice1;
    private GameObject dice2;
    private Animator animator1;
    private Animator animator2;

    private AbilityController abilityController;
    private List<string> activeAbilities;
    private List<string> passiveAbilities;
    private List<string> currentAbilities = new List<string>(){};

    private float rerollAfter = 9.3f;
    private float lockFor = 2.3f;
    private float rollFor = 0.7f;
    private bool locked = false;
    private bool active = false;

    private Vector2 initialDicePosition;


    private Dictionary<string, bool> activeDice = new Dictionary<string, bool>() {
        {"active1", true},
        {"active2", false},
    };
    private Dictionary<string, bool> passiveDice = new Dictionary<string, bool>() {
        {"passive1", true},
    };

    // Start is called before the first frame update
    void Start()
    {
        diceContainer = gameObject;
        dice1 = transform.Find("Dice1").gameObject;
        dice2 = transform.Find("Dice2").gameObject;
        initialDicePosition = dice1.transform.position;

        animator1 = dice1.GetComponentInChildren<Animator>();
        animator2 = dice2.GetComponentInChildren<Animator>();
        animator2.Play("Wuerfel_Still");

        dice2.SetActive(false);
        diceContainer.SetActive(false);
    }

    public void Init()
    {
        active = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        abilityController = player.GetComponent<AbilityController>();
        
        passiveAbilities = abilityController.GetPassiveAbilityNames();

        diceContainer.SetActive(true);
        RollAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RollAll()
    {
        if(locked || !active) {
            return;
        }

        AudioController.Instance.DiceRoll();

        locked = true;
        animator1.Play("Wuerfel_Spin");
        animator2.Play("Wuerfel_Spin");

        StartCoroutine(_Roll());
    }

    private IEnumerator _Roll()
    {
        yield return new WaitForSeconds(rollFor);

        animator1.Play("Wuerfel_Still");
        animator2.Play("Wuerfel_Still");

        dice1.transform.Find("Dice").GetComponent<Image>().color = Color.grey;
        dice2.transform.Find("Dice").GetComponent<Image>().color = Color.grey;

        abilityController.ResetCooldowns();
        abilityController.DespawnPermanents();
        activeAbilities = new List<string>(abilityController.GetActiveAbilityNames());

        StopCoroutine("_RollAll");

        List<string> newAbilities = new List<string>(){};

        foreach(KeyValuePair<string, bool> die in activeDice) {

            if(die.Value) {
                string ability = activeAbilities[Random.Range(0, activeAbilities.Count)];

                int i = 0;

                while(i < 10 && (currentAbilities.Contains(ability) || newAbilities.Contains(ability))) {
                    i++;
                    ability = activeAbilities[Random.Range(0, activeAbilities.Count)];
                }

                newAbilities.Add(ability);

                abilityController.SetActiveAbility(die.Key, ability);
            }
        }
        foreach(KeyValuePair<string, bool> die in passiveDice) {
            if(die.Value) {
                abilityController.SetPassiveAbility(die.Key, passiveAbilities[Random.Range(0, passiveAbilities.Count)]);
            }
        }

        currentAbilities = new List<string>(newAbilities);

        StartCoroutine("_RollAll");
        StartCoroutine("Unlock");
    }

    public void RollSelected(string selected)
    {
        if(locked) {
            return;
        }

        // TODO dice class damit man einzeln locken kann
        // call Abilities.SetActiveAbility
    }

    public void AddDice()
    {
        activeDice["active2"] = true;

        dice1.transform.position = new Vector3(555, 270, 0);
        dice2.SetActive(true);
        animator2.Play("Wuerfel_Still");
    }

    private IEnumerator _RollAll()
    {
        yield return new WaitForSeconds(rerollAfter);

        RollAll();
    }


    private IEnumerator Unlock()
    {
        yield return new WaitForSeconds(lockFor);

        locked = false;

        dice1.transform.Find("Dice").GetComponent<Image>().color = Color.white;
        dice2.transform.Find("Dice").GetComponent<Image>().color = Color.white;
    }

    public void Reset()
    {
        locked = false;
        active = false;

        /* Reset active dice */
        activeDice = new Dictionary<string, bool>() {
            {"active1", true},
            {"active2", false},
        };

        /* Stop coroutines */
        StopCoroutine("_RollAll");
        StopCoroutine("Unlock");

        dice1.transform.position = initialDicePosition;
        dice2.SetActive(false);
        diceContainer.SetActive(false);
    }
}
