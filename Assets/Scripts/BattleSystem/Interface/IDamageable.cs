using UnityEngine;
using System.Collections;

public interface IDamagable 
{
    public int Health { get; } 
    public void SetHealth(int health);
    public void TakeDamage(int damage);
    public void Heal(int heal);
}



