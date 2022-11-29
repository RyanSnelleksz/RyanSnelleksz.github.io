#pragma once

#include "RigidBody.h"

class Box : public RigidBody // AABB
{

public:

	float xMin = FLT_MAX;
	float xMax = -FLT_MAX;
	float yMin = FLT_MAX;
	float yMax = -FLT_MAX;

	Box();

	Box(float minX, float maxX, float minY, float maxY);

	Box& operator=(const Box& other);

	ShapeType GetType() const override { return ShapeType::Box; }

	void Draw(LineRenderer& line) const override {
		line.AddPointToLine({ xMin, yMin }); // box
		line.AddPointToLine({ xMax, yMin });
		line.AddPointToLine({ xMax, yMax });
		line.AddPointToLine({ xMin, yMax });

		line.FinishLineLoop();
	}

	void FixedUpdate(glm::vec2 gravity, float deltaTime) override;
};