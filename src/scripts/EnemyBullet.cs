using Godot;

public partial class EnemyBullet : Bullet
{
    /* VARIABLE DECLARATION */
    private ImpactParticle _impactParticle;
    private GpuParticles2D _muzzleParticle;

    new public float bulletDamage = 10;
    
    /* METHOD DECLARATIONS */
    public override void _Ready()
    {
        AddToGroup("enemy_bullet");
        
        _impactParticle = GetNode<ImpactParticle>("ImpactParticle");
        _muzzleParticle = GetNode<GpuParticles2D>("MuzzleParticle");
        
        despawnTimer = GetNode<Timer>("DespawnTimer");
        
        BodyEntered += OnBodyEntered;
        despawnTimer.Timeout += OnTimerTimeout;
    }


    // TODO --> FIX PLAYER DAMAGE HANDLING
    new public void OnBodyEntered(Node body)
    {
        _muzzleParticle.Emitting = true;
        
        if (body.IsInGroup("player"))
        {
            _impactParticle.Emitting = true;
            despawnTimer.Start();
            
            EventBus.Instance.EmitSignal(EventBus.SignalName.DamageTaken, bulletDamage);
        }
    }
    
    
    public void OnTimerTimeout()
    {
        DestroyBullet();
    }
    
}
