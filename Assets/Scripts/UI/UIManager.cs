using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _lifeTimeText;
    private float _maxLifeTime;

    private void Awake()
    {
        Instance = this;
        SetLifeTime(0);
    }

    public void SetLifeTime(float value)
    {
        if (!(value > _maxLifeTime)) return;
        _maxLifeTime = value;
        _lifeTimeText.text = _maxLifeTime.ToString();
    }
}
