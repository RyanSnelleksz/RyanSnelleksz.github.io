
#include "Renderer.h"

Renderer::Renderer(ShaderProgram* _meshShader, Camera* cam)
{
	meshShader = _meshShader;
	theCam = cam;
}

Renderer::~Renderer()
{
	if (initialised)
	{
		glDeleteBuffers(1, &dynamicShapePositionBufferID);
		glDeleteBuffers(1, &dynamicShapeColourBufferID);

		glDeleteBuffers(1, &staticShapePositionBufferID);
		glDeleteBuffers(1, &staticShapeColourBufferID);
	}
}

void Renderer::Initialise()
{
	initialised = true;
	glGenBuffers(1, &dynamicShapePositionBufferID); // Make space for the buffer id's
	glGenBuffers(1, &dynamicShapeColourBufferID);

	glGenBuffers(1, &staticShapePositionBufferID); 
	glGenBuffers(1, &staticShapeColourBufferID); 

	currentColour = { 1.0f, 1.0f, 1.0f };
}

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//
// DrawIndexedTexturedMesh
// Draws an indexed mesh and textures it. Requires an indexed mesh and diffuse/normal/specular maps.
// 
// // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void Renderer::DrawIndexedTexturedMesh(const Mesh& mesh, glm::mat4 transform, Texture& textureD, Texture& textureN, Texture& textureS)
{
	// The buffer is our current ID

	glBindBuffer(GL_ARRAY_BUFFER, mesh.meshID); // bind this meshID to the gl array buffer target

	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, position));// layout position 0, number of elements per attrib, enum(the type), normalize(not the mathematical normalize), stride(distance between start of one vertex to the next), where to start
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, colour));
	glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, normal));
	glVertexAttribPointer(3, 2, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, texCoord));
	glVertexAttribPointer(4, 4, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, tangent));

	meshShader->SetUniform("camPos", theCam->pos);

	meshShader->SetUniform("mvp", theCam->projection * theCam->view * transform);
	meshShader->SetUniform("modelMatrix", transform);

	glm::mat3 lights = // Each set of floats is a light, if a set has all 0's then it means no light
	{
		1.0f, 0.0f, 0.0f,
		0.0f, 0.0f, 1.0f,
		0.0f, 0.0f, 0.0f
	};

	meshShader->SetUniform("lights", lights);
	meshShader->SetUniform("numLights", 2);

	meshShader->SetUniform("diffuseTextureSampler", 0);
	meshShader->SetUniform("normalTextureSampler", 1);
	meshShader->SetUniform("specularTextureSampler", 2);

	textureD.Bind(0);
	textureN.Bind(1);
	textureS.Bind(2);

	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, mesh.indexID);

	glDrawElements(GL_TRIANGLES, (GLsizei)mesh.verticesSize, GL_UNSIGNED_SHORT, 0);
}


// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// 
// RequestCircle
// Adds circle data to shape positions and shape colours so that it will be drawn when draw shapes is called
// 
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void Renderer::RequestCircle(float cX, float cY, float radius, bool isStatic)
{
	std::vector<glm::vec2>* shapePositions = isStatic ? &staticShapePositions : &dynamicShapePositions;
	std::vector<glm::vec3>* shapeColours = isStatic ? &staticShapeColours : &dynamicShapeColours;

	float theta = 2.0f * 3.1415926f / 64; //get the current angle

	float cosAngle = cos(theta);
	float sinAngle = sin(theta);

	glm::mat2 rotation = { {cosAngle, -sinAngle},{sinAngle, cosAngle} };

	glm::vec2 pos(0, radius);

	for (int i = 0; i <= 64; i++)
	{
		shapePositions->push_back(glm::vec2{ cX, cY } + pos);
		pos = rotation * pos;
		shapePositions->push_back(glm::vec2{ cX, cY } + pos);
		shapeColours->push_back(currentColour);
		shapeColours->push_back(currentColour);
	}
}

void Renderer::RequestLine(float ax, float ay, float bx, float by, bool isStatic)
{
	std::vector<glm::vec2>* shapePositions = isStatic ? &staticShapePositions : &dynamicShapePositions;
	std::vector<glm::vec3>* shapeColours = isStatic ? &staticShapeColours : &dynamicShapeColours;

	shapePositions->push_back(glm::vec2{ ax, ay });
	shapePositions->push_back(glm::vec2{ bx, by });
	shapeColours->push_back(currentColour);
	shapeColours->push_back(currentColour);
}

