using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject player;             // Reference to the player GameObject assigned in Inspector
    private bool isAttacking = false;     // Flag to check if the enemy is in an attack animation
    private bool canHit = true;           // Flag to allow one hit per animation cycle

    private Animator animator;            // Reference to the Animator component

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Check if any attack animation is playing
        bool inAttackAnimation = animator.GetCurrentAnimatorStateInfo(0).IsName("jumpAttack") ||
                                 animator.GetCurrentAnimatorStateInfo(0).IsName("jumpKick") ||
                                 animator.GetCurrentAnimatorStateInfo(0).IsName("stomp") ||
                                 animator.GetCurrentAnimatorStateInfo(0).IsName("swipeAttack");

        // If the enemy is attacking and can hit, set isAttacking to true
        if (inAttackAnimation && canHit)
        {
            isAttacking = true;
        }
        else if (!inAttackAnimation) // Reset canHit when the animation ends
        {
            isAttacking = false;
            canHit = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player GameObject and the enemy is currently attacking
        if (other.gameObject == player && isAttacking && canHit)
        {
            Debug.Log("PLAYER HAS BEEN HIT!!!!!!!!!!!!!!!!");
            canHit = false; // Disable further hits until the next attack animation
        }
    }
}