using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public List<GameObject> currentRopeParts;
    public GameObject rope;
    public float ropeWidth;
    public float ropeHeight;
    public int amountOfDetails;
    
    private void Start()
    {
        for (var i = 0; i < amountOfDetails; i++)
        {
            GameObject newRopePart = Instantiate(rope, transform.position - new Vector3(0f, ropeHeight / amountOfDetails, 0f), Quaternion.identity);

            newRopePart.transform.localScale = new Vector3(ropeWidth, ropeHeight / amountOfDetails, ropeWidth);

            SpringJoint joint = newRopePart.AddComponent<SpringJoint>();

            joint.spring = 10f;
            joint.damper = 1f;

            currentRopeParts.Add(newRopePart);
        }
    }
}
