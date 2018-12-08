using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision : MonoBehaviour {

    public bool blockColl = false;

    private void OnTriggerExit(Collider other)
    {
        if (!blockColl)
        {
            blockColl = true;
            GameController parentScript = transform.parent.GetComponent<GameController>();
            parentScript.CollisionFromChild(transform.tag, other.gameObject.name);
        }
    }

    public void unblockCollision()
    {
        blockColl = false;
    }
}
