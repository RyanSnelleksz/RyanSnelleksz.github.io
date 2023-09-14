
#include "Mesh.h"


Mesh::Mesh()
{

}

Mesh::Mesh(std::vector<Vertex> vertices, int size, ShaderProgram shader)
{
	verticesSize = size;

	theShader = &shader;

	// The buffer is essentially the space(memory) in use

	glGenBuffers(1, &meshID); // Make space for the meshID

	glBindBuffer(GL_ARRAY_BUFFER, meshID); // Bind the new space and the mesh
	glBufferData(GL_ARRAY_BUFFER, sizeof(Vertex) * size, vertices.data(), GL_STATIC_DRAW); // add the data to the space
	glBindBuffer(GL_ARRAY_BUFFER, 0); // clear the buffer
}

Mesh::Mesh(std::string meshFile, glm::vec3 colour, ShaderProgram shader)
{
	theShader = &shader;

	// The buffer is essentially the space(memory) in use

	glGenBuffers(1, &meshID); // Make space for the meshID
	glGenBuffers(1, &indexID); // Make space for the indexID - Index buffer

	std::vector<Vertex> vertices;
	std::vector<unsigned short> indices;

	Assimp::Importer importer;

	const aiScene* scene = importer.ReadFile(meshFile, aiProcess_Triangulate | aiProcess_JoinIdenticalVertices | aiProcess_CalcTangentSpace);

	aiMesh* meshPointer = *scene->mMeshes;

	for (unsigned int i = 0; i < meshPointer->mNumVertices; i++) // Copy the vertices
	{
		Vertex thisVertex;
		thisVertex.position.x = meshPointer->mVertices[i].x;
		thisVertex.position.y = meshPointer->mVertices[i].y;
		thisVertex.position.z = meshPointer->mVertices[i].z;

		thisVertex.colour = colour;

		if (meshPointer->HasNormals())
		{
			thisVertex.normal.x = meshPointer->mNormals[i].x;
			thisVertex.normal.y = meshPointer->mNormals[i].y;
			thisVertex.normal.z = meshPointer->mNormals[i].z;
		}

		if (meshPointer->HasTextureCoords(0))
		{
			thisVertex.texCoord.x = meshPointer->mTextureCoords[0][i].x;
			thisVertex.texCoord.y = meshPointer->mTextureCoords[0][i].y * -1;
		}

		if (meshPointer->HasTangentsAndBitangents())
		{
			thisVertex.tangent.x = meshPointer->mTangents[i].x;
			thisVertex.tangent.y = meshPointer->mTangents[i].y;
			thisVertex.tangent.z = meshPointer->mTangents[i].z;
		}

		vertices.push_back(thisVertex);
	}
	for (unsigned int i = 0; i < meshPointer->mNumFaces; i++) // copy the indicies
	{
		indices.push_back((unsigned short)meshPointer->mFaces[i].mIndices[0]);
		indices.push_back((unsigned short)meshPointer->mFaces[i].mIndices[1]);
		indices.push_back((unsigned short)meshPointer->mFaces[i].mIndices[2]);
	}

	verticesSize = indices.size();

	glBindBuffer(GL_ARRAY_BUFFER, meshID); // Bind the new space and the mesh
	glBufferData(GL_ARRAY_BUFFER, sizeof(Vertex) * vertices.size(), vertices.data(), GL_STATIC_DRAW); // add the data to the space
	glBindBuffer(GL_ARRAY_BUFFER, 0); // clear the buffer

	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexID); // Bind the new space and the mesh
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned short) * indices.size(), indices.data(), GL_STATIC_DRAW); // add the data to the space
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0); // clear the buffer
}

Mesh& Mesh::operator=(Mesh& other)
{
	meshID = other.meshID;
	verticesSize = other.verticesSize;

	return *this;
}

Mesh::~Mesh()
{
	glDeleteBuffers(1, &meshID);
	glDeleteBuffers(1, &indexID);
}
