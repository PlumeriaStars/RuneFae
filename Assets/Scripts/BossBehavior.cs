using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] Transform _currentPatrolPoint;
    [SerializeField] int _currentPatrolPointIndex = 0;

    [Header("Movement")]
    [SerializeField] float _enemySpeed = 5f;
    [SerializeField] float _stunDuration = 0.5f;

    [Header("Health")]
    [SerializeField] int _enemyMaxHealth = 1250;
    [SerializeField] int _enemyCurrentHealth;
    [SerializeField] FloatingHealthBarB _enemyHealth;


    [Header("Attack")]
    [SerializeField] float _enemyRadius = 5f;
    [SerializeField] float _attackCooldown = 2f;
    [SerializeField] float _attackTimer = 2f;
    [SerializeField] float _meleeCooldown = 4f;
    [SerializeField] float _meleeTimer = 4f;
    [SerializeField] LayerMask _playerLayer;
    [SerializeField] GameObject _enemyMeleeVFX = null;
    [SerializeField] GameObject _enemyMeleeLeft = null;
    [SerializeField] GameObject _enemyMeleeRight = null;
    [SerializeField] GameObject[] _enemyBeams = null;

    [Header("SoundFX")]
    [SerializeField] AudioClip _spellSFX;
    [SerializeField] AudioClip _meleeSFX;
    [SerializeField] AudioClip _takeDamageSFX;

    [Header("Flash")]
    [SerializeField] Material[] _flashColors = null;
    [SerializeField] float _flashDuration = 0.3f;

    [Header("Controllers")]
    [SerializeField] Rigidbody2D _rigidBody2D = null;
    [SerializeField] SpriteRenderer _spriteRender = null;
    [SerializeField] Animator _animatorController = null;
    [SerializeField] GameObject _finalDrop;
    [SerializeField] Transform _playerController = null;
    [SerializeField] GameObject _platformController = null;
    [SerializeField] BackgroundFade _bgFade = null;
    [SerializeField] GameObject _deathVFX;

    [SerializeField] bool _followingPlayer = false;
    [SerializeField] Vector2 _pointOnScreen;

    private bool _canMove = true;
    private float _currentSpeed;
    private bool _isFlashing = false;

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), _platformController.GetComponent<Collider2D>());
        _pointOnScreen = patrolPoints[0].transform.position;

        _attackTimer = _attackCooldown;
        _meleeTimer = 0;
        _currentSpeed = _enemySpeed;
        EndMelee();
    }

    // Update is called once per frame
    void Update()
    {
        if(_canMove)
            Movement();
        CheckAttackTimer();
        CheckMeleeTimer();
        CheckOverlapCircle();
    }

    void Movement()
    {
        if (_pointOnScreen.x < transform.position.x)
        {
            _spriteRender.flipX = false;
            _currentSpeed = Mathf.Abs(_currentSpeed);
        }
        if (_pointOnScreen.x > transform.position.x)
        {
            _spriteRender.flipX = true;
            _currentSpeed = Mathf.Abs(_currentSpeed);
        }

        if (Vector2.Distance(transform.position, _pointOnScreen) > 2f && _pointOnScreen.x > patrolPoints[0].position.x && _pointOnScreen.x < patrolPoints[1].position.x)
            transform.position = Vector2.MoveTowards(transform.position, _pointOnScreen, _currentSpeed * Time.deltaTime);
        else
            GetNewPointOnScreen();
    }

    void GetNewPointOnScreen()
    {
        _pointOnScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
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
                MeleeSwipe();
            }
        }

    }
    
    void LookAtPlayer(Transform player)
    {
        if (player.position.x < transform.position.x)
        {
            _spriteRender.flipX = false;
        }
        else
        {
            _spriteRender.flipX = true;
        }
    }

    void CheckAttackTimer()
    {
        _attackTimer -= Time.deltaTime;

        if (_attackTimer <= 0)
        {
            _animatorController.SetTrigger("bossRangeAttack");
            BeamAttack();
            _attackTimer = _attackCooldown;
        }
    }

    void CheckMeleeTimer()
    {
        if(_meleeTimer > 0)
            _meleeTimer -= Time.deltaTime;

    }

    void MeleeSwipe()
    {
        if (_meleeTimer <= 0)
        {
            MeleeAttack();
            _meleeTimer = _meleeCooldown;
        }
    }

    void BeamAttack()
    {
        int pattern = Random.Range(0, 3);
        AudioManager.Instance.PlaySFX(_spellSFX, 1.5f);

        if (pattern == 0)
        {
            Instantiate(_enemyBeams[0], new Vector3(-4f, -1.4f, 0f), Quaternion.Euler(0f, 0f, 0f));
            Instantiate(_enemyBeams[1], new Vector3(0f, -1.4f, 0f), Quaternion.Euler(0f, 0f, 0f));
            Instantiate(_enemyBeams[2], new Vector3(4f, -1.4f, 0f), Quaternion.Euler(0f, 0f, 0f));
        }
        else if(pattern == 1)
        {
            Instantiate(_enemyBeams[0], new Vector3(-5.5f, -1.4f, 0f), Quaternion.Euler(0f, 0f, 0f));
            Instantiate(_enemyBeams[1], new Vector3(-1.5f, -1.4f, 0f), Quaternion.Euler(0f, 0f, 0f));
            Instantiate(_enemyBeams[2], new Vector3(2.5f, -1.4f, 0f), Quaternion.Euler(0f, 0f, 0f));
        }
        else
        {
            Instantiate(_enemyBeams[0], new Vector3(-2.5f, -1.4f, 0f), Quaternion.Euler(0f, 0f, 0f));
            Instantiate(_enemyBeams[1], new Vector3(1.5f, -1.4f, 0f), Quaternion.Euler(0f, 0f, 0f));
            Instantiate(_enemyBeams[2], new Vector3(5.5f, -1.4f, 0f), Quaternion.Euler(0f, 0f, 0f));
        }
    }

    void MeleeAttack()
    {
        _animatorController.SetTrigger("bossMeleeAttack");
        AudioManager.Instance.PlaySFX(_meleeSFX, 1.5f);
        //facing left
        _canMove = false;
        DisableMovment();
        if (!_spriteRender.flipX)
        {
            _enemyMeleeLeft.SetActive(true);
            Instantiate(_enemyMeleeVFX, _enemyMeleeLeft.transform.position, Quaternion.Euler(0f, 0f, 0f));
        }
        else
        {
            _enemyMeleeRight.SetActive(true);
            Instantiate(_enemyMeleeVFX, _enemyMeleeRight.transform.position, Quaternion.Euler(180f, 0f, 180f));
        }

        Invoke("EndMelee", 0.15f);
    }

    void EndMelee()
    {
        _canMove = true;
        EnableMovement();
        _enemyMeleeLeft.SetActive(false);
        _enemyMeleeRight.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BaseProjectile"))
        {
            Destroy(collision.gameObject);

            _enemyCurrentHealth -= 40;
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

            _enemyCurrentHealth -= 50;
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

        if (collision.gameObject.CompareTag("DeathPlane"))
        {
            _enemyCurrentHealth = 0;
        }

        _enemyHealth.UpdateEnemyHealth();

        if (_enemyCurrentHealth <= 0)
            DestroyEnemy();
    }

    public void DestroyEnemy()
    {
        Instantiate(_deathVFX, transform.position, Quaternion.identity);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        gameObject.GetComponentInChildren<Canvas>().enabled = false;
        _bgFade.SetBossToDead();
        Destroy(this);
    }

    public void DisableMovment()
    {
        _currentSpeed = 0;
    }

    public void EnableMovement()
    {
        _currentSpeed = _enemySpeed;
    }

    IEnumerator Stunned()
    {
        float speed = _currentSpeed;
        _currentSpeed = 0;
        yield return new WaitForSeconds(_stunDuration);
        _currentSpeed = speed;
    }

    public int GetEnemeyHealth()
    {
        return _enemyCurrentHealth;
    }

    public void EnemyFlash(int element)
    {
        if (!_isFlashing)
        {
            if(element == 2)
                StartCoroutine(FlashOnDamage(element, _stunDuration));
            else
                StartCoroutine(FlashOnDamage(element, _flashDuration));
        }
    }

    IEnumerator FlashOnDamage(int element, float duration)
    {
        _isFlashing = true;
        Material ogMat = this._spriteRender.material;
        _spriteRender.material = _flashColors[element];

        yield return new WaitForSeconds(duration);

        _isFlashing = false;
        _spriteRender.material = ogMat;
    }
}
