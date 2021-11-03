using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour {
    public GameObject player;

    private void Start() {
        Instantiate(player, transform.position, Quaternion.identity);
    }
}
