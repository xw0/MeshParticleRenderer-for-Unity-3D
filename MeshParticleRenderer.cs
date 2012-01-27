/**************************************

  zlib License

  Copyright (C) 2011 Xiang Wei

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.

  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.

  3. This notice may not be removed or altered from any source distribution.
  
  Xiang Wei (weixiang77 [at] gmail [dot] com)
  
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
		
		Particle[] particles = particleEmitter.particles;
		for (int i=0; i<maximumParticles; ++i)
		{
			GameObject particleObject = particlePool[i];
			if (i >= particles.Length)
			{
				particleObject.renderer.enabled = false;
			}
			else
			{
				particleObject.renderer.enabled = true;
				Particle p = particles[i];
				if (particleEmitter.useWorldSpace)
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
				DestroyImmediate(o);
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
			particleObject.renderer.enabled = false;

			particlePool[i] = particleObject;
		}
	}
}

