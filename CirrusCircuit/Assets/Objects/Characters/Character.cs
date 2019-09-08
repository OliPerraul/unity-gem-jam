﻿//using Cirrus.Circuit.Objects.Actions;
//using Cirrus.Circuit.Objects.Attributes;
using Cirrus.Circuit.Objects.Characters.Controls;
//using Cirrus.Circuit.UI.HUD;
//using Cirrus.Circuit.Objects.Characters.Strategies;
using KinematicCharacterController;
using System;
using System.Collections;
using UnityEngine;
//using Cirrus.Circuit.Actions;
//using Cirrus.Circuit.Conditions;

namespace Cirrus.Circuit.Objects.Characters
{
    [System.Serializable]
    public struct Axes
    {
        public Vector2 Left;
        public Vector2 Right;
    }

    public class Character : BaseObject
    {
        public override ObjectId Id { get { return ObjectId.Character; } }

        [SerializeField]
        private Guide _guide;

        [SerializeField]
        public float _rotationSpeed = 0.6f;

        [SerializeField]
        public Axes Axes;

        [SerializeField]
        public Animator Animator;

        private Vector3 _targetDirection = Vector3.zero;

        private Vector3 _direction = Vector3.zero;

        private bool _wasMovingVertical = false;

        public override Color Color {

            get
            {
                return _color;
            }

            set
            {
                _color = value;
                if (_guide != null)
                {
                    _guide.Color = _color;
                }
            }
        }

        public virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawSphere(_targetPosition, 0.1f);
        }

        protected override void Awake()
        {
            base.Awake(); 
        }       
        
        // Use the same raycast to show guide
        public void TryMove(Vector2 axis)
        {
            bool isMovingHorizontal = Mathf.Abs(axis.x) > 0.5f;
            bool isMovingVertical = Mathf.Abs(axis.y) > 0.5f;

            Vector3 stepHorizontal = new Vector3(_stepDistance * Mathf.Sign(axis.x), 0, 0);
            Vector3 stepVertical = new Vector3(0, 0, _stepDistance * Mathf.Sign(axis.y));

            if (isMovingVertical && isMovingHorizontal)
            {
                //moving in both directions, prioritize later
                if (_wasMovingVertical)
                {
                    if (base.TryMove(stepHorizontal))
                    {
                        _guide.Show(stepHorizontal);
                    }
                }
                else
                {
                    if (base.TryMove(stepVertical))
                    {
                        _guide.Show(stepVertical);
                    }
                }
            }
            else if (isMovingHorizontal)
            {
                if (base.TryMove(stepHorizontal))
                {
                    _guide.Show(stepHorizontal);
                    _wasMovingVertical = false;
                }
            }
            else if (isMovingVertical)
            {
                if (base.TryMove(stepVertical))
                {
                    _guide.Show(stepVertical);
                    _wasMovingVertical = true;
                }
            }
                        
            // Smoothly interpolate from current to target look direction  
            _targetDirection = new Vector3(axis.x, 0.0f, axis.y);
            _direction = Vector3.Lerp(_direction, _targetDirection, _rotationSpeed).normalized;


            if (_direction != Vector3.zero)
                _visual.Parent.transform.rotation = Quaternion.LookRotation(_direction, transform.up);
        }

        public void TryAction0()
        {
            throw new NotImplementedException();
        }

        public void TryAction1()
        {
            throw new NotImplementedException();
        }
    }

}
