using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] Rigidbody2D _rigidBody2D = null;
    [SerializeField] SpriteRenderer _spriteRender = null;
    [SerializeField] Animator _animatorController = null;
    [SerializeField] LevelController _levelController = null;
    [SerializeField] AnimatorOverrideController[] _animatiorElemental = null;

    [Header("Movement")]
    [SerializeField] Transform _groundCheck = null;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float _characterSpeed = 12f;
    [SerializeField] float _groundDistance = 0.15f;
    [SerializeField] float _jumpHeight = 3f;
    //[SerializeField] LayerMask enemyMask;
    [SerializeField] private bool _facingRight = true;
    [SerializeField] private bool _isGrounded = false;
    private bool _canMove = true;

    [Header("Stats")]
    [SerializeField] int _playerStartingHealth = 100;
    [SerializeField] int _playerMaxHealth;
    [SerializeField] int _playerCurrentHealth;
    [SerializeField] int _playerSpiritCount = 0;

    [Header ("Attack")]
    [SerializeField] float _attackDelay = 7f;
    [SerializeField] float _knockBack = 3f;
    [SerializeField] GameObject[] _projectiles;
    [SerializeField] Transform _projectileLeft = null;
    [SerializeField] Transform _projectileRight = null;
    [SerializeField] float _spellDelay = 1f;
    [SerializeField] GameObject[] _baseMelee = null; //object prefabs
    [SerializeField] GameObject[] _meleeLeft = null; //left hitboxes
    [SerializeField] GameObject[] _meleeRight = null; //right hitboxes
    [SerializeField] float _meleeDelay = 0.5f;
    private float _lastAttackTime = 0f;
    private float _lastSpellTime = 0f;
    private float _lastMeleeTime = 0f;
    [SerializeField] bool _disableAttack = false;

    [Header("SoundFX")]
    [SerializeField] AudioClip[] _spellSFX;
    [SerializeField] AudioClip[] _meleeSFX;
    [SerializeField] AudioClip _takeDamageSFX;
    [SerializeField] AudioClip _transformSFX;

    [Header("Special")]
    [SerializeField] GameObject _respawnPoint;
    [SerializeField] bool _canDoubleJump = false;
    private bool _doubleJumpCheck = false;
    [SerializeField] bool _canShield = false;
    [SerializeField] GameObject _shieldObject;

    [Header("Flash")]
    [SerializeField] Material _flashColor = null;
    [SerializeField] float _flashDuration = 0.3f;

    private bool _currentlyShielded = false;
    private bool _inShop = false;
    private bool _isFlashing = false;
    private bool _isAttacking = false;
    private float _playerCurrentSpeed;
    private int[] _runeElement;
    private int _currentElement = 0;
    private bool _inBossRoom = false;
    [SerializeField] Animator _baseAnimator;
    // Start is called before the first frame update
    void Start()
    {
        _levelController = GameObject.FindObjectOfType<LevelController>();
        //_respawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        _playerMaxHealth = _playerStartingHealth;
        _playerCurrentHealth = _playerMaxHealth;
        _playerCurrentSpeed = _characterSpeed;
        _facingRight = true;
        _baseAnimator = _animatorController;

    }

    // Update is called once per frame
    void Update()
    { 
        if(_inBossRoom)
        {
            Debug.Log("with boss");
            _levelController = GameObject.FindObjectOfType<LevelController>();
            _inBossRoom = false;
        }

        if (!_inShop && _canMove)
        {
            Movement();
            Jumping();
            Shield();
            MeleeAttack();
            RangeAttack();
        }

    }

    public void SetBossRoomStatus(bool status)
    {
        _inBossRoom = status;
    }

    public void ChangeElement(int elem)
    {
        AudioManager.Instance.PlaySFX(_transformSFX, 1.5f);
        if (elem == 1)
        {
            _currentElement = 1;
            _animatorController.runtimeAnimatorController = _animatiorElemental[1];
        }
        if (elem == 2)
        {
            _currentElement = 2;
            _animatorController.runtimeAnimatorController = _animatiorElemental[2];
        }
        if (elem == 3)
        {
            _currentElement = 3;
            _animatorController.runtimeAnimatorController = _animatiorElemental[3];
        }
    }

    void Movement()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        Vector2 moveAmount = transform.right * horizontalMovement;
        transform.Translate(moveAmount * Time.deltaTime * _playerCurrentSpeed);

        if (horizontalMovement > 0.01)
        {
            _spriteRender.flipX = true;
            _facingRight = true;
        }
        if (horizontalMovement < -0.01)
        {
            _spriteRender.flipX = false;
            _facingRight = false;
        }

        _animatorController.SetFloat("playerSpeed", Mathf.Abs(horizontalMovement));
    }

    void Jumping()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundDistance, groundMask);

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _rigidBody2D.AddRelativeForce(Vector2.up * _jumpHeight);
        }

        //Duuble Jump
        if(Input.GetKeyDown(KeyCode.E) && _canDoubleJump && _doubleJumpCheck && _rigidBody2D.velocity.y < -2f)
        {
            _rigidBody2D.AddRelativeForce(Vector2.up * (_jumpHeight * 1.8f));
            _doubleJumpCheck = false;
        }
        else if (Input.GetKeyDown(KeyCode.E) && _canDoubleJump && _doubleJumpCheck && _rigidBody2D.velocity.y < 0f)
        {
            _rigidBody2D.AddRelativeForce(Vector2.up * (_jumpHeight * 1.2f));
            _doubleJumpCheck = false;
        }
        else if (Input.GetKeyDown(KeyCode.E) && _canDoubleJump && _doubleJumpCheck && !_isGrounded && _rigidBody2D.velocity.y < 2f)
        {
            _rigidBody2D.AddRelativeForce(Vector2.up * (_jumpHeight * 0.75f));
            _doubleJumpCheck = false;
        }
        else if (Input.GetKeyDown(KeyCode.E) && _canDoubleJump && _doubleJumpCheck && !_isGrounded)
        {
            _rigidBody2D.AddRelativeForce(Vector2.up * (_jumpHeight * 0.5f));
            _doubleJumpCheck = false;
        }

        if (!_animatorController.GetBool("playerJump") && !_isGrounded)
            _animatorController.SetBool("playerJump", true);

        if (_animatorController.GetBool("playerJump") && _isGrounded)
            _animatorController.SetBool("playerJump", false);

        if (_isGrounded && _canDoubleJump)
            _doubleJumpCheck = true;
    }

    void Shield()
    {
        if (_canShield && Input.GetKey(KeyCode.E))
        {
            _disableAttack = true;
            _currentlyShielded = true;
            _shieldObject.SetActive(true);
        }
        else
        {
            _shieldObject.SetActive(false);
            _currentlyShielded = false;
            _disableAttack = false;
        }
    }

    void MeleeAttack()
    {
        if(Input.GetButtonDown("Fire1") && !_disableAttack && Time.time > _lastMeleeTime + _meleeDelay)
        {
            _canMove = false;
            _isAttacking = true;

            if (_currentElement == 0)
                BaseMeleeAttack();
            else if (_currentElement == 1)
                FireMeleeAttack();
            else if (_currentElement == 2)
                WaterMeleeAttack();
            else if (_currentElement == 3)
                WindMeleeAttack();

            Invoke("MeleeAttackFinish", 0.2f);
        }
        
    }

    void BaseMeleeAttack()
    {
        _animatorController.SetTrigger("playerMeleeAttack");
        if (_facingRight)
        {
            _meleeRight[0].SetActive(true);
            Instantiate(_baseMelee[0], _meleeRight[0].transform.position, Quaternion.Euler(0f, 0f, 0));
        }
        else
        {
            _meleeLeft[0].SetActive(true);
            Instantiate(_baseMelee[0], _meleeLeft[0].transform.position, Quaternion.Euler(180f, 0f, 180f));
        }

        AudioManager.Instance.PlaySFX(_meleeSFX[0], 1.5f);
        _lastMeleeTime = Time.time;
    }

    void FireMeleeAttack()
    {
        _animatorController.SetTrigger("playerMeleeAttack");
        if (_facingRight)
        {
            _meleeRight[1].SetActive(true);
            Instantiate(_baseMelee[1], _meleeRight[1].transform.position, Quaternion.Euler(0f, 0f, 0f));
        }
        else
        {
            _meleeLeft[1].SetActive(true);
            Instantiate(_baseMelee[1], _meleeLeft[1].transform.position, Quaternion.Euler(180f, 0f, -180f));
        }

        AudioManager.Instance.PlaySFX(_meleeSFX[1], 1.5f);
        _lastMeleeTime = Time.time;
    }

    void WaterMeleeAttack()
    {
        _animatorController.SetTrigger("playerMeleeAttack");
        if (_facingRight)
        {
            _meleeRight[2].SetActive(true);
            Instantiate(_baseMelee[2], _meleeRight[2].transform.position, Quaternion.Euler(0f, 0f, 0));
        }
        else
        {
            _meleeLeft[2].SetActive(true);
            Instantiate(_baseMelee[2], _meleeLeft[2].transform.position, Quaternion.Euler(180f, 0f, 180f));
        }

        AudioManager.Instance.PlaySFX(_meleeSFX[2], 1.5f);
        _lastMeleeTime = Time.time;
    }

    void WindMeleeAttack()
    {
        _animatorController.SetTrigger("playerMeleeAttack");
        if (_facingRight)
        {
            _meleeRight[3].SetActive(true);
            Instantiate(_baseMelee[3], _meleeRight[3].transform.position, Quaternion.Euler(0f, 0f, 0));
        }
        else
        {
            _meleeLeft[3].SetActive(true);
            Instantiate(_baseMelee[3], _meleeLeft[3].transform.position, Quaternion.Euler(180f, 0f, 180f));
        }

        AudioManager.Instance.PlaySFX(_meleeSFX[3], 1.5f);
        _lastMeleeTime = Time.time;
    }

    public void MeleeAttackFinish()
    {
        if (_facingRight)
        {
            switch(_currentElement)
            {
                case 0: _meleeRight[0].SetActive(false);
                        break;
                case 1: _meleeRight[1].SetActive(false);
                        break;
                case 2: _meleeRight[2].SetActive(false);
                        break;
                case 3: _meleeRight[3].SetActive(false);
                        break;
            }
        }
        else
        {
            switch (_currentElement)
            {
                case 0: _meleeLeft[0].SetActive(false);
                        break;
                case 1: _meleeLeft[1].SetActive(false);
                        break;
                case 2: _meleeLeft[2].SetActive(false);
                        break;
                case 3: _meleeLeft[3].SetActive(false);
                        break;
            }
        }

        _canMove = true;
        _isAttacking = false;
    }

    void RangeAttack()
    {
        if (Input.GetButtonDown("Fire2") && !_disableAttack && Time.time > _lastSpellTime + _spellDelay)
        {

            if (_currentElement == 0)
                BaseRangeAttack();
            else if (_currentElement == 1)
                FireRangeAttack();
            else if (_currentElement == 2)
                WaterRangeAttack();
            else if (_currentElement == 3)
                WindRangeAttack();
        }
    }

    void BaseRangeAttack()
    {
        _animatorController.SetTrigger("playerRangeAttack");

        if (_facingRight)
            Instantiate(_projectiles[0], _projectileRight.position, _projectileRight.rotation);
        else
            Instantiate(_projectiles[0], _projectileLeft.position, Quaternion.Euler(0f, 0f, 180f));

        AudioManager.Instance.PlaySFX(_spellSFX[0], 1.5f);

        _lastSpellTime = Time.time;
    }

    void FireRangeAttack()
    {
        _animatorController.SetTrigger("playerRangeAttack");

        if (_facingRight)
            Instantiate(_projectiles[1], _projectileRight.position, _projectileRight.rotation);
        else
            Instantiate(_projectiles[1], _projectileLeft.position, Quaternion.Euler(0f, 0f, 180f));

        AudioManager.Instance.PlaySFX(_spellSFX[1], 1.5f);

        _lastSpellTime = Time.time;
    }

    void WaterRangeAttack()
    {
        _animatorController.SetTrigger("playerRangeAttack");

        if (_facingRight)
            Instantiate(_projectiles[2], _projectileRight.position, _projectileRight.rotation);
        else
            Instantiate(_projectiles[2], _projectileLeft.position, Quaternion.Euler(0f, 0f, 180f));

        AudioManager.Instance.PlaySFX(_spellSFX[2], 1.5f);

        _lastSpellTime = Time.time;
    }

    void WindRangeAttack()
    {
        _animatorController.SetTrigger("playerRangeAttack");

        if (_facingRight)
            Instantiate(_projectiles[3], _projectileRight.position, _projectileRight.rotation);
        else
            Instantiate(_projectiles[3], _projectileLeft.position, Quaternion.Euler(0f, 0f, 180f));

        AudioManager.Instance.PlaySFX(_spellSFX[3], 1.5f);

        _lastSpellTime = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DeathPlane"))
        {
            KillPlayer();
        }

        if (collision.gameObject.CompareTag("Enemy") && !_currentlyShielded && !_isAttacking)
        {
            _playerCurrentHealth -= 25;
            _levelController.UpdateHealth();
            
            if(_facingRight)
                transform.Translate(Vector2.left * _knockBack);
            else
                transform.Translate(Vector2.right * _knockBack);

            PlayerFlash();

            if (_playerCurrentHealth <= 0)
            {
                KillPlayer();
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !_currentlyShielded && !_isAttacking)
        {
            _playerCurrentHealth -= 15;
            PlayerFlash();
            _levelController.UpdateHealth();

            if (_playerCurrentHealth <= 0)
            {
                KillPlayer();
            }
        }

        if (collision.gameObject.CompareTag("SlimeAttack") && !_currentlyShielded)
        {
            _playerCurrentHealth -= 10;
            PlayerFlash();
            _levelController.UpdateHealth();

            if (_playerCurrentHealth <= 0)
            {
                KillPlayer();
            }
        }

        if (collision.gameObject.CompareTag("ShadowAttack") && !_currentlyShielded)
        {
            _playerCurrentHealth -= 30;
            PlayerFlash();
            _levelController.UpdateHealth();

            if (_playerCurrentHealth <= 0)
            {
                KillPlayer();
            }
        }

        if (collision.gameObject.CompareTag("BossMelee") && !_currentlyShielded)
        {
            _playerCurrentHealth -= 50;
            PlayerFlash();
            _levelController.UpdateHealth();

            if (_facingRight)
                transform.Translate(Vector2.left * _knockBack);
            else
                transform.Translate(Vector2.right * _knockBack);

            if (_playerCurrentHealth <= 0)
            {
                KillPlayer();
            }
        }

        if (collision.gameObject.CompareTag("BeamAttack") && !_currentlyShielded)
        {
            if(collision.gameObject.GetComponent<BeamAttack>().GetBeamElement() != _currentElement)
            {
                _playerCurrentHealth -= collision.gameObject.GetComponent<BeamAttack>().GetBeamDamage();
                PlayerFlash();
                _levelController.UpdateHealth();

                if (_facingRight)
                    transform.Translate(Vector2.left * _knockBack);
                else
                    transform.Translate(Vector2.right * _knockBack);

                if (_playerCurrentHealth <= 0)
                {
                    KillPlayer();
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("ShrineI"))
        {
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                _inShop = true;
                _playerCurrentSpeed = 0;
                _animatorController.SetFloat("playerSpeed", 0f);
                collision.gameObject.GetComponent<ShrineController>().EnableShop();
            }

        }

        if (collision.gameObject.CompareTag("ShrineII"))
        {

            if (Input.GetKeyDown(KeyCode.F))
            {
                _inShop = true;
                _playerCurrentSpeed = 0;
                _animatorController.SetFloat("playerSpeed", 0f);
                collision.gameObject.GetComponent<ShrineControllerTier2>().EnableShop();
            }

        }

        if (collision.gameObject.CompareTag("ShrineIII"))
        {

            if (Input.GetKeyDown(KeyCode.F))
            {
                _inShop = true;
                _playerCurrentSpeed = 0;
                _animatorController.SetFloat("playerSpeed", 0f);
                collision.gameObject.GetComponent<ShrineControllerTier3>().EnableShop();
            }

        }

    }

    private void EnableInput()
    {
        _disableAttack = false;
        _canMove = true;
    }

    public void DisableInput()
    {
        _disableAttack = true;
        _canMove = false;
    }

    private void KillPlayer()
    {
        _playerCurrentHealth = 0;
        _levelController.UpdateHealth();       
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        DisableInput();

        Invoke("RespawnPlayer", 1.5f);
    }

    public void ClosedShop()
    {
        _inShop = false;
        _playerCurrentSpeed = _characterSpeed;
    }

    public void RespawnPlayer()
    {
        _levelController.ReloadLevel();
        _levelController.SetOverlayAndPlayerVisibilty(true);
        _playerMaxHealth = _playerStartingHealth;
        _playerCurrentHealth = _playerStartingHealth;
        _playerSpiritCount = 0;
        _levelController.UpdateSpiritCount();
        _levelController.UpdateHealth();
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        EnableInput();
        _currentElement = 0;
        _animatorController.runtimeAnimatorController = _animatiorElemental[0];
        _canShield = false;
        _canDoubleJump = false;
        _doubleJumpCheck = false;
        _levelController.HideAbilityIcon();
        this.transform.SetPositionAndRotation(new Vector3(-4f, -2f, 0f), Quaternion.Euler(0f, 0f, 0f));
        
        //CinemachineVirtualCamera vCam = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        //vCam.PreviousStateIsValid = false;
    }

    public void EnableMovement()
    {
        _canMove = true;
    }

    public void EnableDoubleJump()
    {
        _canDoubleJump = true;
        _doubleJumpCheck = true;
        _canShield = false;
        _levelController.ShowAbilityIcon(1);
    }

    public void EnableShield()
    {
        _canShield = true;
        _canDoubleJump = false;
        _doubleJumpCheck = false;
        _levelController.ShowAbilityIcon(0);
    }
    public void IncreaseSpiritCount(int numOfSpirits)
    {
        _playerSpiritCount += numOfSpirits;
        _levelController.UpdateSpiritCount();
    }

    public void DecreaseSpiritCount(int numOfSpirits)
    {
        Debug.Log("Current: " + _playerSpiritCount);
        Debug.Log("Removing: " + numOfSpirits);    
        _playerSpiritCount -= numOfSpirits;
        Debug.Log("New: " + _playerSpiritCount);
        _levelController.UpdateSpiritCount();
    }

    public int GetHealth()
    {
        return _playerCurrentHealth;
    }

    public int GetSpiritCount()
    {
        return _playerSpiritCount;
    }

    public int GetStartingHealth()
    {
        return _playerStartingHealth;
    }

    public int GetMaxHealth()
    {
        return _playerMaxHealth;
    }

    public float GetMaxSpeed()
    {
        return _characterSpeed;
    }

    public bool GetAttackStatus()
    {
        return _disableAttack;
    }

    public bool GetShieldStatus()
    {
        return _canShield;
    }

    public bool GetJumpStatus()
    {
        return _canDoubleJump;
    }

    public void DisableAttackStatus(bool status)
    {
        _disableAttack = status;
    }

    public void IncreaseMaxHealth(int addHealth)
    {
        if ((_playerCurrentHealth + (addHealth * 2)) < _playerMaxHealth)
            _playerCurrentHealth += addHealth * 2;
        else
            _playerCurrentHealth = _playerMaxHealth;
        _playerMaxHealth += addHealth;
        _levelController.UpdateMaxHealth();
        _levelController.UpdateHealth();
    }

    public void IncreaseMaxSpeed(float addSpeed)
    {
        _characterSpeed += addSpeed;
    }

    public void DamagePlayer(int dmg)
    {
        _playerCurrentHealth -= dmg;
        _levelController.UpdateHealth();
        PlayerFlash();
    }

    public int GetCurrentElement()
    {
        return _currentElement;
    }

    public void PlayerFlash()
    {
        if (!_isFlashing)
        {
            AudioManager.Instance.PlaySFX(_takeDamageSFX, 1.5f);
            StartCoroutine(FlashOnDamage(_flashDuration));
        }
    }

    public void ResetPosition()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;     
        StartCoroutine(nameof(WaitForLevelLoad));
        this.transform.position = Vector3.zero;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    IEnumerator WaitForLevelLoad()
    {
        yield return new WaitForEndOfFrame();
    }

    IEnumerator FlashOnDamage(float duration)
    {
        _isFlashing = true;
        Material ogMat = this._spriteRender.material;
        _spriteRender.material = _flashColor;

        yield return new WaitForSeconds(duration);

        _isFlashing = false;
        _spriteRender.material = ogMat;
    }
}
