using System.Collections;
using System.Collections.Generic;
using Unfold;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
	[SerializeField] private Button _directButton;
	[SerializeField] private Button _radialButton;
	[SerializeField] private Button _randomButton;
	[SerializeField] private Button _heightMapButton;
	[SerializeField] private Toggle _unfoldToggle;
	[SerializeField] private Button _playButton;

	[SerializeField] private AnimatedMesh _animatedMesh;

	private class PlayData
	{
		public AnimationType Type;
		public bool Unfold;
	}
	private readonly Queue<PlayData> _playData = new Queue<PlayData>();

	private void Awake()
	{
		_directButton.onClick.AddListener(OnDirectButtonClicked);
		_radialButton.onClick.AddListener(OnRadialButtonClicked);
		_randomButton.onClick.AddListener(OnRandomButtonClicked);
		_heightMapButton.onClick.AddListener(OnHeightMapButtonClicked);
		_animatedMesh.AnimationFinished += OnAnimationFinished;
		_playButton.onClick.AddListener(OnPlayButtonClicked);
	}

	private void OnDirectButtonClicked()
	{
		_playData.Enqueue(new PlayData()
		{
			Type = AnimationType.Direct,
			Unfold = _unfoldToggle.isOn
		});
	}

	private void OnRadialButtonClicked()
	{
		_playData.Enqueue(new PlayData()
		{
			Type = AnimationType.Radial,
			Unfold = _unfoldToggle.isOn
		});
	}

	private void OnRandomButtonClicked()
	{
		_playData.Enqueue(new PlayData()
		{
			Type = AnimationType.Random,
			Unfold = _unfoldToggle.isOn
		});
	}

	private void OnHeightMapButtonClicked()
	{
		_playData.Enqueue(new PlayData()
		{
			Type = AnimationType.HeightMap,
			Unfold = _unfoldToggle.isOn
		});
	}

	private void OnPlayButtonClicked()
	{
		DequeueAndPlay();
	}

	private void OnAnimationFinished()
	{
		Debug.Log("Finished");
		DequeueAndPlay();
	}

	private void DequeueAndPlay()
	{
		if (_playData.Count > 0)
		{
			var playData = _playData.Dequeue();
			_animatedMesh.StartAnimation(playData.Type, playData.Unfold);
		}
	}
}
