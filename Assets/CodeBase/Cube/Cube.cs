using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Audio;
using CodeBase.Logic;
using CodeBase.Units;
using CodeBase.Units.Hero;
using DG.Tweening;
using UnityEngine;
using Vertx.Debugging;
using Zenject;

namespace CodeBase.Cube
{
	[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Interactable))]
	public class Cube : MonoBehaviour, IHoldingBtnActivator
	{
		[SerializeField] private float _offsetX = 0.1f;
		[SerializeField] private float _pickUpDuration = 0.1f;
		[Space(10)]
		[SerializeField] private ContactFilter2D _physicalObjFilter;
		[Space(10)]
		[SerializeField] private PushSettings _stayPushSettings;
		[SerializeField] private PushSettings _walkPushSettings;
		[Space(10)]
		[SerializeField] private AudioClip _pickUpSfx;
		[SerializeField] private AudioClip _pushSfx;
		[SerializeField] private AudioClip _landSfx;
		
		private Interactable _interactable;
		private Rigidbody2D _rigidbody;
		private BoxCollider2D _collider;
		private HeroMovement _heroMovement;
		private HeroInteractable _heroInteractable;
		private HeroDeath _heroDeath;
		private bool _pickedUp = false;
		private int _defaultLayer;
		private Sequence _push;
		private SFXPlayer _sfxPlayer;

		[Inject]
		private void Construct(SFXPlayer sfxPlayer)
		{
			_sfxPlayer = sfxPlayer;
		}
		
		private void Awake()
		{
			_interactable = GetComponent<Interactable>();
			_rigidbody = GetComponent<Rigidbody2D>();
			_collider = GetComponent<BoxCollider2D>();
		}
		
		private void OnEnable()
		{
			_interactable.Interacted += OnInteract;
		}

		private void OnDisable()
		{
			_interactable.Interacted -= OnInteract;	
		}

		private void OnDestroy()
		{
			if (_heroDeath && _pickedUp)
				_heroDeath.Died -= OnHeroDie;
		}

		private void Start()
		{
			_defaultLayer = gameObject.layer;
		}

		private void OnCollisionEnter2D(Collision2D col)
		{
			StopPush();
		}

		public void StopPush() => _push?.Kill();

		private void OnInteract(HeroInteractable heroInteractable)
		{
			_heroMovement = heroInteractable.GetComponent<HeroMovement>();

			if (_heroDeath == null)
			{
				_heroDeath = heroInteractable.GetComponent<HeroDeath>();
				_heroDeath.Died += OnHeroDie;
			}
			
			if(_pickedUp)
				Drop();
			else
				PickUp();
		}

		private void OnHeroDie(UnitDeath obj) => Drop();

		private void PickUp()
		{
			_sfxPlayer.Play(_pickUpSfx);
			_pickedUp = true;

			gameObject.layer = 0;
			_collider.isTrigger = true;
			_rigidbody.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
			transform.SetParent(_heroMovement.transform);

			Vector3 pickedPosition = new Vector3(_offsetX, 0, 0);
			transform.DOLocalMove(pickedPosition, _pickUpDuration);
			
			_interactable.StartInteraction();
		}

		private void Drop()
		{
			_heroDeath.Died -= OnHeroDie;
			
			_pickedUp = false;
			_interactable.FinishInteraction();
			
			gameObject.layer = _defaultLayer;
			_collider.isTrigger = false;
			_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
			transform.SetParent(null);
			transform.localScale = Vector3.one;
			transform.rotation = Quaternion.Euler(Vector3.zero);

			Push();
		}

		private void Push()
		{
			_sfxPlayer.Play(_pushSfx);

			float walkPushThreshold = 0.3f;
			float pushDistance = _heroMovement.SpeedPercent > walkPushThreshold ? _walkPushSettings.PushDistance : _stayPushSettings.PushDistance;
			float pushHeight = _heroMovement.SpeedPercent > walkPushThreshold ? _walkPushSettings.PushHeight : _stayPushSettings.PushHeight;
			float pushSecondsPerUnit = _heroMovement.SpeedPercent > walkPushThreshold ? _walkPushSettings.PushSecondsPerUnit : _stayPushSettings.PushSecondsPerUnit;

			Vector3 currentPosition = transform.position;
			Vector3 targetPosition = currentPosition + Vector3.right * _heroMovement.MoveDirection * pushDistance;

			if (TouchGround(targetPosition) == false)
			{
				float newPositionY = GetClosestGroundHeight(targetPosition);
				float differenceY = Mathf.Abs(newPositionY - targetPosition.y);
				targetPosition.y = newPositionY;

				pushHeight += differenceY;
			}

			float pushDuration = Vector3.Distance(currentPosition, targetPosition) * pushSecondsPerUnit;
			
			_push = transform.DOJump(targetPosition, pushHeight, 1, pushDuration)
				.OnKill(PlayLandSfx)
				.OnComplete(PlayLandSfx);
		}

		private void PlayLandSfx() => _sfxPlayer.Play(_landSfx);
		
		private bool TouchGround(Vector2 at)
		{
			List<Collider2D> overlapped = new List<Collider2D>();
			Physics2D.OverlapBox(at, _collider.bounds.size, 0, _physicalObjFilter, overlapped);
			return overlapped.Count(obj => obj.gameObject != gameObject) > 0;
		}

		private float GetClosestGroundHeight(Vector2 point)
		{
			float leftRaycastGroundHeight = GetGroundHeight(point - Vector2.right * _collider.bounds.extents.x);
			float centerRaycastGroundHeight = GetGroundHeight(point);
			float rightRaycastGroundHeight = GetGroundHeight(point + Vector2.right * _collider.bounds.extents.x);
			
			List<float> list = new List<float> { leftRaycastGroundHeight, centerRaycastGroundHeight, rightRaycastGroundHeight };
			float closest = list.Aggregate((x,y) => Math.Abs(x - point.y) < Math.Abs(y - point.y) ? x : y);
			
			return closest;
		}

		private float GetGroundHeight(Vector2 point)
		{
			List<RaycastHit2D> touched = new List<RaycastHit2D>();
			
			Physics2D.Raycast(point, Vector2.down, _physicalObjFilter, touched);
			touched = touched.Where(obj => obj.transform.gameObject != gameObject).ToList();
			
			float cubePositionOnGround = touched.Count > 0 ? touched[0].point.y + _collider.bounds.extents.y : point.y;
			
			return cubePositionOnGround;
		}
	}
}