using UnityEngine;
using System.Collections;

// ProjectileBehavior.cs
public abstract class ProjectileBehavior : MonoBehaviour
{
    protected Bullet bullet;
    
    public virtual void Initialize(Bullet bullet)
    {
        this.bullet = bullet;
    }

    public virtual void UpdateBehavior()
    {
        // Base behavior
    }
}

