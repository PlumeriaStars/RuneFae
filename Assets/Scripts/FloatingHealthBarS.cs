using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBarS : MonoBehaviour
{
    [SerializeField] Slider _enemyHealthBar = null;
    [SerializeField] Color _highHealth;
    [SerializeField] Color _lowHealth;
    [SerializeField] Vector3 _healthSliderOffset;
    [SerializeField] ShadowEnemy _enemyController;
    [SerializeField] Camera _cinemaCamera;

    private void Start()
    {
        _enemyHealthBar.maxValue = _enemyController.gameObject.GetComponent<ShadowEnemy>().GetEnemeyHealth();
    }

    // Update is called once per frame
    void Update()
    {
        _enemyHealthBar.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + _healthSliderOffset);
        _enemyHealthBar.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(_lowHealth, _highHealth, _enemyHealthBar.normalizedValue);

        if (_enemyHealthBar.value <= 0)
            _enemyHealthBar.enabled = false;
    }

    public void UpdateEnemyHealth()
    {
        _enemyHealthBar.value = _enemyController.gameObject.GetComponent<ShadowEnemy>().GetEnemeyHealth();
    }
}
