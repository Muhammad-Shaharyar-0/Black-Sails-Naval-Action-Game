﻿using System;
using System.Collections;
using System.Collections.Generic;
using Eliot.Environment;
using UnityEngine;
#if UNITY_5_5_OR_NEWER
using UnityEngine.AI;
#endif

namespace Eliot.AgentComponents
{
	/// <summary>
	/// Motion Engine that uses Unity's NavMesh to find path.
	/// </summary>
	public class NavMeshMotionEngine : IMotionEngine
	{
		#region FIELDS
		
		public NavMeshAgent NavMeshAgent
		{
			get { return _navMeshAgent; }
		}

		/// Current state of Agent's Motion Component.
		private MotionState State
		{
			get { return _agentMotion.State; }
			set
			{
				{
					_agent.isDoingAction = value == MotionState.Patroling;
					_agentMotion.State = value;
				}
			}
		}
		/// Link to Agent's Motion component.
		private AgentMotion _agentMotion;
		/// Animation component of the Agent.
		private AgentAnimation _agentAnimation;
		/// Resources component of the Agent.
		private AgentResources _agentResources;
		/// Link to Agent Component of gameObject.
		private EliotAgent _agent;
		/// Link to the Agent's gameObject.
		private GameObject _gameObject;
		/// Link to Transform Component of gameObject.
		private Transform _transform;
		/// Link to NavMesh Component of gameObject.
		private NavMeshAgent _navMeshAgent;
		/// Last time Agent used energy to move.
		private float _lastWalkCostUpdate;
		/// Last time Agent used energy to run.
		private float _lastRunCostUpdate;
		/// If true, Agent is allowed to dodge.
		private bool _canDodge = true;
		/// If true, Agent is not allowed to move in any way.
		private bool _locked;
		/// Make sure Agent's Motion never misses a target.
		private Transform _defaultTarget;
		/// Time at which Agent started staying at place while patroling.
		private float _patrolStartTime;
		/// Last time Agent's Motion component made a sound.
		private float _lastSoundTime;
		/// Last position picked as destination while moving around.
		private Vector3 _lastMoveAroundDestination;
		private NavMeshPath navMeshPath;
		/// Action Interface for this Motion instance.
		private MotionActionInterface _actionInterface;
		/// Condition Interface for this Motion instance.
		private MotionConditionInterface _conditionInterface;
		/// Maximum distance at which Agent can pick random target position.
		private const float MaxWanderingAroundDistance = 25f;
		/// Defines the error of calculating if the Agent is at target position already.
		private const float DistanceError = 0.1f;
		#endregion

		#region INTERFACE METHODS IMPLEMENTATION
		/// <summary>
		/// Return the position of current Agent's target.
		/// </summary>
		/// <returns></returns>
		public Vector3 GetTarget()
		{
			return _navMeshAgent.destination;
		}

		/// <summary>
		/// Assign a new target position.
		/// </summary>
		/// <param name="pos"></param>
		public void SetTarget(Vector3 pos)
		{
			NavMeshHit hit;
			if (NavMesh.SamplePosition(pos, out hit, 10.0f, _navMeshAgent.areaMask)) {
				_navMeshAgent.SetDestination(hit.position);
			}
		}

		/// <summary>
		/// Initialize an Agent so that it has all necessary parameters for using the engine.
		/// </summary>
		/// <param name="agent"></param>
		public void Init(EliotAgent agent)
		{
			_agentMotion = agent.GetAgentComponent<Eliot.AgentComponents.AgentMotion>();
			_gameObject = agent.gameObject;
			_transform = _gameObject.GetComponent<Transform>();
			_lastMoveAroundDestination = _transform.position;
			_agent = agent;
			_agentAnimation = _agent.GetAgentComponent<Eliot.AgentComponents.AgentAnimation>();
			_agentResources = _agent.GetAgentComponent<Eliot.AgentComponents.AgentResources>();
			if (_agentMotion.Type == MotionEngine.NavMesh)
			{
				_navMeshAgent = _gameObject.GetComponent<NavMeshAgent>();
				if (_gameObject.GetComponent<NavMeshAgent>())
					_navMeshAgent = _gameObject.GetComponent<NavMeshAgent>();
				else
					_navMeshAgent = _gameObject.AddComponent<NavMeshAgent>();
				_navMeshAgent.speed = _agentMotion.WalkSpeed;
				_navMeshAgent.updateRotation = true;
				navMeshPath = new NavMeshPath();
			}
		}

