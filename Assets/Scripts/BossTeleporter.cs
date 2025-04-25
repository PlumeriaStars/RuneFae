using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTeleporter : MonoBehaviour
{
    [SerializeField] LevelController _levelController = null;
    // Start is called before the first frame update
    void Start()
    {
        _levelController = FindObjectOfType<LevelController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.F))
        {
            _levelController.LoadBossRoom();
        }
    }
}
