﻿//using Cirrus.Circuit.World.Objects.Actions;
//using Cirrus.Circuit.World.Objects.Attributes;
using Cirrus.Circuit.Controls;
using Cirrus.Circuit.World.Objects.Characters.Controls;
//using Cirrus.Circuit.UI.HUD;
//using Cirrus.Circuit.World.Objects.Characters.Strategies;
using KinematicCharacterController;
using System;
using System.Collections;
using UnityEngine;
//using Cirrus.Circuit.Actions;
//using Cirrus.Circuit.Conditions;

using UnityInputs = UnityEngine.InputSystem;
//using Cirrus.Circuit.Playable;

namespace Cirrus.Circuit.World.Objects.Characters
{
    [Serializable]
    public struct Axes
    {
        public Vector2 Left;
        public Vector2 Right;
    }

    public class Character : BaseObject, ICharacterAnimatorWrapper
    {
        public override ObjectType Type => ObjectType.Character;

        public Player _controller;

        [SerializeField]
        private Guide _guide;

        public const float MoveDelay = 0.1f;
        
        public const float RotationSpeed = 0.6f;

        private const float MoveIdleTransitionTime = 0.6f;

        public override bool IsSlidable => false;

        [SerializeField]
        public Axes Axes;

        [SerializeField]
        public Animator Animator;

        private Timer _moveIdleTransitionTimer;

        private CharacterAnimatorWrapper _animatorWrapper;

        public override Color Color {

            get => _color;            

            set
            {
                _color = value;
                if (_guide != null)
                {
                    _guide.Color = _color;
                }
            }
        }

        public float BaseLayerLayerWeight { set => throw new NotImplementedException(); }

        protected override void Awake()
        {
            base.Awake();

            _moveIdleTransitionTimer = new Timer(MoveIdleTransitionTime, start: false, repeat: false);
            _moveIdleTransitionTimer.OnTimeLimitHandler += OnMoveIdleTransitionTimeout;
            _animatorWrapper = new CharacterAnimatorWrapper(Animator);
        }

        public override void Start()
        {
            base.Start();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        private bool _wasMovingVertical = false;


        public void OnMoveIdleTransitionTimeout()
        {
            Play(CharacterAnimation.Character_Idle);
        }


        public override void OnRoundEnd()
        {
            // Remove callbacks??
            //_controller.SetCallbacks(this);
        }

        // Cancel
        public void OnAction0(UnityInputs.InputAction.CallbackContext context)
        {
            //Game.Instance.HandleAction0(this);
        }

        // Accept
        public void OnAction1(UnityInputs.InputAction.CallbackContext context)
        {
            //Game.Instance.HandleAction1(this);
        }

        private Coroutine _moveCoroutine = null;

        public IEnumerator Coroutine_Cmd_Move(Vector2 axis)
        {            
            bool isMovingHorizontal = Mathf.Abs(axis.x) > 0.5f;
            bool isMovingVertical = Mathf.Abs(axis.y) > 0.5f;

            Vector3Int stepHorizontal = new Vector3Int(StepSize * Math.Sign(axis.x), 0, 0);
            Vector3Int stepVertical = new Vector3Int(0, 0, StepSize * Math.Sign(axis.y));

            Move move = new Move
            {
                User = this,
                Position = _gridPosition,                
            };

            switch (_state)
            {
                case ObjectState.Disabled:
                case ObjectState.Falling:                    
                    move.Type = MoveType.Direction;                    
                    break;

                default:
                    move.Type = MoveType.Moving;
                    break;
            }


            if (isMovingVertical && isMovingHorizontal)
            {
                move.Step = _wasMovingVertical ? stepHorizontal : stepVertical;
            }
            else if (isMovingHorizontal)
            {
                move.Step = stepHorizontal;
                _wasMovingVertical = false;
            }
            else if (isMovingVertical)
            {
                move.Step = stepVertical;
                _wasMovingVertical = true;
            }
            else move = null;

            if (move != null)
            {
                Cmd_Move(move);
            }

            yield return new WaitForSeconds(MoveDelay);
            _moveCoroutine = null;
            yield return null;
        }

        // Use the same raycast to show guide
        public void Cmd_Move(Vector2 axis)
        {
            switch (_state)
            {               
                case ObjectState.Moving:
                case ObjectState.Falling:
                case ObjectState.Idle:
                case ObjectState.Disabled:                
                    
                    if (_moveCoroutine == null) _moveCoroutine = StartCoroutine(Coroutine_Cmd_Move(axis));

                    break;

                default:
                    break;

            }
        }

        public override void Move(MoveResult result)
        {
            base.Move(result);

            switch (result.MoveType)
            {
                case MoveType.Moving:
                    Play(CharacterAnimation.Character_Walking, false);
                    //_guide.Show(move.Step);
                break;
            }
        }

        public override void Land()
        {
            base.Land();
            Play(CharacterAnimation.Character_Landing);
        }


        public override void FSM_Update()
        {
           base.FSM_Update();
        }

        public void DoAction0()
        {
            //throw new NotImplementedException();
        }

        public void DoAction1()
        {
            //throw new NotImplementedException();
        }

        public float GetStateSpeed(CharacterAnimation state)
        {
            throw new NotImplementedException();
        }

        public void Play(CharacterAnimation animation, float normalizedTime, bool reset = true)
        {
            _animatorWrapper.Play(animation, normalizedTime, reset);
        }

        public void Play(CharacterAnimation animation, bool reset = true)
        {
            _animatorWrapper.Play(animation, reset);
        }
    }

}
