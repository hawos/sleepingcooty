using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource win;
    public AudioSource lose;
    public AudioSource levelUp;
    public AudioSource cootsHit;
    public AudioSource enemyHit;
    public AudioSource ludwigHit;
    public AudioSource diceRoll;

    public static AudioController Instance;

    private void Awake()
    {
        if(null != Instance) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LevelUp()
    {
        levelUp.Play();
    }

    public void CootsHit()
    {
        cootsHit.Play();
    }

    public void Win()
    {
        win.Play();
    }

    public void Lose()
    {
        lose.Play();
    }

    public void EnemyHit()
    {
        enemyHit.Play();
    }

    public void LudwigHit()
    {
        ludwigHit.Play();
    }

    public void DiceRoll()
    {
        diceRoll.Play();
    }
}
