using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.TextCore.Text;

public static class CharacterStateWorkflowsDataSheet
{
    public abstract class WorkflowBase : IWorkflow<State>
    {
        public abstract State ID { get; }

        public int Current => current;

        public virtual bool CanExecute => true;

        protected int current;

        protected CharacterMachine machine;
        protected Transform transform;
        protected Rigidbody2D rigidbody;
        protected CapsuleCollider2D[] colliders;
        protected Animator animator;
        protected bool hasFixedUpdatedAtVeryFirst;


        public WorkflowBase(CharacterMachine machine)
        {
            this.animator = machine.animator;
            this.machine = machine;
            this.transform = machine.transform;
            this.rigidbody = machine.GetComponent<Rigidbody2D>();
            this.colliders = machine.GetComponentsInChildren<CapsuleCollider2D>();
        }

        public virtual State OnUpdate()
        {
            return hasFixedUpdatedAtVeryFirst ? ID : State.None;
        }             
                        
        public virtual void OnFixdeUpdate()
        {
            if (hasFixedUpdatedAtVeryFirst == false)
            {
                hasFixedUpdatedAtVeryFirst = true;
            }
        }
        



        public void Reset()
        {
            current = 0;
        }

        public virtual void OnEnter(object[] parameters) 
        {
            hasFixedUpdatedAtVeryFirst = false;
            Reset();
        }
        public virtual void OnExit() {  }

    }
    public class Idle : WorkflowBase
    {
        public override State ID => State.Idle;
        public Idle(CharacterMachine machine) : base(machine)
        {
        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.hasJumped = false;
            machine.hasSecondJumped = false;
            machine.isDirectionChangeable = true;
            machine.isMovable = true;
            animator.Play("Idle");
            current++;

        }
        public override State OnUpdate()


        {
            State next = base.OnUpdate();

            if (next == State.None)
                return ID;

            switch (current)
            {
                
                default:
                    {
                        if (Mathf.Abs(machine.horizontal) > 0)
                            next = State.Move;
                        // todo -> X 축 입력 절댓값이 0보다 크면 next = State.Move

                        if (machine.isGrounded == false)
                            next = State.Fall;
                        // todo -> Ground 가 감지되지 않으면 next = State.Fall
                    }
                    break;
            }

            return next;
        }
    }


