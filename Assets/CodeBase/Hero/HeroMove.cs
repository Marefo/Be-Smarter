using CodeBase.Services;
using UnityEngine;
using Vertx.Debugging;
using Zenject;

namespace CodeBase.Hero
{
	[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
	public class HeroMove : MonoBehaviour
	{
		[SerializeField] private float _checkGroundRadius = 0.5f;
		
		private float _moveSpeed = 2;
		private float _jumpForce = 3;
		
		private IInputService _inputService;
		private Rigidbody2D _rigidbody;
		private BoxCollider2D _collider;
		private SpriteRenderer _sprite;
		private Vector2 _colliderBottomCenter;
		private bool _isGrounded;
		private LayerMask _groundLayer;

		[Inject]
		private void Construct(IInputService inputService)
		{
			_inputService = inputService;
			_rigidbody = GetComponent<Rigidbody2D>();
			_collider = GetComponent<BoxCollider2D>();
			_sprite = GetComponent<SpriteRenderer>();
		}
		
		private void Start()
		{
			_groundLayer = 1 << LayerMask.NameToLayer("Ground");
			
			_inputService.JumpBtnPressed += Jump;
		}

		private void OnDestroy()
		{
			_inputService.JumpBtnPressed -= Jump;
		}

		private void Update()
		{
			CheckGrounded();
			DebugUtils.DrawCircle2D(_colliderBottomCenter, _checkGroundRadius, Color.red);


			FlipSprite();
			Move();
		}

		private void FlipSprite()
		{
			if (_inputService.AxisX > 0)
				_sprite.flipX = false;
			else if (_inputService.AxisX < 0)
				_sprite.flipX = true;
		}

		private void Move()
		{
			transform.position += new Vector3(_inputService.AxisX, 0, 0) * _moveSpeed * Time.deltaTime;
		}

		private void CheckGrounded()
		{
			_colliderBottomCenter = new Vector2(_collider.bounds.center.x, _collider.bounds.min.y);
			Collider2D[] overlapped = new Collider2D[1];
			_isGrounded = Physics2D.OverlapCircleNonAlloc(_colliderBottomCenter, _checkGroundRadius, overlapped, _groundLayer) > 0;
		}

		private void Jump()
		{
			if(_isGrounded == false) return;
			
			_rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
		}
	}
}