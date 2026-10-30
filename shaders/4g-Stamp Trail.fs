/*{
    "DESCRIPTION": "GPT Stamp Trail v7 - lightweight persistent image history. No time-based lifetime. One persistent buffer. Trail Amount is a cleanup control; 0% clears the trail.",
    "CREDIT": "Custom ISF shader generated for VDMX6 / GPT",
    "ISFVSN": "2",
    "CATEGORIES": ["Filter", "Stylize"],
    "INPUTS": [
        {"NAME":"inputImage","TYPE":"image"},
        {"NAME":"trailAmount","TYPE":"float","LABEL":"Trail Amount (%)","DEFAULT":100.0,"MIN":0.0,"MAX":100.0},
        {"NAME":"cleanupRate","TYPE":"float","LABEL":"Cleanup Rate","DEFAULT":0.04,"MIN":0.001,"MAX":0.2},
        {"NAME":"opacity","TYPE":"float","LABEL":"Opacity (%)","DEFAULT":100.0,"MIN":0.0,"MAX":100.0},
        {"NAME":"threshold","TYPE":"float","LABEL":"Stamp Threshold","DEFAULT":0.01,"MIN":0.0,"MAX":1.0}
    ],
    "PASSES": [
        {"TARGET":"trailState","PERSISTENT":true,"FLOAT":true}
    ]
}*/

/*
 GPT Stamp Trail v7

 Lightweight design:
 - no Lifetime / seconds-based expiration
 - no array of individual stamps
 - no multiple full-resolution frame history
 - one persistent RGBA texture

 Persistent state:
   RGB = accumulated trail image
   A   = history strength

 Trail Amount:
   100% = preserve existing history
   0%   = clear all history
   1-99% = continuously clean accumulated history

 This intentionally does not count individual stamps. New marks can be
 added while Trail Amount is being moved.
*/

void main() {
    vec2 uv = isf_FragNormCoord;
    vec4 current = IMG_NORM_PIXEL(inputImage, uv);
    vec4 previous = IMG_THIS_PIXEL(trailState);

    float amount = clamp(trailAmount / 100.0, 0.0, 1.0);

    /* 0% is an explicit hard clear. */
    if (amount <= 0.0001) {
        gl_FragColor = vec4(0.0);
        return;
    }

    /*
       Current non-transparent pixels are the newest part of the trail.
       They immediately reinforce/replace the stored image at this pixel.
    */
    if (current.a > threshold) {
        gl_FragColor = vec4(current.rgb, 1.0);
        return;
    }

    /* No new image here: carry the accumulated history forward. */
    float strength = previous.a;

    /*
       100% means no intentional cleanup. The trail therefore does not
       disappear merely because time has passed.
    */
    if (amount >= 0.9999) {
        gl_FragColor = previous;
        return;
    }

    /*
       Lower values increase cleanup. This is deliberately gradual rather
       than a hard age/count cutoff, so the control can be moved while new
       stamps are simultaneously being created.

       At 99%, cleanup is very weak.
       At 50%, cleanup is moderate.
       Near 0%, cleanup is strong.
    */
    float cleanup = (1.0 - amount) * cleanupRate;
    strength = max(0.0, strength - cleanup);

    if (strength <= 0.0005) {
        gl_FragColor = vec4(0.0);
    }
    else {
        gl_FragColor = vec4(previous.rgb, strength);
    }
}
