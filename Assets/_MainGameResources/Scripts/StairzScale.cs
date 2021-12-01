using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class StairzScale : MonoBehaviour
{
	#region var
	[SerializeField] float scaleMargin;
	[SerializeField] Transform stairs;

	[SerializeField] private MeshRenderer []stairsMeshes;

	[SerializeField] private TextMeshPro textPrefab;

	[SerializeField] private Vector3 textOffset = new Vector3(-7, 2.8f, 3f);
	#endregion


	#region unity 

	[ContextMenu("Scale")]
	private void Start()
	{
		for(int i = 0; i < stairs.childCount; i++)
		{
			Transform child = stairs.GetChild( i );
			child.localScale = new Vector3(scaleMargin, 1,1);
			child.localPosition -= new Vector3(scaleMargin, 0,0);
			scaleMargin -= .01f;
		}
	}

	[ContextMenu("GetStairsMeshes")]
	void GetStairsMeshes()
	{
		stairsMeshes = transform.GetComponentsInChildren<MeshRenderer>();
		foreach (var mesh in stairsMeshes)
		{
			mesh.transform.SetParent(transform);
		}
	}
	
	[ContextMenu("PlaceTexts")]
	void PlaceTexts()
	{
		float number = 1.0f;
		for (int i = 0; i < stairsMeshes.Length; i++)
		{
			var t = Instantiate(textPrefab, 
				stairsMeshes[i].transform.position + textOffset,
				textPrefab.transform.rotation,
				transform);
			t.text = "x" + number.ToString("F1");
			number += .1f;
		}
	}
	
	
	[ContextMenu("TextsPositioning")]
	void TextsPositioning()
	{
		TextMeshPro[] texts = GetComponentsInChildren<TextMeshPro>(); 
		for (int i = 0; i < texts.Length; i++)
		{
			var t = texts[i];
			texts[i].transform.position = stairsMeshes[i].transform.position + textOffset;
		}
	}
	
	#endregion
}
