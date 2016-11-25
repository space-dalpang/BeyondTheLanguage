namespace BeyondTheLanguage
{
    using UnityEngine;

    public class MouseCamera : MonoBehaviour
    {
        public Transform target;
        public float distance = 10.0f;

        public float xSpeed = 250.0f;
        public float ySpeed = 120.0f;

        public float yMinLimit = -20f;
        public float yMaxLimit = 80f;

        private float x;
        private float y;

        private void Start()
        {
            var angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;

            // Make the rigid body not change rotation
            if (GetComponent<Rigidbody>())
                GetComponent<Rigidbody>().freezeRotation = true;

//			var rotation = Quaternion.Euler(y, x, 0);
//			var position = rotation*new Vector3(0.0f, 0.0f, -distance);
//
//			transform.rotation = rotation;
//			transform.position = position;
			prevTarget = target;
        }

        public void UpdateAngles()
        {
            var angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
        }

		private Transform prevTarget = null;

		private void Update()
		{
			float range = Vector3.Distance (transform.position, target.position);

			if (range >= 10f && (prevTarget != target)) {
				var speed = 100f;
//				transform.localPosition = Vector3.MoveTowards (transform.localPosition, target.position + new Vector3(0, 0, -10), speed * Time.deltaTime);
//				transform.rotation = Quaternion.RotateTowards (transform.localRotation, new Quaternion(0, 0, 0, 0), speed * Time.deltaTime);

//				if (Vector3.Distance (target.position + new Vector3 (0, 0, -10), transform.position) < 0.1f) {
//					Debug.Log ("done");	
//					prevTarget = target;
//				}

				Vector3 dir = target.transform.position - transform.position;
				dir = dir.normalized;
				transform.Translate (dir * speed * Time.deltaTime, Space.World);

				var targetRotation = Quaternion.LookRotation (target.transform.position - transform.position);
				transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, 3 * Time.deltaTime);
//				transform.LookAt (target);

			} else {
				prevTarget = target;
			}
		}

        private void LateUpdate()
        {
            if (target && Input.GetKey(KeyCode.Mouse1))
            {
				//todo : keep rotation around object when clicked after camera transition
                x += Input.GetAxis("Mouse X")*xSpeed*0.02f;
                y -= Input.GetAxis("Mouse Y")*ySpeed*0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);

				var rotation = Quaternion.Euler(y, x, 0);
                var position = rotation*new Vector3(0.0f, 0.0f, -distance) + target.position;

                transform.rotation = rotation;
                transform.position = position;
            }
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
    }
}