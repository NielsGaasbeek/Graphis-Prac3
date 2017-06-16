using OpenTK;
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

        public void loadMesh(string id, string path, string parent = "")
        {
            temp = new Mesh(path);  //tijdelijke opslag mesh

            if (parent != "")     //als demesh een child is, is de parent niet null
            {
                children.Add(id, temp); //dus komt hij in de lijst van children

                //als de parent in de lijst van hoofdmeshes staat, voeg hem dan ook als child van de mesh toe
                if (graph.ContainsKey(parent))  
                {
                    graph[parent].Children.Add(id, temp);
                }
                else if (children.ContainsKey(parent)) //als het de child van een child (van een child etc etc), voeg hem dan daaraan toe
                {
                    children[parent].Children.Add(id, temp);
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

        public void Render(Shader shader, Matrix4 transform, Texture texture)
        {
            //de hooflijst word gerenderd, elke mesh in de hoofdlijst 
            //heeft zijn eigen lijst met children die recursief worden gerenderd.
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
