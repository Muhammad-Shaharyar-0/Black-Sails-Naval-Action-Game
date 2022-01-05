using Eliot.AgentComponents;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Eliot.Environment;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class BasicTargetActionInterface : TargetActionInterface {


    int pathcompleted=0;
    Vector3 previousTarget = Vector3.zero;
    Vector3 PlayerLocation = Vector3.zero;
    bool Moving;
    public BasicTargetActionInterface(EliotAgent agent) : base(agent)
	{
	}

	/// <summary>
	/// Reset the target to the default one.
	/// </summary>
	[IncludeInBehaviour] public void ResetTarget()
	{
		Agent.Target = Agent.GetDefaultTarget();
	}
    public void MoveToTarget(Vector3 Target)
    {
        NavMeshPath path = new NavMeshPath();
        NavMeshAgent agent = Agent.gameObject.GetComponent<NavMeshAgent>();
        Enemy_health health = Agent.gameObject.GetComponent<Enemy_health>();
        
       // agent.updatePosition = false;
        //agent.updateRotation = false;
        if (health.isDead != true)
        {
            if (agent.CalculatePath(Target, path))
            {
                //Agent.transform.Translate(Vector3.forward * 50f * Time.deltaTime);
                for (int i = 0; i < path.corners.Length - 1; i++)
                    Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);

                Move(path, path.corners.Length);
            }
        }
       
    }
    [IncludeInBehaviour]
    public void MoveToPlayer()
    {
        NavMeshPath path = new NavMeshPath();
        NavMeshAgent agent = Agent.gameObject.GetComponent<NavMeshAgent>();
        Enemy_health health = Agent.gameObject.GetComponent<Enemy_health>();
        if (health.isDead != true)
        {
            if (agent.CalculatePath(Agent.Target.transform.position, path))
            {
                Move(path, path.corners.Length);
            }
        }
    }
    public void Move(NavMeshPath path,int length)
    {
        NavMeshAgent agent = Agent.gameObject.GetComponent<NavMeshAgent>();
        if(pathcompleted>=length)
        {
            pathcompleted = 0;
        }
        Vector3 Target = path.corners[pathcompleted];
        Enemy_Movement E = Agent.gameObject.GetComponent<Enemy_Movement>();
        float RotationSpeed = E.RotationSlowSpeed;
        if (Vector3.Distance(Agent.transform.position, Target) > 25f)
        {
            float speed = E.ForwardFullSpeed;
//            Debug.Log(Vector3.Distance(Agent.transform.position, Target));
            if (Vector3.Distance(Agent.transform.position, Target) < 45f)
            {
                speed = E.ForwardSlowSpeed;
                RotationSpeed = E.RotationSlowSpeed;
            }
            Agent.transform.Translate(Vector3.forward * speed * Time.deltaTime);
            agent.nextPosition = Agent.transform.position;
            Quaternion _lookRotation;
            Vector3 _direction;
            _direction = (Target - Agent.transform.position).normalized;

            //create the rotation we need to be in to look at the target
            _lookRotation = Quaternion.LookRotation(_direction);

            //rotate us over time according to speed until we are in the required rotation
            Agent.transform.rotation = Quaternion.Slerp(Agent.transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);
            
        }
        else
        {
            pathcompleted++;
            if (pathcompleted == length)
            {
                pathcompleted = 0;
            }
        }

    }
    [IncludeInBehaviour]
    public void FlankTarget()
    {
        Enemy_Combat E = Agent.gameObject.GetComponent<Enemy_Combat>();
        int distance = 300;
        Vector3 directionforward = E.Player.transform.forward * distance;
        Vector3 directionSideways = E.Player.transform.right * distance;
        Vector3 target1 = new Vector3(E.Player.transform.position.x + directionforward.x, E.Player.transform.position.y, E.Player.transform.position.z + directionforward.z);
        Vector3 target2 = new Vector3(E.Player.transform.position.x - directionforward.x, E.Player.transform.position.y, E.Player.transform.position.z - directionforward.z);
        Vector3 target3 = new Vector3(E.Player.transform.position.x + directionSideways.x, E.Player.transform.position.y, E.Player.transform.position.z + directionSideways.z);
        Vector3 target4 = new Vector3(E.Player.transform.position.x - directionSideways.x, E.Player.transform.position.y, E.Player.transform.position.z - directionSideways.z);
        float target1Distance = Vector3.Distance(Agent.transform.position, target1);
        float target2Distance = Vector3.Distance(Agent.transform.position, target2);
        float target3Distance = Vector3.Distance(Agent.transform.position, target3);
        float target4Distance = Vector3.Distance(Agent.transform.position, target4);
        float shortestDistance = Mathf.Min(target1Distance, target2Distance, target3Distance, target4Distance);
        Vector3 target = target1;
        if (Agent.Ismoving != false)
        {          
            if (target2Distance == shortestDistance)
            {

                target = target2;

            }
            else if (target3Distance == shortestDistance)
            {

                target = target3;

            }
            else if (target4Distance == shortestDistance)
            {

                target = target4;
            }
        }
        if (target != Vector3.zero)
        {
            if (Vector3.Distance(Agent.transform.position, target) > 25.5f)
            {

                Agent.Ismoving = true;
                Agent.CustomTarget = target;
                MoveToTarget(Agent.CustomTarget);
            }
            else
            {         
                Agent.Ismoving = false;
            }
        }
    }
    [IncludeInBehaviour]
    public void FlankTarget1()
    {
        if (Agent.CustomTarget == Vector3.zero)
        {
            SelectTargetForFlankging(Vector3.zero);
        }
        if (Vector3.Distance(Agent.transform.position, Agent.CustomTarget) > 25.5f)
        {

            Agent.Ismoving = true;
            MoveToTarget(Agent.CustomTarget);
        }
        else
        {
            SelectTargetForFlankging(Agent.CustomTarget);
            Agent.Ismoving = false;
        }
        
    }
    [IncludeInBehaviour]
    public void SelectTargetForFlankging(Vector3 previousetarget)
    {
        Enemy_Combat E = Agent.gameObject.GetComponent<Enemy_Combat>();
        int distance = 400;
        Vector3 directionforward = E.Player.transform.forward * distance;
        Vector3 directionSideways = E.Player.transform.right * distance;
        Vector3 target1 = new Vector3(E.Player.transform.position.x + directionforward.x, E.Player.transform.position.y, E.Player.transform.position.z + directionforward.z);
        Vector3 target2 = new Vector3(E.Player.transform.position.x - directionforward.x, E.Player.transform.position.y, E.Player.transform.position.z - directionforward.z);
        Vector3 target3 = new Vector3(E.Player.transform.position.x + directionSideways.x, E.Player.transform.position.y, E.Player.transform.position.z + directionSideways.z);
        Vector3 target4 = new Vector3(E.Player.transform.position.x - directionSideways.x, E.Player.transform.position.y, E.Player.transform.position.z - directionSideways.z);
        float target1Distance = Vector3.Distance(Agent.transform.position, target1);
        float target2Distance = Vector3.Distance(Agent.transform.position, target2);
        float target3Distance = Vector3.Distance(Agent.transform.position, target3);
        float target4Distance = Vector3.Distance(Agent.transform.position, target4);
        float shortestDistance = Mathf.Min(target1Distance, target2Distance, target3Distance, target4Distance);
        float shortestDistancewithout1 = Mathf.Min(target2Distance, target3Distance, target4Distance);
        Vector3 target = target1;
        if (target1 == previousetarget || target1Distance != shortestDistance)
        {
            if (target2Distance == shortestDistancewithout1 && target2 != previousetarget)
            {

                target = target2;

            }
           if (target3Distance == shortestDistancewithout1 && target3 != previousetarget)
            {

                target = target3;

            }
            if (target4Distance == shortestDistancewithout1 && target4 != previousetarget)
            {

                target = target4;

            }
        }
        Agent.CustomTarget = target;
    }
    [IncludeInBehaviour]
    public void AimAtPlayer()
    {
        if(Agent.Ismoving == false)
        {
            Rigidbody rb = Agent.gameObject.GetComponent<Rigidbody>();
            float RotationSpeed = 1.2f;
            Enemy_Combat E = Agent.gameObject.GetComponent<Enemy_Combat>();
            Vector3 target = E.Player.transform.position;
            Quaternion _lookRotation;
            Vector3 _direction;
            _direction = (target - Agent.transform.position).normalized;
            //create the rotation we need to be in to look at the target
            _lookRotation = Quaternion.LookRotation(_direction);
            _lookRotation = Quaternion.EulerRotation(_lookRotation.x, _lookRotation.y, _lookRotation.z + 90);
            //rotate us over time according to speed until we are in the required rotation
            Agent.transform.rotation = Quaternion.Slerp(Agent.transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);
        }    
      
    }
    [IncludeInBehaviour]
    public void MoveAroundWaypoints()
    {
        if (!Agent.curWayPoint)
        {
            Agent.curWayPoint = Agent.WayPoints[0];
            Agent.CustomTarget = Agent.curWayPoint.transform.position;
        }
        if (Vector3.Distance(Agent.transform.position, Agent.CustomTarget) > 25f)
        {
            MoveToTarget(Agent.CustomTarget);
        }
        else
        {
            if (Agent.WayPoints != null)
            {
                Agent.curWayPoint = Agent.curWayPoint.Next();
                if (Agent.curWayPoint)
                {
                    Agent.CustomTarget = Agent.curWayPoint.transform.position;
                }
            }

        }

    }
    [IncludeInBehaviour]
    public void FireCannons()
    {
        Enemy_Combat ship = Agent.gameObject.GetComponent<Enemy_Combat>();
        ship.FireCannons();
    }

    [IncludeInBehaviour]
    public void FireMortar()
    {
        Enemy_Combat ship = Agent.gameObject.GetComponent<Enemy_Combat>();
        ship.FireMotar1();
    }
    [IncludeInBehaviour]
    public void LuanchFireBarrels()
    {
        Enemy_Combat ship = Agent.gameObject.GetComponent<Enemy_Combat>();
        ship.LuanchFireBarrels();
    }
}
