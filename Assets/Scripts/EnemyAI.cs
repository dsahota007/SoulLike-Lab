using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float chaseRange = 10f;
    public float attackRange = 2f;
    public float chaseSpeed = 3f;
    public float timeBetweenAttacks = 2f;

    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            animator.SetBool("isMoving", false);
            if (!isAttacking)
            {
                StartCoroutine(AttackPlayer());
            }
        }
        else if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            animator.SetBool("isMoving", false);
            Idle();
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        animator.SetBool("isMoving", true);
    }

    void Idle()
    {
        animator.SetBool("isMoving", false);
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;

        int randomAttack = Random.Range(0, 4);

        switch (randomAttack)
        {
            case 0:
                animator.SetTrigger("jumpAttack");
                //Debug.Log("Enemy performs jump attack.");
                break;
            case 1:
                animator.SetTrigger("jumpKick");
                //Debug.Log("Enemy performs jump kick.");
                break;
            case 2:
                animator.SetTrigger("stomp");
                //Debug.Log("Enemy performs stomp.");
                break;
            case 3:
                animator.SetTrigger("swipeAttack");
                //Debug.Log("Enemy performs swipe attack.");
                break;
        }

        // Wait before allowing the next attack
        yield return new WaitForSeconds(timeBetweenAttacks);
        isAttacking = false;
    }
}
