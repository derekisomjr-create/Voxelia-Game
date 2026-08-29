#version 330 core
            
uniform sampler2D uTexture;

out vec4 out_color;
in vec2 frag_TexCoords;

void main()
{
    //out_color = vec4(1.0, 0.5, 0.2, 1.0);
    out_color = texture(uTexture, frag_TexCoords);
}