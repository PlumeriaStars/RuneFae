using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CinemaCamera : MonoBehaviour
{
    // Start is called before the first frame update
    private CinemachineVirtualCamera vCam; 
    private CinemachineConfiner vConfiner;
    private PolygonCollider2D _background;
    void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        _background = GameObject.FindGameObjectWithTag("BG").GetComponent<PolygonCollider2D>();
    }

    void Start()
    {
        vCam.PreviousStateIsValid = false;
        vCam.enabled = false; 

        if(GameObject.FindGameObjectWithTag("BossRoom") == false)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            vCam.transform.position = player.position;
            vCam.m_Follow = player;
        }
        else
        {
            Debug.Log("In Boss Room");
            vCam.m_Follow = GameObject.FindGameObjectWithTag("CameraFocus").transform;
            vCam.m_LookAt = GameObject.FindGameObjectWithTag("CameraFocus").transform;
        }
        
        vCam.enabled = true;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if(vCam.enabled)
            vCam.enabled = false;

        vCam.enabled = true; 
    }

}
