using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SlingshotRubberPresenter : PresenterBehaviour<Slingshot>
{	
	[SerializeField] private Transform _leftAnchor;
	[SerializeField] private Transform _rightAnchor;
	[SerializeField] private Transform _leftBone;
	[SerializeField] private Transform _rightBone;

	private void Update()
	{
		_leftBone.position = _leftAnchor.position;
		_rightBone.position = _rightAnchor.position;
	}

}
