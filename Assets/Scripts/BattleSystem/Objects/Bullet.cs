// Bullet.cs

using System;
using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public int Damage { get; private set; }
    public float Speed { get; private set; }
    public Vector3 Direction { get; set; } // Changed to set-able
    public float LifeTime { get; private set; }
    public float InitialLifeTime { get; private set; }
    public bool IsAlive { get; private set; }

    private List<ProjectileBehavior> behaviors = new List<ProjectileBehavior>();
    private Quaternion targetRotation;
    
    public void Initialize(int damage, float speed, Vector3 direction, float lifeTime)
    {
        Damage = damage;
        Speed = speed;
        Direction = direction.normalized;
        LifeTime = lifeTime;
        InitialLifeTime = lifeTime;
        IsAlive = true;
        
        // Set initial rotation
        targetRotation = Quaternion.LookRotation(Direction) * Quaternion.Euler(0, -90, 0);
        transform.rotation = targetRotation;
    }

    public void AddBehavior<T>(T behavior) where T : ProjectileBehavior
    {
        var newBehavior = gameObject.AddComponent<T>();
        newBehavior.Initialize(this);
        behaviors.Add(newBehavior);
    }

    private void Update()
    {
        transform.position += Direction * Speed * Time.deltaTime;
    }

    public void Launch()
    {
        if (!IsAlive) return;

        // Update all behaviors
        foreach (var behavior in behaviors)
        {
            behavior.UpdateBehavior();
        }

        // Basic movement
        

        // Update lifetime
        if (LifeTime <= 0)
        {
            IsAlive = false;
            Destroy(gameObject);
        }
    }
}