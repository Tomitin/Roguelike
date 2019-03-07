using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {
    //This scripts checks if a Game Manager has been instantiated, and if not, instantiate one from our prefabs
    public GameObject gameManager;
	// Use this for initialization
	void Awake () {
        if (GameManager.instance == null)
            Instantiate(gameManager);
		
	}

}
