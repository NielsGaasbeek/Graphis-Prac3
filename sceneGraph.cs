using OpenTK;
using System;
using System.Collections.Generic;

namespace Template_P3
{
    class sceneGraph
    {
        public List<Mesh> meshes = new List<Mesh>();

        public sceneGraph()
        {

        }

        public void loadMesh(string path)
        {
            meshes.Add(new Mesh(path));
        }

        public void Render(Matrix4 transform)
        {

        }
    }
}
