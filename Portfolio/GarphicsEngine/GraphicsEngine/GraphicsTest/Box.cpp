
#include "Box.h"

Box::Box(glm::vec3 pos, Mesh* mesh)
{
	boxMesh = mesh;

	transform[0][0] = pos.x;
	transform[1][1] = pos.y;
	transform[2][2] = pos.z;

	position = pos;
}

Box::~Box()
{
}