    public class Move : WorkflowBase
    {

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = true;
            machine.isMovable = true;
            animator.Play("Move");
            current++;
        }
        public override State ID => State.Move;
        public Move(CharacterMachine machine) : base(machine)
        {
        }
        public override State OnUpdate()
        {
            State next = base.OnUpdate();

            if (next == State.None)
                return ID;

            switch (current)
            {
                
                default:
                    {
                        if ((machine.horizontal) == 0.0f)

                            next = State.Idle;

                        // todo -> X 축 입력 절댓값이 0보다 크면 next = State.Move

                        if (machine.isGrounded == false)
                            next = State.Fall;
                        // todo -> Ground 가 감지되지 않으면 next = State.Fall
                    }
                    break;
            }

            return next;
        }
    }


    public class Jump : WorkflowBase
    {


        public override State ID => State.Jump;
        public override bool CanExecute => base.CanExecute &&
                                            machine.hasJumped == false &&
                                            (((machine.current == State.Idle || machine.current == State.Move) && machine.isGrounded) || 
                                            machine.current == State.LadderClimbing ||
                                            machine.current == State.Ledge ||
                                            machine.current == State.WallSlide);

        private float _force;

                
        public Jump(CharacterMachine machine, float force) : base(machine)
        {
            _force = force;
        }

        
        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.hasJumped = true;
            machine.isDirectionChangeable = true;
            machine.isMovable = false;
            rigidbody.velocity = machine.previous == State.LadderClimbing || machine.previous == State.Ledge || machine.previous == State.WallSlide ? 
                                new Vector2(machine.horizontal * machine.speed, 0.0f) : new Vector2(rigidbody.velocity.x, 0.0f);
            rigidbody.AddForce(Vector2.up * _force, ForceMode2D.Impulse);
            animator.Play("Jump");
            
        }
        public override State OnUpdate()
        {
            State next = base.OnUpdate();

            if (next == State.None)
                return ID;

            switch (current)
            {
               
                default:
                    {
                        if (rigidbody.velocity.y <= 0.0f)
                        {
                            next = machine.isGrounded ? State.Idle : State.Fall;
                        }
                    }
                    break;
            }

            return next;
        }
    }
    public class JumpDown : WorkflowBase
    {


        public override State ID => State.JumpDown;
        public override bool CanExecute => base.CanExecute &&
                                           machine.current == State.Crouch &&
                                           machine.isGroundExistBelow;

        private float _force;
        private float _groundIgnoreTime;
        private float _timeMark;
        private Collider2D _ground;

        public JumpDown(CharacterMachine machine,float force, float groundIgnoreTime) : base(machine)
        {
            _force = force;
            _groundIgnoreTime = groundIgnoreTime;
        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.hasJumped = true;
            machine.isDirectionChangeable = true;
            machine.isMovable = false;
            _ground = machine.ground;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0.0f);
            rigidbody.AddForce(Vector2.up * _force, ForceMode2D.Impulse);
            animator.Play("Jump");
            
        }
        public override void OnExit()
        {
            base.OnExit();
            for (int i = 0; i < colliders.Length; i++)
            {
                Physics2D.IgnoreCollision(colliders[i], _ground, false);
            }
        }

        public override State OnUpdate()
        {
            State next = base.OnUpdate();

            if (next == State.None)
                return ID;

            switch (current)
            {
                case 0:
                    {
                        for (int i = 0; i < colliders.Length; i++)
                        {
                            Physics2D.IgnoreCollision(colliders[i], _ground, true);
                        }
                        _timeMark = Time.time;
                        current++;
                    }
                    break;
                    case 1:
                    {
                        if (rigidbody.velocity.y <= 0)
                        {
                            animator.Play("Fall");
                            current++;
                        }
                    }
                    break;
                case 2:
                    {
                        if (Time.time - _timeMark > _groundIgnoreTime)
                        {
                            for (int i = 0; i < colliders.Length; i++)
                            {
                                Physics2D.IgnoreCollision(colliders[i], _ground, false);
                            }
                            current++;
                        }
                    }
                    break;
                default:
                    {
                        if (rigidbody.velocity.y <= 0.0f)
                        {
                            next = machine.isGrounded ? State.Idle : State.Fall;
                        }
                    }
                    break;
            }

            return next;
        }
    }

    public class SecondJump : WorkflowBase
    {


        public override State ID => State.SecondJump;
        public override bool CanExecute => base.CanExecute &&
                                            machine.hasSecondJumped == false && 
                                            (machine.current == State.Jump ||
                                             machine.current == State.Fall) &&
                                            machine.isGrounded == false;

        private float _force;

        public SecondJump(CharacterMachine machine, float force) : base(machine)
        {
            _force = force;
        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.hasJumped = true;
            machine.isDirectionChangeable = true;
            machine.isMovable = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = new Vector2(machine.horizontal * machine.speed, 0.0f);
            rigidbody.AddForce(Vector2.up * _force, ForceMode2D.Impulse);
            animator.Play("SecondJump");
        }
        public override State OnUpdate()
        {
            State next = base.OnUpdate();

            if (next == State.None)
                return ID;

            switch (current)
            {
                case 0:
                    {
                        machine.hasSecondJumped = true;
                        machine.isDirectionChangeable = true;
                        machine.isMovable = false;
                        rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0.0f);
                        rigidbody.AddForce(Vector2.up * _force, ForceMode2D.Impulse);
                        animator.Play("SecondJump");
                        current++;
                    }
                    break;
                default:
                    {
                        if (rigidbody.velocity.y <= 0.0f)
                        {
                            next = machine.isGrounded ? State.Idle : State.Fall;
                        }
                    }
                    break;
            }

            return next;
        }
    }
    public class Fall : WorkflowBase
    {


        public override State ID => State.Fall;
        private float _landingDistance;
        private float _startPosY;


        public Fall(CharacterMachine machine, float landingDistance) : base(machine)
        {
            _landingDistance = landingDistance;
        }


        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = true;
            machine.isMovable = false;
            _startPosY = rigidbody.position.y;
            animator.Play("Fall");
            current++;
        }
        public override State OnUpdate()
        {
            State next = base.OnUpdate();

            if (next == State.None)
                return ID;

            switch (current)
            {
                
                default:
                    {
                        if (machine.isGrounded)
                        {
                            next = (_startPosY - rigidbody.position.y) < _landingDistance ? State.Idle : State.Land;
                        }
                    }
                    break;
            }

            return next;
        }
    }

    public class Land : WorkflowBase
    {

        public override State ID => State.Land;
        public Land(CharacterMachine machine) : base(machine)
        {
        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = true;
            machine.isMovable = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = Vector2.zero;
            animator.Play("Land");
            current++;
        }
        public override State OnUpdate()
        {
            State next = base.OnUpdate();

            if (next == State.None)
                return ID;

            switch (current)
            {
               
                default:
                    {
                        // 현재 애니메이터의 재생중인 상태의 정보에서 일반화된 시간이 1.0f 이된다.
                        // == 현재 상태 애니메이션 클립 재생이 끝났다.
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            next = State.Idle;
                        }
                    }
                    break;
            }
            return next;
        }
    }

    public class Crouch : WorkflowBase
    {

        public override State ID => State.Crouch;

        public override bool CanExecute => base.CanExecute &&
                                            (machine.current == State.Idle ||
                                             machine.current == State.Move) &&
                                             machine.isGrounded;
        private Vector2 _offsetCrouched;
        private Vector2 _sizeCrouched;
        private Vector2 _offsetOrigin;
        private Vector2 _sizeOrigin;

        public Crouch(CharacterMachine machine, Vector2 offsetCrouched, Vector2 sizeCrouched) : base(machine)
        {
            _offsetCrouched = offsetCrouched;
            _sizeCrouched = sizeCrouched;
            _offsetOrigin = colliders[0].offset;
            _sizeOrigin = colliders[0].size;
        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = true;
            machine.isMovable = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = Vector2.zero;
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].offset = _offsetCrouched;
                colliders[i].size = _sizeCrouched;

            }
            animator.Play("Crouch");
        }
        public override void OnExit()
        {
            base.OnExit();
            
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].offset = _offsetOrigin;
                colliders[i].size = _sizeOrigin;
            }
            
        }
        public override State OnUpdate()
        {
            State next = base.OnUpdate();

            if (next == State.None)
                return ID;

            switch (current)
            {
                
                case 0:
                    {
                       
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            animator.Play("CrouchIdle");
                            current++;
                        }
                    }
                    break;
                default:
                    {
                        if (machine.isGrounded == false)
                        {
                            next = State.Fall;
                        }
                    }
                    break;
            }
            return next;
        }
    }

    public class LadderClimbing : WorkflowBase
    {

        public override State ID => State.LadderClimbing;

        public override bool CanExecute => base.CanExecute &&
                                           (machine.current == State.Idle ||
                                            machine.current == State.Move ||
                                            machine.current == State.Jump ||
                                            machine.current == State.Fall);
        private Ladder _ladder;
        private float _climbingSpeed;
        private float _vertical;
        public LadderClimbing(CharacterMachine machine, float climbingSpeed) : base(machine)
        {
            _climbingSpeed = climbingSpeed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters">0 : (Ladder), 1 : 위/아래(int) </param>

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = false;
            machine.isMovable = false;
            machine.hasJumped = false;
            machine.hasSecondJumped = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = Vector2.zero;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            animator.Play("LadderClimbing");
            animator.speed = 0.0f;
            _ladder = (Ladder)parameters[0];
            int toward = (int)parameters[1];
            if (toward > 0)
            {
                transform.position = transform.position.y > _ladder.upEndPos.y ?
                                     new Vector3(_ladder.upStartPos.x, transform.position.y) : _ladder.upStartPos;
            }
            else if (toward < 0)
            {
                transform.position = transform.position.y > _ladder.downEndPos.y ?
                                     new Vector3(_ladder.downStartPos.x, transform.position.y) : _ladder.downStartPos;
            }
            else
                throw new System.Exception($"[{machine.gameObject.name} - LadderClimbing] : toward wrong");
        }

        public override void OnExit()
        {
            base.OnExit();
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
            animator.speed = 1.0f;
        }
        public override State OnUpdate()
        {
            State next = base.OnUpdate();

            if (next == State.None)
                return ID;

            switch (current)
            {
                case 0:
                {
                        // nothing to do
                }
                    break;

                default:
                    {
                        if (machine.isGrounded)
                        {
                            next = State.Idle;
                        }
                        else if (transform.position.y > _ladder.upEndPos.y)
                        {
                            transform.position = _ladder.top;
                            next = State.Idle;
                        }
                        else if (transform.position.y < _ladder.downEndPos.y)
                        {
                            next = State.Idle;
                        }
                        else
                        {
                            _vertical = machine.vertical;
                        }
                    }
                    break;
            }
            return next;
        }

        public override void OnFixdeUpdate()
        {
            base.OnFixdeUpdate();
            switch (current)
            {
                case 0:
                    {
                        if (machine.isGrounded == false)
                            current++; // Wait 1 frame (그라운드 감지 센서 갱신될때까지);
                    }
                    break;
                default:
                    {
                        if (machine.isGrounded)
                        {
                            //nothing to do
                        }
                        else if (transform.position.y > _ladder.upEndPos.y)
                        {
                            //nothing to do

                        }
                        else if (transform.position.y < _ladder.downEndPos.y)
                        {
                            //nothing to do
                        }
                        else
                        {
                            animator.speed = Mathf.Abs(_vertical);
                            transform.position += Vector3.up * _vertical * _climbingSpeed * Time.fixedDeltaTime;
                        }
                    }
                    break;
            }
            
        }
    }
    public class Ledge : WorkflowBase
    {
        public Ledge(CharacterMachine machine) : base(machine)
        {
        }
        public override State ID => State.Ledge;
        public override bool CanExecute => base.CanExecute &&
                                            (machine.current == State.Jump ||
                                             machine.current == State.SecondJump ||
                                             machine.current == State.Fall);
        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = false;
            machine.isMovable = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = Vector2.zero;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            transform.position = machine.ledgePoint - new Vector2(machine.ledgeDetectOffset.x * machine.direction, machine.ledgeDetectOffset.y);
            animator.Play("LedgeStart");
            
        }

        public override void OnExit()
        {
            base.OnExit();
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
        
        
        public override State OnUpdate()
        {
            State next = base.OnUpdate();

            if (next == State.None)
                return ID;

            switch (current)
            {
                case 0:
                    {
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                            current++;
                    }
                     break;
                case 1:
                    {
                        animator.Play("LedgeIdle");
                        current++;
                    }
                    break;
                default:
                    {
                        // nothing to do,
                    }
                    break;

            }

            return next;
        }
    }

    public class LedgeClimb : WorkflowBase
    {
       
        public override State ID => State.LedgeClimb;
        public override bool CanExecute => base.CanExecute &&
                                            machine.current == State.Ledge;

        private Vector2 _different; // btw ledge & transform
        private Vector2 _differentRatio;
        private Vector2 _startPos;
        private float _clipLength;
        private float _timer;
         public LedgeClimb(CharacterMachine machine) : base(machine)
        {
        }                                    
        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = false;
            machine.isMovable = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = Vector2.zero;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _different = machine.ledgePoint - (Vector2)transform.position;
            Vector2 distance = new Vector2(Mathf.Abs(_different.x), Mathf.Abs(_different.y));
            _differentRatio = new Vector2(_different.y / (_different.x + _different.y), _different.x / (_different.x + _different.y));
            _startPos = transform.position;
            animator.Play("LedgeClimb");
            _clipLength = animator.GetCurrentAnimatorStateInfo(0).length;
            _timer = 0.0f;
            

        }

        public override void OnExit()
        {
            base.OnExit();
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }


        public override State OnUpdate()
        {
            State next = base.OnUpdate();

            if (next == State.None)
                return ID;

            switch (current)
            {
                // y 축 이동
                case 0:
                    {
                        transform.position =
                        Vector2.Lerp(_startPos,
                                     _startPos + Vector2.up * _different.y,
                                     _timer / ( _clipLength * _differentRatio.y));
                       
                        if (_timer >= _clipLength * _differentRatio.y)
                        {
                            _startPos = transform.position;
                            _timer = 0.0f;
                            current++; 
                        }
                    }
                    break;
                // x 축 이동
                case 1:
                    {
                        transform.position =
                            Vector2.Lerp(_startPos,
                                         _startPos + Vector2.right * _different.x,
                                         _timer / (_clipLength * _differentRatio.y));
                       
                        if (_timer >= _clipLength * _differentRatio.y)
                        {
                            current++;
                        }
                    }
                    break;
                default:
                    {
                        next = State.Idle;
                    }
                    break;

            }

            _timer += Time.deltaTime * 3.0f;
            return next;
        }
    }

    public class WallSlide : WorkflowBase
    {
        public WallSlide(CharacterMachine machine, float dampingFactor) : base(machine)
        {
            _dampingFactor = dampingFactor;
        }
        public override State ID => State.WallSlide;
        public override bool CanExecute => base.CanExecute &&
                                            (machine.isWallDetected &&
                                             machine.current == State.Fall);
        private float _dampingFactor;
        private Vector2 _velocity;
        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = false;
            machine.isMovable = false;
            machine.hasJumped = false;
            machine.hasSecondJumped = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = Vector2.zero;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _velocity = Vector2.zero;
            animator.Play("WallSlide");

        }

        public override void OnExit()
        {
            base.OnExit();
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }


        public override State OnUpdate()
        {
            State next = base.OnUpdate();

            if (next == State.None)
                return ID;

            switch (current)
            {
               
                default:
                    {
                        if (machine.isWallDetected == false)
                        {
                            next = State.Idle;
                        }
                    }
                    break;

            }

            return next;
        }

        public override void OnFixdeUpdate()
        {
            base.OnFixdeUpdate();
            _velocity += Physics2D.gravity * (1.0f - _dampingFactor) * Time.fixedDeltaTime;
            transform.position += (Vector3)_velocity * Time.fixedDeltaTime;
        }
    }

    public class Attack : WorkflowBase
    {
        public override State ID => State.Attack;

        public override bool CanExecute
        {
            get
            {
                if (_combo > 0 &&
                    Time.time - _exitTimeMark >= _comboResetTime)
                {
                    _combo = 0;
                    _hasHit = false;
                }

                if ( _combo > _comboMax)
                {
                    return false;
                }

                if ( base.CanExecute &&
                    ((_combo == 0) || (_combo > 0 && _hasHit)) &&
                    (machine.current == State.Idle ||
                     machine.current == State.Move ||
                     machine.current == State.Jump ||
                     machine.current == State.Fall ||
                     machine.current == State.SecondJump))
                {
                    return true;
                }
                return false;
            }
        }

        private int _comboMax;
        private int _combo;
        private float _comboResetTime;
        private float _exitTimeMark;
        private bool _hasHit;
        private AnimatorEvents _animatorEvents;


        public class AttackSetting
        {
            public Vector2 center;
            public Vector2 size;
            public LayerMask targetMask;
            public float distance;
            public int targetMax;
            public float damageGain;

            public AttackSetting(Vector2 conter, Vector2 size, LayerMask targetMask, float distance, int targetMax, float damageGain)
            {
                this.center = center;
                this.size = size;
                this.targetMask = targetMask;
                this.distance = distance;
                this.targetMax = targetMax;
                this.damageGain = damageGain;
            }
        }
        private AttackSetting[] _attackSettings;
        private List<CharacterMachine> _targets = new List<CharacterMachine>();

        public Attack(CharacterMachine machine, int comboMax, float comboResetTime, AttackSetting[] attackSettings) : base(machine)
        {
            _comboMax = comboMax;
            _comboResetTime = comboResetTime;
            _animatorEvents = machine.GetComponentInChildren<AnimatorEvents>();
            _attackSettings = attackSettings;

            _animatorEvents.onAttackHit += () =>
            {
                foreach (var target in _targets)
                {
                    if (target != null)
                    {
                        continue;
                    }
                    float damage = Random.Range(machine._attackForceMin, machine._attackForceMax) * _attackSettings[_combo - 1].damageGain;
                    target.DepleteHp(machine, damage);
                    target.KnockBack(new Vector2(machine.direction, 0.0f));
                    DamagePopUp.Create(target.transform.position + Vector3.up * 0.5f,
                                       (int)damage,
                                       machine.gameObject.layer);
                    
                }

                _hasHit = true;
            };

        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = false;
            machine.isMovable = false;
            
            if (machine.isGrounded)
            {
                machine.move = Vector2.zero;
                rigidbody.velocity = Vector2.zero;
            }
           
            AttackSetting setting = _attackSettings[_combo];

            RaycastHit2D[] hits =
            Physics2D.BoxCastAll(origin: rigidbody.position + new Vector2(setting.center.x * machine.direction, setting.center.y),
                                 size: setting.size,
                                 angle: 0.0f,
                                 direction: Vector2.right * machine.direction,
                                 distance: setting.distance,
                                 layerMask: setting.targetMask);
            _targets.Clear();

            for (int i = 0; i < hits.Length; i++)
            {
                if (i >= setting.targetMax)
                    break;

                if (hits[i].collider.TryGetComponent(out CharacterMachine character))
                {
                    _targets.Add(character);
                    
                }
            }
            _hasHit = false;
            animator.SetFloat("attackComboStack", _combo++);
            animator.Play("Attack");

        }

        public override void OnExit()
        {
            base.OnExit();
            _exitTimeMark = Time.time;
        }

        public override State OnUpdate()


        {
            State next = base.OnUpdate();

            if (next == State.None)
                return ID;

            switch (current)
            {

                default:
                    {
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            next = State.Idle;
                        }

                        if (_hasHit)
                        {
                            AttackSetting setting = _attackSettings[_combo - 1];
                            Vector2 center = rigidbody.position + new Vector2(setting.center.x * machine.direction, setting.center.y);
                            Vector2 size = setting.size;
                            float distdance = setting.distance;

                            Debug.DrawLine(center + new Vector2(-size.x / 2.0f * machine.direction, size.y / 2.0f),
                                           center + new Vector2(+size.x / 2.0f * machine.direction, size.y / 2.0f) + Vector2.right * machine.direction * distdance);
                            
                            Debug.DrawLine(center + new Vector2(-size.x / 2.0f * machine.direction, -size.y / 2.0f),
                                           center + new Vector2(+size.x / 2.0f * machine.direction, -size.y / 2.0f) + Vector2.right * machine.direction * distdance);
                            
                            Debug.DrawLine(center + new Vector2(-size.x / 2.0f * machine.direction, size.y / 2.0f),
                                           center + new Vector2(+size.x / 2.0f * machine.direction, -size.y / 2.0f));

                            Debug.DrawLine(center + new Vector2(-size.x / 2.0f * machine.direction, -size.y / 2.0f) + Vector2.right * machine.direction * distdance,
                                           center + new Vector2(+size.x / 2.0f * machine.direction, size.y / 2.0f) + Vector2.right * machine.direction * distdance);
                        }   
                    }
                    break;
            }

            return next;
        }
    }

    public class Hurt : WorkflowBase
    {

        public override State ID => State.Hurt;
        public override bool CanExecute => base.CanExecute &&
                                           (machine.current == State.Idle ||
                                           machine.current == State.Move ||
                                           machine.current == State.Jump ||
                                           machine.current == State.Fall ||
                                           machine.current == State.Land ||
                                           machine.current == State.Crouch ||
                                           machine.current == State.JumpDown ||
                                           machine.current == State.SecondJump ||
                                           machine.current == State.Ledge ||
                                           machine.current == State.LedgeClimb ||
                                           machine.current == State.LadderClimbing ||
                                           machine.current == State.WallSlide); 


        public Hurt(CharacterMachine machine) : base(machine)
        {
        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = false;
            machine.isMovable = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = Vector2.zero;
            animator.Play("Hurt");
            
        }
        public override State OnUpdate()
        {
            State next = base.OnUpdate();

            if (next == State.None)
                return ID;

            switch (current)
            {

                default:
                    {
                        // 현재 애니메이터의 재생중인 상태의 정보에서 일반화된 시간이 1.0f 이된다.
                        // == 현재 상태 애니메이션 클립 재생이 끝났다.
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            next = State.Idle;
                        }
                    }
                    break;
            }
            return next;
        }
    }

    public class Die : WorkflowBase
    {

        public override State ID => State.Die;

        public Die(CharacterMachine machine) : base(machine)
        {
        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isInvincible = true;
            machine.isDirectionChangeable = false;
            machine.isMovable = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = Vector2.zero;
            animator.Play("Die");

        }
        public override State OnUpdate()
        {
            State next = base.OnUpdate();

            if (next == State.None)
                return ID;

           
            return next;
        }
    }


    public static IEnumerable<KeyValuePair<State, IWorkflow<State>>> GetWorkflowsForPlayer(CharacterMachine machine)
    {

        return new Dictionary<State, IWorkflow<State>>()
        {
        { State.Idle, new Idle(machine) },
        { State.Move, new Move(machine) },
        { State.Jump, new Jump(machine, 3.0f)},
        { State.JumpDown, new JumpDown(machine, 1.0f, 0.5f)},
        { State.SecondJump, new SecondJump(machine, 3.0f)},
        { State.Fall, new Fall(machine, 1.0f)},
        { State.Land, new Land(machine) },
        { State.Crouch, new Crouch(machine, new Vector2 (0.0f, 0.06f), new Vector2(0.12f, 0.12f)) },
        { State.LadderClimbing, new LadderClimbing(machine, 1.0f) },
        { State.Ledge, new Ledge(machine) },
        { State.LedgeClimb, new LedgeClimb(machine) },
        { State.WallSlide, new WallSlide(machine, 0.8f) },
        { State.Attack, new Attack(machine, 2, 0.3f, new Attack.AttackSetting[3]
            {
            new Attack.AttackSetting(new Vector2(0.2f, 0.18f), new Vector2(0.45f, 0.4f), LayerMask.NameToLayer("EnemyTrigger"),0.0f, 2, 0.8f),
            new Attack.AttackSetting(new Vector2(0.2f, 0.18f), new Vector2(0.45f, 0.4f), LayerMask.NameToLayer("EnemyTrigger"),0.0f, 2, 0.95f),
            new Attack.AttackSetting(new Vector2(0.2f, 0.18f), new Vector2(0.45f, 0.4f), LayerMask.NameToLayer("EnemyTrigger"),0.2f, 2, 1.3f),
            }) },
        { State.Hurt, new Hurt(machine) },
        { State.Die, new Die(machine) },
         

        };
    }


    public static IEnumerable<KeyValuePair<State, IWorkflow<State>>> GetWorkflowsForEnemy(CharacterMachine machine)
    {

        return new Dictionary<State, IWorkflow<State>>()
        {
        { State.Idle, new Idle(machine) },
        { State.Move, new Move(machine) },
        { State.Jump, new Jump(machine, 2.0f)},
        { State.JumpDown, new JumpDown(machine, 1.0f, 0.5f)},
        { State.SecondJump, new SecondJump(machine, 3.0f)},
        { State.Fall, new Fall(machine, 1.0f)},
        { State.Land, new Land(machine) },
        { State.Crouch, new Crouch(machine, new Vector2 (0.0f, 0.06f), new Vector2(0.12f, 0.12f)) },
        { State.LadderClimbing, new LadderClimbing(machine, 1.0f) },
        { State.Ledge, new Ledge(machine) },
        { State.LedgeClimb, new LedgeClimb(machine) },
        { State.WallSlide, new WallSlide(machine, 0.8f) },
        { State.Attack, new Attack(machine, 0, 0.0f, null
        ) },
        { State.Hurt, new Hurt(machine) },
        { State.Die, new Die(machine) },

        };
    }
}