		/// <summary>
		/// Check if the Agent's motion is currently locked.
		/// </summary>
		/// <returns></returns>
		public bool Locked()
		{
			return _locked;
		}

		/// <summary>
		/// Lock the Agent's Motion component.
		/// </summary>
		public void Lock()
		{
			_locked = true;
			if (_navMeshAgent)
				_navMeshAgent.speed = 0;
		}

		/// <summary>
		/// Unlock the Agent's Motion component.
		/// </summary>
		public void Unlock()
		{
			_locked = false;
		}

		/// <summary>
		/// Rotate Agent towards its target.
		/// </summary>
		/// <param name="targetPosition"></param>
		public void LookAtTarget(Vector3 targetPosition)
		{
			var applyRootMotion = _agentAnimation && _agentAnimation.ApplyRootMotion;
			if (!applyRootMotion || !_agentAnimation)
			{
				var newDir = Vector3.RotateTowards(_transform.forward,
					targetPosition - _transform.position, _agentMotion.RotationSpeed * Time.deltaTime, 0F);
				_transform.rotation = Quaternion.LookRotation(newDir);
				_transform.eulerAngles = new Vector3(0, _transform.eulerAngles.y, 0);
			}
			else
			{
				var targetV = targetPosition - _transform.position;
				float angle = 0;
#if UNITY_2017_1_OR_NEWER
				angle = Vector3.SignedAngle(_transform.forward, targetV, _transform.up);
#else
				var unsignedangle = Vector3.Angle(targetV, _transform.forward);
				var sign = Mathf.Sign(Vector3.Dot(targetV, _transform.right));
				angle = sign * unsignedangle;
#endif
				var turnAmount = Mathf.Clamp(angle / 180f * _agent.GetAgentComponent<AgentMotion>().RotationSpeed, -1f, 1f);
				
				if (!_agentAnimation.RootMotionRotation)
				{
					_agentAnimation.Animator.applyRootMotion = false;
					_agentAnimation.Animator.transform.parent = _agent.transform;
					var newDir = Vector3.RotateTowards(_transform.forward,
						targetPosition - _transform.position, _agentMotion.RotationSpeed * Time.deltaTime, 0F);
					_transform.rotation = Quaternion.LookRotation(newDir);
					_transform.eulerAngles = new Vector3(0, _transform.eulerAngles.y, 0);
					
					if(_agentAnimation)
						_agentAnimation.Animator.SetFloat(_agent.GetAgentComponent<AgentAnimation>().Parameters.Horizontal, turnAmount);

					_agentAnimation.Animator.applyRootMotion = true;
					_agentAnimation.Animator.transform.parent = null;
				}
				else
				{
					if (_agentAnimation)
						_agentAnimation.Animator.SetFloat(
							_agent.GetAgentComponent<AgentAnimation>().Parameters.Horizontal, turnAmount);
				}
			}
		}

		/// <summary>
		/// Stand and relax playing default animation.
		/// </summary>
		public void Idle()
		{
			if (_locked) return;
			State = MotionState.Idling;
			_agentMotion.Animate(AnimationState.Idling);
			SetSpeed(0);
			if (_agentMotion.Target != _transform.position) _agentMotion.Target = _transform.position;
		}

		/// <summary>
		/// Stand still idling, with different animation from idle one.
		/// </summary>
		public void Stand()
		{
			if (_locked) return;
			State = MotionState.Standing;
			_agentMotion.Animate(AnimationState.Idling);
			SetSpeed(0);
			if (_agent.Target != _transform)
				_agent.Target = _transform;
		}

