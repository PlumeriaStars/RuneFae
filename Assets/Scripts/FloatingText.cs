using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    [SerializeField] Text _shrineText;
    [SerializeField] Vector3 _textOffset;
    [SerializeField] ShrineController _shrineController;
    [SerializeField] Camera _cinemaCamera;

    // Update is called once per frame
    void Update()
    {
        _shrineText.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + _textOffset);
    }
}
