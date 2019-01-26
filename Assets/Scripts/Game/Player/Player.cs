﻿using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour
    {
        public string PlayerName = null;
        public Health Health { get; private set; }

        public void Awake()
        {
            Health = GetComponentInChildren<Health>(true);
        }
    }
}