#pragma once

#include "RigidBody.h"

class Plane : public RigidBody
{

public:

	glm::vec2 posA = { 0,0 };
	glm::vec2 posB = { 0,0 };

	glm::vec2 normal = { 0,0 };

	float d = 0;

	Plane();

	Plane(float xA, float yA, float xB, float yB);

	Plane& operator=(const Plane& other);

	ShapeType GetType() const override { return ShapeType::Plane; }

	void Draw(LineRenderer& line) const override { line.DrawLineSegment(posA, posB, { 0, 1, 0 }); line.DrawCross(position + normal, 0.1f, {1, 0, 0}); }

	void FixedUpdate(glm::vec2 gravity, float deltaTime) override;
};