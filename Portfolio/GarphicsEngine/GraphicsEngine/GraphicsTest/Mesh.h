#pragma once

#include <vector>
#include <string>
#include "Graphics.h"

#include <ext/matrix_clip_space.hpp>
#include <ext/matrix_transform.hpp>

#include "ShaderProgram.h"
#include "Vertex.h"

#include <assimp/Importer.hpp>
#include <assimp/scene.h>
#include <assimp/postprocess.h>

class Mesh
{
	ShaderProgram* theShader;

public:

	GLuint meshID;
	GLuint indexID;
	int verticesSize = 0;


	Mesh();

	Mesh(std::vector<Vertex> vertices, int size, ShaderProgram shader);

	Mesh(std::string meshFile, glm::vec3 colour, ShaderProgram shader);

	Mesh& operator=(Mesh& other);

	~Mesh();

};