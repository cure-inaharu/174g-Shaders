/*{
  "ISFVSN": "2",
  "CATEGORIES": ["Strobe", "Stylize"],
  "DESCRIPTION": "Three strobe modes: white-black-original, inverted image, or white flash.",
  "INPUTS": [
    {"NAME":"inputImage", "TYPE":"image"},
    {"NAME":"mode", "LABEL":"Mode", "TYPE":"long", "VALUES":[0,1,2], "LABELS":["White → Black → Original", "Invert", "White"], "DEFAULT":0},
    {"NAME":"frequency", "LABEL":"Frequency (Hz)", "TYPE":"float", "DEFAULT":4.0, "MIN":0.1, "MAX":30.0},
    {"NAME":"intensity", "LABEL":"Intensity (%)", "TYPE":"float", "DEFAULT":100.0, "MIN":0.0, "MAX":100.0}
  ]
}*/

void main() {
  vec4 original = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord);
  float phase = fract(TIME * max(frequency, 0.1));
  vec3 color = original.rgb;

  if (mode == 0) {
    // Show white, black, and the source for equal thirds of each cycle.
    if (phase < 1.0 / 3.0) {
      color = vec3(1.0);
    } else if (phase < 2.0 / 3.0) {
      color = vec3(0.0);
    }
  } else if (phase < 0.5) {
    if (mode == 1) {
      color = vec3(1.0) - original.rgb;
    } else {
      color = vec3(1.0);
    }
  }

  gl_FragColor = vec4(mix(original.rgb, color, clamp(intensity / 100.0, 0.0, 1.0)), original.a);
}
