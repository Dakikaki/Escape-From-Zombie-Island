using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class UnitController : MonoBehaviour
{
    [Header("Unit Stats")]
    public int maxHealth = 1;
    [HideInInspector] public int currentHealth;

    [Header("Action Points")]
    public int maxAP = 5;
    [HideInInspector] public int currentAP;

    [Header("Unit Movement")]
    public float moveSpeed = 4.0f;

    public Tile currentTile { get; protected set; }
    protected bool isMoving = false;
    protected GridManager gridManager;

    public static event Action<int, int> OnAPChanged;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    protected virtual void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        StartCoroutine(InitializeUnitPosition());
    }

    private IEnumerator InitializeUnitPosition()
    {
        yield return new WaitForEndOfFrame();
        UpdateCurrentTile();
        if (currentTile == null)
        {
            Debug.LogError($"Unit '{gameObject.name}' could not find a tile at its starting position.", gameObject);
        }
    }

    public virtual void ResetAP()
    {
        currentAP = maxAP;
        InvokeOnAPChanged();
    }

    public virtual bool SpendAP(int amount)
    {
        if (currentAP >= amount)
        {
            currentAP -= amount;
            InvokeOnAPChanged();
            return true;
        }
        return false;
    }

    // Protected method to allow derived classes to invoke the static event
    protected void InvokeOnAPChanged()
    {
        OnAPChanged?.Invoke(currentAP, maxAP);
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected virtual IEnumerator MoveAlongPath(List<Tile> path)
    {
        isMoving = true;
        foreach (Tile targetTile in path)
        {
            Vector3 targetPosition = targetTile.transform.position;
            targetPosition.y = transform.position.y;
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPosition;
            currentTile = targetTile;
        }
        isMoving = false;
    }

    public void UpdateCurrentTile()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.2f);
        foreach (var collider in colliders)
        {
            Tile tile = collider.GetComponent<Tile>();
            if (tile != null)
            {
                currentTile = tile;
                return;
            }
        }
        currentTile = null;
    }
}