void Renderer::RequestQuadrilateral(float x, float y, float width, bool isStatic)
{
	std::vector<glm::vec2>* shapePositions = isStatic ? &staticShapePositions : &dynamicShapePositions;
	std::vector<glm::vec3>* shapeColours = isStatic ? &staticShapeColours : &dynamicShapeColours;

	float height = width;

	width = width / 2.0f;
	height = height / 2.0f;

	shapePositions->push_back(glm::vec2{ x - width, y - height }); // Top side
	shapePositions->push_back(glm::vec2{ x + width, y - height });
	shapeColours->push_back(currentColour);
	shapeColours->push_back(currentColour);

	shapePositions->push_back(glm::vec2{ x - width, y - height }); // Left
	shapePositions->push_back(glm::vec2{ x - width, y + height });
	shapeColours->push_back(currentColour);
	shapeColours->push_back(currentColour);

	shapePositions->push_back(glm::vec2{ x - width, y + height }); // Bottom
	shapePositions->push_back(glm::vec2{ x + width, y + height });
	shapeColours->push_back(currentColour);
	shapeColours->push_back(currentColour);

	shapePositions->push_back(glm::vec2{ x + width, y + height }); // Right
	shapePositions->push_back(glm::vec2{ x + width, y - height });
	shapeColours->push_back(currentColour);
	shapeColours->push_back(currentColour);
}

void Renderer::RequestQuadrilateral(float x, float y, float width, float height, bool isStatic)
{
	std::vector<glm::vec2>* shapePositions = isStatic ? &staticShapePositions : &dynamicShapePositions;
	std::vector<glm::vec3>* shapeColours = isStatic ? &staticShapeColours : &dynamicShapeColours;

	width = width / 2.0f;
	height = height / 2.0f;

	shapePositions->push_back(glm::vec2{ x - width, y - height }); // Top side
	shapePositions->push_back(glm::vec2{ x + width, y - height });
	shapeColours->push_back(currentColour);
	shapeColours->push_back(currentColour);

	shapePositions->push_back(glm::vec2{ x - width, y - height }); // Left
	shapePositions->push_back(glm::vec2{ x - width, y + height });
	shapeColours->push_back(currentColour);
	shapeColours->push_back(currentColour);

	shapePositions->push_back(glm::vec2{ x - width, y + height }); // Bottom
	shapePositions->push_back(glm::vec2{ x + width, y + height });
	shapeColours->push_back(currentColour);
	shapeColours->push_back(currentColour);

	shapePositions->push_back(glm::vec2{ x + width, y + height }); // Right
	shapePositions->push_back(glm::vec2{ x + width, y - height });
	shapeColours->push_back(currentColour);
	shapeColours->push_back(currentColour);
}

void Renderer::RequestTriangle(float x, float y, float size, bool isStatic)
{
	std::vector<glm::vec2>* shapePositions = isStatic ? &staticShapePositions : &dynamicShapePositions;
	std::vector<glm::vec3>* shapeColours = isStatic ? &staticShapeColours : &dynamicShapeColours;

	size = size / 2.0f;

	shapePositions->push_back(glm::vec2{ x, y + size }); // Left
	shapePositions->push_back(glm::vec2{ x - size, y - size });
	shapeColours->push_back(currentColour);
	shapeColours->push_back(currentColour);

	shapePositions->push_back(glm::vec2{ x - size, y - size }); // Bottom
	shapePositions->push_back(glm::vec2{ x + size, y - size });
	shapeColours->push_back(currentColour);
	shapeColours->push_back(currentColour);

	shapePositions->push_back(glm::vec2{ x + size, y - size }); // Right
	shapePositions->push_back(glm::vec2{ x, y + size });
	shapeColours->push_back(currentColour);
	shapeColours->push_back(currentColour);
}

void Renderer::RequestCross(float x, float y, float size, bool typeTwo, bool isStatic)
{
	std::vector<glm::vec2>* shapePositions = isStatic ? &staticShapePositions : &dynamicShapePositions;
	std::vector<glm::vec3>* shapeColours = isStatic ? &staticShapeColours : &dynamicShapeColours;

	size = size / 2.0f;

	if (!typeTwo)
	{
		shapePositions->push_back(glm::vec2{ x - size, y - size });
		shapePositions->push_back(glm::vec2{ x + size, y + size });
		shapeColours->push_back(currentColour);
		shapeColours->push_back(currentColour);
		shapePositions->push_back(glm::vec2{ x + size, y - size });
		shapePositions->push_back(glm::vec2{ x - size, y + size });
		shapeColours->push_back(currentColour);
		shapeColours->push_back(currentColour);
	}
	else
	{
		shapePositions->push_back(glm::vec2{ x, y - size });
		shapePositions->push_back(glm::vec2{ x, y + size });
		shapeColours->push_back(currentColour);
		shapeColours->push_back(currentColour);
		shapePositions->push_back(glm::vec2{ x + size, y });
		shapePositions->push_back(glm::vec2{ x - size, y });
		shapeColours->push_back(currentColour);
		shapeColours->push_back(currentColour);
	}
}

