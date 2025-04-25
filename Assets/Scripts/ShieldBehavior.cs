using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject _platformObject;
    [SerializeField] GameObject _shieldObject;

    private void Start()
    {
        _platformObject = GameObject.FindGameObjectWithTag("Platform");
        Physics2D.IgnoreLayerCollision(_shieldObject.layer, _platformObject.layer);
    }
}
