using Godot;
using System;

// TODO --> CODE REFACTOR OF BULLET AS A CLASS WITH SUBCLASSES
public partial class Bullet : RigidBody2D
{
    /* VARIABLE DECLARATION */
    private GpuParticles2D _impactParticle;
    private GpuParticles2D _muzzleParticle;
    
    public Timer despawnTimer;
    
    /* METHOD DECLARATIONS */
    public override void _Ready()
    {
        AddToGroup("bullet");
        
        _impactParticle = GetNode<ImpactParticle>("ImpactParticle");
        _muzzleParticle = GetNode<GpuParticles2D>("MuzzleParticle");
        
        despawnTimer = GetNode<Timer>("DespawnTimer");
        
        BodyEntered += OnBodyEntered;
        despawnTimer.Timeout += OnTimerTimeout;
    }


    public void OnBodyEntered(Node body)
    {
        _muzzleParticle.Emitting = true;

        if (body.IsInGroup("enemy"))
        {
            var enemyBody = body as EnemyController;
            _impactParticle.Emitting = true;
            enemyBody.EnemyDamaged();
            enemyBody.CallDeferred(MethodName.QueueFree);
            despawnTimer.Start();
        }

        if (body.IsInGroup("asteroid"))
        {
            var ast = body as Asteroid;
            _impactParticle.Amount *= 5;
            _impactParticle.Emitting = true;
            ast.CallDeferred(MethodName.QueueFree);
            EventBus.Instance.EmitSignal(EventBus.SignalName.PlayerHealed, ast.healingAmount * ast._scaleAmount);
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