		/// <summary>
		/// Move in the given direction.
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="run"></param>
		public void Move(Vector3 direction, bool run)
		{
			if (_locked) return;
			if (direction == Vector3.zero)
			{
				Idle();
			}
			else
			{
				if (run && !_agentMotion.CanRun) run = false;
				var deltaPos = _transform.position + direction;
				var targetPos = GetSamplePosition(deltaPos, 3f, NavMesh.AllAreas);
				if (run)
				{
					MoveTowards(targetPos, _agentMotion.RunSpeed, MotionState.Running, AnimationState.Running,
						0.5f, _agentMotion.RunCost, WalkToTarget, ref _lastRunCostUpdate, Idle);
				}
				else
				{
					MoveTowards(targetPos, _agentMotion.WalkSpeed, MotionState.Walking,
						AnimationState.Walking, 0.5f, _agentMotion.WalkCost,
						Idle, ref _lastWalkCostUpdate, Idle);
				}
			}

			_agentMotion.PlayAudio(_agentMotion.WalkingStepSounds, _agentMotion.WalkingStepPing);
		}

	    /// <summary>
	    /// Walk to target, playing corresponding animation and audio. Idle if out of energy.
	    /// </summary>
	    public void WalkToTarget()
	    {
		    MoveTowards(_agentMotion.TargetTransform, _agentMotion.WalkSpeed, MotionState.Walking, AnimationState.Walking,
			    0.5f, _agentMotion.WalkCost, Idle, ref _lastWalkCostUpdate, Idle);
		    _agentMotion.PlayAudio(_agentMotion.WalkingStepSounds, _agentMotion.WalkingStepPing);
	    }

	    /// <summary>
	    /// Walk from target, playing corresponding animation and audio. Idle if out of energy.
	    /// </summary>
	    public void WalkFromTarget()
	    {
		    var deltaPos = _transform.position + (_transform.position - _agent.Target.position).normalized;
		    var targetPos = GetSamplePosition(deltaPos, 3f, NavMesh.AllAreas);
		    MoveTowards(targetPos, _agentMotion.WalkSpeed, MotionState.WalkingAway,
			    AnimationState.Walking, 0.5f, _agentMotion.WalkCost, Idle, ref _lastWalkCostUpdate, Idle);
		    _agentMotion.PlayAudio(_agentMotion.WalkingStepSounds, _agentMotion.WalkingStepPing);
	    }

	    /// <summary>
	    /// Run to target, playing corresponding animation and audio. Walk to target if out of energy.
	    /// </summary>
	    public void RunToTarget()
	    {
		    if (!_agentMotion.CanRun)
		    {
			    WalkToTarget();
			    return;
		    }
		    MoveTowards(_agentMotion.TargetTransform, _agentMotion.RunSpeed, MotionState.Running, AnimationState.Running,
			    0.5f, _agentMotion.RunCost, WalkToTarget, ref _lastRunCostUpdate, Idle);
		    _agentMotion.PlayAudio(_agentMotion.RunningStepSounds, _agentMotion.RunningStepPing);
	    }

	    /// <summary>
	    /// Run from target, playing corresponding animation and audio. Walk from target if out of energy.
	    /// </summary>
	    public void RunFromTarget()
	    {
		    if (!_agentMotion.CanRun)
		    {
			    WalkFromTarget();
			    return;
		    }
		    var deltaPos = _transform.position + (_transform.position - _agent.Target.position).normalized;
		    var targetPos = GetSamplePosition(deltaPos, 3f, NavMesh.AllAreas);
		    MoveTowards(targetPos, _agentMotion.RunSpeed, MotionState.RunningAway,
			    AnimationState.Running, 0.5f, _agentMotion.RunCost, WalkFromTarget, ref _lastRunCostUpdate, Idle);
		    _agentMotion.PlayAudio(_agentMotion.RunningStepSounds, _agentMotion.RunningStepPing);
	    }

	    /// <summary>
	    /// Get random point on the NavMesh and walk to it.
	    /// </summary>
		public void WanderAround()
		{
			MoveAround(MotionState.Walking, WalkToTarget, randomly: true);
		}

