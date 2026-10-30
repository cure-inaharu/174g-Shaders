/*{
  "ISFVSN": "2",
  "CATEGORIES": ["Glitch", "Geometry Adjustment"],
  "CREDIT": "Based on Resize Glitch by VIDVOX; customized timing controls",
  "DESCRIPTION": "Timed resize glitches with independent interval, duration, change speed and random scale ranges.",
  "INPUTS": [
    {"NAME":"inputImage", "TYPE":"image"},
    {"NAME":"autoGlitch", "LABEL":"Auto Glitch", "TYPE":"bool", "DEFAULT":true},
    {"NAME":"glitchNow", "LABEL":"Glitch Now", "TYPE":"event"},
    {"NAME":"interval", "LABEL":"Interval (seconds)", "TYPE":"float", "DEFAULT":1.0, "MIN":0.05, "MAX":30.0},
    {"NAME":"duration", "LABEL":"Duration (seconds)", "TYPE":"float", "DEFAULT":0.25, "MIN":0.01, "MAX":30.0},
    {"NAME":"chance", "LABEL":"Auto Chance", "TYPE":"float", "DEFAULT":1.0, "MIN":0.0, "MAX":1.0},
    {"NAME":"changeSpeed", "LABEL":"Changes per Second", "TYPE":"float", "DEFAULT":12.0, "MIN":0.0, "MAX":60.0},
    {"NAME":"levelX", "LABEL":"Width Scale", "TYPE":"float", "DEFAULT":2.0, "MIN":0.01, "MAX":10.0},
    {"NAME":"levelY", "LABEL":"Height Scale", "TYPE":"float", "DEFAULT":2.0, "MIN":0.01, "MAX":10.0},
    {"NAME":"randomizeWidth", "LABEL":"Randomize Width", "TYPE":"bool", "DEFAULT":true},
    {"NAME":"randomizeHeight", "LABEL":"Randomize Height", "TYPE":"bool", "DEFAULT":true},
    {"NAME":"minRandomScale", "LABEL":"Random Scale Minimum", "TYPE":"float", "DEFAULT":0.1, "MIN":0.01, "MAX":1.0},
    {"NAME":"maxRandomScale", "LABEL":"Random Scale Maximum", "TYPE":"float", "DEFAULT":1.0, "MIN":0.01, "MAX":2.0},
    {"NAME":"center", "LABEL":"Center", "TYPE":"point2D", "DEFAULT":[0.5,0.5], "MIN":[0,0], "MAX":[1,1]},
    {"NAME":"randomizeCenter", "LABEL":"Randomize Center", "TYPE":"bool", "DEFAULT":true},
    {"NAME":"mixAmount", "LABEL":"Mix", "TYPE":"float", "DEFAULT":1.0, "MIN":0.0, "MAX":1.0}
  ],
  "PASSES": [
    {"TARGET":"eventState", "PERSISTENT":true, "FLOAT":true, "WIDTH":1, "HEIGHT":1},
    {}
  ]
}*/

// Resize Glitch by VIDVOX: the original applies independent X/Y scaling around
// a center and makes pixels outside the source transparent.
float randomValue(vec2 st) {
  return fract(sin(dot(st, vec2(12.9898, 78.233))) * 43758.5453123);
}

void main() {
  if (PASSINDEX == 0) {
    vec4 previous = IMG_NORM_PIXEL(eventState, vec2(0.5));
    // B marks a valid trigger, so the initial zero-filled buffer is inactive.
    if (glitchNow) {
      gl_FragColor = vec4(TIME, float(FRAMEINDEX), 1.0, 1.0);
    } else {
      gl_FragColor = previous;
    }
  } else {
    vec4 state = IMG_NORM_PIXEL(eventState, vec2(0.5));
    float period = max(interval, 0.05);
    float cycle = floor(TIME / period);
    float autoAge = mod(TIME, period);
    float manualAge = TIME - state.r;
    bool manualActive = state.b > 0.5 && manualAge >= 0.0 && manualAge < duration;
    bool autoActive = autoGlitch && autoAge < min(duration, period)
      && (chance >= 1.0 || (chance > 0.0 && randomValue(vec2(cycle, 9.731)) < chance));
    bool active = manualActive || autoActive;
    float age = manualActive ? manualAge : autoAge;
    float seed = manualActive ? state.g + 1000.0 : cycle;
    float stepIndex = floor(age * max(changeSpeed, 0.0));
    vec2 key = vec2(seed, stepIndex);
    float lowScale = min(minRandomScale, maxRandomScale);
    float highScale = max(minRandomScale, maxRandomScale);
    float width = randomizeWidth ? levelX * mix(lowScale, highScale, randomValue(key + vec2(0.315, 32.0))) : levelX;
    float height = randomizeHeight ? levelY * mix(lowScale, highScale, randomValue(key + vec2(0.942, 43.0))) : levelY;
    vec2 pivot = randomizeCenter
      ? vec2(randomValue(key + vec2(1.24, 0.234)), randomValue(key + vec2(2.93, 1.234)))
      : center;
    vec2 loc = (isf_FragNormCoord - pivot) / max(vec2(width, height), vec2(0.001)) + pivot;
    vec4 original = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord);
    vec4 resized = vec4(0.0);
    if (loc.x >= 0.0 && loc.y >= 0.0 && loc.x <= 1.0 && loc.y <= 1.0) {
      resized = IMG_NORM_PIXEL(inputImage, loc);
    }
    gl_FragColor = active ? mix(original, resized, mixAmount) : original;
  }
}
