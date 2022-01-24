using UnityEngine;

public class SlideAudio : MonoBehaviour
{
	public PlayerMovement player;

	private AudioSource sfx;

	public AudioSource startSlideSfx;

	public static SlideAudio Instance { get; set; }

	private void Awake()
	{
		sfx = GetComponent<AudioSource>();
	}

	private void Start()
	{
		Instance = this;
	}

	private void Update()
	{
		float b = 0f;
		if (player.isGrounded && player.isSliding)
		{
			b = player.GetVelocity().magnitude;
			b = Mathf.Clamp(b * 0.0125f, 0f, 0.6f);
		}
		sfx.volume = Mathf.Lerp(sfx.volume, b, Time.deltaTime * 15f);
	}

	public void PlayStartSlide()
	{
		startSlideSfx.Play();
	}
}
