[VertexShader]
attribute vec3 tangent;

varying vec3 v_V;
varying vec3 v_N;
varying vec3 etangent;
varying vec3 ebitangent;
varying vec4 efragCoord;

void main() {
	gl_Position = ftransform();
	gl_TexCoord[0] = gl_MultiTexCoord0;
	v_V = (gl_ModelViewMatrix * gl_Vertex).xyz;
	v_N = gl_NormalMatrix * gl_Normal;
	efragCoord = gl_ModelViewMatrix * gl_Vertex;

	etangent = gl_NormalMatrix * tangent;
	ebitangent = cross(v_N, etangent); 
}
[FragmentShader]
varying vec3 v_V;
varying vec3 v_N;
varying vec3 etangent;
varying vec3 ebitangent;
varying vec4 efragCoord;

uniform sampler2D heightmap;
uniform sampler2D texture;
uniform sampler2D normalmap;
uniform float depth;

float find_intersection(vec2 dp, vec2 ds) {
	const int linear_steps = 10;
	const int binary_steps = 5;
	float depth_step = 1.0 / linear_steps;
	float size = depth_step;
	float depth = 1.0;
	float best_depth = 1.0;
	for (int i = 0 ; i < linear_steps - 1 ; ++i) {
		depth -= size;
		vec4 t = texture2D(heightmap, dp + ds * depth);
		if (depth >= 1.0 - t.r)
			best_depth = depth;
	}
	depth = best_depth - size;
	for (int i = 0 ; i < binary_steps ; ++i) {
		size *= 0.5;
		vec4 t = texture2D(heightmap, dp + ds * depth);
		if (depth >= 1.0 - t.r) {
			best_depth = depth;
			depth -= 2 * size;
		}
		depth += size;
	}
	return best_depth;
}

void main() {
	vec3 N = normalize(v_N);
	vec3 V = normalize(v_V);

	// e: eye space
	// t: tangent space
	vec3 eview = normalize(efragCoord.xyz);
	vec3 tview = normalize(vec3(dot(eview, etangent), dot(eview, ebitangent), dot(eview, -N)));
	vec2 ds = tview.xy * depth / tview.z;
	vec2 dp = gl_TexCoord[0].xy;
	float dist = find_intersection(dp, ds);
	vec2 uv = dp + dist * ds;

	vec3 tnormal = 2 * texture2D(normalmap, uv).xyz - vec3(1.0);
	vec3 mapN = normalize(-tnormal.x * etangent - tnormal.y * ebitangent + tnormal.z * N);
	vec3 R = reflect(V, mapN);
	vec3 L = normalize(efragCoord.xyz - gl_LightSource[0].position.xyz);
	vec4 texcol = texture2D(texture, uv);
	vec4 lightcol = vec4(225.0/255.0, 215.0/255.0, 200.0/255.0, 1.0);
	vec4 ambient = texcol * gl_FrontMaterial.ambient;
	vec4 diffuse = /* gl_FrontMaterial.diffuse */ lightcol * texcol * max(dot(-L, mapN), 0.0);
	vec4 specular = /*gl_FrontMaterial.specular */ lightcol * 0.25 * texcol *
		pow(max(dot(R, -L), 0.0), 32.0/*gl_FrontMaterial.shininess*/);

	dp = uv;
	vec3 tlight = normalize(vec3(dot(L, etangent), dot(L, ebitangent), dot(L, -N)));
	ds = tlight.xy * depth / tlight.z;
	dp -= ds * dist;
	float lightdist = find_intersection(dp, ds);
	if (tlight.z < 0.0 || lightdist < dist - 0.05) {
	  diffuse = vec4(0.0);
	  specular = vec4(0.0);
	}
	
	vec4 addcol = vec4(53.0/255.0, 69.0/255.0, 85.0/255.0, 1.0);
	for (int i = 1 ; i < 3 ; ++i) {
	  L = normalize(efragCoord.xyz - gl_LightSource[i].position.xyz);
	  diffuse += addcol * texcol * max(dot(-L, mapN), 0.0);
	  specular += /*gl_FrontMaterial.specular */ addcol * 0.25 * texcol *
	    pow(max(dot(R, -L), 0.0), 32.0/*gl_FrontMaterial.shininess*/);
	}

	gl_FragColor = ambient + diffuse + specular;
}

[Parameters]
float depth = 0.05;
sampler2D heightmap = load("../../shaderdesigner/textures/mur_Hauteur.png");
sampler2D normalmap = load("../../shaderdesigner/textures/mur_NormalMap.png");
sampler2D texture = load("../../shaderdesigner/textures/mur_Ambiant.png");
