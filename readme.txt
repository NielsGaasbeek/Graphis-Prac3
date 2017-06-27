Brian van Beusekom 	- 5899192
Job van Zelm		- 5984394
Niels Gaasbeek		- 5850983

Extra Assignments:
- Vignetting + Chromatic Abberation
(Chromatic abberation intensity can be adjusted with the float "abberation" in the fs_post.glsl-file. 
Increasing this will increase the visual effect, while lowering it will decrease the effect)

Camera controls:
A: Left
S: Backwards
W: Forwards
D: Right
Q: Roll left.
R: Roll right
Left arrow: look left
Right arrow: look right
Up arrow: look up
Down arrow: look down

SceneGraph:
The scenegraph is built using Dictionaries with a KeyValuePair of String and Mesh.
The Key is a String which is an ID for the Mesh. The Mesh itself is the Value.
There are two dictionaries, one for the meshes at the top level of the scenegraph, and one for all of the children.
This is so each child can easily be found and adjusted, regardless of depth.
Each Mesh also has a List containing Meshes, it's children. This list is used for recursively rendering a mesh and it's children.
The top mesh is rendered, and if it's list contains meshes, each mesh is rendered and it's list get checked again.
