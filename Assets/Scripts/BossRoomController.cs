using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossRoomController : MonoBehaviour
{
    [SerializeField] Transform _playerController = null;
    private void Awake()
    {
        Transform _spawnPoint = GameObject.FindGameObjectWithTag("Respawn").GetComponent<Transform>();
        _playerController = FindObjectOfType<PlayerBehavior>().transform;
        _playerController.position = _spawnPoint.position; 

       // CinemachineVirtualCamera vCam = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        //vCam.m_Follow = null;
    }
}
