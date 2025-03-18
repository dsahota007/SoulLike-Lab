using UnityEngine;
using System.Collections;


public class PlayerAttack : MonoBehaviour
{
    private Animator animator;       // Reference to the Animator component
    private bool isAttacking;        // Flag to check if currently attacking

    void Start()
    {
        animator = GetComponent<Animator>();  // Get the Animator component on start
    }

    void Update()
    {
        HandleAttackInput();
    }

    private void HandleAttackInput()
    {
        // Only allow starting a new attack if not already in an attack state
        if (!isAttacking)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))  // Example: Press "1" to Punch
            {
                TriggerAttack("leftHook");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))  // Example: Press "2" to Kick
            {
                TriggerAttack("rightHook");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))  // Example: Press "3" for SwordSwing
            {
                TriggerAttack("rightHighKick");
            }
        }
    }

    private void TriggerAttack(string attackTrigger)
    {
        // Set the attack flag to true to prevent other attacks from interrupting
        isAttacking = true;

        // Trigger the attack animation
        animator.SetTrigger(attackTrigger);

        // Start the coroutine to reset the attack state once the animation completes
        StartCoroutine(ResetAttackFlag());
    }

    private IEnumerator ResetAttackFlag()
    {
        // Wait for the duration of the current animation before resetting the attack flag
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Reset the attack flag so another attack can be triggered
        isAttacking = false;
    }
}
