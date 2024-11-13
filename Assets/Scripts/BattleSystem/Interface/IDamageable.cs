using UnityEngine;
using System.Collections;

public interface IDamagable 
{
    public int Health { get; } 
    public void Damage(int damage);
}



