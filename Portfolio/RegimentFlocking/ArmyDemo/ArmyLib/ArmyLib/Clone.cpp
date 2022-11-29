
#include "Clone.h"

Clone::Clone()
{
}

Clone::Clone(float myMaxSpeed, glm::vec2* myPos, glm::vec2* myVel, CloneArmy* army)
{
	maxSpeed = myMaxSpeed;

	position = myPos;

	velocity = myVel;

	myArmy = army;
}

Clone::~Clone()
{
}

// Name: Movement
// Parameter: 
// Description: Moves all the soldiers to their target position and reports when done
// Return Value: 
void Clone::Movement()
{
	if (myArmy->stance != Stance::Flee)
	{
		if (myArmy->rotated == false) 
		{
			glm::vec2 desiredVelocity = targetPosition - *position;
			float distance = glm::distance(targetPosition, *position);

			// Check if we are getting close
			float slowingRadius = 10.0f;
			if (distance < slowingRadius)
			{
				if (desiredVelocity != glm::vec2{ 0, 0 })
				{
					desiredVelocity = glm::normalize(desiredVelocity) * maxSpeed * (distance / slowingRadius);
				}

				*velocity = desiredVelocity;

				if (distance < 0.1f && CheckOthers()) // Report when done
				{
					myArmy->rotated = true;
					myArmy->moved = false;
				}
				else
				{
					inPosition = true;
				}
			}
			else
			{
				if (desiredVelocity != glm::vec2{ 0, 0 })
				{
					desiredVelocity = glm::normalize(desiredVelocity) * maxSpeed;
				}
				*velocity = desiredVelocity;
			}
		}
		else
		{
			glm::vec2 desiredVelocity = targetPosition - *position; // Move to target position
			float distance = glm::distance(targetPosition, *position);

			// Check if we are getting close
			float slowingRadius = 10.0f;
			if (distance < slowingRadius)
			{
				if (desiredVelocity != glm::vec2{ 0, 0 })
				{
					desiredVelocity = glm::normalize(desiredVelocity) * maxSpeed * (distance / slowingRadius);
				}

				*velocity = desiredVelocity;
				if (distance < 0.1f)
				{
					myArmy->moved = true;
				}
			}
			else
			{
				if (desiredVelocity != glm::vec2{ 0, 0 })
				{
					desiredVelocity = glm::normalize(desiredVelocity) * maxSpeed;
				}
				*velocity = desiredVelocity;
			}
		}
	}
	else
	{
		glm::vec2 desiredVelocity = *position - targetPosition; // Flee from target position
		float distance = glm::distance(targetPosition, *position);

		// Check if we are getting close
		float slowingRadius = 10.0f;
		if (distance < slowingRadius)
		{
			if (desiredVelocity != glm::vec2{ 0, 0 })
			{
				desiredVelocity = glm::normalize(desiredVelocity) * maxSpeed * (distance / slowingRadius);
			}

			*velocity = desiredVelocity;
			if (distance < 0.1f)
			{
				myArmy->moved = true;
			}
		}
		else
		{
			if (desiredVelocity != glm::vec2{ 0, 0 })
			{
				desiredVelocity = glm::normalize(desiredVelocity) * maxSpeed;
			}
			*velocity = desiredVelocity;
		}
	}
}

// Name: CheckOthers
// Parameter: 
// Description: Checks if all the soldiers are in position
// Return Value: Returns true if all the soldieers are in position
bool Clone::CheckOthers()
{
	bool ready = true;

	for (Clone* clone : myArmy->clones)
	{
		if (clone->inPosition == false)
		{
			ready = false;
		}
	}

	return ready;
}
