using System.Collections.Generic;
using UnityEngine;

public class MortarBall : MonoBehaviour
{
    public Transform towerPos;
    public Walker targetedEnemy;
    public GameObject mortarBall;
    private GameObject shadow;
    public float attackDamage = 10f;
    public float flightDuration = 0.8f;
    public float arcHeight = 2.5f;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 shadowPos;

    private float flightTime = 0f;
    private bool flying = true;

    void Start()
    {
        startPoint = towerPos.position + new Vector3(0f, 0.5f, 0); 
        endPoint = targetedEnemy.transform.position;

        transform.position = startPoint;
        shadowPos = startPoint;
        transform.localScale = new Vector3(1.5f, 1.5f, 0);
        CreateShadow();
    }

    void Update()
    {
        UpdateShadowPos();
        if (!flying) return;

        flightTime += Time.deltaTime;
        float t = Mathf.Clamp01(flightTime / flightDuration);

        Vector3 currentPos = Vector3.Lerp(startPoint, endPoint, t);

        float height = arcHeight * 4 * (t - t * t);

        currentPos.y += height;
        shadowPos = new Vector3(currentPos.x, currentPos.y - height, 1);
        
        transform.position = currentPos;

        if (t < 1f)
        {
            Vector3 direction = (currentPos - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
        }

        // hit  
        if (t >= 1f)
        {
            Vector3 explosionPosition = transform.position;
            float radius = 1.5f;
            LayerMask enemyMask = LayerMask.GetMask("Enemy");
            List<GameObject> enemiesNearby = GetObjectsInRadius(explosionPosition, radius, enemyMask);
            foreach (GameObject enemy in enemiesNearby)
            {
                if (enemy != null)
                {
                    Walker enemyScript = enemy.GetComponent<Walker>();
                    if (enemyScript != null)
                    {
                        enemyScript.health -= attackDamage;
                    }
                }
            }

            flying = false;
            CreateExplosion();
            if (shadow != null)
            {
                Destroy(shadow);
            }
            Destroy(gameObject, 0.1f); // Small delay to ensure explosion is created
        }
    }

    public List<GameObject> GetObjectsInRadius(Vector3 center, float radius, LayerMask layerMask)
    {
        List<GameObject> objectsInRange = new List<GameObject>();
        
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius, layerMask);

        foreach (Collider2D col in hitColliders)
        {
            objectsInRange.Add(col.gameObject);

        }

        return objectsInRange;
    }

    private void CreateShadow()
    {
        shadow = new GameObject("shadow");
        shadow.transform.position = transform.position;
        shadow.layer = 9;
        shadow.transform.SetParent(transform);
        SpriteRenderer sr = shadow.AddComponent<SpriteRenderer>();
        sr.sprite = level_settings.Instance.enemySettings.Shadow.sprite;
        sr.color = level_settings.Instance.enemySettings.Shadow.color;
    }

    private void UpdateShadowPos()
    {
        shadow.transform.position = shadowPos;
    }

    public void CreateExplosion()
    {
        if (this == null || gameObject == null) return; // Safety check

        GameObject explosionGO = new GameObject("ExplosionParticles");
        explosionGO.transform.position = transform != null ? 
            new Vector3(transform.position.x, transform.position.y, -1f) : 
            Vector3.zero;
        explosionGO.layer = 9;

        ParticleSystem ps = explosionGO.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.duration = 0.5f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.5f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 6f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.7f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Gradient
            {
                colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(new Color(1f, 0.4f, 0f), 0f),
                    new GradientColorKey(Color.yellow, 0.5f),
                    new GradientColorKey(Color.white, 1f)
                },
                alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            });
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 2f;
        main.maxParticles = 100;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 50)
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        // Set all velocity curves to use the same mode (Curve)
        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        
        AnimationCurve xCurve = new AnimationCurve();
        xCurve.AddKey(0f, 0f);
        xCurve.AddKey(0.5f, 5f);
        xCurve.AddKey(1f, 0f);
        
        AnimationCurve yCurve = new AnimationCurve();
        yCurve.AddKey(0f, 2f);
        yCurve.AddKey(0.5f, 10f);
        yCurve.AddKey(1f, 2f);
        
        AnimationCurve zCurve = new AnimationCurve();
        zCurve.AddKey(0f, 0f);
        zCurve.AddKey(1f, 0f);
        
        velocity.x = new ParticleSystem.MinMaxCurve(1f, xCurve);
        velocity.y = new ParticleSystem.MinMaxCurve(1f, yCurve);
        velocity.z = new ParticleSystem.MinMaxCurve(1f, zCurve);  // Add z curve to match x and y mode

        // Rotation randomness
        var rotation = ps.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = new ParticleSystem.MinMaxCurve(-360f, 360f);

        // Fade out
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient fade = new Gradient();
        fade.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fade);

        // Shrink over time
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve shrink = new AnimationCurve();
        shrink.AddKey(0f, 1.2f);
        shrink.AddKey(1f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, shrink);

        // Dampen speed
        var limit = ps.limitVelocityOverLifetime;
        limit.enabled = true;
        limit.dampen = 0.4f;
        limit.limit = 7f;

        // Setup material
        Material explosionMat = new Material(Shader.Find("Particles/Standard Unlit"));
        explosionMat.SetFloat("_Mode", 2f);
        explosionMat.SetFloat("_BlendOp", (float)UnityEngine.Rendering.BlendOp.Add);
        explosionMat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        explosionMat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.One);
        explosionMat.SetFloat("_ZWrite", 0f);
        explosionMat.DisableKeyword("_ALPHATEST_ON");
        explosionMat.EnableKeyword("_ALPHABLEND_ON");
        explosionMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        explosionMat.renderQueue = 3000;

        var psr = explosionGO.GetComponent<ParticleSystemRenderer>();
        if (psr != null)
        {
            psr.material = explosionMat;
            psr.sortingLayerName = "Enemy";
            psr.sortingOrder = 100;
            psr.renderMode = ParticleSystemRenderMode.Billboard;
        }

        ps.Play();
        
        // Make sure to destroy both the GameObject and the material
        float totalDuration = main.duration + main.startLifetime.constantMax;
        Destroy(explosionMat, totalDuration);
        Destroy(explosionGO, totalDuration);
    }
}