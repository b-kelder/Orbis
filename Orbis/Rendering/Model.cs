namespace Orbis.Rendering
{
    /// <summary>
    /// Combination of Mesh and Material.
    /// </summary>
    class Model
    {
        public Mesh Mesh { get; set; }
        public Material Material { get; set; }

        public Model(Mesh mesh, Material material)
        {
            Mesh = mesh;
            Material = material;
        }
    }
}
