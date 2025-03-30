using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Walker : MonoBehaviour
{
    public float spriteScale = 0.25f;
    [Tooltip("Prvy waypoint je startovacia pozicia")]
    public List<Vector3> waypoints;
    private int currentWaypoint = 1; // prvy vykreslime na Start()
    public float speed;
    private Vector3 baseScale;
    private float xOffset = 0.5f;
    private bool isWalking = true;
    private Animator animator;
    private float lastDirection = 1f;
    private GameObject player;
    public bool toBeDestroyed = false;
    public bool toAttack = false;
    private float destroyDelay = 0.58f; 
    public float health = 2f;
    private Animator player_animator;
    private static List<Walker> activeWalkers = new List<Walker>();

    void Start()
    {
        transform.position = waypoints[0];
        baseScale = new Vector3(spriteScale, spriteScale, 1);
        transform.localScale = baseScale;
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        player_animator = player.GetComponent<Animator>();
        activeWalkers.Add(this);
    }

    void OnDestroy()
    {
        activeWalkers.Remove(this);
        RestoreIdleAnimation();
    }

    void Update()
    {
        // najskor riesime animaciu smrti az potom attack / move
        if (health <= 0)
        {
            toBeDestroyed = true;
        }

        if (toBeDestroyed)
        {
            if (animator != null)
            {
                animator.SetTrigger("Death");
                isWalking = false;
            }
            Destroy(gameObject, destroyDelay);
            return;
        }

        if (toAttack)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
                player_animator.SetTrigger("Hurt");
                isWalking = false;
            }
            // tu nesmie byt return, chceme pokracovat v update
        }

        if (waypoints.Count == 0 || !isWalking) return;
        
        Vector3 target = waypoints[currentWaypoint];
        Vector3 direction = target - transform.position;

        if (player != null && !toBeDestroyed)
        {
            float distanceToPlayer = Vector2.Distance(
                new Vector2(transform.position.x, transform.position.y),
                new Vector2(player.transform.position.x, player.transform.position.y)
            );
            // simple mechanika aby sme vedeli kedy prestat animovat player "Hurt" 
            if (distanceToPlayer < 1f)
            {
                if (!toAttack)
                {
                    toAttack = true;
                }
            }
            else if (toAttack)
            {
                toAttack = false;
                RestoreIdleAnimation();
            }
        }

        // pri poslednom checkpointe sa neotaca
        if (isWalking && Mathf.Abs(direction.x) > 0.01f && currentWaypoint < waypoints.Count -1 )
        {
            lastDirection = Mathf.Sign(direction.x); 
            baseScale.x = Mathf.Abs(spriteScale) * lastDirection;
            transform.localScale = baseScale;
        }
        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(target.x + xOffset, target.y, -1);
        transform.position = Vector3.MoveTowards(currentPos, targetPos, speed * Time.deltaTime);
        float distance = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(target.x + xOffset, target.y)
        );

        if (distance < 0.1f)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Count)
            {
                isWalking = false;
                transform.position = targetPos;
                if (animator != null)
                {
                    animator.enabled = false;
                }
                // toto zabezpeci, ze sa zachova posledny smer
                baseScale.x = Mathf.Abs(spriteScale) * lastDirection;
                transform.localScale = baseScale;
            }
        }
    }

    private void RestoreIdleAnimation()
    {
        // toto je velky goofy vec co som nasiel, ale funguje to
        bool anyEnemyAttacking = activeWalkers.Any(w => w != null && w.toAttack);
        if (!anyEnemyAttacking && player_animator != null)
        {
            player_animator.SetTrigger("Idle");
        }
    }
}
