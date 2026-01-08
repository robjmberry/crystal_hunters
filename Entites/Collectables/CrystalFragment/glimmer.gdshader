shader_type canvas_item;

// --- Shine Settings ---
uniform vec4 shine_color : source_color = vec4(1.0, 1.0, 1.0, 0.8);
uniform float shine_width : hint_range(0.0, 1.0) = 0.15;
uniform float shine_angle : hint_range(0.0, 360.0) = 45.0;
uniform float shine_interval : hint_range(0.1, 10.0) = 3.0;
uniform float shine_speed : hint_range(0.1, 5.0) = 1.0;
uniform float shine_smoothness : hint_range(0.0, 1.0) = 0.05;
uniform float time_offset = 0.0;

// --- Line Art Protection ---
// Set this to your specific "Dark Purple" using the eyedropper
uniform vec4 line_color : source_color = vec4(0.2, 0.1, 0.3, 1.0); 

// How close a pixel color must be to the line_color to be ignored.
// 0.0 = Exact match only (bad for anti-aliasing). 
// 0.4 = Covers a wider range of dark purples.
uniform float line_tolerance : hint_range(0.0, 1.0) = 0.1;

void fragment() {
    vec4 texture_color = texture(TEXTURE, UV);

    // --- Time Calculation ---
    float adjusted_time = TIME + time_offset;
    float time_mod = mod(adjusted_time, shine_interval);
    float progress = (time_mod * shine_speed) - 1.0;

    // --- Projection ---
    float rad = radians(shine_angle);
    vec2 uv_centered = UV - vec2(0.5);
    float projected_pos = uv_centered.x * cos(rad) + uv_centered.y * sin(rad);

    // --- Shine Intensity ---
    float dist = abs(projected_pos - progress);
    float shine_intensity = 1.0 - smoothstep(shine_width * 0.5, (shine_width * 0.5) + shine_smoothness, dist);

    // --- Line Art Masking ---
    // Calculate how different this pixel is from the line color
    float color_diff = distance(texture_color.rgb, line_color.rgb);
    
    // Create a mask:
    // If color_diff is SMALL (pixel is purple), mask approaches 0.0 (Block shine)
    // If color_diff is LARGE (pixel is crystal color), mask approaches 1.0 (Allow shine)
    // We add a small 0.1 fade to line_tolerance to handle anti-aliased edges smoothly
    float line_mask = smoothstep(line_tolerance, line_tolerance + 0.1, color_diff);

    // --- Compositing ---
    // Multiply shine_intensity by line_mask to remove shine from outlines
    float final_shine = shine_intensity * line_mask;
    
    vec3 final_rgb = mix(texture_color.rgb, shine_color.rgb, final_shine * shine_color.a);

    COLOR = vec4(final_rgb, texture_color.a);
}