using UnityEngine;
using System.Collections;

public class ScrollingUVs_Layers : MonoBehaviour 
{
	//public int materialIndex = 0;
	public Vector2 uvAnimationRate = new Vector2( 1.0f, 0.0f );
	public string textureName = "_MainTex";
	
	Vector2 uvOffset = Vector2.zero;
	Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    void LateUpdate() 
	{
		uvOffset += ( uvAnimationRate * Time.deltaTime );
		if(_renderer.enabled )
		{
			_renderer.sharedMaterial.SetTextureOffset( textureName, uvOffset );
		}
	}
}