		/// <summary>
	    /// Get random point on the NavMesh and run to it.
	    /// </summary>
	    public void RunAround()
	    {
		    if (!_agentMotion.CanRun)
		    {
			    WanderAround();
			    return;
		    }
		    MoveAround(MotionState.Running, RunToTarget, randomly: true);
	    }

		/// <summary>
		/// Set next way point as a target and walk to it. Repeat when Agent got to way point.
		/// </summary>
		public void WalkAroundWayPoints()
		{
			//For Agents without a WayPointsGroup reference, it is the same as WanderAround
			MoveAround(MotionState.Walking, WalkToTarget, randomly: !_agent.WayPoints);
		}

		/// <summary>
		/// Set next way point as a target and run to it. Repeat when Agent got to way point.
		/// </summary>
		public void RunAroundWayPoints()
		{
			if (!_agentMotion.CanRun)
			{
				WalkAroundWayPoints();
				return;
			}
			//For Agents without a WayPointsGroup reference, it is the same as WanderAround
			MoveAround(MotionState.Running, RunToTarget, randomly: !_agent.WayPoints);
		}

	    /// <summary>
	    /// Get random point on the NavMesh and walk to it. Stand for specified amount of time. Walk again.
	    /// </summary>
	    public void Patrol()
	    {
		    Patrol(MotionState.Patroling, WalkToTarget, Idle, randomly:  !_agent.WayPoints, waitTime: _agentMotion.PatrolTime);
	    }

		/// <summary>
		/// Quick move in certain direction, with a corresponding animation.
		/// </summary>
		/// <param name="direction"></param>
		public void Dodge(string direction = "b")
		{
			if (_locked) return;
			if (!_canDodge) return;

			_canDodge = false;
			_agent.StartCoroutine(DodgeEnum(direction));

			_agentMotion.PlayAudio(_agentMotion.DodgeSounds, 0);
		}

		/// <summary>
		/// Let other Agents push this one around.
		/// </summary>
		/// <param name="pusherPos"></param>
		/// <param name="pushPower"></param>
		public void Push(Vector3 pusherPos, float pushPower)
		{
			var sudoTarget = _transform.position - pusherPos; sudoTarget.y = 0;
			_agent.StartCoroutine(WarpEnum(sudoTarget, pushPower / _agentMotion.Weight, 0.1f));
		}

		/// <summary>
		/// Check whether Agent can reach the destination.
		/// </summary>
		/// <returns></returns>
		public bool PathIsValid()
		{
			NavMeshHit navMeshHit;
			NavMesh.SamplePosition(_agent.Target.position, out navMeshHit, 10f, NavMesh.AllAreas);
			NavMeshAgent.CalculatePath(navMeshHit.position, navMeshPath);
			return navMeshPath.status == NavMeshPathStatus.PathComplete;
		}

		#endregion

		#region UTILITY
		/// <summary>
		/// Change speed on all dependent components.
		/// </summary>
		/// <param name="speed"></param>
		private void SetSpeed(float speed)
		{
			if(_navMeshAgent)
				_navMeshAgent.speed = speed;

			if (_agentAnimation && _agentAnimation.AnimationMode == AnimationMode.Mecanim)
				_agentAnimation.SetSpeed(State);
		}

		/// <summary>
		/// Get position by projecting specified vector onto the walkable surface.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="range"></param>
		/// <param name="areaMask"></param>
		/// <returns></returns>
		private Vector3 GetSamplePosition(Vector3 position, float range, int areaMask)
		{
			NavMeshHit hit;
			NavMesh.SamplePosition(position, out hit, range, areaMask);
			return hit.position;
		}

