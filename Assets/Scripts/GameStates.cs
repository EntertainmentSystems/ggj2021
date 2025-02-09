﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStates : MonoBehaviour
{
    public bool debug = false;
    
    public static GameStates current;

    [SerializeField] private GameObject[] phaseGroups;
    private int groupIndex;
    
    //How much the high decays?
    [SerializeField] private float changeDecayStep = .01f;
    //How often the high decays
    [SerializeField] private float changeSpeed = 20f;

    public int PlayerLives;

    //Clamp high value to [0, 1]
    private float _high;
    public float High
    {
        get => _high;
        set
        {
            _high = value < 0 ? 0 : value > _targetHigh ? _targetHigh : value;
            if(debug)
                Debug.Log("Target high: " + _targetHigh + ", " + "Current high: " + _high);
        }
    }
    
    private float _targetHigh;
    public float TargetHigh
    {
        get => _targetHigh;
        set => _targetHigh = value < 0 ? 0 : value > 1 ? 1 : value;
    }

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        High = 0;
        PlayerLives = 3;
        groupIndex = 0;
        GameEvents.current.PillPicked += OnPillPicked;
        GameEvents.current.PlayerAttacked += OnPlayerAttacked;

        foreach (var group in phaseGroups)
        {
            group.gameObject.SetActive(false);
        }
            
    }

    private void OnPlayerAttacked(int id)
    {
        if (--PlayerLives > 0)
        {
            GameEvents.current.OnPlayerLostLife(PlayerLives);
            phaseGroups[groupIndex++].gameObject.SetActive(true);
        }
        else
        {
            GameEvents.current.OnPlayerKilled(id);
        }
    }


    private void Update()
    {    
        //Target decays decay val per second
        float decay = Time.deltaTime * changeDecayStep;
        TargetHigh -= decay;
        
        //If the difference between target and current hight is bigger than tolerance, update current high
        if (TargetHigh - High > .01f)
        {
            High += changeDecayStep * changeSpeed * Time.deltaTime;
        }
        else
        {            
            High -= decay;
        }
    }

    private void OnPillPicked(float highValue)
    {
        TargetHigh += highValue;
    }
}
