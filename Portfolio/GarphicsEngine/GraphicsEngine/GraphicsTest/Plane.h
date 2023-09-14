#pragma once

#include "RigidBody.h"

class Plane : public RigidBody
{

public:

	glm::vec2 posA = { 0,0 };
	glm::vec2 posB = { 0,0 };

	glm::vec2 normal = { 0,0 };

	float d = 0; // scalar offset

	Plane();

	Plane(float xA, float yA, float xB, float yB);

	Plane& operator=(const Plane& other);

	ShapeType GetType() const override { return ShapeType::Plane; }

	void Draw(Renderer& renderer) const override { renderer.RequestLine(posA.x, posA.y, posB.x, posB.y, false); } // Static rigidbodies drawn as dynamic for sake of demo

	void FixedUpdate(glm::vec2 gravity, float deltaTime) override;
};