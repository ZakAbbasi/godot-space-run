using Godot;
using System;

/*
 * TODO --> Character Movement i.e. Rotate LEFT, RIGHT and thrust Forward
 * TODO --> Character Projectiles, Extensible? Projectile as its own class?
 * TODO --> Rudimentary UI to see data handling and basic FX
 */
public partial class PlayerController : RigidBody2D
{   
    /* VARIABLE DECLARATION */
    private float _playerSpeed = 25;
    private float _playerTorque = 45;
    private float _playerBulletSpeed = 75;
    
    private Vector2 _playerThrust = new Vector2(0, -1);
    
    private Marker2D _bulletMarker;
    private Area2D _renderingArea;
    private CpuParticles2D _thrusterParticles;
    
    /* METHOD DECLARATIONS */
    public override void _Ready()
    {
        _bulletMarker = GetNode<Marker2D>("BulletMarker");
        _renderingArea = GetNode<Area2D>("RenderingCheck");
        _thrusterParticles = GetNode<CpuParticles2D>("ThrusterParticles");
        
        EventBus.Instance.PlayerShoot += OnPlayerShoot;
        _renderingArea.BodyExited += OnBodyExited;
    }
    
    
    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        PlayerMovement();
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("shoot"))
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.PlayerShoot);
        }
    }

    
    // TODO --> TIGHTER TURNING AND DECELERATION 
    private void PlayerMovement()
    {
        float playerRotation = Input.GetAxis("left", "right");
        ApplyTorque(playerRotation * _playerTorque);

        if (Input.IsActionPressed("up"))
        {
            Vector2 forwardForce = _playerThrust * _playerSpeed; // DOWN IS UP
            ApplyForce(forwardForce.Rotated(Rotation));
        }
    }


    private void OnPlayerShoot()
    {
        var bulletScene = ResourceLoader.Load<PackedScene>("res://scenes/bullet.tscn").Instantiate() as Bullet;
        Owner.AddChild(bulletScene);
        bulletScene.Transform = _bulletMarker.GlobalTransform;
        
        Vector2 forwardForce = _playerThrust * _playerBulletSpeed; // DOWN IS UP
        bulletScene.ApplyImpulse(forwardForce.Rotated(Rotation));
    }


    private void OnBodyExited(Node2D body)
    {
        if (body.IsInGroup("bullet"))
        {
            body.QueueFree();
        }
    }
    
    
    private void PlayerDamaged()
    {
        
    }
}
