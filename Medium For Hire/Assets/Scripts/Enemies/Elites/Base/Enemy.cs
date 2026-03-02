//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Enemy : MonoBehaviour, IMoveable, ITriggerCheckable
//{
//    [field: SerializeField] public float MaxHealth { get; set; }
//    public float CurrentHealth { get; set; }
//    public Rigidbody2D RB { get ; set; }
//    public bool isFacingRight { get; set; } = true;

//    public SpriteRenderer SpriteRenderer;

//    // STATES
//    public EnemyStateMachine StateMachine { get; set; }
//    public ApproachState ApproachState { get; set; }
//    public LureState LureState { get; set; }
//    public AttackState AttackState { get; set; }
//    public bool IsAggroed { get; set; }
//    public bool IsWithinStrikingDistance { get ; set; }

//    // TRIGGER CHECKS


//    private void Awake()
//    {
//        StateMachine = new EnemyStateMachine();
//        ApproachState = new ApproachState(this, StateMachine);
//        LureState = new LureState(this, StateMachine);
//        AttackState = new AttackState(this, StateMachine);
//    }

//    private void Start()
//    {
//        CurrentHealth = MaxHealth;
//        RB = GetComponent<Rigidbody2D>();
//        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();

//        StateMachine.Initialize(ApproachState);
//    }

//    private void Update()
//    {
//        StateMachine.CurrentState.FrameUpdateState();
//    }

//    private void FixedUpdate()
//    {
//        StateMachine.CurrentState.PhysicsUpdateState();
//    }

//    // DAMAGE
//    public void Damage(float damage)
//    {
//        CurrentHealth -= damage;
//        if (CurrentHealth <= 0)
//        {
//            Die();
//        }
//    }

//    // DIE
//    public void Die()
//    {
//        Destroy(gameObject);
//    }

//    // MOVEMENT
//    public void Move(Vector2 velocity)
//    {
//        RB.velocity = velocity;
//        Flip(velocity);
//    }

//    // FLIP
//    public void Flip(Vector2 velocity)
//    {
//        if (isFacingRight && velocity.x < 0)
//        {
//            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
//            transform.rotation = Quaternion.Euler(rotator);
//            isFacingRight = false;

//        }
//        else if (!isFacingRight && velocity.x > 0)
//        {
//            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
//            transform.rotation = Quaternion.Euler(rotator);
//            isFacingRight = true;
//        }
//    }

//    // TRIGGER CHECKS
//    public void SetAggroStatus(bool isAggroed)
//    {
//        IsAggroed = isAggroed;
//    }
//    public void SetStrikingDistanceBool(bool isWithinStrikingDistance)
//    {
//        IsWithinStrikingDistance = isWithinStrikingDistance;
//    }

//    // ANIMATION TRIGGER
//    private void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
//    {
//        StateMachine.CurrentState.AnimationTriggerEvent(triggerType);
//    }

//    public enum AnimationTriggerType
//    {
//        Attack,
//        Lure,
//        Approach
//    }
//}
