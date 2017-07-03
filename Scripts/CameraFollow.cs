using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	private Vector2 velocity;

	public float smoothTimeY;
	public float smoothTimeX;

	public GameObject player;

	public bool cameraBounded;

	public Vector3 minCameraPos;
	public Vector3 maxCameraPos;

	void Start () {

	}

	void FixedUpdate() {
		// perform smoothing calculations
		float posX = Mathf.SmoothDamp (transform.position.x, player.transform.position.x, ref velocity.x, smoothTimeX);
		float posY = Mathf.SmoothDamp (transform.position.y, player.transform.position.y, ref velocity.y, smoothTimeY);

		transform.position = new Vector3 (posX, posY, transform.position.z);

		if (cameraBounded) {
			// make sure the camera never goes outside the bounds, if they are set
			transform.position = new Vector3 (Mathf.Clamp (transform.position.x, minCameraPos.x, maxCameraPos.x),
				Mathf.Clamp(transform.position.y, minCameraPos.y, maxCameraPos.y),
				Mathf.Clamp(transform.position.z, minCameraPos.z, maxCameraPos.z));
		}
	}

	// UI buttons to make setting min/max camera positions easier
	public void SetMinCamPosition() {
		minCameraPos = gameObject.transform.position;
	}

	public void SetMaxCamPosition() {
		maxCameraPos = gameObject.transform.position;
	}
}
