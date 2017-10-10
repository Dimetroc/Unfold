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
	[SerializeField] private Toggle _unfoldToggle;

	[SerializeField] private AnimatedMesh _animatedMesh;

	private void Awake()
	{
		_directButton.onClick.AddListener(OnDirectButtonClicked);
		_radialButton.onClick.AddListener(OnRadialButtonClicked);
		_randomButton.onClick.AddListener(OnRandomButtonClicked);
		_animatedMesh.AnimationFinished += OnAnimationFinished;
	}

	private void OnDirectButtonClicked()
	{
		_animatedMesh.StartAnimation(AnimationType.Direct, _unfoldToggle.isOn);
	}

	private void OnRadialButtonClicked()
	{
		_animatedMesh.StartAnimation(AnimationType.Radial, _unfoldToggle.isOn);
	}

	private void OnRandomButtonClicked()
	{
		_animatedMesh.StartAnimation(AnimationType.Random, _unfoldToggle.isOn);
	}

	private void OnAnimationFinished()
	{
		Debug.Log("Finished");
	}
}
