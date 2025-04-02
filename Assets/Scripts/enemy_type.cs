using UnityEngine;

[System.Serializable]
public class EnemyType
{
    public string enemyName;
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;
    public float health = 2f;
    public float attackDamage = 1f;
    public float attackSpeed = 0.8f;
    public float moveSpeed = 2f;
    public float scale = 0.25f;
    public float destroyDelay;

}