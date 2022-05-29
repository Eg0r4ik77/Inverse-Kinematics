using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLegsMovement : MonoBehaviour
{
    [SerializeField] private GameObject[] legTips;
    [SerializeField] private GameObject[] targets;
    [SerializeField] private float bodyOffset;
    private Vector3[] positions = new Vector3[4];

    private float maxDist = 1.5f;
    private bool[] isLegMoving = new bool[4] { false, false, false, false };

    private float groundHeight;

    private Vector3[] prevFramePos = new Vector3[4];

    private float speed;
    private Vector3 prevPos;
    private float prevRot;

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            prevFramePos[i] = legTips[i].transform.position;
        }
        prevPos = transform.position;
        prevRot = transform.rotation.y;
        groundHeight = legTips[0].transform.position.y;
    }

    void Update()
    {
        RaycastFromTargets();
        MoveLegs();
        OffsetBody();
        speed = Vector3.Distance(transform.position, prevPos) / Time.deltaTime;
        prevPos = transform.position;
        prevRot = transform.rotation.y;
    }

    void OffsetBody()
    {
        float currentHeight = 0;
        foreach (var legTip in legTips)
            currentHeight += legTip.transform.position.y - groundHeight;
        currentHeight /= 4;
        transform.position = new Vector3(transform.position.x, currentHeight + bodyOffset, transform.position.z);
    }

    void RaycastFromTargets()
    {
        RaycastHit[] hits;
        for (int i = 0; i < 4; i++)
        {
            float minDistance = 5;
            hits = Physics.RaycastAll(targets[i].transform.position, Vector3.down);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject.GetComponent<World>())
                {
                    if (Vector3.Distance(targets[i].transform.position, hit.point) < minDistance)
                        positions[i] = hit.point;
                    minDistance = Vector3.Distance(targets[i].transform.position, hit.point);
                }
            }
        }
    }

    void MoveLegs()
    {
        for (int i = 0; i < 4; i++)
        {
            if (Vector3.Distance(legTips[i].transform.position, positions[i]) > maxDist && !isLegMoving[i])
            {
                if ((i == 2 || i == 0) && (!isLegMoving[1] && !isLegMoving[3]) || ((i == 3 || i == 1) && (!isLegMoving[2] && !isLegMoving[0])))
                {
                    isLegMoving[i] = true;
                    StartCoroutine(MoveLeg(legTips[i], positions[i], i));
                }
                else
                    legTips[i].transform.position = prevFramePos[i];
            }
            else if (!isLegMoving[i])
            {
                legTips[i].transform.position = prevFramePos[i];
            }
            prevFramePos[i] = legTips[i].transform.position;
        }
    }

    public IEnumerator MoveLeg(GameObject tip, Vector3 target, int i)
    {
        Vector3 _start = tip.transform.position;
        float totalMovementTime = 0.2f;
        float currentMovementTime = 0f;
        while (true)
        {
            currentMovementTime += Time.deltaTime;
            tip.transform.position = Vector3.Lerp(_start, target, currentMovementTime / totalMovementTime);
            if (Vector3.Distance(tip.transform.position, target) <= 0.03f)
            {
                isLegMoving[i] = false;
                break;
            }
            yield return null;
        }
    }
}

