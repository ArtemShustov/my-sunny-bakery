using System;
using System.Collections.Generic;
using System.Linq;
using MySunnyBakery.Characters.MovementStates;
using UnityEngine;

namespace MySunnyBakery.Characters {
	public class CharacterMovement: MonoBehaviour {
		[Header("Settings")]
		[SerializeField] private float _rotationSpeed = 5f;
		[SerializeField] public SharedData Data = SharedData.Default;
		[Header("Components")]
		[SerializeReference, SubclassSelector] private State[] _states = { new Idle(), new Move() };
		[SerializeField] private Character _character;
		
		private readonly Dictionary<Type, State> _stateMap = new Dictionary<Type, State>();
		private State _current;
		
		private void Awake() {
			foreach (var state in _states) {
				_stateMap.Add(state.GetType(), state);
				state.Init(_character);
			}
		}
		private void Start() {
			_current = _states.FirstOrDefault();
			_current?.OnEnter(null);
		}
		private void Update() {
			_current?.CheckTransition(Time.deltaTime);
			_current?.OnUpdate(Time.deltaTime);
		}

		public void ChangeState(State state) {
			var previous = _current;
			_current?.OnExit(state);
			_current = state;
			_current?.OnEnter(previous);
		}
		public void ChangeState<T>() where T: State {
			var state = _stateMap[typeof(T)];
			ChangeState(state);
		}

		private void OnEnable() {
			_character.Model.AnimatorMoved += OnAnimatorMoved;
		}
		private void OnDisable() {
			_character.Model.AnimatorMoved -= OnAnimatorMoved;
		}
		private void OnAnimatorMoved(Vector3 deltaPosition, Quaternion deltaRotation) {
			_character.Controller.Move(deltaPosition);
			_character.Controller.Move(Physics.gravity * Time.deltaTime);
			
			Rotate();

			void Rotate() {
				var characterTransform = _character.Controller.transform;
				var targetDirection = Data.Movement;
				targetDirection.y = 0;
				if (targetDirection.sqrMagnitude < Mathf.Epsilon) {
					return;
				}
				var targetRotation = Quaternion.LookRotation(targetDirection);
				
				characterTransform.rotation = Quaternion.Slerp(
					characterTransform.rotation, 
					targetRotation, 
					Time.deltaTime * _rotationSpeed
				);
			}
		}

		[Serializable]
		public abstract class State {
			public Character Character { get; private set; }

			public void Init(Character character) {
				Character = character;
			}
			
			public virtual void OnEnter(State previousState) { }
			public virtual void OnExit(State nextState) { }

			public virtual bool CheckTransition(float deltaTime) => false;
			public virtual void OnUpdate(float deltaTime) { }
		}
		
		[Serializable]
		public struct SharedData {
			public Vector3 Movement;

			public static SharedData Default => new SharedData() { };
		}
	}
}
