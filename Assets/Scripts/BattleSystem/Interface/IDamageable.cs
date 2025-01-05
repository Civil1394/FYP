using UnityEngine;
using System.Collections;

public interface IDamagable 
{
    public float Health { get; } 
    public void SetHealth(float health);
    public void TakeDamage(float damage);
    public void Heal(float heal);
}



