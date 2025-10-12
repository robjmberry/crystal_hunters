@tool
extends Node2D

@export var initial_velocity: Vector2 = Vector2(200, -600)
@export var jump_gravity: float = 600.0
@export var fall_gravity: float = 1200.0
@export var resolution: int = 60
@export var time_step: float = 0.05
@export var color: Color = Color(0.524, 0.546, 0.0, 0.929)
@export var line_width: float = 5.0
@export var max_time: float = 2.5 # stop drawing after this duration


func _draw():
	var points: Array[Vector2] = []
	var pos: Vector2 = Vector2.ZERO
	var vel: Vector2 = initial_velocity
	var t: float = 0.0

	while t < max_time:
		# choose gravity depending on direction of travel
		var current_gravity = jump_gravity if vel.y < 0 else fall_gravity

		# integrate motion
		vel.y += current_gravity * time_step
		pos += vel * time_step

		points.append(pos)
		if pos.y > 0: # back to ground level, stop early
			break

		t += time_step

	# draw the curve
	for i in range(points.size() - 1):
		draw_line(points[i], points[i + 1], color, line_width)

func _process(_delta):
	queue_redraw()
