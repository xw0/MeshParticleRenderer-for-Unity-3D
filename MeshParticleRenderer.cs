/**************************************

MIT License

Copyright (C) 2011 Xiang Wei
Updated       2015 Rici Underwood

Permission is hereby granted, free of charge, to any person obtaining a copy of 
this software and associated documentation files (the "Software"), to deal in 
the Software without restriction, including without limitation the rights to use, 
copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
Software, and to permit persons to whom the Software is furnished to do so, subject 
to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
  
***************************************/

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Particles/Mesh Particle Renderer")]
public class MeshParticleRenderer : MonoBehaviour
{
	public int maximumParticles = 1;
	public Mesh particleMesh;
	public Material[] particleMaterials;
	public float meshScale = 1.0f;
	public Vector3 rotationAxis = Vector3.up;

	GameObject[] particlePool;

	void Start ()
	{
		ResetSubParticles();
	}
	
	void OnEnable()
	{
		ResetSubParticles();
	}
	
	void OnDisable()
	{
		RemoveSubParticles();
	}
	
	void OnDestroy()
	{
		RemoveSubParticles();
	}
	
	void LateUpdate ()
	{
		if (particleMesh == null || maximumParticles <= 0) return;
		
		if (Application.isEditor)
		{
			ResetSubParticles();
		}
	
        Particle[] particles = GetComponent<ParticleEmitter>().particles;
		for (int i=0; i<maximumParticles; ++i)
		{
			GameObject particleObject = particlePool[i];
			if (i >= particles.Length)
			{
                particleObject.GetComponent<Renderer>().enabled = false;
			}
			else
			{
                particleObject.GetComponent<Renderer>().enabled = true;
				Particle p = particles[i];
				if (GetComponent<ParticleEmitter>().useWorldSpace)
				{
					particleObject.transform.position = p.position;
					particleObject.transform.rotation = Quaternion.AngleAxis(p.rotation, rotationAxis);
				}
				else
				{
					particleObject.transform.localPosition = p.position;
					particleObject.transform.localRotation = Quaternion.AngleAxis(p.rotation, rotationAxis);
				}
				float scale = p.size * meshScale;
				particleObject.transform.localScale = new Vector3(scale, scale, scale);
			}
		}
	}
	
	void RemoveSubParticles()
	{
		if (particlePool != null)
		{
			foreach (var o in particlePool)
			{
				if (Application.isEditor)
				{
					DestroyImmediate(o);
				}
				else
				{
					Destroy(o);
				}
			}
			particlePool = null;
		}
	}
	
	void ResetSubParticles()
	{
		if (particleMesh == null || maximumParticles <= 0) return;
		
		RemoveSubParticles();
		
		particlePool = new GameObject[maximumParticles];

		for (int i=0; i<maximumParticles; ++i)
		{
			GameObject particleObject = new GameObject();
			particleObject.name = "ParticleMesh";
			MeshFilter mf = particleObject.AddComponent<MeshFilter>();
			mf.mesh = particleMesh;
			MeshRenderer mr = particleObject.AddComponent<MeshRenderer>();
			mr.materials = particleMaterials;
			
			particleObject.transform.parent = transform;
			particleObject.transform.localScale = new Vector3(meshScale, meshScale, meshScale);
            particleObject.GetComponent<Renderer>().enabled = false;

			particlePool[i] = particleObject;
		}
	}
}

