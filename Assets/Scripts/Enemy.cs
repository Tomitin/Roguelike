using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    public int playerDamage;


    private Animator animator;
    private Transform target;
    private bool skipMove;
    public AudioClip attackSound1;
    public AudioClip attackSound2;

    protected override void Start () {
        GameManager.instance.AddEnemyToList(this); // add itself to the list in game manager
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start(); //calls start function of moving object
	}
	

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon) //if the player and the enemy are in the same x, 
            yDir = target.position.y > transform.position.y ? 1 : -1; //move in y
        else //if the player and enemy are in the same y
            xDir = target.position.x > transform.position.x ? 1 : -1; //move in x

        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        hitPlayer.LoseFood(playerDamage);

        animator.SetTrigger("enemyAttack");

        SoundManager.instance.RandomizeSfx(attackSound1, attackSound2);
    }
}
