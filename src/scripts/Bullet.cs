using Godot;
using System;

// TODO --> CODE REFACTOR OF BULLET AS A CLASS WITH SUBCLASSES
public partial class Bullet : RigidBody2D
{
    /* VARIABLE DECLARATION */
    private GpuParticles2D _impactParticles;
    
    public Timer despawnTimer;

    public float bulletDamage = 10;
    
    /* METHOD DECLARATIONS */
    public override void _Ready()
    {
        AddToGroup("bullet");
        
        _impactParticles = GetNode<GpuParticles2D>("ImpactParticles");
        despawnTimer = GetNode<Timer>("DespawnTimer");
        
        BodyEntered += OnBodyEntered;
        despawnTimer.Timeout += OnTimerTimeout;
    }


    public void OnBodyEntered(Node body)
    {
        _impactParticles.Emitting = true;

        if (body.IsInGroup("enemy"))
        {
            despawnTimer.Start();
        }
    }

    
    public void OnTimerTimeout()
    {
        DestroyBullet();    
    }
    
    
    public void DestroyBullet()
    {
        CallDeferred(MethodName.QueueFree);
    }
    
}
