using Godot;

public partial class EnemyBullet : Bullet
{
    /* VARIABLE DECLARATION */
    private GpuParticles2D _impactParticles;
    
    /* METHOD DECLARATIONS */
    public override void _Ready()
    {
        AddToGroup("enemy_bullet");
        _impactParticles = GetNode<GpuParticles2D>("ImpactParticles");
        
    }


    // TODO --> FIX PLAYER DAMAGE HANDLING
    new public void OnBodyEntered(Node body)
    {
        _impactParticles.Emitting = true;
        
        if (body.IsInGroup("player"))
        {
            despawnTimer.Start();
            EventBus.Instance.EmitSignal(EventBus.SignalName.DamageTaken, bulletDamage);
        }
    }
    
}
