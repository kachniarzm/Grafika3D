varying vec4 f_position;  // position of the vertex (and fragment) in world space
varying vec4 color;
varying vec3 varyingNormalDirection;  // surface normal vector in world space
varying vec2 f_texcoord;
varying vec2 f_marktexcoord;

uniform sampler2D myTexture;
uniform sampler2D myMarkTexture;
uniform sampler2D myHeightTexture;

uniform mat4 m_transform;
uniform mat4 m_view;
uniform mat4 m_view_inv;
uniform mat4 m_projection;
uniform float tick;
uniform vec3 cameraPosition;
uniform float fogRatio;
uniform float opacity;

uniform int ifTextured;
uniform int ifMarkTextured;
uniform int ifMaxLight;
uniform int ifHeightTextured;
 
struct lightSource
{
  vec4 position;
  vec4 diffuse;
  vec4 specular;
  float constantAttenuation, linearAttenuation, quadraticAttenuation;
  float spotCutoff, spotExponent;
  vec3 spotDirection;
};

const int numberOfLights = 3;
lightSource lights[numberOfLights];

lightSource light0 = lightSource( // reflector far
  vec4(35.0, 8.0, 26.0, 1.0),
  vec4(1.0, 1.0, 1.0, 1.0),
  vec4(0.1, 0.1, 0.1, 0.1),
  0.0, 0.05, 0.0,
  15.0 + tick*10, 1.0,
  vec3(-1.0, -1.0, -1.0)
);

lightSource light1 = lightSource( // reflector near
  vec4(3.0, 8.0, 26.0, 1.0),
  vec4(1.0, 1.0, 1.0, 1.0),
  vec4(0.1, 0.1, 0.1, 0.1),
  0.0, 0.05, 0.0,
  15.0 + tick*10, 1.0,
  vec3(1.0, -1.0, -1.0)
);

lightSource light2 = lightSource( // red lamp far
  vec4(32.0, 6.0, 26.0, 1.0),
  vec4(tick, 1.0 - tick, 0.0, 1.0),
  vec4(0.1, 0.1, 0.1, 0.1),
  0.0, 0.5, 0.0,
  180.0, 20.0,
  vec3(1.0, -1.0, -1.0)
);

//lightSource light3 = lightSource( // green lamp near
//  vec4(4.0, 6.0, 26.0, 1.0),
//  vec4(0.0, 1.0, 0.0, 1.0),
//  vec4(0.1, 0.1, 0.1, 0.1),
//  0.0, 0.5, 0.0,
//  180.0, 20.0,
//  vec3(1.0, -1.0, -1.0)
//);

/*lightSource light0 = lightSource( // reflector from (5.0, 5.0, 5.0) power = 0.05, angle = 20
  vec4(5.0, 5.0, 5.0, 1.0),
  vec4(1.0, 1.0, 1.0, 1.0),
  vec4(1.0, 1.0, 1.0, 1.0),
  0.0, 0.05, 0.0,
  20.0, 1.0,
  vec3(1.0, -0.5, 1.0)
);*/

/*lightSource light0 = lightSource( // direction light from (-5.0, 5.0, -5.0)
  vec4(-5.0, 5.0, -5.0, 0.0),
  vec4(1.0, 1.0, 1.0, 1.0),
  vec4(1.0, 1.0, 1.0, 1.0),
  0.0, 0.5, 0.0,
  180.0, 20.0,
  vec3(1.0, -0.3, 1.0)
);*/

/*lightSource light0 = lightSource( //point light in (14.5,  3.0,  14.5) power = 0.5
  vec4(14.5,  3.0,  14.5, 1.0),
  vec4(1.0,  1.0,  1.0, 1.0),
  vec4(1.0,  1.0,  1.0, 1.0),
  0.0, 0.5, 0.0,
  180.0, 20.0,
  vec3(1.0, -0.3, 1.0)
);*/

vec4 scene_ambient = vec4(0.2, 0.2, 0.2, 1.0);
 
