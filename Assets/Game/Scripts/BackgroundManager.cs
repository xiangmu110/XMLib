﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMLib;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField]
    private List<SpriteRenderer> _backgrounds;

    [SerializeField]
    private float _scale = 0.1f;

    [SerializeField]
    private float[] _offsets;

    PlayerController _player;
    TargetController _target;

    private void Awake()
    {
        _offsets = new float[_backgrounds.Count];
    }

    private void Start()
    {
        _player = App.Make<PlayerController>();
        _target = App.Make<TargetController>();
    }

    private void Update()
    {
        Vector3 position = this.transform.position;
        float currentSpeed = _target.currentSpeed;
        this.transform.Translate(currentSpeed * Time.deltaTime, 0, 0);

        for (int i = 0; i < _backgrounds.Count; i++)
        {
            _offsets[i] += Time.deltaTime * currentSpeed * _scale * (i + 1);
            _backgrounds[i].material.SetFloat("_xOffset", _offsets[i]);
        }
    }

}