		/// <summary>
		/// Update Agent's position and animation on the way to a target position.
		/// </summary>
		/// <param name="targetTransform"></param>
		/// <param name="moveSpeed"></param>
		/// <param name="motionState"></param>
		/// <param name="animationState"></param>
		/// <param name="animationFadeSpeed"></param>
		/// <param name="energyCost"></param>
		/// <param name="actionOnNotEnoughEnergy"></param>
		/// <param name="updateTimeVariable"></param>
		/// <param name="stopAction"></param>
		private void MoveTowards(Transform targetTransform, float moveSpeed, MotionState motionState, AnimationState animationState,
			float animationFadeSpeed, List<ResourceAction> energyCost, Action actionOnNotEnoughEnergy, ref float updateTimeVariable,
			Action stopAction)
		{
			Vector3? targetPosition = null;
			if (targetTransform) targetPosition = targetTransform.position;
			MoveTowards(targetPosition, moveSpeed, motionState, animationState,
				animationFadeSpeed, energyCost, actionOnNotEnoughEnergy, ref updateTimeVariable, stopAction);
		}

		/// <summary>
		/// Update Agent's position and animation on the way to a target position.
		/// </summary>
		/// <param name="targetPosition"></param>
		/// <param name="moveSpeed"></param>
		/// <param name="motionState"></param>
		/// <param name="animationState"></param>
		/// <param name="animationFadeSpeed"></param>
		/// <param name="energyCost"></param>
		/// <param name="actionOnNotEnoughEnergy"></param>
		/// <param name="updateTimeVariable"></param>
		/// <param name="stopAction"></param>
		private void MoveTowards(Vector3? targetPosition, float moveSpeed, MotionState motionState, AnimationState animationState,
			float animationFadeSpeed, List<ResourceAction> energyCost, Action actionOnNotEnoughEnergy, ref float updateTimeVariable,
			Action stopAction)
		{
			if (_locked) return;
			if (targetPosition == null) return;
			Vector3 closestPoint;
			var targetCollider = _agent.Target.GetComponent<Collider>();
			if (targetCollider)
			{
				closestPoint = targetCollider.ClosestPoint(_transform.position);
			}
			else closestPoint = targetPosition.Value;
			if (Vector3.Distance(_transform.position, closestPoint) <= _navMeshAgent.stoppingDistance)
			{
				Idle();
				return;
			}

			try
			{
				if (_agent.Target)
				{
					var wayPoint = _agent.Target.GetComponent<EliotWayPoint>();
					if (Vector3.Distance(_agent.transform.position, wayPoint.transform.position) <=
					    wayPoint.ActionDistance)
					{
						if (wayPoint && wayPoint.ApplyChangesToAgent && _agent.ApplyChangesOnWayPoints)
						{
							_agent.StartWayPointAction(wayPoint);
						}
					}
				}
			}
			catch (System.Exception)
			{
				//Debug.Log(e);
			}

			if (Vector3.Distance(_agent.transform.position, targetPosition.Value) <= DistanceError)
			{
				stopAction();
				return;
			}
			State = motionState;
			_agentMotion.Animate(animationState, animationFadeSpeed);
			SetSpeed(moveSpeed);
			if (_agentAnimation && _agentAnimation.ApplyRootMotion && !_agentAnimation.RootMotionRotation)
			{
				LookAtTarget(_agent.Target.position);
			}
			if (energyCost == null || energyCost.Count == 0 || !_agentResources)
			{
				targetPosition = GetSamplePosition(targetPosition.Value, 10f, NavMesh.AllAreas);
				if (_agentMotion.Target != targetPosition)
					_agentMotion.Target = targetPosition.Value;
			}
			else
			{
				if (!_agentResources.CanHandle(energyCost)) { actionOnNotEnoughEnergy(); return; }
				targetPosition = GetSamplePosition(targetPosition.Value, 10f, NavMesh.AllAreas);
				if (_agentMotion.Target != targetPosition) _agentMotion.Target = targetPosition.Value;
				if (Time.time > updateTimeVariable + 1)
				{
					_agentResources.Action(energyCost);
					updateTimeVariable = Time.time;
				}
			}
		}

		/// <summary>
		/// Set target position to a random point or to the next Waypoint in WaypointsGroup.
		/// </summary>
		/// <param name="moveAction"></param>
		/// <param name="randomly"></param>
		/// <returns></returns>
		private Vector3 SetTargetPoint(Action moveAction, bool randomly)
		{
			Vector3 point;
			if (randomly)
			{
				RandomPoint(_transform.position, MaxWanderingAroundDistance, out point);
			}
			else
			{
				NextPoint(out point);
			}
			moveAction();
			return point;
		}

