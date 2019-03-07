using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {
    //Abstract classes are super classes that can only be extended, and cannot be directly instantiated(create an object).

    public float moveTime = 0.1f;
    public LayerMask blockingLayer; //This is the layer in which we're going to check Collision as we're moving to determine if a space is open to be moved in to.


    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2d;
    private float inverseMoveTime; //Make the movement calculations more efficient

    // Use this for initialization
    protected virtual void Start () { //protected virtual can be overriden by their inheriting classes
                                      //protected means that it is visible only inside this class and classes derived from it.
                                      //virtual means that it can be overriden in derived classes.
        boxCollider = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime; //We divide because is more efficient computationally. EFFICIENT

    }

    protected bool Move (int xDir, int yDir, out RaycastHit2D hit) //out keyword causes argument to be passed by reference. We use it to return the raycasthit2d too apart from the bool
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false; //When we're casting our own ray we're making sure that we will not hit our own collider
        hit = Physics2D.Linecast(start, end, blockingLayer); //Cast a line from our start point to our end pont checking collision on blocking layer
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            //If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
            StartCoroutine(SmoothMovement(end));
            return true; //we were able to move
        }

        return false;
    }

    protected IEnumerator SmoothMovement (Vector3 end) //end is to were will move to
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude; //calculate the remaining distance to move based on the sqrMagnitude

        while (sqrRemainingDistance > float.Epsilon) //not 0 because math it's not precise
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime); //rb2d.position is where the object is, and end is were the object wants to go
            rb2d.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude; //recalculate the remaining distance after we've moved
            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
    }
    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T : Component //We use generic because we don't know in advance what parameters are talking about bc it can be a player interacting with a wall or a zombie with a player
    {
        //Hit will store whatever our linecast hits when Move is called.
        RaycastHit2D hit;

        //Set canMove to true if Move was successful, false if failed.
        bool canMove = Move (xDir, yDir, out hit);

        //Check if nothing was hit by linecast
        if (hit.transform == null)
            //If nothing was hit, return and don't execute further code.
            return;

        //Get a component reference to the component of type T attached to the object that was hit
        T hitComponent = hit.transform.GetComponent <T> ();

        //If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
        if (!canMove && hitComponent != null)
        {
            //Call the OnCantMove function and pass it hitComponent as a parameter.
            OnCantMove(hitComponent);
        }
    }

    protected abstract void OnCantMove <T> (T component)
        where T : Component;
}