void Renderer::RequestCustomShape(std::vector<glm::vec2> positions, bool isStatic)
{
	if (positions.size() <= 1)
	{
		return;
	}

	std::vector<glm::vec2>* shapePositions = isStatic ? &staticShapePositions : &dynamicShapePositions;
	std::vector<glm::vec3>* shapeColours = isStatic ? &staticShapeColours : &dynamicShapeColours;

	for (glm::vec2 pos : positions)
	{
		shapePositions->push_back(pos);
		shapeColours->push_back(currentColour);
	}
}

void Renderer::RequestCustomShape(std::vector<glm::vec2> positions, std::vector<glm::vec3> colours, bool isStatic)
{
	if (positions.size() <= 1)
	{
		return;
	}

	std::vector<glm::vec2>* shapePositions = isStatic ? &staticShapePositions : &dynamicShapePositions;
	std::vector<glm::vec3>* shapeColours = isStatic ? &staticShapeColours : &dynamicShapeColours;

	if (positions.size() > colours.size()) // positions and colours need to be same size
	{
		float difference = positions.size() - colours.size();

		for (int i = 0; i < difference; i++)
		{
			colours.push_back(currentColour);
		}
	}
	else if (positions.size() < colours.size())
	{
		float difference = colours.size() - positions.size();

		for (int i = 0; i < difference; i++)
		{
			colours.erase(colours.begin());
		}
	}

	for (glm::vec2 pos : positions)
	{
		shapePositions->push_back(pos);
	}
	for (glm::vec3 c : colours)
	{
		shapeColours->push_back(c);
	}
}

void Renderer::CompileStaticShapes()
{
	if (staticShapePositions.size() > 0)
	{
		glBindBuffer(GL_ARRAY_BUFFER, staticShapePositionBufferID); // Bind the buffer to the shape position buffer
		glBufferData(GL_ARRAY_BUFFER, sizeof(glm::vec2) * staticShapePositions.size(), staticShapePositions.data(), GL_DYNAMIC_DRAW); // Add the position data
		glBindBuffer(GL_ARRAY_BUFFER, staticShapeColourBufferID); // Now bind the colour buffer
		glBufferData(GL_ARRAY_BUFFER, sizeof(glm::vec3) * staticShapeColours.size(), staticShapeColours.data(), GL_DYNAMIC_DRAW); // And give it it's data
	}
}

void Renderer::CompileDynamicShapes()
{
	if (dynamicShapePositions.size() > 0)
	{
		glBindBuffer(GL_ARRAY_BUFFER, dynamicShapePositionBufferID); // Bind the buffer to the shape position buffer
		glBufferData(GL_ARRAY_BUFFER, sizeof(glm::vec2) * dynamicShapePositions.size(), dynamicShapePositions.data(), GL_DYNAMIC_DRAW); // Add the position data
		glBindBuffer(GL_ARRAY_BUFFER, dynamicShapeColourBufferID); // Now bind the colour buffer
		glBufferData(GL_ARRAY_BUFFER, sizeof(glm::vec3) * dynamicShapeColours.size(), dynamicShapeColours.data(), GL_DYNAMIC_DRAW); // And give it it's data
	}
}

void Renderer::DrawDynamicShapes()
{
	glBindBuffer(GL_ARRAY_BUFFER, dynamicShapePositionBufferID); // Rebind the position buffer
	glEnableVertexAttribArray(0); // Enable the vertex attribute
	glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, sizeof(glm::vec2), 0); // Set the pointer, "where to start"

	glBindBuffer(GL_ARRAY_BUFFER, dynamicShapeColourBufferID); // Bind the colour buffer
	glEnableVertexAttribArray(1); // Enable it's vertex attrribute
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(glm::vec3), 0); // Set the pointer, "where to start"

	glBindBuffer(GL_ARRAY_BUFFER, 0); // Get rid of the buffer

	glDrawArrays(GL_LINES, 0, (GLsizei)dynamicShapePositions.size()); // Draw!

	ClearDynamicData(); // Get rid of data for next draw
}

void Renderer::DrawStaticShapes()
{
	glBindBuffer(GL_ARRAY_BUFFER, staticShapePositionBufferID); // Rebind the position buffer
	glEnableVertexAttribArray(0); // Enable the vertex attribute
	glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, sizeof(glm::vec2), 0); // Set the pointer, "where to start"

	glBindBuffer(GL_ARRAY_BUFFER, staticShapeColourBufferID); // Bind the colour buffer
	glEnableVertexAttribArray(1); // Enable it's vertex attrribute
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(glm::vec3), 0); // Set the pointer, "where to start"

	glBindBuffer(GL_ARRAY_BUFFER, 0); // Get rid of the buffer

	glDrawArrays(GL_LINES, 0, (GLsizei)staticShapePositions.size()); // Draw!
}

void Renderer::ClearDynamicData()
{
	dynamicShapePositions.clear();
	dynamicShapeColours.clear();
}
