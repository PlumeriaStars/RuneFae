using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterPlacement : MonoBehaviour
{
    [SerializeField] Transform[] _spawnPoints;
    [SerializeField] GameObject _teleporter;
    // Start is called before the first frame update
    void Start()
    {
        int num = Random.Range(0, 3);

        Instantiate(_teleporter, _spawnPoints[num].transform.position, Quaternion.Euler(0f, 0f, 0f));
    }
}
