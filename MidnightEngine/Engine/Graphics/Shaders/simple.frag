#version 330 core

in vec3 vertexColor;
out vec4 FragColor;

uniform float time;

void main()
{
    // Parzamos szinek a vertextol
    vec3 color = vertexColor;
    
    // Opcionalis pulzalas effekt
    // color *= 0.5 + 0.5 * sin(time);
    
    FragColor = vec4(color, 1.0);
}