using OpenTK;
using System;
using System.Collections.Generic;

namespace Template_P3
{
    class sceneGraph
    {
        public Dictionary<string, Mesh> graph;

        public sceneGraph()
        {
            graph = new Dictionary<string, Mesh>();
        }

        public void loadMesh(string id, string path, Mesh parent = null)
        {
            if(parent != null)
            {
                graph[id].Children.Add(id, new Mesh(path));
            }
            else
            {
                graph.Add(id, new Mesh(path));
            }
        }

        public void Render(Shader shader, Matrix4 transform, Texture texture)
        {
            foreach(KeyValuePair<string, Mesh> M in graph)
            {
                M.Value.Render(shader, transform, texture);
                if(M.Value.Children.Count > 0)
                {
                    //render children
                }
            }
        }
    }
}
