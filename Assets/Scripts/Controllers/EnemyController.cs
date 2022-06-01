using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] Transform _muzzle;
    [SerializeField] NavMeshAgent _navigation;// component that allows navmesh navigation
    [SerializeField] LayerMask _whatIsPlayer;
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] float _timeBetweenAttacks = 1f;
    [SerializeField] float _wanderRange = 5f;
    [SerializeField] float _sightRange = 10f;
    [SerializeField] float _attackRange = 5f;
    
    bool _playerInAttackRange, _playerInSightRange;
    float _lastAttackTime = float.MinValue;
    
    void Update ()
    {
        //Check for Sight and Attack Range
        Vector3 position = transform.position;
        _playerInSightRange = Physics.CheckSphere( position , _sightRange , _whatIsPlayer );
        _playerInAttackRange = Physics.CheckSphere( position , _attackRange , _whatIsPlayer );

        if( !_playerInSightRange && !_playerInAttackRange )         Wander();
        else if( _playerInSightRange && !_playerInAttackRange )     MoveInOnPlayer();
        else if( _playerInSightRange && _playerInAttackRange )      AttackPlayer();
        //else if (!playerInLineOfSight && playerInAttackRange) FlankPlayer();
    }

    void Wander ()
    {
        if( _navigation.isStopped )
        // if( _navigation.remainingDistance <= _navigation.stoppingDistance )
        {
            _navigation.isStopped = false;
            _navigation.updateRotation = true;

            Vector3 dst = FindRandomDestination( transform.position , _wanderRange );
            _navigation.SetDestination( dst );
        }
    }

    Vector3 FindRandomDestination ( Vector3 start , float maxRange )
	{
		Vector2 randomDirection = Random.insideUnitCircle * maxRange;
		Vector3 p = start + new Vector3( randomDirection.x , 0 , randomDirection.y );
		if( NavMesh.SamplePosition( p , out var hit , maxRange , NavMesh.AllAreas ) )
			return hit.position;
		else
			return start;
	}
    
    void MoveInOnPlayer ()
    {
        _navigation.SetDestination( _target.position );
    }

    void AttackPlayer ()
    {
        Vector3 position = transform.position;
        Vector3 targetPosition = _target.position;
        
        // rotate body to face the enemy:
        _navigation.isStopped = true;
        _navigation.updateRotation = false;
        transform.rotation = Quaternion.LookRotation(
            Vector3.Scale( targetPosition - position , new Vector3(1,0,1) ) ,
            Vector3.up
        );

        // point muzzle at target position:
        _muzzle.LookAt( targetPosition );
        Debug.DrawLine( _muzzle.position , targetPosition , Color.red );

        // attack code:
        if( Time.time > _lastAttackTime+_timeBetweenAttacks )
        {
            _lastAttackTime = Time.time;

            if( _projectilePrefab!=null )
            {
                var instance = Instantiate( _projectilePrefab , _muzzle.position , _muzzle.rotation );
                Rigidbody rb = instance.GetComponent<Rigidbody>();
                if( rb!=null )
                {
                    rb.AddForce( _muzzle.forward*32f , ForceMode.Impulse );
                }
            }
        }
    }

    #if UNITY_EDITOR
    void OnDrawGizmos ()
    {
        if( Application.isPlaying )
        {
            Vector3 position = transform.position;

            if( _navigation.hasPath )
            {
                UnityEditor.Handles.color = Color.cyan;
                UnityEditor.Handles.DrawPolyLine( _navigation.path.corners );
            }

            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.CircleHandleCap( -1 , position , Quaternion.Euler(90,0,0) , _sightRange , EventType.Repaint );
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.CircleHandleCap( -1 , position , Quaternion.Euler(90,0,0) , _attackRange , EventType.Repaint );

            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.magenta;
            UnityEditor.Handles.Label(
                position ,
                $"nav stopped: {_navigation.isStopped}\nnav rotation: {(_navigation.updateRotation?"auto":"manual")}" ,
                labelStyle
            );
        }
    }
    #endif

    //void FlankPlayer ()
   // {

   // }
    /*
    int LineOfSight(bool lineOfSight)
    {
        
    }
    */
}
