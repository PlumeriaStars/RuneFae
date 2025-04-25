using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject _healthObject = null;
    [SerializeField] GameObject _spiritObject = null;
    [SerializeField] GameObject _runeObject = null;
    [SerializeField] Slider _healthBar = null;
    [SerializeField] Text _spiritCount = null;
    [SerializeField] GameObject _specialRune = null;
    [SerializeField] Sprite[] _runeAbilities = null;

    [Header("Sounds")]
    [SerializeField] AudioClip _levelBGM;
    [SerializeField] AudioClip _bossBGM;
    [SerializeField] AudioClip _victoryBGM;

    private PlayerBehavior _player;
    private Transform _respawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindObjectOfType<PlayerBehavior>();
        //_respawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;
        _healthBar.maxValue = _player.GetComponent<PlayerBehavior>().GetStartingHealth();
        _healthBar.value = _healthBar.maxValue;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Instantiate(_player, _respawnPoint);
        AudioManager.Instance.PlaySong(_levelBGM);
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //exit level
        if (Input.GetKeyDown(KeyCode.Escape))
            ExitGame();

        if (Input.GetKeyDown(KeyCode.R))
            _player.RespawnPlayer();

        if (Input.GetKeyDown(KeyCode.B))
        {
           LoadBossRoom();     
        }
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    public void ReloadLevel()
    {
         StartCoroutine(LoadingNextRoom(1));
        Time.timeScale = 1;;
        AudioManager.Instance.PlaySong(_levelBGM);
        _healthBar.maxValue = _player.GetComponent<PlayerBehavior>().GetStartingHealth();
        _healthBar.value = _healthBar.maxValue;
        SceneManager.LoadScene(1);
    }

    public void LoadBossRoom()
    {
        AudioManager.Instance.PlaySong(_bossBGM);
        StartCoroutine(LoadingNextRoom(2));
        //_player.ResetPosition();    
    }

    IEnumerator LoadingNextRoom(int sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void LoadVictory()
    {
        AudioManager.Instance.PlaySong(_victoryBGM);
    }

    public void UpdateHealth()
    {
        _healthBar.value = _player.gameObject.GetComponent<PlayerBehavior>().GetHealth();
    }

    public void UpdateMaxHealth()
    {
        _healthBar.maxValue = _player.gameObject.GetComponent<PlayerBehavior>().GetMaxHealth();
    }

    public void UpdateSpiritCount()
    {
        _spiritCount.text = "" + _player.gameObject.GetComponent<PlayerBehavior>().GetSpiritCount();
    }
    
    public void ShowAbilityIcon(int abilNum)
    {
        _specialRune.SetActive(true);
        _specialRune.GetComponent<Image>().sprite = _runeAbilities[abilNum];
    }

    public void HideAbilityIcon()
    {
        _specialRune.SetActive(false);
    }

    public void SetOverlayAndPlayerVisibilty(bool visibilty)
    {
        _runeObject.SetActive(visibilty);
        _healthObject.SetActive(visibilty);
        _spiritObject.SetActive(visibilty);
        _player.GetComponent<SpriteRenderer>().enabled = visibilty;
        _player.GetComponent<CapsuleCollider2D>().enabled = visibilty;
    }
}
