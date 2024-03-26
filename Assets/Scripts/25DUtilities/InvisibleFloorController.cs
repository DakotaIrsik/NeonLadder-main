//using System;
//using UnityEngine;

//public class InvisibleFloorController : MonoBehaviour
//{
//    private BoxCollider2D floorCollider;
//    public float floorWidth = 1.0f; // Width of the floor to accommodate character movement
//    public float floorThickness = 0.1f; // Thin but solid collider for standing on
//    private BoxCollider2D temporaryFloorOnThisCollider;
//    private float distanceFromFloorToPlayer;
//    private Vector2 temporaryFloorColliderSize;

//    private void Start()
//    {
//        try
//        {

            
//            floorCollider = GameObject.FindGameObjectWithTag("Floor").GetComponent<BoxCollider2D>();
//            Debug.Log("Floor dimensions: " + floorCollider.bounds.size);
//            if (floorCollider == null)
//            {
//                Debug.LogError("No object with tag of Floor found.");
//            }
//            else
//            {
//                //use the y position of the parent object to calculate the distance from the floor to the player
//                distanceFromFloorToPlayer = transform.parent.transform.position.y - floorCollider.bounds.size.y / 2;
                
//                Debug.Log("Distance from floor to player: " + distanceFromFloorToPlayer);
//            }


//            if (temporaryFloorOnThisCollider == null)
//            {
//                Debug.LogError("No BoxCollider2D found on this object");
//            }


//            var parentTransform = transform.parent.transform.position;
//            temporaryFloorOnThisCollider.size = temporaryFloorColliderSize = new Vector2(floorWidth, distanceFromFloorToPlayer + floorThickness + 40);
//            temporaryFloorOnThisCollider.transform.position = new Vector2(parentTransform.x, parentTransform.y);

//            Debug.Log("Temporary floor dimensions: " + temporaryFloorOnThisCollider.size);

//        }
//        catch (NullReferenceException e)
//        {
//            // Log the name of the property that's null
//            Debug.LogError(e.Message);
//        }
//    }

//    private void Update()
//    {
//        Debug.Log("temporaryFloorColliderSize: " + temporaryFloorColliderSize); 
//        Debug.Log("Distance from floor to player: " + distanceFromFloorToPlayer);
//    }
//}
