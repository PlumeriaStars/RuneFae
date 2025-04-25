using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundEnemy : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] Transform _currentPatrolPoint;
    [SerializeField] int _currentPatrolPointIndex = 0;

    [Header("Movement")]
    [SerializeField] float _enemySpeed = 12f;
    [SerializeField] float _stunDuration = 1f;

    [Header("Health")]
    [SerializeField] int _enemyMaxHealth = 250;
    [SerializeField] int _enemyCurrentHealth;
    [SerializeField] FloatingHealthBar _enemyHealth;


    [Header("Attack")]
    [SerializeField] float _enemyRadius = 4f;
    [SerializeField] float _attackCooldown = 2f;
    [SerializeField] float _attackTimer = 2f;
    [SerializeField] LayerMask _playerLayer;
    [SerializeField] GameObject _enemyProjectile = null;

    [Header("SoundFX")]
    [SerializeField] AudioClip _attackSFX;
    [SerializeField] AudioClip _takeDamageSFX;

    [Header("Controllers")]
    [SerializeField] Rigidbody2D _rigidBody2D = null;
    [SerializeField] SpriteRenderer _spriteRender = null;
    [SerializeField] Animator _animatorController = null;
    [SerializeField] GameObject _spiritDrop;
    [SerializeField] Transform _playerController = null;

    [Header("Flash")]
    [SerializeField] Material[] _flashColors = null;
    [SerializeField] float _flashDuration = 0.3f;

    [SerializeField] bool _followingPlayer = false;
    private bool _isFlashing = false;

    // Start is called before the first frame update
    void Start()
    {
        _playerController = FindObjectOfType<PlayerBehavior>().GetComponent<Transform>();
        _currentPatrolPoint = patrolPoints[_currentPatrolPointIndex];
        _enemyCurrentHealth = _enemyMaxHealth;
        _attackTimer = _attackCooldown;
        _enemyHealth.UpdateEnemyHealth();
        SetSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!_followingPlayer)
        {
            Move();
            if (Vector2.Distance(_currentPatrolPoint.position, transform.position) < 0.75f)
                GetNextPatrolPoint();
        }

        CheckOverlapCircle();
    }

    private void Move()
    {
        transform.Translate(Vector2.right * Time.deltaTime * _enemySpeed);
    }

    private void MoveTowardsPlayer(Transform player)
    {
        if (Vector2.Distance(player.position, transform.position) > 1f)
            Move();

        
    }

    void CheckOverlapCircle()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _enemyRadius, _playerLayer);
        int playerLayerValue = LayerMask.NameToLayer("Player");

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.gameObject.layer == playerLayerValue)
            {
                LookAtPlayer(hitCollider.transform);
                //follow player             
                _followingPlayer = true;
                MoveTowardsPlayer(hitCollider.transform);
                CheckAttackCooldown();
            }
        }

        if(hitColliders.Length == 0)
        {
            _followingPlayer = false;
            SetSpeed();
        }
    }

    void LookAtPlayer(Transform player)
    {
        if (player.position.x < transform.position.x)
        {
            _enemySpeed = Mathf.Abs(_enemySpeed) * -1;
            _spriteRender.flipX = false;
        }
        else
        {
            _enemySpeed = Mathf.Abs(_enemySpeed);
            _spriteRender.flipX = true;
        }
    }
    private void CheckAttackCooldown()
    {

        _attackTimer -= Time.deltaTime;

        if (_attackTimer <= 0)
        {
            _animatorController.SetTrigger("slimeAttack");
            AudioManager.Instance.PlaySFX(_attackSFX, 1.5f);
            //facing left
            if (!_spriteRender.flipX)
                Instantiate(_enemyProjectile, transform.position + new Vector3(-0.5f, 0f, 0f), Quaternion.Euler(0f, 0f, -180f));
            else
                Instantiate(_enemyProjectile, transform.position + new Vector3(0.5f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
            _attackTimer = _attackCooldown;
        }
    }

    void GetNextPatrolPoint()
    {
        if (_currentPatrolPointIndex < patrolPoints.Length - 1)
            _currentPatrolPointIndex++;
        else
            _currentPatrolPointIndex = 0;

        _currentPatrolPoint = patrolPoints[_currentPatrolPointIndex];
        SetSpeed();
    }

    void SetSpeed()
    {
        if (_currentPatrolPoint.position.x < transform.position.x)
        {
            _enemySpeed = Mathf.Abs(_enemySpeed) * -1;
            _spriteRender.flipX = false;
        }
        else
        {
            _enemySpeed = Mathf.Abs(_enemySpeed);
            _spriteRender.flipX = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DeathPlane"))
        {
            _enemyCurrentHealth = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BaseProjectile"))
        {
            Destroy(collision.gameObject);

            _enemyCurrentHealth -= 50;
            AudioManager.Instance.PlaySFX(_takeDamageSFX, 1.5f);
            EnemyFlash(0);
        }

        if (collision.CompareTag("FireProjectile"))
        {
            Destroy(collision.gameObject);

            _enemyCurrentHealth -= 70;
            AudioManager.Instance.PlaySFX(_takeDamageSFX, 1.5f);
            EnemyFlash(1);
        }

        if (collision.CompareTag("WaterProjectile"))
        {
            Destroy(collision.gameObject);

            _enemyCurrentHealth -= 60;
            AudioManager.Instance.PlaySFX(_takeDamageSFX, 1.5f);
            StartCoroutine(Stunned());
            EnemyFlash(2);
        }

        if (collision.CompareTag("WindProjectile"))
        {
            Destroy(collision.gameObject);

            _enemyCurrentHealth -= 40;
            AudioManager.Instance.PlaySFX(_takeDamageSFX, 1.5f);
            EnemyFlash(3);

            if (!_spriteRender.flipX)
                transform.Translate(Vector2.right * 1f);
            else
                transform.Translate(Vector2.left * 1f);
        }

        if (collision.CompareTag("BaseMelee"))
        {
            _enemyCurrentHealth -= 100;
            AudioManager.Instance.PlaySFX(_takeDamageSFX, 1.5f);
            EnemyFlash(0);
        }

        if (collision.CompareTag("FireMelee"))
        {
            _enemyCurrentHealth -= 200;
            AudioManager.Instance.PlaySFX(_takeDamageSFX, 1.5f);
            EnemyFlash(1);
        }

        if (collision.CompareTag("WaterMelee"))
        {
            _enemyCurrentHealth -= 125;
            AudioManager.Instance.PlaySFX(_takeDamageSFX, 1.5f);
            _enemyHealth.UpdateEnemyHealth();
            EnemyFlash(2);
            StartCoroutine(Stunned());
        }

        if (collision.CompareTag("WindMelee"))
        {
            _enemyCurrentHealth -= 75;
            AudioManager.Instance.PlaySFX(_takeDamageSFX, 1.5f);
            EnemyFlash(3);

            if (!_spriteRender.flipX)
                transform.Translate(Vector2.right * 1.75f);
            else
                transform.Translate(Vector2.left * 1.75f);
        }

        _enemyHealth.UpdateEnemyHealth();

        if (_enemyCurrentHealth <= 0)
            DestroyEnemy();   
    }

    public void DestroyEnemy()
    {
        Instantiate(_spiritDrop, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    IEnumerator Stunned()
    {
        float speed = _enemySpeed;
        _enemySpeed = 0;
        yield return new WaitForSeconds(_stunDuration);
        _enemySpeed = speed;
    }

    public void EnemyFlash(int element)
    {
        if (!_isFlashing)
        {
            if (element == 2)
                StartCoroutine(FlashOnDamage(element, _stunDuration));
            else
                StartCoroutine(FlashOnDamage(element, _flashDuration));
        }
    }

    IEnumerator FlashOnDamage(int element, float duration)
    {
        Material ogMat = this._spriteRender.material;
        _spriteRender.material = _flashColors[element];

        yield return new WaitForSeconds(duration);

        _spriteRender.material = ogMat;
    }
    public int GetEnemeyHealth()
    {
        return _enemyCurrentHealth;
    }
    
}
