using UnityEngine;
using System.Collections.Generic;


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
    public float maxhealht;
    public float health = 2f;
    private Animator player_animator;
    private float nextAttackTime = 0f;
    private float attackDamage;
    private float attackSpeed;
    public float priority = 0f;
    private bool hasStartedAttack = false;
    private bool hasDealtFirstHit = false;
    private float firstHitTime = 0f;
    private float droppedMoney;
    private float searchRadius = 5f;
    private Vector3 mousePos;
    public GameObject barOutline;
    public GameObject barFill;
    private EnemyType enemyType;
    private gameloop gameloopInstance;
    private GameObject shadow;
    private Vector3 shadowOffset = new Vector3(0, -0.7f, 0);

    
    public void Initialize(EnemyType type)
    {
        enemyType = type;
        health = type.health;
        attackDamage = type.attackDamage;
        attackSpeed = type.attackSpeed;
        speed = type.moveSpeed;
        spriteScale = type.scale;
        destroyDelay = type.destroyDelay;
        droppedMoney = type.droppedMoney;
        
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = type.sprite;
        }
        if (type.animatorController != null)
        {
            animator.runtimeAnimatorController = type.animatorController;
        }
    }

    void Start()
    {
        gameloopInstance = FindFirstObjectByType<gameloop>();
        transform.position = waypoints[0];
        baseScale = new Vector3(spriteScale, spriteScale, 1);
        transform.localScale = baseScale;
        animator = GetComponent<Animator>();
        if (!level_settings.Instance.playerSettings.playerDeath){
            player = GameObject.Find("Player");
            player_animator = player.GetComponent<Animator>();
        }
        gameloop.AddWalker(this);
        barOutline.transform.position = transform.position;
        barFill = barOutline.transform.GetChild(0).gameObject;
        maxhealht = health;
        CreateShadow();
    }

    void OnDestroy()
    {
        
        gameloop.RemoveWalker(this);
        RestoreIdleAnimation();
    }
    void Update()
    {
        if (health <= 0)
        {
            if (!toBeDestroyed)
            {
                toBeDestroyed = true;
                animator.SetTrigger("Death");
                gameloopInstance.money += droppedMoney;
            }
        }

        barOutline.transform.position = transform.position + new Vector3(0,1,0);
        UpdateHealthBar();
        ShowHealthBar();
    }
    void FixedUpdate()
    {
        if (toBeDestroyed)
        {
            Destroy(barOutline);
            if (animator != null)
            {
                animator.SetTrigger("Death");
                isWalking = false;
            }
            Destroy(gameObject, destroyDelay);
            return;
        }

        if (toAttack && animator != null)
        {
            animator.SetTrigger("Attack");
            if (player != null)
            {
                FaceTarget(player.transform.position);
            }

            if (!level_settings.Instance.playerSettings.playerDeath && Time.time >= nextAttackTime)
            {
                if (!hasDealtFirstHit)
                {
                    DealDamage();
                    hasDealtFirstHit = true;
                }
                else
                {
                    DealDamage();
                }
                nextAttackTime = Time.time + attackSpeed;
            }
            isWalking = false;
        }
        else
        {
            hasStartedAttack = false;
            hasDealtFirstHit = false;
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
            FaceTarget(target);
        }
        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(target.x + xOffset, target.y, -1);
        transform.position = Vector3.MoveTowards(currentPos, targetPos, speed * Time.deltaTime);
        float distance = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(target.x + xOffset, target.y)
        );
        // aby ratalo vydialenost prejdenu nie cas ktory zije
        priority += speed*Time.deltaTime;

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

        UpdateShadowPos();
    }

    private void DealDamage()
    {
        if (!toBeDestroyed){
        level_settings.Instance.playerSettings.health -= attackDamage;
        float healthPercent = Mathf.Max(0, level_settings.Instance.playerSettings.health / 
                                         level_settings.Instance.playerSettings.maxHealth);
        player_animator.Play("Player_hurt");
        gameloop.Instance.UpdateHealthBar(healthPercent);
        nextAttackTime = Time.time + attackSpeed;
        }
    }

    private void RestoreIdleAnimation()
    {
        if (player_animator == null || level_settings.Instance.playerSettings.playerDeath)
            return;

        if (!gameloop.IsAnyWalkerAttacking())
        {
            player_animator.Play("Player_idle");
        }
    }

    private void FaceTarget(Vector3 target)
    {
        float direction = target.x - transform.position.x;
        if (Mathf.Abs(direction) > 0.01f)
        {
            lastDirection = Mathf.Sign(direction);
            baseScale.x = Mathf.Abs(spriteScale) * lastDirection;
            transform.localScale = baseScale;
        }
    }

    private void ShowHealthBar()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float dist = Vector2.Distance(mousePos, transform.position);
        if (dist <= searchRadius)
        {
            barOutline.SetActive(true);
            
        }
        else
        {
            barOutline.SetActive(false);
        }
    }

    private void UpdateHealthBar()
    {
        barFill.transform.localPosition = new Vector3(0 - (maxhealht- health) * 0.45f / maxhealht, 0, 0);
        barFill.transform.localScale = new Vector3(0.9f  - 0.9f/maxhealht * (maxhealht- health), 0.7f, 1);

    }
    private void CreateShadow()
    {
        shadow = new GameObject("shadow");
        shadow.transform.position = transform.position + shadowOffset;
        shadow.layer = 9;
        shadow.transform.SetParent(transform);
        SpriteRenderer sr = shadow.AddComponent<SpriteRenderer>();
        sr.sprite = level_settings.Instance.enemySettings.Shadow.sprite;
        sr.color = level_settings.Instance.enemySettings.Shadow.color;
    }

    private void UpdateShadowPos()
    {
        shadow.transform.position = transform.position + shadowOffset;
    }
}
