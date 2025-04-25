using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamAttack : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float _projectileSpeed = 10f;
    [SerializeField] float _duration = 0.5f;

    [Header("Damage")]
    [SerializeField] int _damage = 25;
    [SerializeField] int _beamElement = 0;

    private bool _isMoving = false;
    private BoxCollider2D _collider = null;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(_isMoving)
        {
            transform.Translate(Vector2.up * Time.deltaTime * _projectileSpeed);
            CheckDuration();
        }
    }

    void CheckDuration()
    {
        _duration -= Time.deltaTime;

        if (_duration <= 0)
            Destroy(gameObject);
    }

    public int GetBeamDamage()
    {
        return _damage;
    }

    public int GetBeamElement()
    {
        return _beamElement;
    }

    public void setToMove()
    {
        _isMoving = true;
        _collider.enabled = true;
    }
}
