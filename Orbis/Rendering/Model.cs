﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    class Model
    {
        public Mesh Mesh { get; set; }
        public Material Material { get; set; }

        public Model(Mesh mesh, Material material)
        {
            Mesh = mesh;
            Material = material;
        }

        public RenderInstance CreateRenderInstance(Matrix matrix)
        {
            return new RenderInstance
            {
                material = Material,
                mesh = Mesh.RenderableMesh,
                matrix = matrix,
            };
        }
    }
}