using Godot;
using System;

public partial class EnemyController : RigidBody2D
{
    /*  VARIABLE DECLARATION */
    private float _enemySpeed = 25;
    private float _enemyTorque = 45;
    private Vector2 _enemyThrust = new Vector2(1, 0);
    private float _enemyBulletSpeed = 75;

    public float enemyHealth = 100;
    
    private Marker2D _bulletMarker;
    private RayCast2D _bulletPath;
    private Area2D _patrollingArea;
    private Timer _attackTimer;
    private GpuParticles2D _thrusterParticles;
    
    private int _thrusterParticlesAmount = 1;
    
    public int thrusterParticlesAmount
    {
        get => _thrusterParticlesAmount;
        set => _thrusterParticlesAmount = Mathf.Clamp(value, 1, 55);
    }
    
    private enum State
    {
        Idle,
        Attacking
    }
    
    private State _enemyState;
    
    /* METHOD DECLARATIONS */
    public override void _Ready()
    {
        AddToGroup("enemy");
        _enemyState = State.Idle;

        _bulletPath = GetNode<RayCast2D>("RayCast2D");
        _bulletMarker = GetNode<Marker2D>("BulletMarker");
        _patrollingArea = GetNode<Area2D>("PatrollingArea");
        _attackTimer = GetNode<Timer>("AttackTimer");
        _thrusterParticles = GetNode<GpuParticles2D>("ThrusterParticles");
        
        _patrollingArea.BodyEntered += OnBodyEntered;
        _patrollingArea.BodyExited += OnBodyExited;
        
        _attackTimer.Timeout += OnTimerTimeout;
    }


    public override void _PhysicsProcess(double delta)
    {
        if (_enemyState == State.Attacking)
        {
            LookAt(GameManager.Instance.player.GlobalPosition);
            
            _thrusterParticles.Emitting = true;
            _thrusterParticles.Amount = thrusterParticlesAmount--;
        }
        
        else if (_enemyState == State.Idle)
        {
            _thrusterParticles.Emitting = true;
            _thrusterParticles.Amount = thrusterParticlesAmount++;
            
            Vector2 forwardForce = _enemyThrust * _enemySpeed; // DOWN IS UP
            Vector2 targetPosition = GlobalPosition - GameManager.Instance.player.GlobalPosition;
            
            ApplyForce(forwardForce.Rotated(Rotation), targetPosition.Normalized());
        }
    }

    
    public void OnBodyEntered(Node body)
    {
        if (body.IsInGroup("player"))
        {
            _enemyState = State.Attacking;
            _attackTimer.Paused = false;
            _attackTimer.Start();
        }
    }


    public void OnBodyExited(Node body)
    {
        if (body.IsInGroup("player"))
        {
            _enemyState = State.Idle;
            _attackTimer.Paused = true;
        }
    }
    
    
    public void OnTimerTimeout()
    {
        EnemyAttack();
    }
    
    
    // TODO --> Shoot bullet to target position
    // NOTE --> IDK WHY USING OWNER BUGS THE ADD_CHILD OUT
    public void EnemyAttack()
    {
        var bulletScene = ResourceLoader.Load<PackedScene>("res://scenes/enemy_bullet.tscn").Instantiate() as EnemyBullet;
        GameManager.Instance.MainNode.AddChild(bulletScene);
        bulletScene.Transform = _bulletMarker.GlobalTransform;
        
        Vector2 forwardForce = _enemyThrust * _enemyBulletSpeed; // DOWN IS UP
        bulletScene.ApplyImpulse(forwardForce.Rotated(Rotation));
    }


    // TODO --> IMPROVE DAMAGE EFFECT
    public void EnemyDamaged()
    {
        GameManager.Instance.MainNode.enemiesRemaining--;
        EventBus.Instance.EmitSignal(EventBus.SignalName.ScoreUpdated, 50f);
        
        Tween damageTween = CreateTween();
        var sprite = GetChild(0) as Sprite2D;
        
        if (sprite != null)
        {
            damageTween.TweenProperty(sprite, "modulate", Colors.Red, 1.0f);
        }
    }
}
