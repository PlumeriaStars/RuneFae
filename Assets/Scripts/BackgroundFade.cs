using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundFade : MonoBehaviour
{
    [SerializeField] SpriteRenderer _endBG;
    [SerializeField] bool _bossDead;
    [SerializeField] LevelController _levelcontroller;
    [SerializeField] GameObject _platformController;
    int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        _levelcontroller = GameObject.FindObjectOfType<LevelController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (count == 0 && _bossDead)
        {
            StartCoroutine("EnableEndScreen");
            count++;
        }
    }

    public void SetBossToDead()
    {
        _bossDead = true;
    }

    IEnumerator EnableEndScreen()
    {
        yield return new WaitForSeconds(2f);
        _endBG.enabled = true;
        _levelcontroller.SetOverlayAndPlayerVisibilty(false);
        _levelcontroller.LoadVictory();
        _platformController.SetActive(false);
    }
}
