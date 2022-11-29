#pragma once

#include "RigidBody.h"

class Circle : public RigidBody
{

public:

	float radius = 0.0f;

	bool dead = false;

	Circle();

	Circle(float x, float y, float newRadius);

	Circle& operator=(const Circle& other);

	ShapeType GetType() const override { return ShapeType::Circle; }

	void Draw(LineRenderer& line) const override { line.DrawCircle(position, 1.0f, { 1,1,1 }); }

	void FixedUpdate(glm::vec2 gravity, float deltaTime) override;
};