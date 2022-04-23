using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public float Y_Offset;
    public bool ReachedBossArea;
    public bool BossIsDefeated;

    private Transform PlayerRef;
    private bool CanFollowPlayer = true;

    // Start is called before the first frame update
    void Start()
    {
        PlayerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void LateUpdate()
    {

        if (ReachedBossArea)
        {
            CanFollowPlayer = false;
            transform.position = Vector3.Lerp(transform.position, new Vector3(263f, -12f, transform.position.z), .05f);

            if (GetComponent<Camera>().orthographicSize < 11f)
            {
                GetComponent<Camera>().orthographicSize += 0.2f;
            }
            else
            {
                ReachedBossArea = false;
            }
        }

        if (BossIsDefeated && !CanFollowPlayer)
        {
            if (GetComponent<Camera>().orthographicSize > 5f)
            {
                GetComponent<Camera>().orthographicSize -= 0.1f;
            }

            transform.position = new Vector3(PlayerRef.position.x, PlayerRef.position.y + Y_Offset, transform.position.z);
        }

        if (CanFollowPlayer)
        {
            transform.position = new Vector3(PlayerRef.position.x, PlayerRef.position.y + Y_Offset, transform.position.z);
        }
    }
}
