using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeath : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _isFalling = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isFalling && transform.position.y > -7)
            transform.Translate(Vector2.down * Time.deltaTime * 8f);
    }

    public void SetToFall()
    {
        _isFalling = true;
    }
}
