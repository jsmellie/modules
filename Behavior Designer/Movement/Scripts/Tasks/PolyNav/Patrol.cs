using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement.PolyNav
{
    [TaskDescription("Patrol around the specified waypoints using PolyNav.")]
    [TaskCategory("Movement/PolyNav")]
    [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=7")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PatrolIcon.png")]
    public class Patrol : PolyNavMovement
    {
        [Tooltip("Should the agent patrol the waypoints randomly?")]
        public SharedBool randomPatrol = false;
        [Tooltip("The length of time that the agent should pause when arriving at a waypoint")]
        public SharedFloat waypointPauseDuration = 0;
        [Tooltip("The waypoints to move to")]
        public SharedGameObjectList waypoints;

        [Space()]
        [Tooltip("The LayerMask of the objects that we are searching for")]
        public LayerMask objectLayerMask;
        [Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
        public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
        [Tooltip("The field of view angle of the agent (in degrees)")]
        public SharedFloat fieldOfViewAngle = 90;
        [Tooltip("The distance that the agent can see")]
        public SharedFloat viewDistance = 10;
        [Tooltip("The object that is within sight")]
        public SharedGameObject returnedObject;

        // The current index that we are heading towards within the waypoints array
        private int waypointIndex;
        private float waypointReachedTime;

        public override void OnStart()
        {
            base.OnStart();

            // initially move towards the closest waypoint
            float distance = Mathf.Infinity;
            float localDistance;
            for (int i = 0; i < waypoints.Value.Count; ++i)
            {
                if ((localDistance = Vector3.Magnitude(transform.position - waypoints.Value[i].transform.position)) < distance)
                {
                    distance = localDistance;
                    waypointIndex = i;
                }
            }
            waypointReachedTime = -waypointPauseDuration.Value;
            SetDestination(Target());
        }

        // Patrol around the different waypoints specified in the waypoint array. Always return a task status of running. 
        public override TaskStatus OnUpdate()
        {
            //Check to see if the object is within sight
            returnedObject.Value = MovementUtility.WithinSight2D(transform, Vector3.zero, fieldOfViewAngle.Value, viewDistance.Value, objectLayerMask, Vector3.zero, ignoreLayerMask);

            //If we found something, return success
            if (returnedObject.Value != null)
            {
                return TaskStatus.Success;
            }

            if (HasArrived())
            {
                if (waypointReachedTime == -waypointPauseDuration.Value)
                {
                    waypointReachedTime = Time.time;
                }
                // wait the required duration before switching waypoints.
                if (waypointReachedTime + waypointPauseDuration.Value <= Time.time)
                {
                    if (randomPatrol.Value)
                    {
                        if (waypoints.Value.Count == 1)
                        {
                            waypointIndex = 0;
                        }
                        else
                        {
                            // prevent the same waypoint from being selected
                            var newWaypointIndex = waypointIndex;
                            while (newWaypointIndex == waypointIndex)
                            {
                                newWaypointIndex = Random.Range(0, waypoints.Value.Count - 1);
                            }
                            waypointIndex = newWaypointIndex;
                        }
                    }
                    else
                    {
                        waypointIndex = (waypointIndex + 1) % waypoints.Value.Count;
                    }
                    SetDestination(Target());
                    waypointReachedTime = -waypointPauseDuration.Value;
                }
            }

            return TaskStatus.Running;
        }

        // Return the current waypoint index position
        private Vector3 Target()
        {
            if (waypointIndex >= waypoints.Value.Count)
            {
                return transform.position;
            }
            return waypoints.Value[waypointIndex].transform.position;
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();

            randomPatrol = false;
            waypointPauseDuration = 0;
            waypoints = null;

            fieldOfViewAngle = 90;
            viewDistance = 10;
        }

        // Draw a gizmo indicating a patrol 
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (waypoints == null)
            {
                return;
            }
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            for (int i = 0; i < waypoints.Value.Count; ++i)
            {
                if (waypoints.Value[i] != null)
                {
                    UnityEditor.Handles.SphereCap(0, waypoints.Value[i].transform.position, waypoints.Value[i].transform.rotation, 1);
                }
            }
            UnityEditor.Handles.color = oldColor;

            MovementUtility.DrawLineOfSight(Owner.transform, Vector3.zero, fieldOfViewAngle.Value, viewDistance.Value, true);
#endif
        }
    }
}