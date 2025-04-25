using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMelee : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float _duration = 0.75f;

    void Update()
    {
        CheckDuration();
    }

    void CheckDuration()
    {
        _duration -= Time.deltaTime;

        if (_duration <= 0)
            Destroy(gameObject);
    }
}
