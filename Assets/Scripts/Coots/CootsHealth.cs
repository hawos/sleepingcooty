using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CootsHealth : MonoBehaviour
{
    public Canvas livesCanvas;
    private int lives = 3;
    private bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        livesCanvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!active) {
            if(MainManager.Instance.HasGameStarted()) {
                active = true;
                livesCanvas.gameObject.SetActive(true);
            }
        }
    }

    public void Damage()
    {
        AudioController.Instance.CootsHit();

        lives--;

        livesCanvas.transform.GetChild(lives).gameObject.SetActive(false);

        if(0 >= lives) {
            MainManager.Instance.GameOver();
        }
    }

    public void Reset()
    {
        lives = 3;
        active = false;
        livesCanvas.transform.GetChild(0).gameObject.SetActive(true);
        livesCanvas.transform.GetChild(1).gameObject.SetActive(true);
        livesCanvas.transform.GetChild(2).gameObject.SetActive(true);
        livesCanvas.gameObject.SetActive(false);
    }
}
