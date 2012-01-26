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
					particleObject.transform.rotation = Quaternion.AngleAxis(p.rotation, Vector3.up);
				}
				else
				{
					particleObject.transform.localPosition = p.position;
					particleObject.transform.localRotation = Quaternion.AngleAxis(p.rotation, Vector3.up);
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

