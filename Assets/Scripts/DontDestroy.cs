using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private void Start()
    {
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] canvas = GameObject.FindGameObjectsWithTag("Canvas");
        GameObject[] controller = GameObject.FindGameObjectsWithTag("GameController");

        if (player.Length > 1)
            Destroy(player[1]);
        if (canvas.Length > 1)
            Destroy(canvas[1]);
        if(controller.Length > 1)
            Destroy(controller[1]);
        
        DontDestroyOnLoad(this.gameObject);
    }
}