struct material
{
  vec4 ambient;
  vec4 diffuse;
  vec4 specular;
  float shininess;
};
material frontMaterial = material(
  vec4(0.5, 0.5, 0.5, 1.0),
  vec4(1.0, 0.8, 0.8, 1.0),
  vec4(1.0, 1.0, 1.0, 1.0),
  50.0
);
 
void main()
{
	lights[0] = light0;
	lights[1] = light1;
	lights[2] = light2;

	vec3 normalDirection = normalize(varyingNormalDirection);
	vec3 viewDirection = normalize(vec3(m_view_inv * vec4(0.0, 0.0, 0.0, 1.0) - f_position));
	vec3 lightDirection;
	float attenuation;

	 // initialize total lighting with ambient lighting
	vec3 totalLighting = vec3(scene_ambient) * vec3(frontMaterial.ambient);
 
	for (int index = 0; index < numberOfLights; index++) // for all light sources
    {
		if (0.0 == lights[index].position.w) // directional light?
		{
			attenuation = 1.0; // no attenuation
			lightDirection = normalize(vec3(lights[index].position));
		} 
		else // point light or spotlight (or other kind of light) 
		{
			vec3 positionToLightSource = vec3(lights[index].position - f_position);
			float distance = length(positionToLightSource);
			lightDirection = normalize(positionToLightSource);
			attenuation = 1.0 / (lights[index].constantAttenuation
							   + lights[index].linearAttenuation * distance
							   + lights[index].quadraticAttenuation * distance * distance);
 
			if (lights[index].spotCutoff <= 90.0) // spotlight?
			{
				float clampedCosine = max(0.0, dot(-lightDirection, normalize(lights[index].spotDirection)));
				if (clampedCosine < cos(lights[index].spotCutoff * 3.14159 / 180.0)) // outside of spotlight cone
				{
					attenuation = 0.0;
				}
				else
				{
					attenuation = attenuation * pow(clampedCosine, lights[index].spotExponent);   
				}
			}
		}
 
		vec3 diffuseReflection = attenuation 
			* vec3(lights[index].diffuse) * vec3(frontMaterial.diffuse)
			* max(0.0, dot(normalDirection, lightDirection));
 
		vec3 specularReflection;
		if (dot(normalDirection, lightDirection) < 0.0) // light source on the wrong side?
		{
			specularReflection = vec3(0.0, 0.0, 0.0); // no specular reflection
		}
		else // light source on the right side
		{
			specularReflection = attenuation * vec3(lights[index].specular) * vec3(frontMaterial.specular) 
				* pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), frontMaterial.shininess);
		}
 
		totalLighting = totalLighting + specularReflection + diffuseReflection;
	}
	
	// texture
	vec4 actualColor;
	if (ifTextured == 1)
	{
		if (ifMarkTextured == 1)
		{
			actualColor = texture2D(myMarkTexture, f_marktexcoord);
			if (actualColor.w != 1)
			{
				actualColor = texture2D(myTexture, f_texcoord);
			}		
		}
		else
		{
			actualColor = texture2D(myTexture, f_texcoord);
		}
		
		// test
		//if (ifHeightTextured == 1)
		//{
		//	actualColor = texture2D(myHeightTexture, f_texcoord);
		//}
	}
	else
	{
		actualColor = color;
	}

	// light
	if (ifMaxLight == 0)
	{
		actualColor = actualColor * vec4(totalLighting, 1.0);
	}
	
	// opacity
	actualColor = actualColor * vec4(1.0, 1.0, 1.0, opacity);
	
	// fog
	vec3 positionToCamera = cameraPosition - vec3(f_position);
	float distance = length(positionToCamera);
	float fog = distance * fogRatio;
	if (fog > 1) fog  = 1;
	actualColor = actualColor * (1 - fog) + vec4(0.5, 0.5, 0.5, actualColor.w) * fog;
	

	gl_FragColor = actualColor;	
}