using Godot;
using System.Collections.Generic;
using Vector2 = Godot.Vector2;

/*
 * TODO --> Character Movement i.e. Rotate LEFT, RIGHT and thrust Forward
 * TODO --> Character Projectiles, Extensible? Projectile as its own class?
 * TODO --> Rudimentary UI to see data handling and basic FX
 */
public partial class PlayerController : RigidBody2D
{   
    /* VARIABLE DECLARATION */
    private float _playerSpeed = 75;
    private float _playerTorque = 200;
    private float _playerBulletSpeed = 150;
    
    
    private float _playerHealth = 100;
    public float PlayerHealth
    {
        get => _playerHealth;
        set => _playerHealth = Mathf.Clamp(value, 0, 100);
    }
    
    
    private float _playerScore = 0;

    private Vector2 _playerThrust = new Vector2(0, -1);

    private TextureProgressBar _healthBar;
    private Label _scoreBar;
    
    private Marker2D _bulletMarker;
    public Area2D _renderingArea;
    public Camera2D _playerCamera;
    
    private GpuParticles2D _thrusterParticles;
    private int _thrusterParticlesAmount = 1;

    public List<EnemyController> enemyCount;
    
    public int thrusterParticlesAmount
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
        _playerCamera = GetNode<Camera2D>("PlayerCamera");
        _thrusterParticles = GetNode<GpuParticles2D>("ThrusterParticles");
        _healthBar = GetNode<TextureProgressBar>("PlayerUI/TextureProgressBar");
        _scoreBar = GetNode<Label>("PlayerUI/Label");
        
        EventBus.Instance.PlayerShoot += OnPlayerShoot;
        EventBus.Instance.DamageTaken += OnPlayerDamaged;
        EventBus.Instance.PlayerHealed += OnPlayerHealed;
        EventBus.Instance.ScoreUpdated += OnScoreUpdated;
        
        _renderingArea.BodyExited += OnBodyExited;

        _thrusterParticles.Emitting = false;
        _healthBar.Value = _playerHealth;
        _scoreBar.Text = _playerScore.ToString();
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
            Vector2 forwardForce = _playerThrust * _playerSpeed * -1;
            ApplyForce(forwardForce.Rotated(Rotation));
            
            thrusterParticlesAmount--;
            _thrusterParticles.Amount = thrusterParticlesAmount;
            _thrusterParticles.Emitting = false;
        }

        // BACKWARD
        if (Input.IsActionPressed("down"))
        {
            Vector2 backwardForce = -0.5f * _playerThrust * _playerSpeed;
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
        if (body.IsInGroup("bullet") || body.IsInGroup("enemy_bullet"))
        {
            body.QueueFree();
        }
    }
    
    
    private void OnPlayerDamaged(float damage)
    {
        PlayerHealth -= damage;
        _healthBar.Value = _playerHealth;

        if (PlayerHealth <= 0)
        {
            Godot.GD.Print("PLAYER DEAD");
        }
    }


    private void OnPlayerHealed(float health)
    {
        PlayerHealth += health;
        _healthBar.Value = _playerHealth;
    }
    

    private void OnScoreUpdated(float value)
    {
        _playerScore += value;
        _scoreBar.Text = _playerScore.ToString();
    }
}
