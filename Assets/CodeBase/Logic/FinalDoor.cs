using System;
using CodeBase.Infrastructure;
using UnityEngine;
using Zenject;

namespace CodeBase.Logic
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class FinalDoor : MonoBehaviour
	{
		[SerializeField] private GameObject _visual;
		
		private GameStatus _gameStatus;
		private BoxCollider2D _collider;

		[Inject]
		private void Construct(GameStatus gameStatus)
		{
			_gameStatus = gameStatus;
		}

		private void Awake()
		{
			_collider = GetComponent<BoxCollider2D>();
		}

		private void OnEnable()
		{
			_gameStatus.Won += Open;
		}

		private void OnDisable()
		{
			_gameStatus.Won -= Open;
		}

		private void Open()
		{
			_visual.SetActive(false);
			_collider.enabled = false;
		}
	}
}