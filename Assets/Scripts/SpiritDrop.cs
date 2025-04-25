using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritDrop : MonoBehaviour
{
    [Header("Drop Range")]
    [SerializeField] int _lowBoundNum;
    [SerializeField] int _highBoundNum;

    [Header("Other")]
    [SerializeField] float _rotateSpeed;
    [SerializeField] PlayerBehavior _playerController;
    [SerializeField] AudioClip _pickupSFX;

    private int _dropNumber;

    // Start is called before the first frame update
    void Start()
    {
        //Random int -> minInclusive, maxExclusive
        _dropNumber = Random.Range(_lowBoundNum, _highBoundNum + 1);
        _playerController = GameObject.FindObjectOfType<PlayerBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, _rotateSpeed, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerController.IncreaseSpiritCount(_dropNumber);
            AudioManager.Instance.PlaySFX(_pickupSFX, 1.5f);
            Destroy(gameObject);
        }

        if (collision.CompareTag("DeathPlane"))
            Destroy(gameObject);

    }

}
