extends CharacterBody2D

@export var move_speed: float = 300
@export var acceleration : float = 50
@export var breaking : float = 20
@export var gravity: float = 2500
@export var jump_force : float = 900

var move_input: float

@onready var sprite : Sprite2D = $Sprite
@onready var anim : AnimationPlayer = $AnimationPlayer

func _physics_process(delta: float):
	
	if not is_on_floor():
		velocity.y +=gravity * delta
	
	move_input = Input.get_axis("move_left", "move_right")
	
	if move_input != 0: 
		velocity.x = lerp(velocity.x, move_input * move_speed, acceleration * delta)
	else:
		velocity.x = lerp(velocity.x, 0.0, breaking* delta)
	
	
	if Input.is_action_pressed("jump") and is_on_floor():
		velocity.y -= jump_force
	
	move_and_slide()
	
#func _process(delta: float):
	#sprite.flip_h = velocity.x > 0
	#
	#if not is_on_floor():
		#anim.play("jump")
	#elif move_input != 0:
		#anim.play("run")
	#else: anim.play("idle")
		#
#
#func apply_damage (amount: int):
	#print ("Hit ", amount)
		