		/// <summary>
		/// Move to the next Way Point or pick a random point and walk to it.
		/// Repeat when target position is reached.
		/// </summary>
		/// <param name="state"></param>
		/// <param name="moveAction"></param>
		/// <param name="randomly"></param>
		private void MoveAround(MotionState state, Action moveAction, bool randomly)
		{
			try
			{
				if (!_agent.Target)
				{
					SetTargetPoint(moveAction, randomly);
					_lastMoveAroundDestination = _agent.Target.position;
				}

				if (_agent.Target.GetComponent<EliotWayPoint>() && _agent.Target.GetComponent<EliotWayPoint>().automatic &&  _agent.curWayPoint)
				{
					_agent.Target = _agent.curWayPoint.transform;
				}

				var wayPoint = _agent.Target.GetComponent<EliotWayPoint>();
				if (!wayPoint)
					_agent.Target = _agent.GetDefaultTarget(_lastMoveAroundDestination);

				var agentPosition = _agent.transform.position;
				agentPosition.y = 0;
				var targetPosition = _lastMoveAroundDestination;
				targetPosition.y = 0;

				if (randomly && Vector3.Distance(agentPosition, targetPosition) >= MaxWanderingAroundDistance)
				{
					_lastMoveAroundDestination = SetTargetPoint(moveAction, randomly);
				}

				if (randomly)
				{
					NavMeshHit navMeshHit;
					var sampledProperly = NavMesh.SamplePosition(targetPosition, out navMeshHit, 10f, NavMesh.AllAreas);
					NavMeshAgent.CalculatePath(navMeshHit.position, navMeshPath);
					var dist = _agent.Target.GetComponent<EliotWayPoint>()
						? _agent.Target.GetComponent<EliotWayPoint>().ActionDistance
						: 0.5f;
					if (Vector3.Distance(agentPosition, targetPosition) <= dist
					    || navMeshPath.status != NavMeshPathStatus.PathComplete || !sampledProperly)
					{
						_lastMoveAroundDestination = SetTargetPoint(moveAction, randomly);
						_agent.GetDefaultTarget(_lastMoveAroundDestination);
					}
				}

				moveAction();
				if (State != state)
					_lastMoveAroundDestination = SetTargetPoint(moveAction, randomly);

				if (_agentAnimation && _agentAnimation.ApplyRootMotion && !_agentAnimation.RootMotionRotation)
				{
					LookAtTarget(_lastMoveAroundDestination);
				}
			}
			catch(System.Exception e){Debug.LogError(e, _agent);}
		}

		/// <summary>
		/// Move to the next Waypoint or pick a random point and walk to it.
		/// Idle for specified amount of time.
		/// Repeat when target position is reached.
		/// </summary>
		/// <param name="state"></param>
		/// <param name="moveAction"></param>
		/// <param name="waitAction"></param>
		/// <param name="randomly"></param>
		/// <param name="waitTime"></param>
		private void Patrol(MotionState state, Action moveAction, Action waitAction, bool randomly, float waitTime)
		{
			if (!_agent.Target || !_agent.Target.GetComponent<EliotWayPoint>())
			{
				_agent.Target = _agent.GetDefaultTarget(SetTargetPoint(moveAction, randomly));
				if(!randomly)  _agent.Target = _agent.curWayPoint.transform;
			}

			var agentPosition = _agent.transform.position;
			agentPosition.y = 0;
			var targetPosition = _agent.Target.position;
			targetPosition.y = 0;
            
			float thresholdDistance = randomly
				? _navMeshAgent.stoppingDistance
				: _agent.Target.GetComponent<EliotWayPoint>().ActionDistance;
            
			if (Vector3.Distance(agentPosition, targetPosition) <= thresholdDistance
			    || (randomly && Vector3.Distance(agentPosition, targetPosition) >= MaxWanderingAroundDistance))
			{
				if (Time.time <= _patrolStartTime + waitTime)
				{
					waitAction();
					State = state;
				}
				else
				{
					_agent.Target = _agent.GetDefaultTarget(SetTargetPoint(moveAction, randomly));
					if(!randomly)  _agent.Target = _agent.curWayPoint.transform;
				}
			}
			else
			{
				_patrolStartTime = Time.time;
				moveAction();
				State = state;
			}
		}

