using UnityEngine;
using System.Collections;

public class RocketControl : MonoBehaviour {
	public int enginePower = 1500;
	public int rotationSpeed = 300;
	public int maxSpeed = 30;
	public Sprite destroyedRocketSprite;
	public string loseMessage = "LOOOOSEEER!!!! HA! HA! HA!";
	public string winMessage = "OK you won... Next level...";

	private bool b_isAlive = true;
	private LoadMap map_class;

	// Update is called once per frame
	void Update () 
	{
		if (b_isAlive)
		{
			if (Input.GetButtonDown ("Jump")) 
				particleEmitter.emit = true;
			if (Input.GetButton ("Jump"))
				rigidbody2D.AddForce ((Vector2)transform.up * enginePower * Time.deltaTime);
			if (Input.GetButtonUp ("Jump")) 
				particleEmitter.emit = false;

			float rotation = Input.GetAxis ("Horizontal");
			if (rotation != 0) {
				rigidbody2D.fixedAngle = true;
				if (rotation > 0)
					transform.Rotate (0, 0, -1 * rotationSpeed * Time.deltaTime);
				else
					transform.Rotate (0, 0, rotationSpeed * Time.deltaTime);
				rigidbody2D.fixedAngle = false;
				//rigidbody2D.AddTorque(0.0f);
				//rigidbody2D.AddTorque(-rotation * rotationSpeed * Time.deltaTime);
				//Debug.Log(rotation);
			}
		}
		checkSpeed ();
	}

	void OnCollisionEnter2D(Collision2D myCollision) 
	{   
		if (b_isAlive) 
		{
			b_isAlive = false;
			rigidbody2D.fixedAngle = false;
			if (destroyedRocketSprite) {
					gameObject.GetComponent<SpriteRenderer> ().sprite = destroyedRocketSprite;
			}
			map_class = GameObject.Find ("map").GetComponent<LoadMap>();
			//Destroy (gameObject, 5);
			map_class.showMessage(loseMessage, Color.red);
			StartCoroutine (restartScene ());

			//Debug.Log(myCollision.gameObject.name);  
		}
	}
	IEnumerator restartScene()
	{
		yield return new WaitForSeconds(2.0f);
		map_class.hideMessage();
		map_class.restartLevel();
	}

	void OnTriggerEnter2D(Collider2D myCollision) 
	{   
		if (b_isAlive && myCollision.gameObject.name == "end_location") 
		{
			map_class = GameObject.Find ("map").GetComponent<LoadMap>();
			b_isAlive = false;
			map_class.showMessage(winMessage, Color.green);
			StartCoroutine(startNextLevel());
		}
		//Debug.Log(myCollision.gameObject.name);  
	}
	IEnumerator startNextLevel()
	{
		yield return new WaitForSeconds(2.0f);
		map_class.hideMessage();
		map_class.loadNextLevel();
	}
	
	void checkSpeed()
	{
		float x=rigidbody2D.velocity.x, y=rigidbody2D.velocity.y;
		if (x > maxSpeed)
		{
			x = maxSpeed;
		}
		else if (x < -maxSpeed)
		{
			x = -maxSpeed;
		}
		if (y > maxSpeed)
		{
			y = maxSpeed;
		}
		else if (y < -maxSpeed)
		{
			y = -maxSpeed;
		}
		rigidbody2D.velocity = new Vector2(x, y);
	}

}
