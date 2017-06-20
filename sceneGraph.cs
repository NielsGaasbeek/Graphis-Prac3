﻿using OpenTK;
using System;
using System.Collections.Generic;

namespace Template_P3
{
    class sceneGraph
    {
        public Dictionary<string, Mesh> graph;      //een lijst met de 'hoofd meshes'
        public Dictionary<string, Mesh> children;   //lijst waarin alle meshes met parent != null staan

        Mesh temp;

        public sceneGraph()
        {
            graph = new Dictionary<string, Mesh>();
            children = new Dictionary<string, Mesh>();
        }

        public void loadMesh(string id, string path, Vector3 positie, string parent = "")
        {
            temp = new Mesh(path);  //tijdelijke opslag mesh
            temp.modelMatrix = Matrix4.CreateTranslation(positie);

            if (parent != "")     //als de mesh een child is, is de parent niet null
            {
                children.Add(id, temp); //dus komt hij in de lijst van children

                //als de parent in de lijst van hoofdmeshes staat, voeg hem dan ook als child van de mesh toe
                if (graph.ContainsKey(parent))
                {
                    graph[parent].Children.Add(temp);
                }
                else if (children.ContainsKey(parent)) //als het de child van een child (van een child etc etc), voeg hem dan daaraan toe
                {
                    children[parent].Children.Add(temp);
                }
                else //extra voor het geval de parent niet gevonden is
                {
                    Console.WriteLine("Parent mesh '" + id + "' not found");
                }
            }
            else //heeft het geen parent, dan is het een 'hoofd mesh' en komt hij in de hoofdlijst
            {
                graph.Add(id, temp);
            }
        }

        public void Render(Shader shader, Matrix4 transform, Matrix4 toWorld, Texture texture)
        {
            //de hooflijst word gerenderd, elke mesh in de hoofdlijst...
            //...heeft zijn eigen lijst met children die recursief worden gerenderd.
            foreach (KeyValuePair<string, Mesh> M in graph)
            {
                M.Value.Render(shader, M.Value.modelMatrix * transform * toWorld, toWorld, texture);

                if (M.Value.Children.Count > 0)
                {
                    //render children
                    foreach(Mesh L in M.Value.Children)
                    {
                        RenderChild(L, shader, M.Value.modelMatrix * transform, toWorld, texture);
                    }
                }
            }
        }

        public void RenderChild(Mesh mesh, Shader shader, Matrix4 transform, Matrix4 toWorld, Texture texture)
        {
            mesh.Render(shader, mesh.modelMatrix * transform * toWorld, toWorld, texture);

            if(mesh.Children.Count > 0)
            {
                foreach(Mesh M in mesh.Children)
                {
                    RenderChild(M, shader, mesh.modelMatrix * transform, toWorld, texture);
                }
            }
        }
    }
}
