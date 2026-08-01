using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

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
    private float _playerHealth = 100;
    private Vector2 _playerThrust = new Vector2(0, -1);
    
    private Marker2D _bulletMarker;
    private Area2D _renderingArea;
    
    private GpuParticles2D _thrusterParticles;

    private int _thrusterParticlesAmount = 1;
    
    private int thrusterParticlesAmount
    {
        get => _thrusterParticlesAmount;
        set => _thrusterParticlesAmount = Mathf.Clamp(value, 1, 55);
    }
    
    /* METHOD DECLARATIONS */
    public override void _Ready()
    {
        AddToGroup("player");
        GameManager.Instance.player = this;
        
        _bulletMarker = GetNode<Marker2D>("BulletMarker");
        _renderingArea = GetNode<Area2D>("RenderingCheck");
        _thrusterParticles = GetNode<GpuParticles2D>("ThrusterParticles");
        
        EventBus.Instance.PlayerShoot += OnPlayerShoot;
        EventBus.Instance.DamageTaken += PlayerDamaged;
        _renderingArea.BodyExited += OnBodyExited;

        _thrusterParticles.Emitting = false;
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

    
    // TODO --> TIGHTER TURNING AND DECELERATION & FIXING EFFECTS
    private void PlayerMovement()
    {
        float playerRotation = Input.GetAxis("left", "right");
        ApplyTorque(playerRotation * _playerTorque);
        
        // FORWARD
        if (Input.IsActionPressed("up"))
        {
            Vector2 forwardForce = _playerThrust * _playerSpeed; // DOWN IS UP
            ApplyForce(forwardForce.Rotated(Rotation));

            _thrusterParticles.Emitting = true;
            _thrusterParticles.Amount = thrusterParticlesAmount++;
        }

        if (Input.IsActionJustReleased("up"))
        {
            thrusterParticlesAmount--;
            _thrusterParticles.Amount = thrusterParticlesAmount;
            _thrusterParticles.Emitting = false;
        }

        // BACKWARD
        if (Input.IsActionPressed("down"))
        {
            Vector2 backwardForce = -1 * _playerThrust * _playerSpeed;
            ApplyForce(backwardForce.Rotated(Rotation));

            _thrusterParticles.Explosiveness = 5f;
            _thrusterParticles.Emitting = true;
            _thrusterParticles.Amount = thrusterParticlesAmount++;
        }

        if (Input.IsActionJustReleased("down"))
        {
            _thrusterParticles.Explosiveness = 0f;
            thrusterParticlesAmount--;
            _thrusterParticles.Amount = thrusterParticlesAmount;
            _thrusterParticles.Emitting = false;
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
    
    
    private void PlayerDamaged(float damage)
    {
        _playerHealth -= damage;
        GD.Print("DAMAGE TAKEN");
    }
}
