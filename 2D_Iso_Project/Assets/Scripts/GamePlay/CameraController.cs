using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
    public static CameraController Instance { get; private set; }

    //Reference
    public Camera GameplayCamera;
	public Transform followTarget;

    // params
	public float moveSpeed;
    public Vector2 offset = new Vector2(0f, 2f);

    private Vector3 startingPosition;
    private Vector3 targetPos;

    void Awake()
	{
        Instance = this;
        startingPosition = transform.position;
        GameplayCamera = GetComponent<Camera>();
    }

	void Update () 
	{
		if(followTarget != null)
		{
			targetPos = new Vector3(followTarget.position.x + offset.x, 
                followTarget.position.y + offset.y, 
                transform.position.z);
			Vector3 velocity = (targetPos - transform.position) * moveSpeed;
			transform.position = Vector3.SmoothDamp (transform.position, targetPos, ref velocity, 1.0f, Time.deltaTime);
		}
	}
}

