using UnityEngine;

namespace PMB
{
    [RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
    public class PMBComponent:MonoBehaviour
    {
        public Sprite Sprite;
        public Vector3 Scale = Vector3.one;
        public MeshQuality Quality = MeshQuality.High;
        public bool Optimized = true;
        public bool RecalcNormals = true;
        public bool RunOnStart = false;
        public Material Material;
        public bool UseAutoMaterial = true;
        public Shader Shader;
        public bool UseUnlit = true;

        private void Start()
        {
            if(RunOnStart) GenerateMesh();
        }

        public void GenerateMesh()
        {
            var meshFilter = GetComponent<MeshFilter>();
            var meshRenderer = GetComponent<MeshRenderer>();
            if (meshFilter == null)
            {
                Debug.LogError("Pixel Mesh Builder.Mesh filter component could not be found! Mesh will not be build!");
                return;
            }
            if (meshRenderer == null)
            {
                Debug.LogError("Pixel Mesh Builder.MeshRenderer component could not be found! Mesh will not be build!");
                return;
            }
            if (Sprite == null)
            {
                Debug.LogError("Pixel Mesh Builder. Source sprite is null! Mesh will not be build!");
                return;
            }
            var mesh = new PixelMeshBuilder(Quality).BuildMesh(Sprite, Scale, Optimized, RecalcNormals);
            mesh.name = "mesh_" + Sprite.name;
            meshFilter.mesh = mesh;
            if (!UseAutoMaterial)
            {
                if (Material == null)
                {
                    meshRenderer.material = GenerateMaterial();
                    Debug.LogWarning("Pixel Mesh Builder. Material is set to null! Using default material!");
                }
                else
                {
                    meshRenderer.material = Material;
                }
            }
            else
            {
                if (UseUnlit)
                {
                    meshRenderer.material = GenerateMaterial();
                }
                else
                {
                    if (Shader == null)
                    {
                        meshRenderer.material = GenerateMaterial();
                        Debug.LogWarning("Pixel Mesh Builder. Shader is set to null! Using default material!");
                    }
                    else
                    {
                        meshRenderer.material = GenerateMaterial(Shader);
                    }
                }
            }

        }

        private Material GenerateMaterial(Shader shader = null)
        {
            return shader == null?  new Material(Shader.Find("Unlit/Texture")) { mainTexture = Sprite.texture }: new Material(shader) { mainTexture = Sprite.texture };
        }
        
    }
}
