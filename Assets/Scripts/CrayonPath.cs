using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CrayonPath : MonoBehaviour
{
	LineRenderer pathRenderer;

	private void Awake()
	{
		pathRenderer = GetComponent<LineRenderer>();
	}

	public void Display(Stack<Node> path)
	{
		Vector3[] positions = path.Select(n => new Vector3(n.XPos + 0.5f, 0, n.YPos + 0.5f)).ToArray();

		pathRenderer.positionCount = positions.Length;
		pathRenderer.SetPositions(positions);

		pathRenderer.Simplify(0.1f);
	}
}
