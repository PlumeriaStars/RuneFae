using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float _projectileSpeed = 6f;
    [SerializeField] float _duration = 5f;


    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * Time.deltaTime * _projectileSpeed);
        CheckDuration();
    }

    void CheckDuration()
    {
        _duration -= Time.deltaTime;

        if (_duration <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            Destroy(gameObject);
    }
}
