#pragma once

#include "RigidBody.h"
#include "Mesh.h"
#include "glm.hpp"
#include <ext/matrix_transform.hpp>
#include <ext/matrix_clip_space.hpp>

class Circle : public RigidBody
{

public:

	float radius;

	Circle();

	Circle(float x, float y, float newRadius);

	Circle& operator=(const Circle& other);

	ShapeType GetType() const override { return ShapeType::Circle; }

	void Draw(Renderer& renderer) const override { renderer.RequestCircle(position.x, position.y, radius, false); }

	void FixedUpdate(glm::vec2 gravity, float deltaTime) override;

};