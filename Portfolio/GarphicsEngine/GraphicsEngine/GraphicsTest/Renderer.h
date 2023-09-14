#pragma once

#include "ShaderProgram.h"
#include "Camera.h"
#include "Mesh.h"
#include "Texture.h"

#include <ext/matrix_transform.hpp>
#include <ext/matrix_clip_space.hpp>

class Renderer
{
	ShaderProgram* meshShader;
	Camera* theCam;

	bool initialised = false;

	GLuint dynamicShapePositionBufferID; // Meshs have their own buffer id but we will do shapes all with one
	GLuint dynamicShapeColourBufferID; // They need a colour id as well

	GLuint staticShapePositionBufferID; // Static and dynamic shapes need seperate ones
	GLuint staticShapeColourBufferID;

	std::vector<glm::vec2> dynamicShapePositions;
	std::vector<glm::vec3> dynamicShapeColours;

	std::vector<glm::vec2> staticShapePositions;
	std::vector<glm::vec3> staticShapeColours;


	glm::vec3 currentColour;


public:

	Renderer() = delete;

	Renderer(ShaderProgram* _meshShader, Camera* cam);

	~Renderer();

	void Initialise();

	void DrawIndexedTexturedMesh(const Mesh& mesh, glm::mat4 transform, Texture& textureD, Texture& textureN, Texture& textureS);

	void RequestCircle(float cX, float cY, float radius, bool isStatic = false);

	void RequestLine(float ax, float ay, float bx, float by, bool isStatic = false);

	void RequestQuadrilateral(float x, float y, float width, bool isStatic = false);

	void RequestQuadrilateral(float x, float y, float width, float height, bool isStatic = false);

	void RequestTriangle(float x, float y, float size, bool isStatic = false);

	void RequestCross(float x, float y, float size, bool typeTwo = false, bool isStatic = false);

	void RequestCustomShape(std::vector<glm::vec2> positions, bool isStatic = false);

	void RequestCustomShape(std::vector<glm::vec2> positions, std::vector<glm::vec3> colours, bool isStatic = false);

	void CompileStaticShapes();

	void CompileDynamicShapes();

	void DrawDynamicShapes();

	void DrawStaticShapes();

	void ClearDynamicData();

};