		/// <summary>
		/// Move Agent's position in a given direction for a certain amount of time.
		/// </summary>
		/// <param name="warpDir"></param>
		/// <param name="warpStrength"></param>
		/// <param name="warpTime"></param>
		/// <returns></returns>
		private IEnumerator WarpEnum(Vector3 warpDir, float warpStrength, float warpTime)
		{
			var finishTime = Time.time + warpTime;
			while (Time.time < finishTime){
				if (_navMeshAgent)
					_navMeshAgent.Warp(_transform.position + warpDir * warpStrength * Time.deltaTime);
				yield return null;
			}
		}

		/// <summary>
		/// Dodge for a certain amount of time.
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		private IEnumerator DodgeEnum(string direction)
		{
			yield return new WaitForSeconds(_agentMotion.DodgeDelay);
			_locked = true;
			State = MotionState.Dodging;
			_agentMotion.Animate(AnimationState.Dodging, 0);
			Vector3 dir;
			switch (direction)
			{
				case "f":case"forward": dir = _agent.Target.position - _transform.position; dir.y = 0; break;
				case "r":case"right": dir = _transform.right; dir.y = 0; break;
				case "l":case"left": dir =  -_transform.right; dir.y = 0; break;
				case "random":
				{
					var chance = UnityEngine.Random.Range(0, 100);
					if (chance >= 0 && chance < 25) dir = _transform.forward;
					else if (chance >= 25 && chance < 50) dir = _transform.right;
					else if (chance >= 50 && chance < 75) dir = -_transform.right;
					else dir = _transform.position - _agent.Target.position;
					dir.y = 0;
					break;
				}
				case "left_or_right":
				{
					var chance = UnityEngine.Random.Range(0, 100);
					if (chance >= 0 && chance < 50) dir = _transform.right;
					else dir = -_transform.right;
					dir.y = 0;
					break;
				}
				default: dir = _transform.position - _agent.Target.position; dir.y = 0; break;
			}
			_agent.StartCoroutine(WarpEnum(dir, _agentMotion.DodgeSpeed, _agentMotion.DodgeDuration));
			_locked = false;
			yield return new WaitForSeconds(_agentMotion.DodgeCoolDown);
			_canDodge = true;
		}

		/// <summary>
		/// Return a random position in a specified radius from the given center.
		/// </summary>
		/// <param name="center"></param>
		/// <param name="range"></param>
		/// <param name="result"></param>
		private void RandomPoint(Vector3 center, float range, out Vector3 result)
		{
			NavMeshHit hit;
			bool success;
			if (_agent.WayPoints != null)
			{
				var point = _agent.WayPoints.RandomPoint();
				success = NavMesh.SamplePosition(point, out hit, 10.0f, NavMesh.AllAreas);
			}
			else
			{
				var randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
				success = NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas);
			}

			result = success ? hit.position : _transform.position;
		}

		/// <summary>
		/// Get next Way Point in the Way Points Group.
		/// </summary>
		/// <param name="result"></param>
		private void NextPoint(out Vector3 result)
		{
			if (_agent.WayPoints != null)
			{
				NavMeshHit hit;
				if (!_agent.curWayPoint)
				{
					_agent.curWayPoint = _agent.WayPoints[0];
				}
				else
				{
					_agent.curWayPoint = _agent.curWayPoint.Next();
				}

				if (_agent.curWayPoint)
				{
					_agent.Target = _agent.curWayPoint.transform;
					NavMesh.SamplePosition(_agent.curWayPoint.transform.position, out hit, 1.0f,
						NavMesh.AllAreas);
					result = hit.position;
				}
				else result = _transform.position;
			}
			else result = _transform.position;
		}

		#endregion
	}
}
